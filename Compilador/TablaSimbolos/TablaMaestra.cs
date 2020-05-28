using Compilador.Transversal;

namespace Compilador.TablaSimbolos
{
    public static class TablaMaestra
    {
        public static void SincronizarSimbolo(ComponenteLexico componente)
        {
            if (componente != null)
            {
                if(TablaPalabrasReservadas.ValidarSiEsPalabraReservada(componente) && componente.Tipo!= TipoComponente.DUMMY)
                {
                    componente.Tipo = TipoComponente.PALABRA_RESERVADA;
                }

                switch (componente.Tipo)
                {
                    case TipoComponente.SIMBOLO:

                        //tabla de simbolos completa
                        if (componente.Categoria.Equals(Categoria.IDENTIFICADOR))
                        {
                            TablaSimbolos.Agregar(componente);
                        }
                        // tabla de simbolos resumida
                        else
                        {
                            componente.Tipo = TipoComponente.LITERAL;
                            TablaLiterales.Agregar(componente);
                        }
                        break;
                    case TipoComponente.DUMMY:
                        TablaDummys.Agregar(componente);
                        break;

                    case TipoComponente.PALABRA_RESERVADA:
                        TablaPalabrasReservadas.Agregar(componente);
                        break;
                }
            }
        }
    }
}
