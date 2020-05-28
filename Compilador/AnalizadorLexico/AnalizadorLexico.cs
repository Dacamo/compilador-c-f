using Compilador.Clases;
using Compilador.ManejadorErrores;
using Compilador.TablaSimbolos;
using Compilador.Transversal;
using System;
using System.Windows.Forms;

namespace Compilador
{
    public class AnalizadorLexico
    {
        private int NumeroLineaActual;
        private int Puntero;
        private string CaracterActual;
        private Linea lineaActual;
        string lexema;
        bool mostrarcomponete = false;


        public AnalizadorLexico()
        {
            CargarNuevaLinea();
        }

        private void CargarNuevaLinea()
        {
            NumeroLineaActual++;
            lineaActual = Entrada.ObtenerLinea(NumeroLineaActual);
            if (lineaActual.Contenido.Equals("@EOF@"))
            {
                NumeroLineaActual = lineaActual.Numero;
            }

            Puntero = 1;
        }

        private void DevolverPuntero()
        {
            Puntero -= 1;
        }

        public void LeerSiguienteCaracter()
        {
            if (lineaActual.Contenido.Equals("@EOF@"))
            {
                CaracterActual = lineaActual.Contenido;
            }
            else if (Puntero > lineaActual.Contenido.Length)
            {
                CaracterActual = "@FL@";
                Puntero++;
            }
            else
            {
                CaracterActual = lineaActual.Contenido.Substring(Puntero -1, 1);
                Puntero++;
            }
        }

        private void concatenarLexema()
        {
            lexema = lexema + CaracterActual;
        }

        private void limpiarLexema()
        {
            lexema = "";
        }

        private void DevorarEspaciosBlanco()
        {
            while (CaracterActual.Equals(" "))
            {
                LeerSiguienteCaracter();
            }
        }

        public bool EsLetra(string simbolo)
        {

            return Char.IsLetter(simbolo, 0);
        }

        public bool EsDigito(string simbolo)
        {

            return Char.IsDigit(simbolo, 0);
        }

        public bool EsLetraODigito(string simbolo)
        {

            return EsLetra(simbolo) || EsDigito(simbolo);
        }

        public ComponenteLexico Analizar()
        {
            ComponenteLexico componenteLexico = new ComponenteLexico();
            limpiarLexema();
            int estadoActual = 0;
            bool continuarAnalisis = true;
            while (continuarAnalisis)
            {    
                if (estadoActual == 0)
                {
                    LeerSiguienteCaracter();
                    DevorarEspaciosBlanco();

                    if (EsLetra(CaracterActual) || CaracterActual.Equals("$") || CaracterActual.Equals("_"))
                    {
                        estadoActual = 4;
                        concatenarLexema();
                    }
                    else if (EsDigito(CaracterActual))
                    {
                        estadoActual = 1;
                        concatenarLexema();
                    }
                    else if (CaracterActual.Equals("+"))
                    {
                        estadoActual = 5;
                        concatenarLexema();
                    }
                    else if (CaracterActual.Equals("-"))
                    {
                        estadoActual = 6;
                        concatenarLexema();
                    }
                    else if (CaracterActual.Equals("*"))
                    {
                        estadoActual = 7;
                        concatenarLexema();
                    }
                    else if (CaracterActual.Equals("/"))
                    {
                        estadoActual = 8;
                        concatenarLexema();
                    }
                    else if (CaracterActual.Equals("("))
                    {
                        estadoActual = 10;
                        concatenarLexema();
                    }
                    else if (CaracterActual.Equals(")"))
                    {
                        estadoActual = 11;
                        concatenarLexema();
                    }
                    else if (CaracterActual.Equals("@EOF@"))
                    {
                        estadoActual = 12;
                        concatenarLexema();
                    }
                    else if (CaracterActual.Equals("="))
                    {
                        estadoActual = 19;
                        concatenarLexema();
                    }
                    else if (CaracterActual.Equals("<"))
                    {
                        estadoActual = 20;
                        concatenarLexema();
                    }
                    else if (CaracterActual.Equals(">"))
                    {
                        estadoActual = 21;
                        concatenarLexema();
                    }
                    else if (CaracterActual.Equals(":"))
                    {
                        estadoActual = 22;
                        concatenarLexema();
                    }
                    else if (CaracterActual.Equals("!"))
                    {
                        estadoActual = 30;
                        concatenarLexema();
                    }
                    else if (CaracterActual.Equals("@FL@"))
                    {
                        estadoActual = 13;
                    }
                    else
                    {
                        estadoActual = 18;
                    }
                }
                else if (estadoActual == 1)
                {

                    LeerSiguienteCaracter();

                    if (EsDigito(CaracterActual))
                    {
                        estadoActual = 1;
                        concatenarLexema();
                    }
                    else if (CaracterActual.Equals(","))
                    {
                        estadoActual = 2;
                        concatenarLexema();
                    }
                    else
                    {
                        estadoActual = 14;
                    }

                }
                else if (estadoActual == 2)
                {
                    LeerSiguienteCaracter();
                    if (EsDigito(CaracterActual))
                    {
                        estadoActual = 3;
                        concatenarLexema();
                    }
                    else
                    {
                        estadoActual = 17;
                    }
                }
                else if (estadoActual == 3)
                {
                    LeerSiguienteCaracter();
                    if (EsDigito(CaracterActual))
                    {
                        estadoActual = 3;
                        concatenarLexema();
                    }
                    else
                    {
                        estadoActual = 15;
                    }

                }
                else if (estadoActual == 4)
                {
                    LeerSiguienteCaracter();
                    if (EsLetraODigito(CaracterActual) || CaracterActual.Equals("$") || CaracterActual.Equals("_"))
                    {
                        estadoActual = 4;
                        concatenarLexema();
                    }
                    else
                    {
                        estadoActual = 16;
                    }
                }
                else if (estadoActual == 5)
                {
                    continuarAnalisis = false;
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.SUMA, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO;    
                    MensajeRetorno(componenteLexico);
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();

                }
                else if (estadoActual == 6)
                {
                    continuarAnalisis = false;
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.RESTA, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO;
                    MensajeRetorno(componenteLexico);
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();
                }
                else if (estadoActual == 7)
                {
                    continuarAnalisis = false;
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.MULTIPLICACION, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO;
                    MensajeRetorno(componenteLexico);
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();
                }
                else if (estadoActual == 8)
                {
                    LeerSiguienteCaracter();
                    if (CaracterActual.Equals("*"))
                    {
                        estadoActual = 34;
                        concatenarLexema();
                    }
                    else if (CaracterActual.Equals("/"))
                    {
                        estadoActual = 36;
                        concatenarLexema();
                    }
                    else
                    {
                        estadoActual = 33;
                    }

                }
                else if (estadoActual == 9)
                {
                    continuarAnalisis = false;
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.MODULO, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO;
                    MensajeRetorno(componenteLexico);
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();

                }
                else if (estadoActual == 10)
                {
                    continuarAnalisis = false;
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.PARENTESIS_ABRE, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO;
                    MensajeRetorno(componenteLexico);
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();
                }
                else if (estadoActual == 11)
                {
                    continuarAnalisis = false;
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.PARENTESIS_CIERRA, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO;
                    MensajeRetorno(componenteLexico);
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();
                }
                //fin de archivo
                else if (estadoActual == 12)
                {
                    continuarAnalisis = false;
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.EOF, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO; 
                    //TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();
                }
                //cargar nueva linea
                else if (estadoActual == 13)
                {
                    CargarNuevaLinea();
                    limpiarLexema();
                    estadoActual = 0;
                    
                }

                else if (estadoActual == 14)
                {
                    continuarAnalisis = false;
                    DevolverPuntero();
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.ENTERO, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO;
                    MensajeRetorno(componenteLexico);
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();
                }
                else if (estadoActual == 15)
                {
                    continuarAnalisis = false;
                    DevolverPuntero();
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.NUMERO_DECIMAL, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO;
                    MensajeRetorno(componenteLexico);
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();
                }
                else if (estadoActual == 16)
                {
                    continuarAnalisis = false;
                    DevolverPuntero();
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.IDENTIFICADOR, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO;
                    MensajeRetorno(componenteLexico);
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();
                    
                }
                //estado de error
                else if (estadoActual == 17)
                {
                    continuarAnalisis = false;
                    DevolverPuntero();

                    Error error = Error.CrearErrorLexico(
                        lexema,
                        Categoria.NUMERO_DECIMAL, NumeroLineaActual,
                        Puntero - lexema.Length, Puntero - 1,
                        "Numero decimal no válido", "Leí" + CaracterActual + "y esperaba un digito del cero al 9",
                        "asegurese que el caracter que reciba sea un caracter del 0 al 9");

                    GestorErrores.Reportar(error);

                    componenteLexico = new ComponenteLexico();
                    componenteLexico.Categoria = Categoria.NUMERO_DECIMAL;
                    componenteLexico.Lexema = lexema + "0";
                    componenteLexico.NumeroLinea = NumeroLineaActual;
                    componenteLexico.PosicionInicial = Puntero - lexema.Length;
                    componenteLexico.PosicionFinal = Puntero - 1;
                    componenteLexico.Tipo = TipoComponente.DUMMY;
                    MensajeRetorno(componenteLexico);

                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();


                }
                //estado de error
                else if (estadoActual == 18)
                {
                    Error error = Error.CrearErrorLexico(
                        CaracterActual,
                        Categoria.CARACTER_NO_VALIDO, NumeroLineaActual,
                        Puntero - 1, Puntero - 1,
                        "Caracter no reconocido", "Leí" + CaracterActual,
                        "Asegurese que el caracter sera valido");

                    GestorErrores.Reportar(error);

                    throw new Exception("Se ha presentado un error de tipo STOPPER durante el analisis lexico. Por favor verifique la consola de errores");
                }
                else if (estadoActual == 19)
                {
                    continuarAnalisis = false;
                    DevolverPuntero();
                    componenteLexico = new ComponenteLexico();
                    componenteLexico.Categoria = Categoria.IGUAL_QUE;
                    componenteLexico.Lexema = lexema;
                    componenteLexico.NumeroLinea = NumeroLineaActual;
                    componenteLexico.PosicionInicial = Puntero - lexema.Length;
                    componenteLexico.PosicionFinal = Puntero - 1;
                    MensajeRetorno(componenteLexico);
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();
                }
                else if (estadoActual == 20)
                {
                    LeerSiguienteCaracter();
                    if (CaracterActual.Equals(">"))
                    {
                        estadoActual = 23;
                        concatenarLexema();
                    }
                    else if (CaracterActual.Equals("="))
                    {
                        estadoActual = 24;
                        concatenarLexema();
                    }
                    else
                    {
                        estadoActual = 24;
                    }

                }
                else if (estadoActual == 21)
                {
                    LeerSiguienteCaracter();
                    if (CaracterActual.Equals("="))
                    {
                        estadoActual = 26;
                        concatenarLexema();
                    }
                    else
                    {
                        estadoActual = 27;
                    }

                }
                else if (estadoActual == 22)
                {
                    LeerSiguienteCaracter();
                    if (CaracterActual.Equals("="))
                    {
                        estadoActual = 28;
                        concatenarLexema();
                    }
                    else
                    {
                        estadoActual = 29;
                    }
                }
                else if (estadoActual == 23)
                {
                    continuarAnalisis = false;
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.DIFERENTE_QUE, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO;
                    MensajeRetorno(componenteLexico);
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();
                }
                else if (estadoActual == 24)
                {
                    continuarAnalisis = false;
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.MENOR_IGUAL_QUE, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO;
                    MensajeRetorno(componenteLexico);
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();
                }
                else if (estadoActual == 25)
                {
                    continuarAnalisis = false;
                    DevolverPuntero();
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.MENOR_QUE, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO;
                    MensajeRetorno(componenteLexico);
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();
                }
                else if (estadoActual == 26)
                {
                    continuarAnalisis = false;
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.MAYOR_IGUAL_QUE, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO;
                    MensajeRetorno(componenteLexico);
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();
                }
                else if (estadoActual == 27)
                {
                    continuarAnalisis = false;
                    DevolverPuntero();
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.MAYOR_QUE, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO;
                    MensajeRetorno(componenteLexico);
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();
                }
                else if (estadoActual == 28)
                {
                    continuarAnalisis = false;
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.ASIGNACION, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO;
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    MensajeRetorno(componenteLexico);
                    limpiarLexema();
                }
                //estado de error
                else if (estadoActual == 29)
                {
                    continuarAnalisis = false;
                    DevolverPuntero();

                    Error error = Error.CrearErrorLexico(
                        lexema,
                        Categoria.NUMERO_DECIMAL, NumeroLineaActual,
                        Puntero - lexema.Length, Puntero - 1,
                        "Asignación no válida", "Leí" + CaracterActual + " y esperaba =",
                        "asegurese que el caracter que reciba sea =");

                    GestorErrores.Reportar(error);
                }
                else if (estadoActual == 30)
                {
                    LeerSiguienteCaracter();
                    if (CaracterActual.Equals("="))
                    {
                        estadoActual = 31;
                        concatenarLexema();
                    }
                    else
                    {
                        estadoActual = 32;
                    }
                }
                else if (estadoActual == 31)
                {
                    continuarAnalisis = false;
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.DIFERENTE_QUE, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO;
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    MensajeRetorno(componenteLexico);
                    limpiarLexema();
                }
                //estado de error
                else if (estadoActual == 32)
                {
                    continuarAnalisis = false;
                    DevolverPuntero();

                    Error error = Error.CrearErrorLexico(
                        lexema,
                        Categoria.NUMERO_DECIMAL, NumeroLineaActual,
                        Puntero - lexema.Length, Puntero - 1,
                        "Asignación no válida", "Leí" + CaracterActual + " y esperaba *",
                        "asegurese que el caracter que reciba sea *");

                    GestorErrores.Reportar(error);
                }
                else if (estadoActual == 33)
                {
                    continuarAnalisis = false;
                    DevolverPuntero();
                    componenteLexico = ComponenteLexico.crear(lexema, Categoria.DIVISION, NumeroLineaActual, Puntero - lexema.Length, Puntero - 1);
                    componenteLexico.Tipo = TipoComponente.SIMBOLO;
                    MensajeRetorno(componenteLexico);
                    TablaMaestra.SincronizarSimbolo(componenteLexico);
                    limpiarLexema();
                }
                //revisar
                else if (estadoActual == 34)
                {
                    LeerSiguienteCaracter();
                    if (CaracterActual.Equals("@FL@"))
                    {
                        estadoActual = 37;
                        CargarNuevaLinea();
                    }
                    else if (CaracterActual.Equals("*"))
                    {
                        estadoActual = 35;
                        concatenarLexema();
                    }
                    else
                    {
                        estadoActual = 34;
                        concatenarLexema();
                    }
                }
                else if (estadoActual == 35)
                {
                    LeerSiguienteCaracter();
                    if (CaracterActual.Equals("*"))
                    {
                        estadoActual = 35;
                        concatenarLexema();
                    }
                    else if (CaracterActual.Equals("/"))
                    {
                        estadoActual = 0;
                        concatenarLexema();
                    }
                    else
                    {
                        estadoActual = 34;
                        concatenarLexema();
                    }
                }
                else if (estadoActual == 36)
                {
                    LeerSiguienteCaracter();
                    if (CaracterActual.Equals("@FL@"))
                    {
                        estadoActual = 13;
                    }
                    else
                    {
                        estadoActual = 36;
                    }

                }
                //estado para arreglar 
                else if (estadoActual == 37)
                {
                    CargarNuevaLinea();

                    if (CaracterActual.Equals("@FL@"))
                    {
                        estadoActual = 34;
                    }
                }
            }

            return componenteLexico;
        }

        private void MensajeRetorno(ComponenteLexico componente)
        {
            if (mostrarcomponete)
            {
                MessageBox.Show(componente.ToString());
            }
            
        }
    }
}
