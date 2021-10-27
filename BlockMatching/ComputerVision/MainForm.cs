using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Linq;

namespace ComputerVision
{
    public partial class MainForm : Form
    {
        private const int _blockSize = 8;
        private const int _distanceOfBlocks = 9;
        private int[,] _resultMatrix;

        private FastImage _workImageA;
        private FastImage _workImageB;

        public MainForm()
        {
            InitializeComponent();
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            openFileDialogA.ShowDialog();
            panelSourceA.BackgroundImage = new Bitmap(openFileDialogA.FileName);
            _workImageA = new FastImage(new Bitmap(openFileDialogA.FileName));
            _resultMatrix = new int[_workImageA.Height, _workImageA.Width];
        }

        private void buttonLoadB_Click(object sender, EventArgs e)
        {
            openFileDialogB.ShowDialog();
            panelSourceB.BackgroundImage = new Bitmap(openFileDialogB.FileName);
            _workImageB = new FastImage(new Bitmap(openFileDialogB.FileName));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _workImageA.Lock();
            _workImageB.Lock();
            for (int i = 0; i < _workImageA.Width; i += _blockSize)
            {
                for (int j = 0; j < _workImageA.Height; j += _blockSize)
                {
                    var coordSum = new Dictionary<int, int>();

                    for (int k = 1; k <= _distanceOfBlocks; k++) 
                    {
                        if ((k * _blockSize) + i >= _workImageA.Width)
                            continue;

                        var sum = CalculateSumFor2Blocks(i, j, (k * _blockSize) + i, j);
                        coordSum.Add(k, sum);
                    }

                    for (int k = (0 - _distanceOfBlocks); k <= -1; k++)
                    {
                        if ((k * _blockSize) + i < 0)
                            continue;

                        var sum = CalculateSumFor2Blocks(i, j, (k * _blockSize) + i, j);
                        coordSum.Add(k, sum);
                    }

                    var shortestRes = coordSum.OrderBy(x => x.Value).First();
                    _resultMatrix[j, i] = shortestRes.Key;
                }
            }
            _workImageA.Unlock();
            _workImageB.Unlock();

            for (int i = 0; i < _resultMatrix.GetLength(0); i++)
            {
                var newLine = false;
                for (int j = 0; j < _resultMatrix.GetLength(1); j++)
                    if (_resultMatrix[i, j] != 0)
                    {
                        richTextBox.Text += _resultMatrix[i, j].ToString() + (_resultMatrix[i, j] < 0 ? " " : "  ");
                        newLine = true;
                    }
                if (newLine)
                    richTextBox.Text += "\n";
            }
        }

        private int CalculateSumFor2Blocks(int Ax, int Ay, int Bx, int By)
        {
            int sum = 0;
            for (int colA = Ax, colB = Bx; colA < Ax + _blockSize && colB < Bx + _blockSize; colA++, colB++)
            {
                for (int rowA = Ay, rowB = By; rowA < Ay + _blockSize && rowB < By + _blockSize; rowA++, rowB++)
                {
                    var colorA = _workImageA.GetPixel(colA, rowA);
                    var colorB = _workImageB.GetPixel(colB, rowB);

                    sum += Math.Abs(colorA.R - colorB.R);
                    sum += Math.Abs(colorA.G - colorB.G);
                    sum += Math.Abs(colorA.B - colorB.B);
                }
            }
            return sum;
        }
    }
}