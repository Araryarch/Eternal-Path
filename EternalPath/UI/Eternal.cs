using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace EternalPath
{
    public partial class MainMenuForm : Form
    {
        private SoundPlayer _sound;
        public MainMenuForm()
        {
            InitializeComponent();

            _sound = new SoundPlayer(EternalPath.Path.Sound.Get("background.wav"));
            _sound.PlayLooping();
        }

        private void InitializeComponent()
        {
            this.Text = "Eternal Path - Main Menu";
            this.Size = new Size(1200, 700);
            this.Icon = new Icon(EternalPath.Path.Icon.Get("favicon.ico"));
            this.BackColor = Color.FromArgb(20, 30, 50);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label titleLabel = new Label
            {
                Text = "Eternal Path",
                Font = new Font("Arial", 24, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true
            };
            titleLabel.Location = new Point((this.Width - titleLabel.PreferredWidth) / 2, 100);
            this.Controls.Add(titleLabel);

            Button startButton = new Button
            {
                Text = "START GAME",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Size = new Size(200, 50),
                BackColor = Color.FromArgb(50, 150, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            startButton.Location = new Point((this.Width - startButton.Width) / 2, 200);
            startButton.Click += StartButton_Click;
            this.Controls.Add(startButton);

            Label instructionsLabel = new Label
            {
                Text = "Controls:\nArrow Keys / WASD - Move\nSpace / Up - Jump (Double Jump)\nLeft Click - Attack\nRight Click - Dash\n\nCollect coins and defeat enemies!\nAvoid falling or losing all health!",
                Font = new Font("Arial", 12),
                ForeColor = Color.LightGray,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            instructionsLabel.Location = new Point((this.Width - instructionsLabel.PreferredWidth) / 2, 300);
            this.Controls.Add(instructionsLabel);

            Button exitButton = new Button
            {
                Text = "EXIT",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Size = new Size(100, 30),
                BackColor = Color.FromArgb(150, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            exitButton.Location = new Point((this.Width - exitButton.Width) / 2, 500);
            exitButton.Click += ExitButton_Click;
            this.Controls.Add(exitButton);

            this.Paint += MainMenuForm_Paint;
        }

        private void MainMenuForm_Paint(object sender, PaintEventArgs e)
        {
            LinearGradientBrush brush = new LinearGradientBrush(
                new Rectangle(0, 0, this.Width, this.Height),
                Color.FromArgb(20, 30, 50),
                Color.FromArgb(40, 60, 90),
                LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(brush, 0, 0, this.Width, this.Height);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            var gameForm = new GameForm();
            gameForm.ShowDialog();
            this.Show();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

    public partial class GameForm : Form
    {
        private Timer _gameTimer;
        private GameManager _gameManager;
        private SoundPlayer _sound;
        private Renderer _renderer;
        private bool[] _keysPressed = new bool[256];
        private Point _mousePosition;

        public GameForm()
        {
            InitializeComponent();

            _sound = new SoundPlayer(EternalPath.Path.Sound.Get("background.wav"));
            _sound.PlayLooping();
        }

        private void InitializeComponent()
        {
            this.Text = "Eternal Path";
            this.Size = new Size(1200, 700);
            this.BackColor = Color.SkyBlue;
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
        }
    }
}