using System;
using System.Threading.Tasks;
using Lumora.ElectronicDocuments.Ecuador.Models;
using Lumora.ElectronicDocuments.Ecuador.Services;

namespace Lumora.ElectronicDocuments.Ecuador.Services
{
    /// <summary>
    /// Servicio para emitir Notas de Crédito Electrónicas (Código 04)
    /// SRI Ecuador 2025 – 100% funcional
    /// </summary>
    public class NotaCreditoService
    {
        private readonly ClaveAccesoService _claveService = new();
        private readonly XmlGeneratorService _xmlService = new();
        private readonly FirmaElectronicaService _firmaService = new();
        private readonly SriReceptionService _sriService = new();
        private readonly RideService _rideService = new();

        public async Task<RespuestaSRI> EmitirNotaCreditoAsync(
            NotaCreditoElectronica notaCredito,
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
                // 1. Generar clave de acceso (código 04)
                notaCredito.InfoTributaria.ClaveAcceso = _claveService.GenerarClaveAcceso(
                    notaCredito.InfoNotaCredito.FechaEmision,
                    "04", // Nota de crédito
                    notaCredito.InfoTributaria.Ruc,
                    notaCredito.InfoTributaria.Ambiente,
                    notaCredito.InfoTributaria.Serie,
                    notaCredito.InfoTributaria.Secuencial
                );

                respuesta.ClaveAcceso = notaCredito.InfoTributaria.ClaveAcceso;

                // 2. Generar XML
                string xmlSinFirma = _xmlService.GenerarXmlNotaCredito(notaCredito);

                // 3. Firmar digitalmente
                string xmlFirmado = _firmaService.FirmarXml(xmlSinFirma, rutaCertificadoP12, passwordCertificado);

                // 4. Enviar al SRI
                var resultadoSri = await _sriService.EnviarComprobanteAsync(
                    xmlFirmado,
                    notaCredito.InfoTributaria.ClaveAcceso,
                    notaCredito.InfoTributaria.Ambiente == "1" ? Ambiente.Pruebas : Ambiente.Produccion
                );

                respuesta.Estado = resultadoSri.Estado;
                respuesta.Mensajes = resultadoSri.Mensajes;
                respuesta.NumeroAutorizacion = resultadoSri.NumeroAutorizacion;
                respuesta.FechaAutorizacion = resultadoSri.FechaAutorizacion;

                // 5. Generar RIDE si está AUTORIZADO
                if (resultadoSri.Estado == "AUTORIZADO")
                {
                    respuesta.RidePdf = _rideService.GenerarRideNotaCredito(notaCredito, resultadoSri);
                    respuesta.XmlAutorizado = resultadoSri.XmlAutorizado;
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Estado = "ERROR";
                respuesta.Mensajes.Add(new MensajeSRI
                {
                    Identificador = "99",
                    Mensaje = "Error interno: " + ex.Message,
                    Tipo = "ERROR"
                });
                return respuesta;
            }
        }
    }
}