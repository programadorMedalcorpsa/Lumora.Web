using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumora.ElectronicDocuments.Ecuador.Models
{
    public class ImpuestoRetencion
    {
        public string Codigo { get; set; } = "1"; // 1=Fuente, 2=IVA
        public string CodigoRetencion { get; set; } = "1"; // Tabla SRI
        public decimal BaseImponible { get; set; }
        public decimal PorcentajeRetener { get; set; }
        public decimal ValorRetenido { get; set; }
        public string CodDocSustento { get; set; } = "01";
        public string NumDocSustento { get; set; } = "";
        public DateTime FechaEmisionDocSustento { get; set; }
    }
}
