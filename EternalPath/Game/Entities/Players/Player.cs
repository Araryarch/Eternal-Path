using System;
using System.Drawing;

namespace EternalPath
{
  public class Player
  {
    public PointF Position { get; set; }
    public PointF Velocity { get; set; }
    public SizeF Size { get; set; }
    public bool IsOnGround { get; set; }
    public bool IsFacingRight { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }

    // Animation properties
    public int CurrentFrame { get; set; }
    public int AnimationTimer { get; set; }
    public PlayerState State { get; set; }

    // Constants
    public const float MaxSpeed = 5f;
    public const float JumpPower = -12f;
    public const float Friction = 0.8f;

    public Player(float x, float y, float width = 32, float height = 32)
    {
      Position = new PointF(x, y);
      Velocity = new PointF(0, 0);
      Size = new SizeF(width, height);
      IsOnGround = false;
      IsFacingRight = true;
      Health = 100;
      MaxHealth = 100;
      State = PlayerState.Idle;
      CurrentFrame = 0;
      AnimationTimer = 0;
    }

    public void Update(float deltaTime)
    {
      // Update animation
      AnimationTimer++;
      if (AnimationTimer >= 10) // Change frame every 10 ticks
      {
        CurrentFrame = (CurrentFrame + 1) % 4;
        AnimationTimer = 0;
      }

      // Update state based on movement
      if (Math.Abs(Velocity.X) > 0.1f && IsOnGround)
        State = PlayerState.Running;
      else if (!IsOnGround && Velocity.Y < 0)
        State = PlayerState.Jumping;
      else if (!IsOnGround && Velocity.Y > 0)
        State = PlayerState.Falling;
      else
        State = PlayerState.Idle;

      // Apply friction when on ground
      if (IsOnGround)
      {
        Velocity = new PointF(Velocity.X * Friction, Velocity.Y);
      }
    }

    public void MoveLeft()
    {
      Velocity = new PointF(Math.Max(Velocity.X - 1f, -MaxSpeed), Velocity.Y);
      IsFacingRight = false;
    }

    public void MoveRight()
    {
      Velocity = new PointF(Math.Min(Velocity.X + 1f, MaxSpeed), Velocity.Y);
      IsFacingRight = true;
    }

    public void Jump()
    {
      if (IsOnGround)
      {
        Velocity = new PointF(Velocity.X, JumpPower);
        IsOnGround = false;
      }
    }

    public RectangleF GetBounds()
    {
      return new RectangleF(Position.X, Position.Y, Size.Width, Size.Height);
    }

    public void TakeDamage(int damage)
    {
      Health = Math.Max(0, Health - damage);
    }

    public void Heal(int amount)
    {
      Health = Math.Min(MaxHealth, Health + amount);
    }

    public bool IsDead()
    {
      return Health <= 0;
    }

    public void Draw(Graphics g)
    {
      // Simple colored rectangle for now
      Color playerColor = Color.Blue;
      if (!IsOnGround) playerColor = Color.LightBlue;

      using (Brush brush = new SolidBrush(playerColor))
      {
        g.FillRectangle(brush, Position.X, Position.Y, Size.Width, Size.Height);
      }

      // Draw direction indicator
      using (Brush eyeBrush = new SolidBrush(Color.White))
      {
        float eyeSize = 4;
        float eyeX = IsFacingRight ? Position.X + Size.Width - 8 : Position.X + 4;
        g.FillEllipse(eyeBrush, eyeX, Position.Y + 6, eyeSize, eyeSize);
      }

      // Draw health bar
      DrawHealthBar(g);
    }

    private void DrawHealthBar(Graphics g)
    {
      float barWidth = Size.Width;
      float barHeight = 4;
      float barX = Position.X;
      float barY = Position.Y - 8;

      // Background
      g.FillRectangle(Brushes.Red, barX, barY, barWidth, barHeight);

      // Health
      float healthPercent = (float)Health / MaxHealth;
      g.FillRectangle(Brushes.Green, barX, barY, barWidth * healthPercent, barHeight);

      // Border
      g.DrawRectangle(Pens.Black, barX, barY, barWidth, barHeight);
    }
  }

  public enum PlayerState
  {
    Idle,
    Running,
    Jumping,
    Falling,
    Attacking,
    Hurt
  }
}