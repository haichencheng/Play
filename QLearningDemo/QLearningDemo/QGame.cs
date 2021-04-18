using System;
using System.Collections.Generic;
using System.Linq;
using TicTacToe;

namespace QLearningDemo
{
    public class QGame : IGame
    {
        private IBoard board;
        private IOutputInputService inputOutputService;
        private IReinforcementLearner learner;
        private readonly Dictionary<GameState, string> GameResultDictionary;
        public QGame(IBoard board, IOutputInputService inputOutputSerice, IReinforcementLearner learner)
        {
            this.board = board;
            this.board.Reset();
            this.inputOutputService = inputOutputSerice;
            this.learner = learner;
            GameResultDictionary = new Dictionary<GameState, string>
            {
                {GameState.Draw,Constants.DrawResult },
                {GameState.Owin,Constants.WinOResult },
                {GameState.Xwin,Constants.WinXResult },
                {GameState.InPlay,Constants.NoResult }
            };
        }
        private int GetBestPrime(int[] primes)
        {
            double best = double.MinValue;
            int bestPrime = -1;
            foreach (var prime in primes)
            {
                (int count, double value) = learner.FitnessDict[prime];
                if (value > best)
                {
                    best = value;
                    bestPrime = prime;
                }

            }
            return bestPrime;
        }
        private Move GetMoveO(int squareIndex, IBoard currentBoard)
        {
            Move move;
            move.Index = squareIndex;
            move.MoveResult = GameState.InPlay;
            foreach (var line in currentBoard.Lines)
            {
                if (line.OScore == Constants.WinOnNextMove && line.Squares.Any(s => s.BoardIndex == squareIndex))
                {

                    move.MoveResult = GameState.Owin;
                    break;
                }
            }

            return move;
        }
        public Move AutoMoveO(IBoard currentBoard)
        {
            int state = currentBoard.BoardToState();
            var primes = learner.StateToStatePrimes(state);
            int bestPrime = GetBestPrime(primes);
            var diff = bestPrime - state;
            int squareIndex = (int)Math.Log(diff, 3);
            return GetMoveO(squareIndex, currentBoard);
        }

        private Move GetMoveX(int squareIndex, IBoard currentBoard)
        {
            Move move;
            move.Index = squareIndex;
            move.MoveResult = GameState.InPlay;
            foreach (var line in currentBoard.Lines)
            {
                if (line.XScore == Constants.WinOnNextMove && line.Squares.Any(s => s.BoardIndex == squareIndex))
                {

                    move.MoveResult = GameState.Xwin;
                    break;
                }
            }

            return move;
        }

        public Move MoveX(IBoard currentBoard)
        {

            int squareIndex = inputOutputService.GetMoveFromPlayer();
            return GetMoveX(squareIndex, currentBoard);
        }

        public Move MoveO(IBoard currentBoard)
        {

            int squareIndex = inputOutputService.GetMoveFromPlayer();
            return GetMoveO(squareIndex, currentBoard);
        }
        public void StartGame()
        {
            this.board.Reset();
            inputOutputService.ResetFreeSquareChoices();
            bool isPlayAgain = true;
            while (isPlayAgain)
            {
                Play();
                isPlayAgain = inputOutputService.GetIsYes(Constants.PromptPlayAgain);
                if (isPlayAgain)
                {
                    board.Reset();
                    inputOutputService.ResetFreeSquareChoices();
                }
            }
        }

        private Move Play()
        {
            inputOutputService.ShowBoard(board);
            char firstPlayer = inputOutputService.GetIsYes(Constants.PromptGoFirst) ? 'X' : 'O';
            char[] Players = firstPlayer == 'O' ? new char[] { 'O', 'X' } : new char[] { 'X', 'O' };
            int moveNumber = 0;
            Move move;
            move.MoveResult = GameState.InPlay;
            move.Index = -1;
            while (move.MoveResult == GameState.InPlay)
            {
                char player = Players[moveNumber % 2];
                move = player == 'X' ? MoveX(board) : AutoMoveO(board);

                board[move.Index].Content = player;
                moveNumber++;
                inputOutputService.ShowBoard(board);
                if (move.MoveResult == GameState.InPlay && !board.Lines.Any(l => !l.IsLineOblocked || !l.IsLineXblocked))
                {
                    move.MoveResult = GameState.Draw;

                }
            }
            string msg = GameResultDictionary[move.MoveResult];
            inputOutputService.OutputGameResult(msg, board);
            return move;
        }
    }
}

