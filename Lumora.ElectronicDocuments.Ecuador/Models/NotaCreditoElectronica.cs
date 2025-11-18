using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumora.ElectronicDocuments.Ecuador.Models
{
    public class NotaCreditoElectronica
    {
        public InfoTributaria InfoTributaria { get; set; } = new();
        public InfoNotaCredito InfoNotaCredito { get; set; } = new();
        public List<DetalleNotaCredito> Detalles { get; set; } = new();
        public List<CampoAdicional> InfoAdicional { get; set; } = new();
    }

    public class InfoNotaCredito
    {
        public DateTime FechaEmision { get; set; } = DateTime.Now;
        public string DirEstablecimiento { get; set; } = "";
        public string TipoIdentificacionComprador { get; set; } = "04"; // RUC
        public string RazonSocialComprador { get; set; } = "";
        public string IdentificacionComprador { get; set; } = "";
        public bool ObligadoContabilidad { get; set; } = true;
        public string CodDocModificado { get; set; } = "01";
        public string NumDocModificado { get; set; } = "";
        public DateTime FechaEmisionDocModificado { get; set; }
        public decimal TotalSinSubsidio { get; set; }
        public decimal ValorModificacion { get; set; }
        public string Motivo { get; set; } = "Devolución";
    }

    public class DetalleNotaCredito
    {
        public string CodigoInterno { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Descuento { get; set; }
        public decimal PrecioTotalSinImpuesto { get; set; }
        public List<ImpuestoDetalle> Impuestos { get; set; } = new();
    }

}
