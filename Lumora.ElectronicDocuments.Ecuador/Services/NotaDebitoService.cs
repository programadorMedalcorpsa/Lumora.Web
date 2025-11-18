using System;
using System.Threading.Tasks;
using Lumora.ElectronicDocuments.Ecuador.Models;
using Lumora.ElectronicDocuments.Ecuador.Services;

namespace Lumora.ElectronicDocuments.Ecuador.Services
{
    /// <summary>
    /// Servicio para emitir Notas de Débito Electrónicas (Código 05)
    /// SRI Ecuador 2025 – 100% funcional
    /// </summary>
    public class NotaDebitoService
    {
        private readonly ClaveAccesoService _claveService = new();
        private readonly XmlGeneratorService _xmlService = new();
        private readonly FirmaElectronicaService _firmaService = new();
        private readonly SriReceptionService _sriService = new();
        private readonly RideService _rideService = new();

        public async Task<RespuestaSRI> EmitirNotaDebitoAsync(
            NotaDebitoElectronica notaDebito,
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
                // 1. Generar clave de acceso (código 05)
                notaDebito.InfoTributaria.ClaveAcceso = _claveService.GenerarClaveAcceso(
                    notaDebito.InfoNotaDebito.FechaEmision,
                    "05", // Nota de débito
                    notaDebito.InfoTributaria.Ruc,
                    notaDebito.InfoTributaria.Ambiente,
                    notaDebito.InfoTributaria.Serie,
                    notaDebito.InfoTributaria.Secuencial
                );

                respuesta.ClaveAcceso = notaDebito.InfoTributaria.ClaveAcceso;

                // 2. Generar XML válido SRI
                string xmlSinFirma = _xmlService.GenerarXmlNotaDebito(notaDebito);

                // 3. Firmar con certificado .p12
                string xmlFirmado = _firmaService.FirmarXml(xmlSinFirma, rutaCertificadoP12, passwordCertificado);

                // 4. Enviar al SRI
                var resultadoSri = await _sriService.EnviarComprobanteAsync(
                    xmlFirmado,
                    notaDebito.InfoTributaria.ClaveAcceso,
                    notaDebito.InfoTributaria.Ambiente == "1" ? Ambiente.Pruebas : Ambiente.Produccion
                );

                respuesta.Estado = resultadoSri.Estado;
                respuesta.Mensajes = resultadoSri.Mensajes;
                respuesta.NumeroAutorizacion = resultadoSri.NumeroAutorizacion;
                respuesta.FechaAutorizacion = resultadoSri.FechaAutorizacion;

                // 5. Generar RIDE si está AUTORIZADO
                if (resultadoSri.Estado == "AUTORIZADO")
                {
                    respuesta.RidePdf = _rideService.GenerarRideNotaDebito(notaDebito, resultadoSri);
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
                    Mensaje = "Error en Nota de Débito: " + ex.Message,
                    Tipo = "ERROR"
                });
                return respuesta;
            }
        }
    }
}