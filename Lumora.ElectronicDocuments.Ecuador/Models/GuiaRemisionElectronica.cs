using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumora.ElectronicDocuments.Ecuador.Models
{
    public class GuiaRemisionElectronica
    {
        public InfoTributaria InfoTributaria { get; set; } = new();
        public InfoGuiaRemision InfoGuiaRemision { get; set; } = new();
        public Destinatario Destinatario { get; set; } = new();
        public List<DetalleGuia> Detalles { get; set; } = new();
    }

    public class InfoGuiaRemision
    {
        public DateTime FechaIniTransporte { get; set; }
        public DateTime FechaFinTransporte { get; set; }
        public string Placa { get; set; } = "";
        public string DirEstablecimiento { get; set; } = "";
        public string DirPartida { get; set; } = "";
        public string RucTransportista { get; set; } = "";
    }

    public class Destinatario
    {
        public string IdentificacionDestinatario { get; set; } = "";
        public string RazonSocialDestinatario { get; set; } = "";
        public string DirDestinatario { get; set; } = "";
        public string MotivoTraslado { get; set; } = "";
        public string DocAduaneroUnico { get; set; } = "";
        public string Ruta { get; set; } = "";
    }

    public class DetalleGuia
    {
        public string CodigoInterno { get; set; } = "";
        public string CodigoAdicional { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public decimal Cantidad { get; set; }
    }

}
