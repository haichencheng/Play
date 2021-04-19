namespace QLearningDemo
{
    public static class Constants
    {
        public const string DrawResult= "The Game was a Draw";
        public const string WinXResult = "The Winner is X";
        public const string WinOResult = "The Winner is O";
        public const string NoResult = "The result is undetermined!";
        public const string NoResultFound = "No Result was found!";
        public const string PromptInput = "Choose an empty Square ";
        public const string PromptGoFirst = "Do you wish to go first (y/n)? ";
        public const string PromptPlayAgain = "Do you wish to play again (y/n)? ";
        public const string Cancelling = "Cancelling.. ";
        public const string TrainingPressEsc = "Training press Esc to cancel";
        public const string TrainingCancelled = "Training was cancelled.Press any key to end";
        public const string TrainingCompleted= "The training completed. Starting a game";
        public const string EnterAnInteger = "Please enter an integer between {0} and {1} or 'exit':";
        public const string Exit = "exit";
        public const string PlayerOFirst = "\r\nPlayer O plays First\r\n";
        public const string PlayerXFirst = "\r\nPlayer X plays First\r\n";
        public const string ProgressReport = "Progress Report. Press Esc to cancel\r\n";
        public const string TotalWinsX = "Total Wins for 'X' ";
        public const string TotalWinsO = "Total Wins for 'O' ";
        public const string TotalDraws = "Total Draws        ";
        public const string TotalGames = "Total Games        ";
        public const string TotalMoves = "Total Moves        ";

        public const int TrainingRunsX = 10000;
        public const int TrainingRunsO = 10000;
        public const int IterationsBeforeReset =20;//15;
        public const int WinOnNextMove = 2;
        public const int WinningScore = 3;
        public const int TotalSquares = 9;
        public const int ReportingFreq = 4000;
        public const int InPlayReward = -1;
        public const int DrawReward = 2;// 6;
        public const int OwinReward =10;// 7;
        public const int XwinReward = -30;// -26;// -25;

        public const double Gamma = 0.9;//0.9

    }
}
