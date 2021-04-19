using TicTacToe;

namespace QLearningDemo
{
    public interface IOutputInputService
    {
        int GetMoveFromPlayer();
        void OutputGameResult(string msg,IBoard board);
        void ShowBoard(IBoard board);
        bool GetIsYes(string prompt);
       void ResetFreeSquareChoices();
        void DisplayMessage(string msg);


    }
}