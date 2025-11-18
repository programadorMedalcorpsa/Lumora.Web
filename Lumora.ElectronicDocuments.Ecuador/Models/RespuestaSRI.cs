using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumora.ElectronicDocuments.Ecuador.Models
{
    internal class RespuestaSRI
    {
        public string ClaveAcceso { get; set; } = "";
        public string Estado { get; set; } = "";
        public string NumeroAutorizacion { get; set; } = "";
        public DateTime? FechaAutorizacion { get; set; }
        public List<MensajeSRI> Mensajes { get; set; } = new();
        public byte[]? RidePdf { get; set; }
        public string? XmlAutorizado { get; set; }
        public DateTime FechaProceso { get; set; }
    }
    public class MensajeSRI
    {
        public string Identificador { get; set; } = "";
        public string Mensaje { get; set; } = "";
        public string Tipo { get; set; } = "";
    }

}
