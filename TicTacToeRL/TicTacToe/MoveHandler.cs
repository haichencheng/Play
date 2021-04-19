using System.Linq;

namespace TicTacToe
{
    public class MoveHandler : IMoveHandler
    {
        public bool IsHandled { get; set; }
        private readonly int[] searchOrder; 
        private IGameScorer gameScorer;
        public Move Result; 
        private IBoard board;
        private ILineService lineService;
        public MoveHandler(IBoard board, ILineService lineService, IGameScorer gameScorer)
        {
            this.lineService = lineService;
            searchOrder = new int[] { 1, 0, 2 };
            this.gameScorer = gameScorer;
            ReSet(board);
        }
        public void ReSet(IBoard board)
        {
            this.board = board;
            Result.Index = -1;
            Result.MoveResult = GameState.InPlay;
            IsHandled = false;
            gameScorer.Reset(board);
        }
        public bool IsGameADraw()
        {
            return !board.Lines.Any(l => !l.IsLineOblocked || !l.IsLineXblocked);
        }
        public MoveHandler HandleXWin()
        {
            if (IsHandled) return this;

            if (gameScorer.BestScoreX == Constants.WinOnNextMove)
            {
                //this is a winning Line it has 2 X and 0 O
                lineService.TryFindEmptySquare(gameScorer.BestLineX, out int index);
                Result.Index = index;
                Result.MoveResult= GameState.Xwin;
                IsHandled = true;
            }
            return this;
        }
        public MoveHandler HandleOWinOnNext()
        {
            if (IsHandled) return this;
            if (gameScorer.BestScoreO == Constants.WinOnNextMove)
            {
                //O could win on its next move if the line is not blocked now   
                lineService.TryFindEmptySquare(gameScorer.BestLineO, out int index);
                Result.Index = index;
                 IsHandled = true;
            }
            return this;
        }

        public MoveHandler HandleStrategicMove()
        {
            if (IsHandled) return this;
            int index = gameScorer.GetBestImpactSquare();
            Result.Index = index;
            IsHandled = true;
            return this;
        }
        public MoveHandler HandlePossibleFork()
        {
            if (IsHandled) return this;
            if (lineService.TrySelectAntiForkSquare(board, out int index))
            {
                Result.Index = index;
                IsHandled = true;
            }
            return this;
        }

    }
}
