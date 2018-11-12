using System;
using System.Text;

namespace Cc.Upt.Common.Isolucion
{
    public class Cripto
    {
        public string CadenaDesencriptada;
        public string CadenaEncriptada;
        public string LlaveEncripcion;
        public string ViEncripcion;

        public string GetCadenaEncriptada()
        {
            var orBytes = Encoding.Unicode.GetBytes(CadenaEncriptada);
            var text = Convert.ToBase64String(orBytes);
            return text;
        }

        public string GetCadenaDesencriptada()
        {
            var orBytes = Encoding.Unicode.GetBytes(CadenaDesencriptada);
            var text = Convert.ToBase64String(orBytes);
            return text;
        }

        public string GetLlaveEncripcion()
        {
            var orBytes = Encoding.Unicode.GetBytes(LlaveEncripcion);
            var text = Convert.ToBase64String(orBytes);
            return text;
        }

        public string GetViEncripcion()
        {
            var orBytes = Encoding.Unicode.GetBytes(ViEncripcion);
            var text = Convert.ToBase64String(orBytes);
            return text;
        }

        public void SetCadenaEncriptada(string texto)
        {
            var orBytes = Convert.FromBase64String(texto);
            var text = Encoding.Unicode.GetString(orBytes);
            CadenaEncriptada = text;
        }

        public void SetLlaveEncripcion(string texto)
        {
            var orBytes = Convert.FromBase64String(texto);
            var text = Encoding.Unicode.GetString(orBytes);
            LlaveEncripcion = text;
        }

        public void SetVi(string texto)
        {
            var orBytes = Convert.FromBase64String(texto);
            var text = Encoding.Unicode.GetString(orBytes);
            ViEncripcion = text;
        }
    }
}