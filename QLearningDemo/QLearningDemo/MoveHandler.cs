using System.Linq;
using System.Text;
using TicTacToe;

namespace QLearningDemo
{
    public  class MoveHandler : IMoveHandler
    {
       
        private int OwinCount;
        private int DrawCount;
        private int MovesCount;
       public int XwinCount { get; set; }
        public bool IsHandled { get; set; }
        public GameState GameResult { get; set; }

        public void Reset()
        {
            XwinCount=OwinCount = DrawCount = MovesCount = 0;
            IsHandled = false;
            GameResult = GameState.InPlay;
        }

     public   MoveHandler HandleXwin(IBoard board,int unoccupiedSquares)
        {
            if (IsHandled) return this;
            if (board.Lines.Any((l) => l.XScore == Constants.WinningScore
                                                           && GameResult == GameState.InPlay))
            {
                // inputOutputSerice.ShowBoard(newBoard);
                XwinCount++;
                MovesCount += unoccupiedSquares + 1;
                GameResult = GameState.Xwin;
                IsHandled = true;

            }
            return this;
        }

        public MoveHandler HandleOwin(IBoard board, int unoccupiedSquares)
        {
            if (IsHandled) return this;
            if (board.Lines.Any((l) => l.OScore == Constants.WinningScore
                                                           && GameResult == GameState.InPlay))
            {
                // inputOutputSerice.ShowBoard(newBoard);
                OwinCount++;
                MovesCount += unoccupiedSquares + 1;
                GameResult = GameState.Owin;
                IsHandled = true;

            }
            return this;
        }

        public MoveHandler HandleDraw(IBoard board, int unoccupiedSquares)
        {
            if (IsHandled) return this;
            if (!board.Lines.Any(l => !l.IsLineOblocked || !l.IsLineXblocked)
                                                          && GameResult == GameState.InPlay)
            {
                MovesCount += Constants.TotalSquares;
                DrawCount++;
                GameResult= GameState.Draw;
                IsHandled = true;
            }
            return this;
        }

        public string FormatResults()
        {
            int totalGames = XwinCount + OwinCount + DrawCount;
            StringBuilder builder = new StringBuilder();
            builder.Append("\r\n")
                   .Append(Constants.TotalWinsX).Append(XwinCount).Append("\n")
                   .Append(Constants.TotalWinsO).Append(OwinCount).Append("\n")
                   .Append(Constants.TotalDraws).Append(DrawCount).Append("\n")
                   .Append(Constants.TotalGames).Append(totalGames).Append("\n")
                   .Append(Constants.TotalMoves).Append(MovesCount).Append("\n");
            return builder.ToString();
        }
    }
    }

