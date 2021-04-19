using System.Collections.Generic;

namespace TicTacToe
{
    public interface IBoard
    {
        Square this[int index] { get; set; }

        List<Line> Lines { get; }
        IBoard Clone();
        int GetOccupiedSquaresCount();
        IEnumerable<int> GetUnOccupiedSquaresIndexes();
        int[,] CornerLines { get; }
        bool Compare(IBoard test);
        void Reset();
        int BoardToState();
        IBoard StateToBoard(int state);
        char[] BoardToCharArray();
    }
}