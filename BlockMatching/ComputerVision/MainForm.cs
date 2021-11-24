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
        private bool _showMatrix = false;
        private const int _blockSize = 2;
        private const int _distanceOfBlocks = 15;
        private int[,] _resultMatrix;
        private Bitmap _outputImage = null;

        private FastImage _workImageA;
        private FastImage _workImageB;
        private FastImage _workOut;

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
            _outputImage = new Bitmap(_workImageA.Width, _workImageA.Height);
            _workOut = new FastImage(_outputImage);
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
                    for (int r = j; r < j + _blockSize; r++)
                        for (int c = i; c < i + _blockSize; c++)
                            _resultMatrix[r, c] = Math.Abs(shortestRes.Key);
                }
            }
            _workImageA.Unlock();
            _workImageB.Unlock();

            BuildOutput();
        }

        private void BuildOutput()
        {
            if (_showMatrix)
            {
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

            Color c;
            _workOut.Lock();
            for (int i = 0; i < _resultMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < _resultMatrix.GetLength(1); j++)
                {
                    //if (_resultMatrix[i, j] != 0)
                    //{
                        var px = _resultMatrix[i, j];
                        switch (px)
                        {
                            case 1:
                                c = Color.Black;
                                break;
                            case _distanceOfBlocks:
                                c = Color.White;
                                break;
                            default:
                            {
                                var range = (255 / _distanceOfBlocks);                                    
                                var color = range * px;
                                c = Color.FromArgb(color, color, color);
                            }
                                break;
                        }

                        _workOut.SetPixel(j, i, c);
                    //}
                }
            }
            //outputPanel.BackgroundImage = null;
            //outputPanel.BackgroundImage = _workOut.GetBitMap();
            _workOut.Unlock();

            ApplySmoothFilter(n: 2);
        }

        private void ApplySmoothFilter(int n)
        {
            Color color;

            int div = ((n + 2) * (n + 2));
            int[,] H = new int[3, 3]
            {
                { 1, n, 1},
                { n, n * n, n},
                { 1, n, 1}
            };

            _workOut.Lock();
            for (int r = 1; r <= _workOut.Height - 2; r++)
            {
                for (int c = 1; c <= _workOut.Width - 2; c++)
                {
                    int sr = 0, sg = 0, sb = 0;
                    for (int row = r - 1; row <= r + 1; row++)
                    {
                        for (int col = c - 1; col <= c + 1; col++)
                        {
                            Color i = _workOut.GetPixel(col, row);
                            byte R = i.R;
                            byte G = i.G;
                            byte B = i.B;

                            sr += R * H[row - r + 1, col - c + 1];
                            sg += G * H[row - r + 1, col - c + 1];
                            sb += B * H[row - r + 1, col - c + 1];
                        }
                    }
                    sr /= div;
                    sg /= div;
                    sb /= div;
                    color = Color.FromArgb(sr, sg, sb);

                    _workOut.SetPixel(c, r, color);
                }
            }
            outputPanel.BackgroundImage = null;
            outputPanel.BackgroundImage = _workOut.GetBitMap();
            _workOut.Unlock();
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