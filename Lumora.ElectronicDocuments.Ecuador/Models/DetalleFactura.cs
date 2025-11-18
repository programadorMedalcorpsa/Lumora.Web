using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumora.ElectronicDocuments.Ecuador.Models
{
    public class DetalleFactura
    {
        public string CodigoPrincipal { get; set; } = "";
        public string CodigoAuxiliar { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Descuento { get; set; }
        public decimal PrecioTotalSinImpuesto { get; set; }
        public List<ImpuestoDetalle> Impuestos { get; set; } = new();
    }


}
