using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace Lumora.ElectronicDocuments.Ecuador.Services
{
    /// <summary>
    /// Firma digital XML con certificado .p12 – OFICIAL SRI 2025
    /// </summary>
    public class FirmaElectronicaService
    {
        public string FirmarXml(string xmlSinFirma, string rutaCertificadoP12, string password)
        {
            // Cargar certificado .p12
            var certificado = new X509Certificate2(rutaCertificadoP12, password,
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

            // Cargar XML
            var documento = new XmlDocument { PreserveWhitespace = true };
            documento.LoadXml(xmlSinFirma);

            // Crear firma
            var clavePrivada = certificado.GetRSAPrivateKey();
            var signedXml = new SignedXml(documento) { SigningKey = clavePrivada };

            // Referencia al documento completo
            var reference = new Reference("");
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigC14NTransform());
            signedXml.AddReference(reference);

            // KeyInfo con X509Data
            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(certificado));
            signedXml.KeyInfo = keyInfo;

            // Calcular firma
            signedXml.ComputeSignature();

            // Insertar firma en el XML
            var xmlDigitalSignature = signedXml.GetXml();
            documento.DocumentElement?.AppendChild(documento.ImportNode(xmlDigitalSignature, true));

            return documento.OuterXml;
        }
    }
}