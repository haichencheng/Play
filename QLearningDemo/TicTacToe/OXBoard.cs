using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe
{
    public class OXBoard : IBoard
    {
        public OXBoard()
        {
            Squares = new Square[9]
        {
            new Square('1',0),
            new Square('2',1),
            new Square('3',2),
            new Square('4',3),
            new Square('5',4),
            new Square('6',5),
            new Square('7',6),
            new Square('8',7),
            new Square('9',8)
        };
            //an alternative (slower?) way to initialise Squares[]
            //for (int i = 0; i < Squares.Length; i++)
            //{
            //    Squares[i] = new Square { Content = (i + 1).ToString()[0], BoardIndex = i };
            //}
            Lines = new List<Line>
            {
                 {new Line(Squares[0],Squares[1],Squares[2]) },//0
                 {new Line(Squares[3],Squares[4],Squares[5]) },//1
                 {new Line(Squares[6],Squares[7],Squares[8]) },//2
                 {new Line(Squares[0],Squares[3],Squares[6]) },//3
                 {new Line(Squares[1],Squares[4],Squares[7]) },//4
                 {new Line(Squares[2],Squares[5],Squares[8]) },//5
                 {new Line(Squares[0],Squares[4],Squares[8]) },//6
                 {new Line(Squares[6],Squares[4],Squares[2]) }//7

            };
           
            CornerLines = new int[,] {
                {0,3,0 },//line 0 and line 3 share  corner square 0
                {0,5,2 },
                {2,3,6},
                {2,5,8 }
            };

        }
        public void Reset()
        {
            for (int i = 0; i < Squares.Length; i++)
            {
                Squares[i].Content = (i + 1).ToString()[0];
            }
        }
        public int[,] CornerLines { get; protected set; }
        private Square[] Squares;
        public List<Line> Lines { get; protected set; }
        //Declare an Indexer so can use something like board[i] to access a Square
        public Square this[int index]
        {
            get { return Squares[index]; }
            set { Squares[index] = value; }

        }
        public int BoardToState()
        {
            int id = 0;
            for (int i = 0; i < Squares.Length; i++)
            {
                int value = 0;
                if (!Squares[i].IsEmpty)
                {
                    value = Squares[i].Content == 'X' ? 1 : 2;
                    id += (int)(value * Math.Pow(3, Squares[i].BoardIndex));
                }
            }
                return id;

        }

        public int[] BoardToIntArray()
        {
            int[] s = new int[9];
            for (int i = 0; i < Squares.Length; i++)
            {
                if (!Squares[i].IsEmpty)
                {
                    s[i] = Squares[i].Content == 'X' ? 1 : 2;
                }
                  
            }
            return s;
        }
            //debugging aid
            public char[] BoardToCharArray()
        {
            char[] c = new char[9]; 
            for (int i = 0; i < Squares.Length; i++)
            {
               
                    c[i] = Squares[i].Content;
            }
            return c;
        }

        public IBoard StateToBoard(int s)
        {
            int state = s;
            char[] c = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var board = new OXBoard();
            for (int i = board.Squares.Length-1; i >=0; i--)
            {
                
                int value =(int)(state/ Math.Pow(3, i));
                if(value!=0)
                {
                    board[i].Content = value == 1 ? 'X' : 'O';
                    c[i] = board[i].Content;
                    state -= (int)(value * Math.Pow(3, i));
                }
              
              
            }
            string boardAsString = c.ToString();
            return board;

        }
     


        public int GetOccupiedSquaresCount()
        {
            return Squares.Count(s => !s.IsEmpty);
        }
        public IEnumerable<int> GetUnOccupiedSquaresIndexes()
        {
            return Squares.Where(s => s.IsEmpty).Select(s => s.BoardIndex);
        }

        #region Debugging
        public bool Compare(IBoard test)
        {
            for (int i = 0; i < Squares.Length; i++)
            {
                if (Squares[i].Content != test[i].Content)
                    return false;
            }
            return true;
        }

        public IBoard Clone()
        {
            OXBoard newBoard = new OXBoard();
            for (int i = 0; i < 9; i++)
            {
                newBoard[i].Content = Squares[i].Content;
            }
            return newBoard;
        }


        public void Initialize(char[] squareContents)
        {
            for (int i = 0; i < 9; i++)
            {
                Squares[i].Content = squareContents[i];
                Squares[i].BoardIndex = i;
            }
        }
        #endregion

    }
}
