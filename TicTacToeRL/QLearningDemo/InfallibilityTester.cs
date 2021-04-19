using System;
using System.Linq;
using System.Text;
using TicTacToe;

namespace QLearningDemo
{
    public class InfallibilityTester : IInfallibilityTester
    {
        private int[] moves;
        private IBoard board;
        private IOutputInputService inputOutputSerice;
        private IReinforcementLearner learner;
        private IMoveHandler moveHandler;

        public InfallibilityTester(IBoard board, IOutputInputService inputOutputSerice,
             IReinforcementLearner QLearner,IMoveHandler moveHandler)
        {
            this.board = board;
            this.inputOutputSerice = inputOutputSerice;
            learner = QLearner;
            this.moveHandler = moveHandler;
        }

        public int RunTester(bool isPlayerOFirst)
        {
            moveHandler.Reset();
            moves = Enumerable.Repeat(-1, 10).ToArray();
            board.Reset();
            PlayRecursiveMove(board, 0, isPlayerOFirst);
            string heading = isPlayerOFirst ? Constants.PlayerOFirst : Constants.PlayerXFirst;
            string msg = heading+moveHandler.FormatResults();
            if (isPlayerOFirst)
            {
                Console.Clear();
                inputOutputSerice.DisplayMessage(Constants.ProgressReport);
            }
            inputOutputSerice.DisplayMessage(msg);
            return moveHandler.XwinCount;
        }

        private GameState PlayRecursiveMove(IBoard board, int depth, bool isPlayerO)
        {
            var unoccupiedSquares = board.GetUnOccupiedSquaresIndexes().ToList();
            IBoard newBoard;
           moveHandler.GameResult = GameState.InPlay;
            moveHandler.IsHandled = false;
            if (!isPlayerO)//player X
            {

                for (int i = 0; i < unoccupiedSquares.Count; i++)
                {
                    moveHandler.GameResult = GameState.InPlay;
                    moveHandler.IsHandled = false;
                   newBoard = board.Clone();
                    newBoard[unoccupiedSquares[i]].Content = 'X';
                    //debugging aid, stores move sequence
                    moves[depth] = unoccupiedSquares[i];
                    moveHandler.HandleXwin(newBoard, unoccupiedSquares.Count)
                               .HandleDraw(newBoard, unoccupiedSquares.Count);
                   
                    if (!moveHandler.IsHandled)
                    {
                        PlayRecursiveMove(newBoard, depth + 1, true);
                    }

                }
                return GameState.SearchCompleted;
            }
            #region PlayerO           
            newBoard = board.Clone();
            var move = AutoMoveO(newBoard);
            newBoard[move.Index].Content = 'O';
            moves[depth] = move.Index;
            moveHandler.HandleOwin(newBoard, unoccupiedSquares.Count)
                       .HandleDraw(newBoard, unoccupiedSquares.Count);
            if (moveHandler.IsHandled)
            {
                return moveHandler.GameResult;
            }
                return PlayRecursiveMove(newBoard, depth + 1, false);
            #endregion
        }

        public Move AutoMoveO(IBoard currentBoard)
        {
 //           char[] c;
//            c = currentBoard.BoardToCharArray();
            int state = currentBoard.BoardToState();
            Move move;
            var primes = learner.StateToStatePrimes(state);
            double best = double.MinValue;
            int bestPrime = int.MinValue;
            foreach (var prime in primes)
            {
                if (!learner.FitnessDict.ContainsKey(prime))
                {
                    learner.FitnessDict.Add(prime, (0, -1));
                }
                (int count, double value) = learner.FitnessDict[prime];
                if (value > best)
                {
                    best = value;
                    bestPrime = prime;
                }

            }
            var diff = bestPrime - state;
            int squareIndex = (int)Math.Log(diff, 3);

            move.Index = squareIndex;
            move.MoveResult = GameState.InPlay;
            foreach (var line in currentBoard.Lines)
            {
                if (line.OScore == 2 && line.Squares.Any(s => s.BoardIndex == squareIndex))
                {

                    move.MoveResult = GameState.Owin;
                    break;
                }
            }
 //           c = currentBoard.BoardToCharArray();
            return move;
        }

        private string FormatResults(int xWins, int draws, int oWins, int movestotal)
        {
            int totalGames = xWins + oWins + draws;
            StringBuilder builder = new StringBuilder();
            builder.Append("\r\n")
                   .Append(Constants.TotalWinsX).Append(xWins).Append("\n")
                   .Append(Constants.TotalWinsO).Append(oWins).Append("\n")
                   .Append(Constants.TotalDraws).Append(draws).Append("\n")
                   .Append(Constants.TotalGames).Append(totalGames).Append("\n")
                   .Append(Constants.TotalMoves).Append(movestotal).Append("\n");
            return builder.ToString();
        }

    }
}
