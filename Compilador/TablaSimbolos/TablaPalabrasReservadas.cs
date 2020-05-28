using Compilador.Transversal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compilador.TablaSimbolos
{
    public class TablaPalabrasReservadas
    {
        private static Dictionary<string, ComponenteLexico> tabla = new Dictionary<string, ComponenteLexico>();
        private static Dictionary<string, List<ComponenteLexico>> simbolos = new Dictionary<string, List<ComponenteLexico>>();

        public static void inicializar()
        {
            tabla.Add("int", ComponenteLexico.crear("int", Categoria.PALABRA_RESERVADA_ON));
            tabla.Add("on", ComponenteLexico.crear("on", Categoria.PALABRA_RESERVADA_OFF));
            tabla.Add("off", ComponenteLexico.crear("off", Categoria.PALABRA_RESERVADA_OFF));

        }

        public static bool ValidarSiEsPalabraReservada(ComponenteLexico componente)
        {
            if (componente != null && tabla.ContainsKey(componente.Lexema))
            {
                //componente.Tipo = TipoComponente.PALABRA_RESERVADA;
                componente.Categoria = tabla[componente.Lexema].Categoria;
                return true;
            }
            return false;
        }

        private static List<ComponenteLexico> ObtenerSimbolos(string clave)
        {
            if (!simbolos.ContainsKey(clave))
            {
                simbolos.Add(clave, new List<ComponenteLexico>());
            }

            return simbolos[clave];

        }

        public static void Agregar(ComponenteLexico componente)
        {
            if (componente != null && !componente.Lexema.Equals("") && componente.Tipo.Equals(TipoComponente.PALABRA_RESERVADA))
            {
                ObtenerSimbolos(componente.Lexema).Add(componente);
            }
        }

        public static List<ComponenteLexico> ObtenerTodosLosSimbolos()
        {
            return simbolos.Values.SelectMany(componente => componente).ToList();
        }

        public static void Limpiar()
        {
            simbolos.Clear();
        }
    }
}
