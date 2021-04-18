using System;
using System.Collections.Generic;
using TicTacToe;

namespace QLearningDemo
{
    public class Qlearner : IReinforcementLearner
    {
        private readonly Dictionary<GameState, int> rewardsDict;
        public Dictionary<int, (int, double)> FitnessDict { get; private set; }
        private readonly int[] states;
        public void Reset()
        {

            FitnessDict.Clear();
            FitnessDict.Add(0, (0, -1));
        }
        public Qlearner()
        {
            rewardsDict = new Dictionary<GameState, int>()
            {
                {GameState.InPlay,Constants.InPlayReward },
                {GameState.Draw,Constants.DrawReward},
                {GameState.Owin,Constants.OwinReward},
                {GameState.Xwin,Constants.XwinReward}

            };
            FitnessDict = new Dictionary<int, (int, double)>
            {
                { 0, (0,-1) }
            };
            states = new int[12];

        }
        public void SetRewards(int oWin, int xWin, int draw)
        {
            rewardsDict[GameState.Draw] = draw;
            rewardsDict[GameState.Xwin] = xWin;
            rewardsDict[GameState.Owin] = oWin;
        }

        public int[] StateToStatePrimes(int state)
        {
            int existingState = state;
            List<int> statePrimes = new List<int>();
            for (int i = 8; i >= 0; i--)
            {

                int value = (int)(state / Math.Pow(3, i));
                if (value == 0)
                {
                    var sPrime = existingState + (int)(2 * Math.Pow(3, i));
                    statePrimes.Add(sPrime);
                }
                state -= (int)(value * Math.Pow(3, i));
            }

            return statePrimes.ToArray();
        }


        public void Learn(int[] states, GameState gameState)
        {
            double gamma = Constants.Gamma; 
            int currentState = states[0];
            int newState = states[1];
            UpdateFitnessDict(newState, gameState);//add if not present
            (int currentStateCount, double currentStateValue)  = FitnessDict[currentState];
            currentStateCount += 1;
            double alpha = 1 / (double)currentStateCount;
            (int newStateCount, double newStateValue) = FitnessDict[newState];
            if (gameState != GameState.InPlay)
            {
              currentStateValue = currentStateValue + (alpha * (rewardsDict[gameState] - currentStateValue));
                FitnessDict[currentState] = (currentStateCount, currentStateValue);
                return;
            }
            currentStateValue = currentStateValue + alpha * (-1 + (gamma*newStateValue) - currentStateValue);
            FitnessDict[currentState] = (currentStateCount, currentStateValue);
        }

        private void UpdateFitnessDict(int newState, GameState gameState)
        {
            if (!FitnessDict.ContainsKey(newState))
            {
                FitnessDict.Add(newState, (0, rewardsDict[gameState]));
            }
        }


    }
}
