namespace EternalPath
{
  public class Enemy
  {
    public float X { get; set; }
    public float Y { get; set; }
    public int Width { get; } = 25;
    public int Height { get; } = 25;
    public bool IsActive { get; set; } = true;
    public int Health { get; private set; }
    public int MaxHealth { get; } = 2;

    private float _velocityX = 1f;
    private float _minX, _maxX;

    public Enemy(float x, float y, float minX, float maxX)
    {
      X = x;
      Y = y;
      _minX = minX;
      _maxX = maxX;
      Health = MaxHealth;
    }

    public void Update()
    {
      if (!IsActive) return;

      X += _velocityX;
      if (X <= _minX || X >= _maxX)
      {
        _velocityX = -_velocityX;
      }
    }

    public void TakeDamage()
    {
      if (IsActive)
      {
        Health--;
      }
    }

    public Rectangle GetBounds()
    {
      return new Rectangle((int)X, (int)Y, Width, Height);
    }
  }

}