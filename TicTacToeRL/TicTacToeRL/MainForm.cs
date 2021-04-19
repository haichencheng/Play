using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MatchBoxCore;
using TicTacToe;
using QLearningDemo;

namespace TicTacToeRL
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            AddCells();

            InitializeGame();
        }

        private void InitializeGame()
        {
            InitBoard();

            ConfigureServices(m_ioc);
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


        private void InitBoard()
        {
            m_board[1, 1] = 1;
            m_board[0, 0] = 1;
            m_board[0, 1] = 2;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    var cell = cells[y, x];
                    switch (m_board[y, x])
                    {
                        case 0:
                            cell.Image = null;
                            break;
                        case 1:
                            cell.Image = GetImage(false);
                            break;
                        case 2:
                            cell.Image = GetImage(true);
                            break;
                    }
                }
            }
        }

        private void AddCells()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    cells[y, x] = new System.Windows.Forms.PictureBox();
                    var cell = cells[y, x];
                    this.tableLayoutPanel1.Controls.Add(cell, x, y);

                    cell.Dock = System.Windows.Forms.DockStyle.Fill;
                    cell.Location = new System.Drawing.Point(4 + x * 299, 4 + y * 292);
                    cell.Name = ("pictureCell"+y)+x;
                    cell.Size = new System.Drawing.Size(292, 299);
                    cell.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                    cell.TabIndex = 0;
                    cell.TabStop = false;

                    Tuple<int, int> tuple = new Tuple<int, int>(y, x);
                    cell.Click += (sender, eventArgs) => CellClick(tuple);
                }
            }

            InitBoard();
        }

        private void CellClick(Tuple<int, int> p_tuple)
        {
           // MessageBox.Show($"From y={p_tuple.Item1}, x={p_tuple.Item2}", "Hi");

            m_board[p_tuple.Item1, p_tuple.Item2]++;
            if (m_board[p_tuple.Item1, p_tuple.Item2] > 2)
            {
                m_board[p_tuple.Item1, p_tuple.Item2] = 0;
            }

            var cell = cells[p_tuple.Item1, p_tuple.Item2]; ;
            switch (m_board[p_tuple.Item1, p_tuple.Item2])
            {
                case 0:
                    cell.Image = null;
                    break;
                case 1:
                    cell.Image = GetImage(false);
                    break;
                case 2:
                    cell.Image = GetImage(true);
                    break;
            }
            cell.Refresh();
        }


        private System.Drawing.Image GetImage(bool p_isX)
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));

            Bitmap image = m_imageO;
            if (p_isX)
            {
                image = m_imageX;
            }
            return (System.Drawing.Image)image;
        }

        System.Windows.Forms.PictureBox[,] cells = new System.Windows.Forms.PictureBox[3, 3];
        int[,] m_board = new int[3, 3];
        MatchBox m_ioc = new MatchBox();
        Bitmap m_imageO = new Bitmap("Images/Letter_o.png");
        Bitmap m_imageX = new Bitmap("Images/Letter_x.png");
        private void buttonStart_Click(object sender, EventArgs e)
        {
            var trainingScheduler = m_ioc.Get<ITrainingScheduler>();
            try
            {
                trainingScheduler.ScheduleTraining().Wait();
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}

