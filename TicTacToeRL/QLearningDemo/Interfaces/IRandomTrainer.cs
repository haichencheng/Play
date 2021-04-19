using System;
using System.Threading;

namespace QLearningDemo
{
    public interface IRandomTrainer
    {
        void Train(int max, bool isOFirst, IProgress<int> progress, CancellationToken ct);
        bool SaveToFile(string filename);
        void LoadDictFromFile(string filename);
        void Reset();
    }
}