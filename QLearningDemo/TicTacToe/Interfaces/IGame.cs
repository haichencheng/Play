namespace TicTacToe
{
    public interface IGame
    {
        Move MoveO(IBoard board);
        Move AutoMoveO(IBoard board);
        Move MoveX(IBoard board);
       void StartGame();
    }
} 