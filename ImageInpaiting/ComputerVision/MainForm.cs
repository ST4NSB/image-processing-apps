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
        private List<Point> _selectedArea;

        private const int _pointSize = 3;
        private const int _penSize = 3;
        private Color _penColor = Color.Red;

        private string sSourceFileName = "";
        private FastImage workImage;
        private Bitmap image = null;

        public MainForm()
        {
            InitializeComponent();
            _selectedArea = new List<Point>();
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();
            sSourceFileName = openFileDialog.FileName;
            panelSource.BackgroundImage = new Bitmap(sSourceFileName);
            image = new Bitmap(sSourceFileName);
            workImage = new FastImage(image);
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

                    byte average = (byte)((R + G + B) / 3);

                    color = Color.FromArgb(average, average, average);

                    workImage.SetPixel(i, j, color);
                }
            }
            panelDestination.BackgroundImage = null;
            panelDestination.BackgroundImage = workImage.GetBitMap();
            workImage.Unlock();
            
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
        }
    }
}