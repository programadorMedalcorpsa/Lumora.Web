using System;
using System.Xml;
using Lumora.ElectronicDocuments.Ecuador.Models;

namespace Lumora.ElectronicDocuments.Ecuador.Services
{
    /// <summary>
    /// Genera XML válido para el SRI Ecuador 2025
    /// Compatible con Factura, Guía, Nota Crédito, etc.
    /// </summary>
    public class XmlGeneratorService
    {
        private readonly XmlWriterSettings _settings;

        public XmlGeneratorService()
        {
            _settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = System.Text.Encoding.UTF8,
                OmitXmlDeclaration = false
            };
        }

        // FACTURA ELECTRÓNICA (01)
        public string GenerarXmlFactura(FacturaElectronica factura)
        {
            using var stringWriter = new System.IO.StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, _settings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("factura");
            xmlWriter.WriteAttributeString("id", "comprobante");
            xmlWriter.WriteAttributeString("version", "1.1.0");

            EscribirInfoTributaria(xmlWriter, factura.InfoTributaria);
            EscribirInfoFactura(xmlWriter, factura.InfoFactura);
            EscribirDetalles(xmlWriter, factura.Detalles);
            EscribirInfoAdicional(xmlWriter, factura.InfoAdicional);

            xmlWriter.WriteEndElement(); // factura
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();

            return stringWriter.ToString();
        }

        // GUÍA DE REMISIÓN (06)
        public string GenerarXmlGuiaRemision(GuiaRemisionElectronica guia)
        {
            using var stringWriter = new System.IO.StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, _settings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("guiaRemision");
            xmlWriter.WriteAttributeString("id", "comprobante");
            xmlWriter.WriteAttributeString("version", "1.1.0");

            EscribirInfoTributaria(xmlWriter, guia.InfoTributaria);
            EscribirInfoGuiaRemision(xmlWriter, guia.InfoGuiaRemision);
            EscribirDestinatarios(xmlWriter, guia.Destinatarios);
            EscribirInfoAdicional(xmlWriter, guia.InfoAdicional);

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();

            return stringWriter.ToString();
        }

        // NOTA DE CRÉDITO (04)
        public string GenerarXmlNotaCredito(NotaCreditoElectronica nota)
        {
            using var stringWriter = new System.IO.StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, _settings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("notaCredito");
            xmlWriter.WriteAttributeString("id", "comprobante");
            xmlWriter.WriteAttributeString("version", "1.1.0");

            EscribirInfoTributaria(xmlWriter, nota.InfoTributaria);
            EscribirInfoNotaCredito(xmlWriter, nota.InfoNotaCredito);
            EscribirDetallesNotaCredito(xmlWriter, nota.Detalles);
            EscribirInfoAdicional(xmlWriter, nota.InfoAdicional);

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();

            return stringWriter.ToString();
        }

        // COMPROBANTE DE RETENCIÓN (07)
        public string GenerarXmlRetencion(RetencionElectronica retencion)
        {
            using var stringWriter = new System.IO.StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, _settings);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("comprobanteRetencion");
            xmlWriter.WriteAttributeString("id", "comprobante");
            xmlWriter.WriteAttributeString("version", "1.1.0");

            EscribirInfoTributaria(xmlWriter, retencion.InfoTributaria);
            EscribirInfoCompRetencion(xmlWriter, retencion.InfoCompRetencion);
            EscribirImpuestosRetencion(xmlWriter, retencion.Impuestos);

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();

            return stringWriter.ToString();
        }

        // MÉTODOS PRIVADOS AUXILIARES
        private void EscribirInfoTributaria(XmlWriter writer, InfoTributaria info)
        {
            writer.WriteStartElement("infoTributaria");
            writer.WriteElementString("ambiente", info.Ambiente);
            writer.WriteElementString("tipoEmision", info.TipoEmision);
            writer.WriteElementString("razonSocial", info.RazonSocial);
            writer.WriteElementString("nombreComercial", info.NombreComercial ?? info.RazonSocial);
            writer.WriteElementString("ruc", info.Ruc);
            writer.WriteElementString("claveAcceso", info.ClaveAcceso);
            writer.WriteElementString("codDoc", info.TipoComprobante);
            writer.WriteElementString("estab", info.Serie.Substring(0, 3));
            writer.WriteElementString("ptoEmi", info.Serie.Substring(3, 3));
            writer.WriteElementString("secuencial", info.Secuencial);
            writer.WriteElementString("dirMatriz", info.DireccionMatriz);
            writer.WriteEndElement();
        }

        private void EscribirInfoFactura(XmlWriter writer, InfoFactura info)
        {
            writer.WriteStartElement("infoFactura");
            writer.WriteElementString("fechaEmision", info.FechaEmision.ToString("dd/MM/yyyy"));
            writer.WriteElementString("dirEstablecimiento", info.DirEstablecimiento);
            writer.WriteElementString("obligadoContabilidad", info.ObligadoContabilidad ? "SI" : "NO");
            writer.WriteElementString("tipoIdentificacionComprador", info.TipoIdentificacionComprador);
            writer.WriteElementString("razonSocialComprador", info.RazonSocialComprador);
            writer.WriteElementString("identificacionComprador", info.IdentificacionComprador);
            writer.WriteElementString("totalSinImpuestos", info.TotalSinImpuestos.ToString("F2"));
            writer.WriteElementString("totalDescuento", info.TotalDescuento.ToString("F2"));
            writer.WriteStartElement("totalConImpuestos");
            foreach (var imp in info.TotalConImpuestos)
            {
                writer.WriteStartElement("totalImpuesto");
                writer.WriteElementString("codigo", imp.Codigo);
                writer.WriteElementString("codigoPorcentaje", imp.CodigoPorcentaje);
                writer.WriteElementString("baseImponible", imp.BaseImponible.ToString("F2"));
                writer.WriteElementString("valor", imp.Valor.ToString("F2"));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteElementString("propina", info.Propina.ToString("F2"));
            writer.WriteElementString("importeTotal", info.ImporteTotal.ToString("F2"));
            writer.WriteElementString("moneda", "DOLAR");
            writer.WriteEndElement();
        }

        private void EscribirDetalles(XmlWriter writer, List<DetalleFactura> detalles)
        {
            writer.WriteStartElement("detalles");
            foreach (var d in detalles)
            {
                writer.WriteStartElement("detalle");
                writer.WriteElementString("codigoPrincipal", d.CodigoPrincipal);
                writer.WriteElementString("descripcion", d.Descripcion);
                writer.WriteElementString("cantidad", d.Cantidad.ToString("F6"));
                writer.WriteElementString("precioUnitario", d.PrecioUnitario.ToString("F6"));
                writer.WriteElementString("descuento", d.Descuento.ToString("F2"));
                writer.WriteElementString("precioTotalSinImpuesto", d.PrecioTotalSinImpuesto.ToString("F2"));
                writer.WriteStartElement("impuestos");
                foreach (var i in d.Impuestos)
                {
                    writer.WriteStartElement("impuesto");
                    writer.WriteElementString("codigo", i.Codigo);
                    writer.WriteElementString("codigoPorcentaje", i.CodigoPorcentaje);
                    writer.WriteElementString("tarifa", i.Tarifa.ToString("F2"));
                    writer.WriteElementString("baseImponible", i.BaseImponible.ToString("F2"));
                    writer.WriteElementString("valor", i.Valor.ToString("F2"));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void EscribirInfoAdicional(XmlWriter writer, List<CampoAdicional> campos)
        {
            if (campos == null || campos.Count == 0) return;

            writer.WriteStartElement("infoAdicional");
            foreach (var c in campos)
            {
                writer.WriteStartElement("campoAdicional");
                writer.WriteAttributeString("nombre", c.Nombre);
                writer.WriteString(c.Valor);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        // Métodos similares para Guía, Nota Crédito, Retención...
        private void EscribirInfoGuiaRemision(XmlWriter writer, InfoGuiaRemision info)
        {
            writer.WriteStartElement("infoGuiaRemision");
            writer.WriteElementString("dirEstablecimiento", info.DirEstablecimiento);
            writer.WriteElementString("dirPartida", info.DirPartida);
            writer.WriteElementString("razonSocialTransportista", info.RazonSocialTransportista);
            writer.WriteElementString("rucTransportista", info.RucTransportista);
            writer.WriteElementString("placa", info.Placa);
            writer.WriteElementString("fechaIniTransporte", info.FechaIniTransporte.ToString("dd/MM/yyyy"));
            writer.WriteElementString("fechaFinTransporte", info.FechaFinTransporte.ToString("dd/MM/yyyy"));
            writer.WriteEndElement();
        }

        private void EscribirDestinatarios(XmlWriter writer, List<Destinatario> destinatarios)
        {
            writer.WriteStartElement("destinatarios");
            foreach (var d in destinatarios)
            {
                writer.WriteStartElement("destinatario");
                writer.WriteElementString("identificacionDestinatario", d.IdentificacionDestinatario);
                writer.WriteElementString("razonSocialDestinatario", d.RazonSocialDestinatario);
                writer.WriteElementString("dirDestinatario", d.DirDestinatario);
                writer.WriteElementString("motivoTraslado", d.MotivoTraslado);
                writer.WriteElementString("ruta", d.Ruta);
                writer.WriteElementString("codDocSustento", d.CodDocSustento);
                writer.WriteElementString("numDocSustento", d.NumDocSustento);
                writer.WriteElementString("numAutDocSustento", d.NumAutDocSustento);
                writer.WriteElementString("fechaEmisionDocSustento", d.FechaEmisionDocSustento.ToString("dd/MM/yyyy"));

                writer.WriteStartElement("detalles");
                foreach (var det in d.Detalles)
                {
                    writer.WriteStartElement("detalle");
                    writer.WriteElementString("codigoInterno", det.CodigoInterno);
                    writer.WriteElementString("descripcion", det.Descripcion);
                    writer.WriteElementString("cantidad", det.Cantidad.ToString("F6"));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement(); // detalles

                writer.WriteEndElement(); // destinatario
            }
            writer.WriteEndElement(); // destinatarios
        }

        private void EscribirInfoNotaCredito(XmlWriter writer, InfoNotaCredito info)
        {
            writer.WriteStartElement("infoNotaCredito");
            writer.WriteElementString("fechaEmision", info.FechaEmision.ToString("dd/MM/yyyy"));
            writer.WriteElementString("dirEstablecimiento", info.DirEstablecimiento);
            writer.WriteElementString("tipoIdentificacionComprador", info.TipoIdentificacionComprador);
            writer.WriteElementString("razonSocialComprador", info.RazonSocialComprador);
            writer.WriteElementString("identificacionComprador", info.IdentificacionComprador);
            writer.WriteElementString("obligadoContabilidad", info.ObligadoContabilidad ? "SI" : "NO");
            writer.WriteElementString("codDocModificado", info.CodDocModificado); // 01
            writer.WriteElementString("numDocModificado", info.NumDocModificado);
            writer.WriteElementString("fechaEmisionDocModificado", info.FechaEmisionDocModificado.ToString("dd/MM/yyyy"));
            writer.WriteElementString("totalSinSubsidio", info.TotalSinSubsidio.ToString("F2"));
            writer.WriteElementString("valorModificacion", info.ValorModificacion.ToString("F2"));
            writer.WriteElementString("motivo", info.Motivo);
            writer.WriteEndElement();
        }

    }
}