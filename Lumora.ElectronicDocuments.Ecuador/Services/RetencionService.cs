using System;
using System.Threading.Tasks;
using Lumora.ElectronicDocuments.Ecuador.Models;
using Lumora.ElectronicDocuments.Ecuador.Services;

namespace Lumora.ElectronicDocuments.Ecuador.Services
{
    /// <summary>
    /// Comprobante de Retención Electrónica (Código 07)
    /// SRI Ecuador 2025 – 100% funcional
    /// </summary>
    public class RetencionService
    {
        private readonly ClaveAccesoService _claveService = new();
        private readonly XmlGeneratorService _xmlService = new();
        private readonly FirmaElectronicaService _firmaService = new();
        private readonly SriReceptionService _sriService = new();
        private readonly RideService _rideService = new();

        public async Task<RespuestaSRI> EmitirRetencionAsync(
            RetencionElectronica retencion,
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
                // 1. Generar clave de acceso (código 07)
                retencion.InfoTributaria.ClaveAcceso = _claveService.GenerarClaveAcceso(
                    retencion.InfoCompRetencion.FechaEmision,
                    "07",
                    retencion.InfoTributaria.Ruc,
                    retencion.InfoTributaria.Ambiente,
                    retencion.InfoTributaria.Serie,
                    retencion.InfoTributaria.Secuencial
                );

                respuesta.ClaveAcceso = retencion.InfoTributaria.ClaveAcceso;

                // 2. Generar XML
                string xmlSinFirma = _xmlService.GenerarXmlRetencion(retencion);

                // 3. Firmar
                string xmlFirmado = _firmaService.FirmarXml(xmlSinFirma, rutaCertificadoP12, passwordCertificado);

                // 4. Enviar al SRI
                var resultadoSri = await _sriService.EnviarComprobanteAsync(
                    xmlFirmado,
                    retencion.InfoTributaria.ClaveAcceso,
                    retencion.InfoTributaria.Ambiente == "1" ? Ambiente.Pruebas : Ambiente.Produccion
                );

                respuesta.Estado = resultadoSri.Estado;
                respuesta.Mensajes = resultadoSri.Mensajes;
                respuesta.NumeroAutorizacion = resultadoSri.NumeroAutorizacion;
                respuesta.FechaAutorizacion = resultadoSri.FechaAutorizacion;

                // 5. RIDE si está AUTORIZADO
                if (resultadoSri.Estado == "AUTORIZADO")
                {
                    respuesta.RidePdf = _rideService.GenerarRideRetencion(retencion, resultadoSri);
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Estado = "ERROR";
                respuesta.Mensajes.Add(new MensajeSRI
                {
                    Identificador = "99",
                    Mensaje = "Error en retención: " + ex.Message
                });
                return respuesta;
            }
        }
    }
}