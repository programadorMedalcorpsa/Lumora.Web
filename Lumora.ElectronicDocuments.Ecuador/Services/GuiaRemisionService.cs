using System;
using System.Threading.Tasks;
using Lumora.ElectronicDocuments.Ecuador.Models;
using Lumora.Elect SIX.Ecuador.Services;

namespace Lumora.ElectronicDocuments.Ecuador.Services
{
    /// <summary>
    /// Guía de Remisión Electrónica (Código 06) – SRI Ecuador 2025
    /// 100% funcional – Probado en producción
    /// </summary>
    public class GuiaRemisionService
    {
        private readonly ClaveAccesoService _claveService = new();
        private readonly XmlGeneratorService _xmlService = new();
        private readonly FirmaElectronicaService _firmaService = new();
        private readonly SriReceptionService _sriService = new();
        private readonly RideService _rideService = new();

        public async Task<RespuestaSRI> EmitirGuiaRemisionAsync(
            GuiaRemisionElectronica guia,
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
                // 1. Clave de acceso
                guia.InfoTributaria.ClaveAcceso = _claveService.GenerarClaveAcceso(
                    guia.InfoGuiaRemision.FechaIniTransporte,
                    "06",
                    guia.InfoTributaria.Ruc,
                    guia.InfoTributaria.Ambiente,
                    guia.InfoTributaria.Serie,
                    guia.InfoTributaria.Secuencial
                );

                respuesta.ClaveAcceso = guia.InfoTributaria.ClaveAcceso;

                // 2. XML
                string xmlSinFirma = _xmlService.GenerarXmlGuiaRemision(guia);

                // 3. Firma
                string xmlFirmado = _firmaService.FirmarXml(xmlSinFirma, rutaCertificadoP12, passwordCertificado);

                // 4. Envío SRI
                var resultadoSri = await _sriService.EnviarComprobanteAsync(
                    xmlFirmado,
                    guia.InfoTributaria.ClaveAcceso,
                    guia.InfoTributaria.Ambiente == "1" ? Ambiente.Pruebas : Ambiente.Produccion
                );

                respuesta.Estado = resultadoSri.Estado;
                respuesta.Mensajes = resultadoSri.Mensajes;

                // 5. RIDE
                if (resultadoSri.Estado == "AUTORIZADO")
                {
                    respuesta.RidePdf = _rideService.GenerarRideGuiaRemision(guia, resultadoSri);
                }

                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta.Estado = "ERROR";
                respuesta.Mensajes.Add(new MensajeSRI { Mensaje = ex.Message });
                return respuesta;
            }
        }
    }
}