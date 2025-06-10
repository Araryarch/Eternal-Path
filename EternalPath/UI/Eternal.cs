using EternalPath.InputHandlers;
using System;
using System.Collections.Generic;
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
        private List<PictureBox> tiles;

        private int scaledTileWidth = 500;
        private int scaledTileHeight = 128;
        private List<(Image image, Rectangle rect)> tileRects;

        public Point PlayerLocation => player?.Location ?? Point.Empty;
        public Size PlayerSize => player?.Size ?? Size.Empty;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                player?.Dispose();
                gameTimer?.Dispose();

                // Dispose semua tiles
                if (tiles != null)
                {
                    foreach (var tile in tiles)
                    {
                        tile?.Dispose();
                    }
                    tiles.Clear();
                }
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(Config.WindowWidth, Config.WindowHeight);
            this.Text = "Eternal Path";

            // Background
            string imagePath = Path.Map.Get("background.png");
            this.BackgroundImage = Image.FromFile(imagePath);
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // Player
            Size playerSize = new Size(256, 256);
            Point playerLocation = new Point(0, 800);
            player = new Player(playerLocation, playerSize);
            string playerPath = Path.Characters.Get("Idle/Idle.gif");
            player.SetImage(playerPath);

            // Initialize tiles list
            tiles = new List<PictureBox>();
            CreateTiles();

            // Timer
            gameTimer = new Timer();
            gameTimer.Interval = 30;
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            // Events
            this.KeyDown += Eternal_KeyDown;
            this.KeyPress += Eternal_KeyPress;
            this.KeyUp += Eternal_KeyUp;

            this.KeyPreview = true;
            this.DoubleBuffered = true;
        }

        private void CreateTiles()
        {
            string tilePath = Path.Map.Get("tileset-1.png");
            Image tilesetImage = Image.FromFile(tilePath);

            tileRects = new List<(Image, Rectangle)>();
            tiles = new List<PictureBox>(); // Pastikan tiles diinisialisasi
            int tilesNeeded = (int)Math.Ceiling((double)ClientSize.Width / scaledTileWidth) + 1;
            int tileY = ClientSize.Height - scaledTileHeight;

            for (int i = 0; i < tilesNeeded; i++)
            {
                Rectangle rect = new Rectangle(i * scaledTileWidth, tileY, scaledTileWidth, scaledTileHeight);
                tileRects.Add((tilesetImage, rect));

                PictureBox tile = new PictureBox
                {
                    Image = tilesetImage,
                    Size = new Size(scaledTileWidth, scaledTileHeight),
                    Location = new Point(i * scaledTileWidth, tileY),
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    BackColor = Color.Transparent
                };

                this.Controls.Add(tile);
                tiles.Add(tile);
                tile.SendToBack();
            }
        }

        public Eternal()
        {
            InitializeComponent();
            KeyHandler.UpdatePhysics();
            Invalidate();
            KeyHandler.PlayerLocation = player.Location;
            KeyHandler.PlayerSize = player.Size;
            KeyHandler.Form = this;
        }

        public void SetPlayerImage(string imagePath)
        {
            player?.SetImage(imagePath);
            Invalidate(player?.GetInvalidateRectangle() ?? Rectangle.Empty);
        }

        private bool CheckCollisionWithTiles(Rectangle playerHitbox)
        {
            foreach (var tile in tiles)
            {
                if (tile.Bounds.IntersectsWith(playerHitbox))
                {
                    return true;
                }
            }
            return false;
        }

        private Point GetValidPlayerPosition(Point nextLocation)
        {
            Rectangle playerBounds = player.GetHitboxAt(nextLocation);

            if (CheckCollisionWithTiles(playerBounds))
            {
                Point validPosition = nextLocation;

                for (int y = nextLocation.Y; y >= 0; y--)
                {
                    Rectangle testBounds = player.GetHitboxAt(new Point(nextLocation.X, y));

                    if (!CheckCollisionWithTiles(testBounds))
                    {
                        validPosition.Y = y;
                        break;
                    }
                }

                if (validPosition.Y + player.Size.Height > ClientSize.Height)
                {
                    validPosition.Y = ClientSize.Height - player.Size.Height;
                }

                return validPosition;
            }

            return nextLocation;
        }


        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (player != null)
            {
                var nextLocation = KeyHandler.PlayerLocation;

                var validLocation = GetValidPlayerPosition(nextLocation);

                KeyHandler.PlayerLocation = validLocation;

                player.Location = validLocation;
                player.FacingLeft = KeyHandler.FacingLeft;
                player.Update();

                Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (tileRects != null)
            {
                foreach (var tile in tileRects)
                {
                    e.Graphics.DrawImage(tile.image, tile.rect);
                }
            }

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

        public void AddTile(Point location)
        {
            string tilePath = Path.Map.Get("tileset-1.png");
            Image tilesetImage = Image.FromFile(tilePath);

            PictureBox newTile = new PictureBox();
            newTile.Image = tilesetImage;
            newTile.Size = new Size(scaledTileWidth, scaledTileHeight);
            newTile.Location = location;
            newTile.SizeMode = PictureBoxSizeMode.StretchImage;
            newTile.BackColor = Color.Transparent;

            this.Controls.Add(newTile);
            tiles.Add(newTile);
            newTile.SendToBack();
        }

        public void RemoveOffscreenTiles()
        {
            for (int i = tiles.Count - 1; i >= 0; i--)
            {
                var tile = tiles[i];
                if (tile.Right < 0 || tile.Left > ClientSize.Width)
                {
                    this.Controls.Remove(tile);
                    tile.Dispose();
                    tiles.RemoveAt(i);
                }
            }
        }
    }
}