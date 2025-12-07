using System;
using System.Linq;
using System.Collections.Generic;

namespace TicTacToeFinal // ¡Asegúrate de que este sea el nombre de tu proyecto!
{
    public class LogicaJuego
    {
        private char[,] tablero;
        private Random generadorAleatorio = new Random();

        public const char JUGADOR = 'X';
        public const char IA = 'O';

        public LogicaJuego()
        {
            tablero = new char[3, 3];
            InicializarTablero();
        }

        // --- MÉTODOS QUE NO CAMBIAN ---

        public void InicializarTablero()
        {
            char numeroCasilla = '1';
            for (int i = 0; i < 3; i++) for (int j = 0; j < 3; j++) tablero[i, j] = numeroCasilla++;
        }

        public char[,] ObtenerTablero() => tablero;

        public void RealizarMovimiento(int posicion, char simbolo)
        {
            int fila = (posicion - 1) / 3;
            int columna = (posicion - 1) % 3;
            tablero[fila, columna] = simbolo;
        }

        public bool MovimientoValido(int posicion)
        {
            int fila = (posicion - 1) / 3;
            int columna = (posicion - 1) % 3;
            return tablero[fila, columna] != JUGADOR && tablero[fila, columna] != IA;
        }

        public bool EsEmpate()
        {
            foreach (char casilla in tablero) if (casilla != JUGADOR && casilla != IA) return false;
            return true;
        }

        public char RevisarGanador()
        {
            for (int i = 0; i < 3; i++)
            {
                if (tablero[i, 0] == tablero[i, 1] && tablero[i, 1] == tablero[i, 2]) return tablero[i, 0];
                if (tablero[0, i] == tablero[1, i] && tablero[1, i] == tablero[2, i]) return tablero[0, i];
            }
            if (tablero[0, 0] == tablero[1, 1] && tablero[1, 1] == tablero[2, 2]) return tablero[0, 0];
            if (tablero[0, 2] == tablero[1, 1] && tablero[1, 1] == tablero[2, 0]) return tablero[0, 2];
            return ' ';
        }

        // --- ¡¡¡ LA NUEVA Y MEJORADA IA INTELIGENTE !!! ---

        public int ObtenerMovimientoIA()
        {
            // Prioridad 1: ¿Puedo ganar en el siguiente movimiento?
            int movimientoGanador = EncontrarMovimientoClave(IA);
            if (movimientoGanador != 0) return movimientoGanador;

            // Prioridad 2: ¿Necesito bloquear al jugador?
            int movimientoBloqueo = EncontrarMovimientoClave(JUGADOR);
            if (movimientoBloqueo != 0) return movimientoBloqueo;

            // Prioridad 3: Estrategia - Tomar el centro si está libre
            if (MovimientoValido(5)) return 5;

            // Prioridad 4: Estrategia - Tomar una esquina libre
            var esquinas = new List<int> { 1, 3, 7, 9 };
            esquinas = esquinas.OrderBy(x => generadorAleatorio.Next()).ToList(); // Desordenar para variedad
            foreach (var esquina in esquinas)
            {
                if (MovimientoValido(esquina)) return esquina;
            }

            // Prioridad 5: Estrategia - Tomar un lado libre
            var lados = new List<int> { 2, 4, 6, 8 };
            lados = lados.OrderBy(x => generadorAleatorio.Next()).ToList(); // Desordenar
            foreach (var lado in lados)
            {
                if (MovimientoValido(lado)) return lado;
            }

            return 0; // No debería llegar aquí si el tablero no está lleno
        }

        // ¡¡¡ MÉTODO AUXILIAR CORREGIDO Y MEJORADO !!!
        private int EncontrarMovimientoClave(char simbolo)
        {
            var lineas = new List<int[]>
            {
                new[] {1,2,3}, new[] {4,5,6}, new[] {7,8,9}, // Filas
                new[] {1,4,7}, new[] {2,5,8}, new[] {3,6,9}, // Columnas
                new[] {1,5,9}, new[] {3,5,7}  // Diagonales
            };

            foreach (var linea in lineas)
            {
                var simbolosEnLinea = new List<char>
                {
                    tablero[(linea[0] - 1) / 3, (linea[0] - 1) % 3],
                    tablero[(linea[1] - 1) / 3, (linea[1] - 1) % 3],
                    tablero[(linea[2] - 1) / 3, (linea[2] - 1) % 3]
                };

                // La condición ahora es: hay 2 fichas del símbolo que busco Y 1 casilla vacía.
                if (simbolosEnLinea.Count(c => c == simbolo) == 2)
                {
                    int indiceCasillaVacia = simbolosEnLinea.FindIndex(c => c != JUGADOR && c != IA);
                    if (indiceCasillaVacia != -1) // Si se encontró una casilla vacía en la línea
                    {
                        return linea[indiceCasillaVacia]; // Devolver la posición de esa casilla vacía
                    }
                }
            }
            return 0; // No se encontró ninguna jugada clave
        }
    }
}