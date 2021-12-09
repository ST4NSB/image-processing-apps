using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Drawing.Drawing2D;

namespace ComputerVision
{
    public partial class MainForm : Form
    {
        private List<Point> _selectedArea;
        //private Point test;

        private const int _pointSize = 1;
        private const int _penSize = 1;
        private Color _penColor = Color.Red;
        private const int _thresholdPerPixel = 45;
        private const int _thresholdPoints = 3;
        private const int _contextSize = 21;
        private const int _searchRadius = 11;

        private int _noContext = 0;
        private int _noSearchContext = 0;

        private string sSourceFileName = "";
        private FastImage workImage;
        private Bitmap image = null;
        private GraphicsPath _graphicPath;

        public MainForm()
        {
            InitializeComponent();
            _selectedArea = new List<Point>();
            _graphicPath = new GraphicsPath();
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            sSourceFileName = openFileDialog.FileName;
            panelSource.BackgroundImage = new Bitmap(sSourceFileName);
            image = new Bitmap(sSourceFileName);
            workImage = new FastImage(image);

           // test = new Point(workImage.Width / 2, workImage.Height / 2);
        }

        private void buttonGrayscale_Click(object sender, EventArgs e)
        {
            Color color;

            workImage.Lock();
            for (int i = 0; i < workImage.Width; i++)
            {
                for (int j = 0; j < workImage.Height; j++)
                {
                    color = workImage.GetPixel(i, j);
                    byte R = color.R;
                    byte G = color.G;
                    byte B = color.B;

                    var panelI = GetNormalizedValue(i, workImage.Width, panelSource.Width);
                    var panelJ = GetNormalizedValue(j, workImage.Height, panelSource.Height);

                    if (!IsPointInPoly(new Point(panelI, panelJ)))
                    {
                        color = Color.Black;
                        workImage.SetPixel(i, j, color);
                        continue;
                    }
                    
                    color = Color.FromArgb(R, G, B);

                    workImage.SetPixel(i, j, color);
                }
            }
            panelDestination.BackgroundImage = null;
            panelDestination.BackgroundImage = workImage.GetBitMap();
            workImage.Unlock();

            //Graphics g = panelSource.CreateGraphics();

            //g.DrawRectangle(new Pen(_penColor, _penSize), new Rectangle(test.X, test.Y, _pointSize, _pointSize));

            // MessageBox.Show($"{GetNormalizedValue(workImage.Width / 2, workImage.Width, panelSource.Width)}");
        }

        private void InPaitingBttn_Click(object sender, EventArgs e)
        {
            Color color;

            workImage.Lock();
            for (int i = 0; i < workImage.Width; i++)
            {
                for (int j = 0; j < workImage.Height; j++)
                {
                    color = workImage.GetPixel(i, j);

                    var panelX = GetNormalizedValue(i, workImage.Width, panelSource.Width);
                    var panelY = GetNormalizedValue(j, workImage.Height, panelSource.Height);

                    if (IsPointInPoly(new Point(panelX, panelY)))
                    {
                        var contextValidPoints = GetContextListValidPoints(i, j);

                        _noContext += !contextValidPoints.Any() ? 1 : 0;
                        if (!contextValidPoints.Any()) continue;

                        var startX = i - ((_contextSize - 1) / 2);
                        var startY = j - ((_contextSize - 1) / 2);

                        var sadDict = GetInPaintingPoint(contextValidPoints, startX, startY);

                        _noSearchContext += !sadDict.Any() ? 1 : 0;
                        if (!sadDict.Any()) continue;

                        int avgR = (int)sadDict.Average(pt => workImage.GetPixel(pt.Key.x, pt.Key.y).R);
                        int avgG = (int)sadDict.Average(pt => workImage.GetPixel(pt.Key.x, pt.Key.y).G);
                        int avgB = (int)sadDict.Average(pt => workImage.GetPixel(pt.Key.x, pt.Key.y).B);

                        //var middleValue = sadDict.OrderBy(x => x.Value).FirstOrDefault().Key;
                        //color = workImage.GetPixel(middleValue.x, middleValue.y);
                        color = Color.FromArgb(avgR, avgG, avgB);
                        workImage.SetPixel(i, j, color);
                        continue;
                    }

                    workImage.SetPixel(i, j, color);
                }
            }
            panelDestination.BackgroundImage = null;
            panelDestination.BackgroundImage = workImage.GetBitMap();
            workImage.Unlock();

            MessageBox.Show($"Number of no contexts: {_noContext} \nNumber of no search contexts: {_noSearchContext}");
        }

        private Dictionary<(int x, int y), int> GetInPaintingPoint(List<(int x, int y)> contextValidPoints, int startX, int startY)
        {
            var sadDict = new Dictionary<(int x, int y), int>();
            int[] dirX = new int[] { 0, 1, 0, -1 };
            int[] dirY = new int[] { -1, 0, 1, 0 };

            for (int k = 0; k < dirX.Length; k++)
            {
                for (int sr = 0; sr < _searchRadius; sr++)
                {
                    var srX = startX + (dirX[k] + (dirX[k] == 0 ? 0 : dirX[k] > 0 ? sr : -sr));
                    var srY = startY + (dirY[k] + (dirY[k] == 0 ? 0 : dirY[k] > 0 ? sr : -sr));

                    var sadValue = GetSumOfAbsDiff(contextValidPoints, startX, startY, srX, srY);

                    if (sadValue >= _thresholdPerPixel && sadValue < (_thresholdPerPixel * contextValidPoints.Count)
                        && sadDict.Count <= _thresholdPoints)
                    {
                        var middleValueX = srX + ((_contextSize - 1) / 2);
                        var middleValueY = srY + ((_contextSize - 1) / 2);

                        sadDict.Add((middleValueX, middleValueY), sadValue);
                        //return sadDict;
                    }
                }
            }

            return sadDict;
        }

        private int GetSumOfAbsDiff(List<(int x, int y)> contextValidPoints, int srcX, int srcY, int trgX, int trgY)
        {
            int sumSad = 0;
            foreach (var item in contextValidPoints)
            {
                var newTargetX = trgX + item.x;
                var newTargetY = trgY + item.y;
                
                // these verification were already done for src
                if (!IsInImageBox(newTargetX, newTargetY)) return int.MaxValue;
                if (IsPointInPoly(new Point(newTargetX, newTargetY))) return int.MaxValue;

                var newSourceX = srcX + item.x;
                var newSourceY = srcY + item.y;

                var colorSource = workImage.GetPixel(newSourceX, newSourceY);
                var colorTarget = workImage.GetPixel(newTargetX, newTargetY);

                sumSad += Math.Abs(colorSource.R - colorTarget.R);
                sumSad += Math.Abs(colorSource.G - colorTarget.G);
                sumSad += Math.Abs(colorSource.B - colorTarget.B);
            }

            return sumSad;
        }

        private List<(int x, int y)> GetContextListValidPoints(int x, int y)
        {
            var contextList = new List<(int x, int y)>();
            int startContext = (_contextSize - 1) / 2;

            for (int i = x - startContext; i < (x - startContext) + _contextSize; i++)
            {
                for (int j = y - startContext; j < (y - startContext) + _contextSize; j++)
                {
                    if (IsInImageBox(i, j))
                    {
                        var ptX = GetNormalizedValue(i, workImage.Width, panelSource.Width);
                        var ptY = GetNormalizedValue(j, workImage.Height, panelSource.Height);

                        if (!IsPointInPoly(new Point(ptX, ptY)))
                        {
                            var matX = i - (x - startContext);
                            var matY = j - (y - startContext);
                            contextList.Add((matX, matY));
                        }
                    }
                }
            }

            return contextList;
        }

        private void panelSource_Click(object sender, EventArgs e)
        {
            Graphics g = panelSource.CreateGraphics();
            var pointClicked = panelSource.PointToClient(Cursor.Position);

            g.DrawRectangle(new Pen(_penColor, _penSize), new Rectangle(pointClicked.X, pointClicked.Y, _pointSize, _pointSize));

            if (_selectedArea.Count > 0)
            {
                Pen blackPen = new Pen(_penColor, _penSize);                
                g.DrawLine(new Pen(_penColor, _penSize), _selectedArea.Last(), pointClicked);
            }

            _selectedArea.Add(pointClicked);
        }

        private void stopBttn_Click(object sender, EventArgs e)
        {
            Graphics g = panelSource.CreateGraphics();
            g.DrawLine(new Pen(_penColor, _penSize), _selectedArea.First(), _selectedArea.Last());
            _graphicPath.AddPolygon(_selectedArea.ToArray());
        }

        private bool IsPointInPoly(Point pt)
        {
            return _graphicPath.IsVisible(pt);
        }

        private int GetNormalizedValue(int oldNumber, int oldMax, int newMax, int oldMin = 0, int newMin = 0)
        {
            return (((oldNumber - oldMin) * (newMax - newMin)) / (oldMax - oldMin)) + newMin;
        }

        private bool IsInImageBox(int x, int y)
        {
            if (x < 0 || x >= workImage.Width) return false;
            if (y < 0 || y >= workImage.Height) return false;

            return true;
        }
    }
}