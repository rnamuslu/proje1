using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge;
using AForge.Imaging.Filters;
using AForge.Imaging;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Math.Geometry;
using System.IO.Ports;//RemoveambiguousnessbetweenAForge.ImageandSystem.Drawing.Image
using Point = System.Drawing.Point; //RemoveambiguousnessbetweenAForge.PointandSystem.Drawing.Point

namespace colortracking
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection VideoCapTureDevices;
        private VideoCaptureDevice Finalvideo;
        SerialPort ardino = new  SerialPort();

        public Form1()
        {
            InitializeComponent();
        }
        int R; //Trackbarındeğişkeneleri
        int G;
        int B;


        private void Form1_Load(object sender, EventArgs e)
        {
            VideoCapTureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo VideoCaptureDevice in VideoCapTureDevices)
            {
                comboBox1.Items.Add(VideoCaptureDevice.Name);
            }
            comboBox1.SelectedIndex = 0;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Finalvideo = new VideoCaptureDevice(VideoCapTureDevices[comboBox1.SelectedIndex].MonikerString);
            Finalvideo.NewFrame += new NewFrameEventHandler(Finalvideo_NewFrame);
            Finalvideo.DesiredFrameRate = 20;//saniyede kaç görüntü alsın istiyorsanız. FPS
            Finalvideo.DesiredFrameSize = new Size(800, 300);//görüntü boyutları
            Finalvideo.Start();
        }
    void Finalvideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {

            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            Bitmap image1 = (Bitmap)eventArgs.Frame.Clone();
            PictureBox1.Image = image;



            if (RadioButton1.Checked)
            {

                // createfilter
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                // set centercololandradius
                filter.CenterColor = new RGB(Color.FromArgb(215, 0, 0));
                filter.Radius = 100;
                // applythefilter
                filter.ApplyInPlace(image1);


                nesnebul(image1);

            }

            if (RadioButton2.Checked)
            {

                // createfilter
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                // set centercolorandradius
                filter.CenterColor = new RGB(Color.FromArgb(30, 144, 255));
                filter.Radius = 100;
                // applythefilter
                filter.ApplyInPlace(image1);

                nesnebul(image1);

            }
            if (RadioButton3.Checked)
            {

                // createfilter
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                // set centercolorandradius
                filter.CenterColor = new RGB(Color.FromArgb(0, 215, 0));
                filter.Radius = 100;
                // applythefilter
                filter.ApplyInPlace(image1);

                nesnebul(image1);



            }


            if (RadioButton4.Checked)
            {

                // createfilter
                EuclideanColorFiltering filter = new EuclideanColorFiltering();
                // set centercololandradius
                filter.CenterColor = new RGB(Color.FromArgb(R, G, B));
                filter.Radius = 100;
                // applythefilter
                filter.ApplyInPlace(image1);

                nesnebul(image1);

            }



        }
    public void nesnebul(Bitmap image)
        {
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.MinWidth = 5;
            blobCounter.MinHeight = 5;
            blobCounter.FilterBlobs = true;
            blobCounter.ObjectsOrder = ObjectsOrder.Size;
            BitmapData objectsData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, image.PixelFormat);
            Grayscale grayscaleFilter = new Grayscale(0.2125, 0.7154, 0.0721);
            UnmanagedImage grayImage = grayscaleFilter.Apply(new UnmanagedImage(objectsData));
            image.UnlockBits(objectsData);


            blobCounter.ProcessImage(image);
            Rectangle[] rects = blobCounter.GetObjectsRectangles();
            Blob[] blobs = blobCounter.GetObjectsInformation();
            PictureBox2.Image = image;



            if (RadioButton5.Checked)
            {
                //Tekli cisim Takibi SingleTracking--------

                foreach (Rectangle recs in rects)
                {
                    if (rects.Length > 0)
                    {
                        Rectangle objectRect = rects[0];
                        //Graphics g = Graphics.FromImage(image);
                        Graphics g = PictureBox1.CreateGraphics();
                        using (Pen pen = new Pen(Color.FromArgb(252, 3, 26), 2))
                        {
                            g.DrawRectangle(pen, objectRect);
                        }
                        //Cizdirilen Dikdörtgenin Koordinatlarialiniyor.
                        int objectX = objectRect.X + (objectRect.Width /2);
                        int objectY = objectRect.Y + (objectRect.Height/2 );
                        //  g.DrawString(objectX.ToString() + "X" + objectY.ToString(), new Font("Arial", 12), Brushes.Red, newSystem.Drawing.Point(250, 1));

                        g.Dispose();

                        if (objectX <= 400 && objectY <= 150)
                        {
                            ardino.Write("1");
                        }
                        else if(objectX > 400 && objectX < 800 && objectY <= 150)
                                            {
                            ardino.Write("2");
                        }
                        else if(objectX >= 800 && objectY <= 150)
                                            {
                            ardino.Write("3");
                        }
                        else if(objectX < 400 && objectY > 150 && objectY < 300)
                                            {
                            ardino.Write("4");
                        }
                        else if(objectX > 400 && objectX < 800 && objectY > 150 && objectY < 300)
                                            {
                            ardino.Write("5");
                        }
                        else if(objectX >800 && objectY > 150 && objectY < 300)
                                            {
                            ardino.Write("6");
                        }
                        else if(objectX < 400 && objectY > 300)
                                            {
                            ardino.Write("7");
                        }
                        else if(objectX > 400 && objectX < 800 && objectY > 300)
                                            {
                            ardino.Write("8");
                        }
                        else if(objectX > 800 && objectY > 300)
                                            {
                            ardino.Write("9");
                        }





                    }
                }
            }








        }

        // Converlist of AForge.NET'spointstoarrayof .NETpoints
        private Point[] ToPointsArray(List<IntPoint> points)
        {
            Point[] array = new Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new Point(points[i].X, points[i].Y);
            }

            return array;

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (Finalvideo.IsRunning)
            {
                Finalvideo.Stop();

   

            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (Finalvideo.IsRunning)
            {
                Finalvideo.Stop();

            }

            Application.Exit();

        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            {
                R = trackBar1.Value;
            }

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            {
                G = trackBar2.Value;
            }

        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {

            B = trackBar3.Value;

        }

        private void Button4_Click(object sender, EventArgs e)
        {
            try
            {
                String portName = comboBox2.Text;
                ardino.PortName = portName;
                ardino.BaudRate = 9600;
                ardino.Open();
                toolStripTextBox1.Text = "bağlandı";
            }
            catch (Exception)
            {

                toolStripTextBox1.Text = " Porta bağlanmadı ,uygun portu seçin";
            }

        }

        private void Button5_Click(object sender, EventArgs e)
        {
            try
            {
                ardino.Close();
                toolStripTextBox1.Text = "Port bağlantısı kesildi ";
            }
            catch (Exception)
            {

                toolStripTextBox1.Text = "İlk önce bağlan sonra bağlantıyı kes";
            }
        }
    }

}
   