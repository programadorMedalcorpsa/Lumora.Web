using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumora.ElectronicDocuments.Ecuador.Models
{
    // Models/NotaDebitoElectronica.cs
    public class NotaDebitoElectronica
    {
        public InfoTributaria InfoTributaria { get; set; } = new();
        public InfoNotaDebito InfoNotaDebito { get; set; } = new();
        public List<MotivoDebito> Motivos { get; set; } = new();
        public List<CampoAdicional> InfoAdicional { get; set; } = new();
    }

    public class InfoNotaDebito
    {
        public DateTime FechaEmision { get; set; } = DateTime.Now;
        public string DirEstablecimiento { get; set; } = "";
        public string TipoIdentificacionComprador { get; set; } = "04";
        public string RazonSocialComprador { get; set; } = "";
        public string IdentificacionComprador { get; set; } = "";
        public string CodDocModificado { get; set; } = "01";
        public string NumDocModificado { get; set; } = "";
        public DateTime FechaEmisionDocModificado { get; set; }
        public decimal TotalSinImpuestos { get; set; }
        public List<ImpuestoDebito> Impuestos { get; set; } = new();
        public decimal ValorTotal { get; set; }
    }

    public class MotivoDebito
    {
        public string Razon { get; set; } = "";
        public decimal Valor { get; set; }
    }

    public class ImpuestoDebito
    {
        public string Codigo { get; set; } = "2";
        public string CodigoPorcentaje { get; set; } = "2";
        public decimal BaseImponible { get; set; }
        public decimal Valor { get; set; }
    }
}
