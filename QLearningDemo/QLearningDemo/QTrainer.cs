using System;
using System.IO;
using System.Linq;
using System.Threading;
using TicTacToe;

namespace QLearningDemo
{
    public class QTrainer : IRandomTrainer
    {
        private IBoard board;
        private IReinforcementLearner learner;
        private Random rand;
        private readonly double epsilon;
        private readonly int[] states;
        public QTrainer(IBoard board, IReinforcementLearner learner)
        {
            this.board = board;
            this.learner = learner;
            rand = new Random();
            states = Enumerable.Repeat(-1, 12).ToArray();
            epsilon = 0.95;// 0.95;//.9
        }

        public bool SaveToFile(string filename)
        {
            StreamWriter file;
            try
            {
                file = new StreamWriter(filename);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            using ( file = new StreamWriter(filename))
            {
                foreach (var keyVal in learner.FitnessDict)
                {//no need to save the count variable, it's a training aid
                    file.WriteLine($"{keyVal.Key},{keyVal.Value.Item2}");
                }
            }
            return true;
        }
        public void Reset()
        {
            learner.Reset();
        }


        public void LoadDictFromFile(string filename)
        {
            learner.FitnessDict.Clear();
            using (var streamReader = new StreamReader(filename))
            {
                while (streamReader.Peek() >= 0)
                {
                    string line = streamReader.ReadLine();
                    string[] keyVal = line.Split(',');
                    (int count, double value) countValue;
                    int key = int.Parse(keyVal[0]);
                    countValue.count = 0;
                    countValue.value = double.Parse(keyVal[1]);
                    learner.FitnessDict.Add(key, countValue);
                }
            }
        }



        public void Train(int max, bool isOFirst, IProgress<int> progress, CancellationToken ct)
        {
            int maxGames = max;
            int[] states = new int[2];
            char[] Players = isOFirst ? new char[] { 'O', 'X' } : new char[] { 'X', 'O' };
            int totalMoves = 0;
            int totalGames = 0;
            Move move;

            while (totalGames < maxGames)
            {
                move.MoveResult = GameState.InPlay;
                move.Index = -1;
                int moveNumber = 0;
                int currentState = 0;
                int newState = 0;
                board.Reset();
                while (move.MoveResult == GameState.InPlay)
                {
                    char player = Players[moveNumber % 2];
                    move = player == 'X' ? RandMoveX() : RandMoveO(currentState);
                    board[move.Index].Content = player;
                    newState = board.BoardToState();
                    if (move.MoveResult == GameState.InPlay && !board.Lines.Any(l => !l.IsLineOblocked || !l.IsLineXblocked))
                    {
                        move.MoveResult = GameState.Draw;
                    }
                    states[0] = currentState;
                    states[1] = newState;
                    learner.Learn(states, move.MoveResult);
                    currentState = newState;
                    moveNumber++;
                }
                totalMoves += moveNumber - 1;
                totalGames += 1;
                if(totalGames % Constants.ReportingFreq == 0)
                {
                    progress.Report(totalGames / maxGames);
                    ct.ThrowIfCancellationRequested();
                }
            }
        }

        private Move RandMoveO(int currentState)
        {
            int squareIndex = ChooseNextSquare(currentState);
            Move move;
            move.Index = squareIndex;
            move.MoveResult = GameState.InPlay;
            foreach (var line in board.Lines)
            {
                if (line.OScore ==Constants.WinOnNextMove 
                    && line.Squares.Any(s => s.BoardIndex == squareIndex))
                {

                    move.MoveResult = GameState.Owin;
                    break;
                }
            }

            return move;
        }

        private int ChooseNextSquare(int currentState)
        {
            int nextState = -1;
            int bestIndex = -1;
            int[] boardIndexes = board.GetUnOccupiedSquaresIndexes().ToArray();
            double r = rand.NextDouble();
            if (r < epsilon)
            {
                int rnd = rand.Next(boardIndexes.Length);
                return boardIndexes[rnd];

            }
            double maxQ = double.MinValue;
            foreach (var index in boardIndexes)
            {
                nextState = currentState + (int)(2 * Math.Pow(3, index));
                if (!learner.FitnessDict.ContainsKey(nextState))
                {
                    learner.FitnessDict.Add(nextState, (0, -1));
                }
                (int count, double value) = learner.FitnessDict[nextState];
                if (value > maxQ)
                {
                    maxQ =value;
                    bestIndex = index;
                }
            }
            return bestIndex;
        }
        private Move RandMoveX()
        {
            var choices = board.GetUnOccupiedSquaresIndexes().ToArray();
            int r = rand.Next(choices.Length);
            int squareIndex = choices[r];
            Move move;
            move.Index = squareIndex;
            move.MoveResult = GameState.InPlay;
            foreach (var line in board.Lines)
            {
                if (line.XScore == 2 && line.Squares.Any(s => s.BoardIndex == squareIndex))
                {

                    move.MoveResult = GameState.Xwin;
                    break;
                }
            }

            return move;
        }
    }
}