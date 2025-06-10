using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace EternalPath
{
  public class Player : IDisposable
  {
    private Image currentImage;
    private Point location;
    private Size size;
    private bool facingLeft;
    private Timer animationTimer;
    private bool disposed = false;

    private float verticalVelocity = 0f;
    private bool isJumping = false;

    private const float Gravity = 5f;
    private const float JumpStrength = -30f;
    private const float MaxFallSpeed = 20f;

    // Harus diatur dari luar (dari Form1)
    public static Form Form1Instance { get; set; }

    public bool IsGrounded
    {
      get
      {
        if (Form1Instance == null) return false;
        int groundLevel = Form1Instance.ClientSize.Height - size.Height;
        return location.Y >= groundLevel;
      }
    }

    public Rectangle GetHitbox()
    {
      int hitboxHeight = 200;
      int marginX = 70;

      return new Rectangle(
        location.X + marginX,
        location.Y + size.Height - hitboxHeight - 50,
        size.Width - 2 * marginX,
        hitboxHeight
      );
    }

    public Rectangle GetHitboxAt(Point newLocation)
    {
      int hitboxHeight = 200;
      int marginX = 70;

      return new Rectangle(
        newLocation.X + marginX,
        newLocation.Y + size.Height - hitboxHeight - 50,
        size.Width - 2 * marginX,
        hitboxHeight
      );
    }

    public Point Location
    {
      get => location;
      set => location = value;
    }

    public Size Size
    {
      get => size;
      set => size = value;
    }

    public bool FacingLeft
    {
      get => facingLeft;
      set => facingLeft = value;
    }

    public Rectangle Bounds => new Rectangle(location, size);

    public Player(Point initialLocation, Size playerSize)
    {
      location = initialLocation;
      size = playerSize;
      facingLeft = false;

      animationTimer = new Timer();
      animationTimer.Interval = 50;
      animationTimer.Tick += AnimationTimer_Tick;
      animationTimer.Start();
    }

    public void SetImage(string imagePath)
    {
      currentImage?.Dispose();
      currentImage = Image.FromFile(imagePath);

      if (ImageAnimator.CanAnimate(currentImage))
      {
        ImageAnimator.Animate(currentImage, OnFrameChanged);
      }
    }

    public void Update()
    {
      // Tambahkan gravitasi jika tidak di tanah
      if (!IsGrounded)
      {
        verticalVelocity += Gravity;
        if (verticalVelocity > MaxFallSpeed)
          verticalVelocity = MaxFallSpeed;

        location.Y += (int)verticalVelocity;
      }
      else
      {
        verticalVelocity = 0;
        isJumping = false;
      }
    }

    public void Jump()
    {
      if (!isJumping && IsGrounded)
      {
        verticalVelocity = JumpStrength;
        isJumping = true;
      }
    }

    public void Draw(Graphics graphics)
    {
      if (currentImage == null) return;

      graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
      graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

      Rectangle destination = new Rectangle(location, size);
      if (facingLeft)
      {
        destination = new Rectangle(location.X + size.Width, location.Y, -size.Width, size.Height);
      }
      graphics.DrawImage(currentImage, destination);

      using (Brush hitboxBrush = new SolidBrush(Color.FromArgb(128, 255, 0, 0)))
      {
        graphics.FillRectangle(hitboxBrush, GetHitbox());
      }
    }

    public void Move(int deltaX, int deltaY)
    {
      location = new Point(location.X + deltaX, location.Y + deltaY);
    }

    public void SetPosition(int x, int y)
    {
      location = new Point(x, y);
    }

    public void SetPosition(Point newLocation)
    {
      location = newLocation;
    }

    public Rectangle GetInvalidateRectangle()
    {
      return new Rectangle(location, size);
    }

    private void AnimationTimer_Tick(object sender, EventArgs e)
    {
      if (currentImage != null && ImageAnimator.CanAnimate(currentImage))
      {
        ImageAnimator.UpdateFrames(currentImage);
      }
    }

    private void OnFrameChanged(object sender, EventArgs e)
    {
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposed)
      {
        if (disposing)
        {
          currentImage?.Dispose();
          animationTimer?.Stop();
          animationTimer?.Dispose();
        }
        disposed = true;
      }
    }

    ~Player()
    {
      Dispose(false);
    }
  }
}
