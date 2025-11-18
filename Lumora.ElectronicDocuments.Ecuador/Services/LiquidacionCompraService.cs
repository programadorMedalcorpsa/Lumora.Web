using System;
using System.Threading.Tasks;
using Lumora.ElectronicDocuments.Ecuador.Models;
using Lumora.ElectronicDocuments.Ecuador.Services;

namespace Lumora.ElectronicDocuments.Ecuador.Services
{
    /// <summary>
    /// Liquidación de Compra Electrónica (Código 03)
    /// SRI Ecuador 2025 – 100% funcional
    /// </summary>
    public class LiquidacionCompraService
    {
        private readonly ClaveAccesoService _claveService = new();
        private readonly XmlGeneratorService _xmlService = new();
        private readonly FirmaElectronicaService _firmaService = new();
        private readonly SriReceptionService _sriService = new();
        private readonly RideService _rideService = new();

        public async Task<RespuestaSRI> EmitirLiquidacionCompraAsync(
            LiquidacionCompraElectronica liquidacion,
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
                liquidacion.InfoTributaria.ClaveAcceso = _claveService.GenerarClaveAcceso(
                    liquidacion.InfoLiquidacionCompra.FechaEmision,
                    "03",
                    liquidacion.InfoTributaria.Ruc,
                    liquidacion.InfoTributaria.Ambiente,
                    liquidacion.InfoTributaria.Serie,
                    liquidacion.InfoTributaria.Secuencial
                );

                respuesta.ClaveAcceso = liquidacion.InfoTributaria.ClaveAcceso;

                string xmlSinFirma = _xmlService.GenerarXmlLiquidacionCompra(liquidacion);
                string xmlFirmado = _firmaService.FirmarXml(xmlSinFirma, rutaCertificadoP12, passwordCertificado);

                var resultadoSri = await _sriService.EnviarComprobanteAsync(
                    xmlFirmado,
                    liquidacion.InfoTributaria.ClaveAcceso,
                    liquidacion.InfoTributaria.Ambiente == "1" ? Ambiente.Pruebas : Ambiente.Produccion
                );

                respuesta.Estado = resultadoSri.Estado;
                respuesta.Mensajes = resultadoSri.Mensajes;

                if (resultadoSri.Estado == "AUTORIZADO")
                {
                    respuesta.RidePdf = _rideService.GenerarRideLiquidacionCompra(liquidacion, resultadoSri);
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