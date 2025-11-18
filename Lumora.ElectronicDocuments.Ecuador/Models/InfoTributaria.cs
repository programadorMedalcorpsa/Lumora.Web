using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumora.ElectronicDocuments.Ecuador.Models
{
    public class InfoTributaria
    {
        public string Ruc { get; set; } = "";
        public string RazonSocial { get; set; } = "";
        public string NombreComercial { get; set; } = "";
        public string DireccionMatriz { get; set; } = "";
        public string Ambiente { get; set; } = "1"; // 1=Pruebas, 2=Producción
        public string TipoEmision { get; set; } = "1"; // 1=Normal
        public string Serie { get; set; } = "001001"; // Ej: 001-001
        public string Secuencial { get; set; } = "000000001";
        public string ClaveAcceso { get; set; } = "";
        public DateTime FechaEmision { get; set; } = DateTime.Now;
        public string TipoComprobante { get; set; } = "01"; // Factura
    }
}
