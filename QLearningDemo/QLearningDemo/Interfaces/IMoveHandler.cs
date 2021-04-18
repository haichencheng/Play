using TicTacToe;

namespace QLearningDemo
{
    public interface IMoveHandler
    {
        int XwinCount { get; set; }
        GameState GameResult { get; set; }
        bool IsHandled { get; set; }
        string FormatResults();
        MoveHandler HandleDraw(IBoard board, int unoccupiedSquares);
        MoveHandler HandleOwin(IBoard board, int unoccupiedSquares);
        MoveHandler HandleXwin(IBoard board, int unoccupiedSquares);
        void Reset();
    }
}