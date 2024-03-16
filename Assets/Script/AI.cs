using UnityEngine.Profiling;

public class AI
{
    // 19*19 tiene un total de 19*15*2 + 15*15*2 posibilidades de los Cinco fichas seguidas
    const int MaxFiveChainCount = 1020;

    //posibilidades del jugador
    bool[,,] playerTable = new bool[Board.CROSSCOUNT, Board.CROSSCOUNT, MaxFiveChainCount];

    //posibilidades IA
    bool[,,] conputerTable = new bool[Board.CROSSCOUNT, Board.CROSSCOUNT, MaxFiveChainCount];

    //Registre todos los números consecutivos posibles de dos jugadores. -1 significa que nunca se podrán lograr 5 cuentas consecutivas.
    int[,] win = new int[2, MaxFiveChainCount];

    //Registre la puntuación de cada cuadrícula.
    
    int[,] cGrades = new int[Board.CROSSCOUNT, Board.CROSSCOUNT];
    int[,] pGrades = new int[Board.CROSSCOUNT, Board.CROSSCOUNT];

    //grabar tablero 
    int[,] board = new int[Board.CROSSCOUNT, Board.CROSSCOUNT];

    int cGrade, pGrade;
    int iCount, m, n;
    int mat, nat, mde, nde;

    public int[,] BoardData
    {
        get { return board; }
        set { board = value; }
    }

    public AI()
    {
        for (int i = 0; i < Board.CROSSCOUNT; i++)
        {
            for (int j = 0; j < Board.CROSSCOUNT; j++)
            {
                pGrades[i, j] = 0;
                cGrades[i, j] = 0;
                board[i, j] = 0;
            }
        }
        //board[9, 9] = 1;

        // Recorrer los pasos de todas las situaciones quíntuples posibles.
        //Vertical 
        for (int i = 0; i < Board.CROSSCOUNT; i++)
        {
            for (int j = 0; j < Board.CROSSCOUNT - 4; j++)
            {
                for (int k = 0; k < 5; k++)
                {
                    playerTable[j + k, i, iCount] = true;
                    conputerTable[j + k, i, iCount] = true;
                }

                iCount++;
            }
        }

        //horizontal 
        for (int i = 0; i < Board.CROSSCOUNT; i++)
        {
            for (int j = 0; j < Board.CROSSCOUNT - 4; j++)
            {
                for (int k = 0; k < 5; k++)
                {
                    playerTable[i, j + k, iCount] = true;
                    conputerTable[i, j + k, iCount] = true;
                }

                iCount++;
            }
        }

        // oblicuo derecho
        for (int i = 0; i < Board.CROSSCOUNT - 4; i++)
        {
            for (int j = 0; j < Board.CROSSCOUNT - 4; j++)
            {
                for (int k = 0; k < 5; k++)
                {
                    playerTable[j + k, i + k, iCount] = true;
                    conputerTable[j + k, i + k, iCount] = true;
                }

                iCount++;
            }
        }

        // oblicuo izquierdo
        for (int i = 0; i < Board.CROSSCOUNT - 4; i++)
        {
            for (int j = Board.CROSSCOUNT - 1; j >= 4; j--)
            {
                for (int k = 0; k < 5; k++)
                {
                    playerTable[j - k, i + k, iCount] = true;
                    conputerTable[j - k, i + k, iCount] = true;
                }

                iCount++;
            }
        }

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < MaxFiveChainCount; j++)
            {
                win[i, j] = 0;
            }
        }

        iCount = 0;
    }

    void CalcScore()
    {
        cGrade = 0;
        pGrade = 0;
        board[m, n] = 1;//ubicación de la computadora     

        for (int i = 0; i < MaxFiveChainCount; i++)
        {
            if (conputerTable[m, n, i] && win[0, i] != -1)
            {
                win[0, i]++;//Cargue el número actual de posibles piezas consecutivas para las cinco piezas consecutivas de piedra blanca.
            }

            if (playerTable[m, n, i])
            {
                playerTable[m, n, i] = false;
                win[1, i] = -1;
            }
        }
    }

    void CalcCore()
    {
        //Recorre todas las coordenadas del tablero. 
        for (int i = 0; i < Board.CROSSCOUNT; i++)
        {
            for (int j = 0; j < Board.CROSSCOUNT; j++)
            {
                //Los puntos de recompensa de manchas solares para esta coordenada se restablecen a cero.
                pGrades[i, j] = 0;

                //Comprueba donde no se han colocado piezas de ajedrez
                if (board[i, j] == 0)
                {
                    //Recorre todos los valores de las piedras negras en los puntos posibles del tablero de ajedrez y suma los puntos de bonificación correspondientes a los puntos de colocación.
                    for (int k = 0; k < MaxFiveChainCount; k++)
                    {
                        if (playerTable[i, j, k])
                        {
                            switch (win[1, k])
                            {
                                case 1://UNAS FICHA 
                                    pGrades[i, j] += 5;
                                    break;
                                case 2://2 FICHAS 
                                    pGrades[i, j] += 50;
                                    break;
                                case 3://3 FICHAS  
                                    pGrades[i, j] += 180;
                                    break;
                                case 4://4 PIEZAS  
                                    pGrades[i, j] += 400;
                                    break;
                            }
                        }
                    }

                    cGrades[i, j] = 0;//Los puntos de bonificación para la pieza blanca en esta coordenada se restablecen a cero. 
                    if (board[i, j] == 0)//BUSCA donde no se han colocado piezas de TABLERO 
                    {
                        //Recorre todos los valores de las piezas blancas en los puntos posibles en el tablero de ajedrez y suma los puntos de bonificación correspondientes a los puntos de colocación.
                        for (int k = 0; k < MaxFiveChainCount; k++)
                        {
                            if (conputerTable[i, j, k])
                            {
                                switch (win[0, k])
                                {
                                    case 1://一连子  
                                        cGrades[i, j] += 5;
                                        break;
                                    case 2: //两连子  
                                        cGrades[i, j] += 52;
                                        break;
                                    case 3://三连子  
                                        cGrades[i, j] += 130;
                                        break;
                                    case 4: //四连子  
                                        cGrades[i, j] += 10000;
                                        break;
                                }
                            }
                        }

                    }


                }
            }
        }

    }

    void SetPlayerPiece(int playerX, int playerY)
    {
        int m = playerX;
        int n = playerY;

        if (board[m, n] == 0)
        {
            board[m, n] = 2;

            for (int i = 0; i < MaxFiveChainCount; i++)
            {
                if (playerTable[m, n, i] && win[1, i] != -1)
                {
                    win[1, i]++;
                }
                if (conputerTable[m, n, i])
                {
                    conputerTable[m, n, i] = false;
                    win[0, i] = -1;
                }
            }
        }
    }

    //  Cálculo de IA, el punto por el que el jugador debe atravesar
    public void ComputerDo(int playerX, int playerY, out int finalX, out int finalY)
    {
        SetPlayerPiece(playerX, playerY);

        CalcCore();

        for (int i = 0; i < Board.CROSSCOUNT; i++)
        {
            for (int j = 0; j < Board.CROSSCOUNT; j++)
            {
                //BUSCAlos pesos máximos de las piedras blancas y negras que se pueden colocar en el tablero de ajedrez, y BUSCA los mejores puntos de colocación para cada una.
                if (board[i, j] == 0)
                {
                    if (cGrades[i, j] >= cGrade)
                    {
                        cGrade = cGrades[i, j];
                        mat = i;
                        nat = j;
                    }

                    if (pGrades[i, j] >= pGrade)
                    {
                        pGrade = pGrades[i, j];
                        mde = i;
                        nde = j;
                    }

                }
            }
        }

        //Si el peso del mejor lugar para la piedra blanca es mayor que el peso del mejor lugar para la piedra negra, entonces el mejor lugar de la computadora es el mejor lugar para la piedra blanca; de lo contrario, ocurre lo contrario.
        if (cGrade >= pGrade)
        {
            m = mat;
            n = nat;
        }
        else
        {
            m = mde;
            n = nde;
        }


        CalcScore();

        finalX = m;
        finalY = n;
    }

    //¿Es posible regresar a este lugar de una vez?
    public bool HasChess(int x, int y)
    {
        return board[x, y] != 0;
    }
}

