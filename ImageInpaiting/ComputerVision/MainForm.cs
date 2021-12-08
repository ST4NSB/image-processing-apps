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

        private const int _pointSize = 3;
        private const int _penSize = 3;
        private Color _penColor = Color.Red;
        private const int _thresholdPerPixel = 30;
        private const int _contextSize = 5;
        private const int _searchRadius = 7;

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

                    if (IsPointInPoly(new Point(panelI, panelJ)))
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
            if (pt.X < 0 || pt.X >= panelSource.Width) return false;
            if (pt.Y < 0 || pt.Y >= panelSource.Height) return false;

            return _graphicPath.IsVisible(pt);
        }

        private int GetNormalizedValue(int oldNumber, int oldMax, int newMax, int oldMin = 0, int newMin = 0)
        {
            return (((oldNumber - oldMin) * (newMax - newMin)) / (oldMax - oldMin)) + newMin;
        }

        private bool IsInImageBox(int i, int j)
        {
            if (i < 0 || i >= workImage.Height) return false;
            if (j < 0 || j >= workImage.Height) return false;

            return true;
        }
    }
}