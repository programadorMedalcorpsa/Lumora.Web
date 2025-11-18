using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Lumora.ElectronicDocuments.Ecuador.Services
{
    /// <summary>
    /// Genera Clave de Acceso SRI Ecuador 2025 – Algoritmo OFICIAL
    /// 49 dígitos – Módulo 11
    /// </summary>
    public class ClaveAccesoService
    {
        /// <summary>
        /// Genera la clave de acceso completa (49 dígitos)
        /// </summary>
        public string GenerarClaveAcceso(
            DateTime fechaEmision,
            string tipoComprobante,    // "01" factura, "06" guía, "04" nota crédito, etc.
            string ruc,
            string ambiente,           // "1" pruebas, "2" producción
            string serie,              // "001001"
            string secuencial,         // "000000001"
            string tipoEmision = "1")  // "1" normal, "2" contingencia
        {
            // 1. Formato fecha: ddMMyyyy
            string fecha = fechaEmision.ToString("ddMMyyyy");

            // 2. Código numérico (8 dígitos) – puedes usar uno fijo o aleatorio
            string codigoNumerico = GenerarCodigoNumerico();

            // 3. Armar cadena base de 41 dígitos
            string cadenaBase = $"{fecha}{tipoComprobante}{ruc}{ambiente}{serie}{secuencial}{codigoNumerico}{tipoEmision}";

            // 4. Calcular dígito verificador (Módulo 11 – Algoritmo SRI)
            string digitoVerificador = CalcularDigitoVerificador(cadenaBase);

            // 5. Clave de acceso final (49 dígitos)
            return cadenaBase + digitoVerificador;
        }

        private string GenerarCodigoNumerico()
        {
            // 8 dígitos aleatorios (o puedes usar un contador)
            Random rnd = new Random();
            return rnd.Next(10000000, 99999999).ToString();
        }

        private string CalcularDigitoVerificador(string cadena)
        {
            int[] factores = { 2, 3, 4, 5, 6, 7 }; // SRI usa estos factores en reversa
            int suma = 0;
            int indiceFactor = 0;

            // Recorrer de derecha a izquierda
            for (int i = cadena.Length - 1; i >= 0; i--)
            {
                int digito = int.Parse(cadena[i].ToString());
                int factor = factores[indiceFactor % 6];
                suma += digito * factor;
                indiceFactor++;
            }

            int residuo = suma % 11;
            int verificador = 11 - residuo;

            if (verificador == 11) verificador = 0;
            if (verificador == 10) verificador = 1;

            return verificador.ToString();
        }
    }
}