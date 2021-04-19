using System;
using System.Threading;
using System.Threading.Tasks;

namespace QLearningDemo
{
    public class TrainingScheduler : ITrainingScheduler
    {
        private Progress<int> progress = new Progress<int>();
        private CancellationTokenSource cts = new CancellationTokenSource();
        private IRandomTrainer trainer;
        private IInfallibilityTester tester;
        public TrainingScheduler(IRandomTrainer trainer, IInfallibilityTester tester)
        {
            this.tester = tester;
            this.trainer = trainer;
            progress.ProgressChanged += ReportProgress;
        }
        private void ReportProgress(object sender, int percent)
        {
            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
            {
                Console.Write(Constants.Cancelling);
                cts.Cancel();
            }
            Console.Write(".");
        }
        public async Task ScheduleTraining()
        {
            int runsX = Constants.TrainingRunsX;
            int runsO = Constants.TrainingRunsO;
            int xFirstResult = -1;
            int oFirstResult = -1;
            bool isPlayerOFirst = true;

            Console.WriteLine(Constants.TrainingPressEsc);
            int iterations = 0;
            while (xFirstResult != 0 || oFirstResult != 0)
            {

                if (iterations == Constants.IterationsBeforeReset)
                {
                    iterations = 0;
                    //Resetting the state values and visit counts is not essential
                    //But it improves efficiency where convergence is slow.
                    trainer.Reset();
                }
                try
                {
                    await Task.Run(() => trainer.Train(runsO, isPlayerOFirst, progress, cts.Token));
                    await Task.Run(() => trainer.Train(runsX, !isPlayerOFirst, progress, cts.Token));
                    oFirstResult = tester.RunTester(isPlayerOFirst);
                    xFirstResult = tester.RunTester(!isPlayerOFirst);
                    iterations += 1;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }

            }
        }
        public bool SaveToFile(string filename)
        {
            return trainer.SaveToFile(filename);
        }
    }
}
