using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleBoard
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            AddCells();
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
                    cell.Location = new System.Drawing.Point(24 + x * 136, 24 + y * 136);
                    cell.Name = "pictureBox11";
                    cell.Size = new System.Drawing.Size(129, 129);
                    cell.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                    cell.TabIndex = 0;
                    cell.TabStop = false;
                    if (0 == (y + x) % 2)
                    {
                        cell.Image = GetImage(false);
                    }
                    else
                    {
                        cell.Image = GetImage(true);
                    }

                    Tuple<int, int> tuple = new Tuple<int, int>(y, x);
                    cell.Click += (sender, eventArgs) => CellClick(tuple);
                }
            }
            cells[1, 1].Image = null;
        }



        private void CellClick(Tuple<int, int> p_tuple)
        {
            // MessageBox.Show($"From y={p_tuple.Item1}, x={p_tuple.Item2}", "Hi");

            m_board[p_tuple.Item1, p_tuple.Item2]++;
            if (m_board[p_tuple.Item1, p_tuple.Item2]>2)
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

            if (p_isX)
            {
                return ((System.Drawing.Image)(resources.GetObject("pictureBox12.Image")));
            }
            return ((System.Drawing.Image)(resources.GetObject("pictureBox11.Image")));
        }


        System.Windows.Forms.PictureBox[,] cells = new System.Windows.Forms.PictureBox[3, 3];
        int[,] m_board = new int[3, 3];
    }


}
