using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumora.ElectronicDocuments.Ecuador.Models
{
    public class FacturaElectronica
    {
        public InfoTributaria InfoTributaria { get; set; } = new();
        public InfoFactura InfoFactura { get; set; } = new();
        public List<DetalleFactura> Detalles { get; set; } = new();
        public List<InfoAdicional> InfoAdicional { get; set; } = new();
    }

    public class InfoFactura
    {
        public DateTime FechaEmision { get; set; } = DateTime.Now;
        public string DirEstablecimiento { get; set; } = "";
        public string ContribuyenteEspecial { get; set; } = "";
        public bool ObligadoContabilidad { get; set; } = true;
        public string TipoIdentificacionComprador { get; set; } = "04"; // 04=RUC, 05=Cédula, 06=Pasaporte
        public string RazonSocialComprador { get; set; } = "";
        public string IdentificacionComprador { get; set; } = "";
        public string DireccionComprador { get; set; } = "";
        public decimal TotalSinImpuestos { get; set; }
        public decimal TotalDescuento { get; set; }
        public List<TotalImpuesto> TotalConImpuestos { get; set; } = new();
        public decimal Propina { get; set; } = 0;
        public decimal ImporteTotal { get; set; }
        public string Moneda { get; set; } = "DOLAR";
    }

    public class TotalImpuesto
    {
        public string Codigo { get; set; } = "2"; // 2=IVA
        public string CodigoPorcentaje { get; set; } = "2"; // 2=12%, 0=0%, 3=14%, etc.
        public decimal BaseImponible { get; set; }
        public decimal Valor { get; set; }
    }


}
