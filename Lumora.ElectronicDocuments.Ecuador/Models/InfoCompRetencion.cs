using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumora.ElectronicDocuments.Ecuador.Models
{
    public class InfoCompRetencion
    {
        public DateTime FechaEmision { get; set; } = DateTime.Now;
        public string DirEstablecimiento { get; set; } = "";
        public string TipoIdentificacionSujetoRetenido { get; set; } = "04";
        public string RazonSocialSujetoRetenido { get; set; } = "";
        public string IdentificacionSujetoRetenido { get; set; } = "";
        public string PeriodoFiscal { get; set; } = ""; // Ej: 10/2025
    }

}
