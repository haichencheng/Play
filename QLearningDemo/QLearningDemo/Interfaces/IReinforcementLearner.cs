using System.Collections.Generic;
using TicTacToe;

namespace QLearningDemo
{
    public interface IReinforcementLearner
    {
        void Learn(int[] states, GameState result);
        void Reset();
        int[] StateToStatePrimes(int state);
        Dictionary<int, (int, double)> FitnessDict { get; }
        void SetRewards(int oWin, int xWin, int draw);
       
}
}