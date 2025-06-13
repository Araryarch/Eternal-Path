namespace EternalPath
{
  public class Platform
  {
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public const int PLATFORM_HEIGHT = 40;

    public Platform(int x, int y, int width, int height)
    {
      X = x;
      Y = y;
      Width = width;
      Height = height;
    }

    public Rectangle GetBounds()
    {
      return new Rectangle(X, Y, Width, Height);
    }
  }

  public class Coin
  {
    public float X { get; set; }
    public float Y { get; set; }
    public int Width { get; } = 20;
    public int Height { get; } = 20;
    public bool IsActive { get; set; } = true;

    public Coin(float x, float y)
    {
      X = x;
      Y = y;
    }

    public Rectangle GetBounds()
    {
      return new Rectangle((int)X, (int)Y, Width, Height);
    }
  }
}