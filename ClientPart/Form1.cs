using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        Socket senderSocket;
        readonly List<Image> imagesGoRigthBoy = new List<Image>();
        readonly List<Image> imagesGoLeftBoy = new List<Image>();

        //string animationNameSpriteBoy = "flatboy"; int countAnimationImgBoy = 15;
        readonly string animationNameSpriteBoy = "cutegirlfiles";
        readonly int countAnimationImgBoy = 20;
        int currentPosBoy = 0;
        bool toRigth = true;
        readonly bool withServ_Dubug = true;
        readonly string serverAddress = "10.10.10.246";
        readonly int serverPort = 11000;
        public Form1()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            if (withServ_Dubug)
            {
                try
                {
                    Task.Run(() => SendMessageFromSocket());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    Console.ReadLine();
                }
            }
            else
                timer1.Start();

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < countAnimationImgBoy; i++)
            {
                var imgRigth = ImageChange.resizeImage($"{Environment.CurrentDirectory}\\..\\..\\{animationNameSpriteBoy}\\Run ({i + 1}).png", new Size(pictureBox1.Width, pictureBox1.Height));
                var imgLeft = new Bitmap(imgRigth);
                imgLeft.RotateFlip(RotateFlipType.RotateNoneFlipX);
                imagesGoRigthBoy.Add(imgRigth);
                imagesGoLeftBoy.Add(imgLeft);
            }

            BackgroundImage = ImageChange.resizeImage($"{Environment.CurrentDirectory}\\..\\..\\background\\field.png", new Size(Width, Height));
            
            if(withServ_Dubug) Hide();
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (toRigth)
            {
                pictureBox1.Image = imagesGoRigthBoy[currentPosBoy];

                pictureBox1.Location = new Point(pictureBox1.Location.X + 15, pictureBox1.Location.Y);
                if (pictureBox1.Location.X > Size.Width-30)
                {
                    if (withServ_Dubug)
                    {
                        timer1.Stop();
                        senderSocket.Send(Encoding.UTF8.GetBytes("Done R"));
                    }
                    else
                        toRigth = false;
                    pictureBox1.Image = null;
                }

            }
            else 
            {
                pictureBox1.Image = imagesGoLeftBoy[currentPosBoy];
                pictureBox1.Location = new Point(pictureBox1.Location.X - 15, pictureBox1.Location.Y);
                if (pictureBox1.Location.X < -300)
                {
                    if (withServ_Dubug)
                    {
                        timer1.Stop();
                        senderSocket.Send(Encoding.UTF8.GetBytes("Done L"));
                    }
                    else
                        toRigth = true;
                        
                    pictureBox1.Image = null;
                    pictureBox1.Location = new Point(-180, pictureBox1.Location.Y);
                }
            }

            currentPosBoy = (currentPosBoy % (countAnimationImgBoy - 1))+1;
        }
        private void SendMessageFromSocket()
        {
            byte[] bytes = new byte[1024];

            IPHostEntry ipHost = Dns.GetHostEntry(serverAddress);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, serverPort);

            senderSocket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            senderSocket.Connect(ipEndPoint);

            int bytesSent = senderSocket.Send(Encoding.UTF8.GetBytes("I'm ready!"));

            while (true)
            {
                int bytesRec = senderSocket.Receive(bytes);
                string msg = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                if (msg != "")
                    if (msg == "R")
                    {
                        toRigth = true;
                        Invoke(new Action(() =>
                        {
                            pictureBox1.Location = new Point(-180, pictureBox1.Location.Y);
                            timer1.Start();
                        }));
                    }
                    else if (msg == "L")
                    {
                        toRigth = false;
                        Invoke(new Action(() =>
                        {
                            pictureBox1.Location = new Point(Size.Width - 30, pictureBox1.Location.Y);
                            timer1.Start();
                        }));
                    }
                    else if (msg == "Start")
                        Invoke(new Action(() => Show()));
            }
        }
    }
}
