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