using MatchBoxCore;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TicTacToe;

namespace QLearningDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var Ioc = new MatchBox();
            ConfigureServices(Ioc);
            var trainingScheduler = Ioc.Get<ITrainingScheduler>();
            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();
            try
            {
                await trainingScheduler.ScheduleTraining();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine(Constants.TrainingCancelled);
                Console.ReadKey();
                return;
            }
            //stopWatch.Stop();
            Console.WriteLine(Constants.TrainingCompleted);
            var game = Ioc.Get<IGame>();
            game.StartGame();
        }

        private static void ConfigureServices(MatchBox Ioc)
        {
            Ioc.RegisterSingleton<IBoard, OXBoard>();
            Ioc.Register<ILineService, LineService>();
            Ioc.Register<IOutputInputService, ConsoleService>();
            Ioc.Register<IGameScorer, GameScorer>();

            Ioc.RegisterSingleton<IReinforcementLearner, Qlearner>();
            Ioc.Register<IGame, QGame>();
            Ioc.Register<IRandomTrainer, QTrainer>();
            Ioc.Register<IMoveHandler, MoveHandler>();
            Ioc.Register<IInfallibilityTester, InfallibilityTester>();
            Ioc.Register<ITrainingScheduler, TrainingScheduler>();
        }




    }
}
