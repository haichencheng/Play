namespace TicTacToe
{
    public interface IMoveHandler
    {
        bool IsHandled { get; set; }

        MoveHandler HandleStrategicMove();
        MoveHandler HandleOWinOnNext();
        MoveHandler HandleXWin();
        bool IsGameADraw();
        void ReSet(IBoard board);
    }
}