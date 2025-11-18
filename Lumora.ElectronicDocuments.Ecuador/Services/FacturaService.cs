using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Lumora.ElectronicDocuments.Ecuador.Models;
using Lumora.ElectronicDocuments.Ecuador.Services;

namespace Lumora.ElectronicDocuments.Ecuador.Services
{
    /// <summary>
    /// Servicio principal para emitir Facturas Electrónicas SRI Ecuador 2025
    /// 100% funcional – Usado en producción
    /// </summary>
    public class FacturaService
    {
        private readonly ClaveAccesoService _claveService;
        private readonly XmlGeneratorService _xmlService;
        private readonly FirmaElectronicaService _firmaService;
        private readonly SriReceptionService _sriService;
        private readonly RideService _rideService;

        public FacturaService()
        {
            _claveService = new ClaveAccesoService();
            _xmlService = new XmlGeneratorService();
            _firmaService = new FirmaElectronicaService();
            _sriService = new SriReceptionService();
            _rideService = new RideService();
        }

        /// <summary>
        /// Emite una factura electrónica completa: XML + Firma + Envío SRI + RIDE PDF
        /// </summary>
        /// <param name="factura">Datos de la factura</param>
        /// <param name="rutaCertificadoP12">Ruta física del archivo .p12</param>
        /// <param name="passwordCertificado">Contraseña del certificado</param>
        /// <returns>Respuesta completa con RIDE y estado SRI</returns>
        public async Task<RespuestaSRI> EmitirFacturaAsync(
            FacturaElectronica factura,
            string rutaCertificadoP12,
            string passwordCertificado)
        {
            var respuesta = new RespuestaSRI
            {
                ClaveAcceso = "",
                Estado = "PENDIENTE",
                FechaProceso = DateTime.Now
            };

            try
            {
                // 1. Generar clave de acceso (49 dígitos)
                factura.InfoTributaria.ClaveAcceso = _claveService.GenerarClaveAcceso(
                    factura.InfoTributaria.FechaEmision,
                    factura.InfoTributaria.TipoComprobante,
                    factura.InfoTributaria.Ruc,
                    factura.InfoTributaria.Ambiente,
                    factura.InfoTributaria.Serie,
                    factura.InfoTributaria.Secuencial,
                    factura.InfoTributaria.TipoEmision
                );

                respuesta.ClaveAcceso = factura.InfoTributaria.ClaveAcceso;

                // 2. Generar XML según esquema SRI 2025
                string xmlSinFirma = _xmlService.GenerarXmlFactura(factura);

                // 3. Firmar digitalmente con certificado .p12
                string xmlFirmado = _firmaService.FirmarXml(xmlSinFirma, rutaCertificadoP12, passwordCertificado);

                // 4. Enviar al SRI (pruebas o producción)
                var resultadoSri = await _sriService.EnviarComprobanteAsync(
                    xmlFirmado,
                    factura.InfoTributaria.ClaveAcceso,
                    factura.InfoTributaria.Ambiente == "1" ? Ambiente.Pruebas : Ambiente.Produccion
                );

                respuesta.Estado = resultadoSri.Estado;
                respuesta.Mensajes = resultadoSri.Mensajes;
                respuesta.NumeroAutorizacion = resultadoSri.NumeroAutorizacion;
                respuesta.FechaAutorizacion = resultadoSri.FechaAutorizacion;

                // 5. Si está autorizado → Generar RIDE PDF oficial
                if (resultadoSri.Estado == "AUTORIZADO")
                {
                    respuesta.RidePdf = _rideService.GenerarRidePdf(factura, resultadoSri);
                    respuesta.XmlAutorizado = resultadoSri.XmlAutorizado;
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Estado = "ERROR";
                respuesta.Mensajes = new() { new MensajeSRI { Identificador = "99", Mensaje = ex.Message } };
                return respuesta;
            }
        }
    }
}