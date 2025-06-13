using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace EternalPath
{
    public partial class GameForm : Form
    {
        private Timer _gameTimer;
        private GameManager _gameManager;
        private SoundPlayer _sound;
        private Renderer _renderer;
        private bool[] _keysPressed = new bool[256];
        private Point _mousePosition;

        private Image _bgImage;
        private bool _bgImageLoadedSuccessfully = false;

        public GameForm()
        {
            InitializeComponent();

            try
            {
                _bgImage = Image.FromFile(EternalPath.Path.Map.Get("background.png"));
                _bgImageLoadedSuccessfully = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load background image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _bgImageLoadedSuccessfully = false;
            }

            _sound = new SoundPlayer(EternalPath.Path.Sound.Get("background.wav"));
            _sound.PlayLooping();
        }

        private void InitializeComponent()
        {
            this.Text = "Eternal Path";
            this.Size = new Size(1000, 700);
            this.Icon = new Icon(EternalPath.Path.Icon.Get("favicon.ico"));
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            this.KeyDown += GameForm_KeyDown;
            this.KeyUp += GameForm_KeyUp;
            this.Paint += GameForm_Paint;
            this.FormClosed += GameForm_FormClosed;
            this.MouseDown += GameForm_MouseDown;
            this.MouseMove += GameForm_MouseMove;
            this.Resize += GameForm_Resize; // Added to handle window resizing

            _gameManager = new GameManager(this.Height);
            _renderer = new Renderer();
            InitializeGameTimer();
        }

        private void InitializeGameTimer()
        {
            _gameTimer = new Timer();
            _gameTimer.Interval = 16; // ~60 FPS
            _gameTimer.Tick += GameTimer_Tick;
            _gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            _gameManager.Update(_keysPressed, _mousePosition);
            Invalidate();
        }

        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            if (_bgImageLoadedSuccessfully)
            {
                // Calculate scaled dimensions to fill the window
                float scaleX = (float)this.ClientSize.Width / _bgImage.Width;
                float scaleY = (float)this.ClientSize.Height / _bgImage.Height;
                float scale = Math.Max(scaleX, scaleY); // Fill the window, may crop
                int scaledWidth = (int)(_bgImage.Width * scale);
                int scaledHeight = (int)(_bgImage.Height * scale);

                // Center the image if scaled larger than the window
                int offsetX = (this.ClientSize.Width - scaledWidth) / 2;
                int offsetY = (this.ClientSize.Height - scaledHeight) / 2;

                Rectangle destRect = new Rectangle(offsetX, offsetY, scaledWidth, scaledHeight);
                Rectangle srcRect = new Rectangle(0, 0, _bgImage.Width, _bgImage.Height);

                e.Graphics.DrawImage(_bgImage, destRect, srcRect, GraphicsUnit.Pixel);
            }
            else
            {
                e.Graphics.Clear(Color.Gray); // Fallback background
            }

            _renderer.Render(e.Graphics, _gameManager, this.Width, this.Height);
        }

        private void GameForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !_gameManager.GameOver)
            {
                _gameManager.Player.StartAttack();
            }
            else if (e.Button == MouseButtons.Right && !_gameManager.GameOver)
            {
                _gameManager.Player.StartDash(_keysPressed);
            }
        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            _keysPressed[(int)e.KeyCode] = true;

            if (_gameManager.GameOver)
            {
                if (e.KeyCode == Keys.R)
                {
                    _gameManager.Restart();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    this.Close();
                }
            }
        }

        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            _keysPressed[(int)e.KeyCode] = false;
        }

        private void GameForm_MouseMove(object sender, MouseEventArgs e)
        {
            _mousePosition = e.Location;
        }

        private void GameForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _gameTimer?.Stop();
            _bgImage?.Dispose();
            _sound?.Dispose();
        }

        private void GameForm_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}