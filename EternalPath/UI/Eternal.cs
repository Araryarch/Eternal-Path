using EternalPath.InputHandlers;
using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
namespace EternalPath
{
    public partial class Eternal : Form
    {
        private System.ComponentModel.IContainer components = null;
        private Image playerImage;
        private Point playerLocation;
        private Size playerSize;
        private Timer gameTimer;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                playerImage?.Dispose();
                gameTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(EternalPath.Config.WindowWidth, EternalPath.Config.WindowHeight);
            this.Text = "Eternal Path";
            string imagePath = EternalPath.Path.Map.Get("background.png");
            this.BackgroundImage = Image.FromFile(imagePath);
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // Initialize player properties
            playerSize = new Size(256, 256);
            playerLocation = new Point(0, 700);
            string playerPath = EternalPath.Path.Sprite.Get("Characters/Idle/Idle.gif");
            playerImage = Image.FromFile(playerPath);

            // Initialize game timer
            gameTimer = new Timer();
            gameTimer.Interval = 1000 / 120; // ~120 FPS
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            this.KeyDown += new KeyEventHandler(Eternal_KeyDown);
            this.KeyPress += new KeyPressEventHandler(Eternal_KeyPress);
            this.KeyUp += new KeyEventHandler(Eternal_KeyUp);

            this.KeyPreview = true;
            this.DoubleBuffered = true; // Enable double buffering to reduce flicker
        }

        public Eternal()
        {
            InitializeComponent();
            KeyHandler.PlayerLocation = playerLocation;
            KeyHandler.PlayerSize = playerSize;
            KeyHandler.Form = this;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // Update player position based on key states
            playerLocation = KeyHandler.PlayerLocation; // Sync local playerLocation
            Invalidate(); // Redraw the form
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (playerImage != null)
            {
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.DrawImage(playerImage, new Rectangle(playerLocation, playerSize));
            }
        }

        private void Eternal_KeyDown(object sender, KeyEventArgs e)
        {
            KeyHandler.HandleKeyDown(e);
        }

        private void Eternal_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyHandler.HandleKeyPress(e);
        }

        private void Eternal_KeyUp(object sender, KeyEventArgs e)
        {
            KeyHandler.HandleKeyUp(e);
        }
    }
}