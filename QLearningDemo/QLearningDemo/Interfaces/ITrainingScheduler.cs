using System.Threading.Tasks;

namespace QLearningDemo
{
    public interface ITrainingScheduler
    {
        Task ScheduleTraining();
    }
}