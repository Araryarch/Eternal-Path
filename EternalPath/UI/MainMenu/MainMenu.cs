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
      this.BackgroundImage = Image.FromFile(EternalPath.Path.Map.Get("background.png"));
      this.BackgroundImageLayout = ImageLayout.Stretch;
      this.StartPosition = FormStartPosition.CenterScreen;
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;

      Label titleLabel = new Label
      {
        Text = "Eternal Path",
        Font = new Font("Arial", 24, FontStyle.Bold),
        ForeColor = Color.White,
        BackColor = Color.Transparent,
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
        BackColor = Color.Black,
        TextAlign = ContentAlignment.MiddleCenter,
        Padding = new Padding(10),
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
      exitButton.Location = new Point((this.Width - exitButton.Width) / 2, 550);
      exitButton.Click += ExitButton_Click;
      this.Controls.Add(exitButton);
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
}