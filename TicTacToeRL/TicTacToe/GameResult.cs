namespace TicTacToe
{
    public enum GameState
    {
        InPlay=0,
        Xwin=-10,
        Owin=15,
        Draw=10,
        SearchCompleted=1
    }
    public struct Move
    {
       public int Index;
       public  GameState MoveResult;
    }
}
