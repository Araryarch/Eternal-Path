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
        private Player player;
        private Timer gameTimer;

        public Point PlayerLocation => player?.Location ?? Point.Empty;
        public Size PlayerSize => player?.Size ?? Size.Empty;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                player?.Dispose();
                gameTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(Config.WindowWidth, Config.WindowHeight);
            this.Text = "Eternal Path";

            string imagePath = Path.Map.Get("background.png");
            this.BackgroundImage = Image.FromFile(imagePath);
            this.BackgroundImageLayout = ImageLayout.Stretch;

            Size playerSize = new Size(256, 256);
            Point playerLocation = new Point(0, 800);
            player = new Player(playerLocation, playerSize);

            string playerPath = Path.Characters.Get("Idle/Idle.gif");
            player.SetImage(playerPath);

            gameTimer = new Timer();
            gameTimer.Interval = 30;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            this.KeyDown += Eternal_KeyDown;
            this.KeyPress += Eternal_KeyPress;
            this.KeyUp += Eternal_KeyUp;

            this.KeyPreview = true;
            this.DoubleBuffered = true;
        }

        public Eternal()
        {
            InitializeComponent();

            KeyHandler.PlayerLocation = player.Location;
            KeyHandler.PlayerSize = player.Size;
            KeyHandler.Form = this;
        }

        public void SetPlayerImage(string imagePath)
        {
            player?.SetImage(imagePath);
            Invalidate(player?.GetInvalidateRectangle() ?? Rectangle.Empty);
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (player != null)
            {
                player.Location = KeyHandler.PlayerLocation;
                player.FacingLeft = KeyHandler.FacingLeft;

                player.Update();

                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            player?.Draw(e.Graphics);
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