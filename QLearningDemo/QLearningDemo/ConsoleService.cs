using System;
using System.Collections.Generic;
using TicTacToe;

namespace QLearningDemo
{
    public class ConsoleService : IOutputInputService
    {
        private List<int> FreeSquareNmbers;

        public ConsoleService(IBoard board)
        {
            ResetFreeSquareChoices();
            for (int i = 0; i < Constants.TotalSquares; i++)
            {
                board[i].SquareContentChangedEvent += SquareChangedEventHandler;
            }
        }

        public void DisplayMessage(string msg)
        {
            Console.WriteLine(msg);
        }
        public void ResetFreeSquareChoices()
        {
            FreeSquareNmbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        }
        private void SquareChangedEventHandler(object sender, EventArgs e)
        {
            Square square = sender as Square;
            UpdateInputChoices(square.BoardIndex);
        }
        public void ShowBoard(IBoard board)
        {
            int ColCount = 0;
            Console.Write("\r\n|");
            for (int i = 0; i < Constants.TotalSquares; i++)
            {
                Console.Write(board[i].Content);
                Console.Write('|');
                ColCount++;
                if (ColCount == 3 && i < 8)
                {
                    ColCount = 0;
                    Console.Write("\r\n|");
                }
            }
            Console.WriteLine();
        }
        private void UpdateInputChoices(int boardIndex)
        {
            FreeSquareNmbers.Remove(boardIndex + 1);
        }

        public int GetMoveFromPlayer()
        {
            bool isMoveValid = false;
            int squareIndex = 0;
            while (!isMoveValid)
            {
                Console.WriteLine();
                Console.Write(Constants.PromptInput);
                string cellChoice = Console.ReadKey().KeyChar.ToString();
                //squareNumber will be set to 0 if TryParse fails
                int.TryParse(cellChoice, out int squareNumber);
                if (FreeSquareNmbers.Contains(squareNumber))
                {
                    FreeSquareNmbers.Remove(squareNumber);
                    squareIndex = squareNumber - 1;
                    isMoveValid = true;
                }
            }
            return squareIndex;
        }

        public int GetNumberFromUser(int min, int max)
        {
            int value;
            while (true)
            {
                Console.Write(Constants.EnterAnInteger, min, max);
                string line = Console.ReadLine();
                if (line == Constants.Exit)
                {
                    Environment.Exit(0);
                }
                if (int.TryParse(line, out value))
                {
                    if (value <= max && value >= min)
                    {
                        break;
                    }
                }
            }
            return value;
        }

        public bool GetIsYes(string prompt)
        {

            Console.Write(prompt);
            return Console.ReadKey().Key == ConsoleKey.Y;

        }

        public void OutputGameResult(string msg, IBoard board)
        {

            Console.WriteLine("\r\n" + msg);
            ShowBoard(board);
        }
    }
}
