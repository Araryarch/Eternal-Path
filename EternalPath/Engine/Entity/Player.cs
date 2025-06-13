namespace EternalPath
{
  public class Player
  {
    public float X { get; set; }
    public float Y { get; set; }
    public float VelocityX { get; set; }
    public float VelocityY { get; set; }
    public float Scale { get; set; } = 3.5f;
    public int Width => (int)(35 * Scale);
    public int Height => (int)(35 * Scale);
    public bool OnGround { get; set; }
    public int Health { get; private set; }
    public int MaxHealth { get; } = 5;
    public bool IsInvulnerable { get; private set; }
    public bool IsAttacking { get; private set; }
    public bool IsDashing { get; private set; }
    public int DashTimer { get; private set; }
    public int DashCooldown { get; private set; }
    public float DashDirection { get; private set; }
    public bool FacingRight { get; private set; }

    private int _jumpCount;
    private int _invulnerabilityTimer;
    private int _attackTimer;
    private bool _jumpKeyWasPressed;
    private Animation _idleAnimation;
    private Animation _runAnimation;
    private Animation _jumpAnimation;
    private Animation _attackAnimation;
    private Animation _dashAnimation;
    private Animation _currentAnimation;

    private const float GRAVITY = 0.5f;
    private const float JUMP_POWER = -12f;
    private const float MOVE_SPEED = 5f;
    private const float MAX_FALL_SPEED = 15f;
    private const float DASH_SPEED = 15f;
    private const int DASH_DURATION = 10;
    private const int DASH_COOLDOWN = 60;
    private const int MAX_JUMPS = 2;
    private const int HITBOX_HORIZONTAL_SHRINK = 50;
    private const int HITBOX_VERTICAL_SHRINK = 50;

    public Player(float x, float y)
    {
      X = x;
      Y = y;
      Health = MaxHealth;
      _jumpCount = 0;
      FacingRight = true;
      _jumpKeyWasPressed = false;
      InitializeAnimations();
      _currentAnimation = _idleAnimation;
    }

    private void InitializeAnimations()
    {
      try
      {
        _idleAnimation = new Animation(new List<Image>
        {
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-idle-00.png")),
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-idle-01.png")),
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-idle-02.png")),
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-idle-03.png"))
        }, 10);

        _runAnimation = new Animation(new List<Image>
        {
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-run-00.png")),
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-run-01.png")),
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-run-02.png")),
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-run-03.png")),
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-run-04.png")),
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-run-05.png"))
        }, 8);

        _jumpAnimation = new Animation(new List<Image>
        {
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-jump-00.png")),
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-jump-01.png")),
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-jump-02.png")),
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-jump-03.png"))
        }, 12, false);

        _attackAnimation = new Animation(new List<Image>
        {
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-attack3-00.png")),
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-attack3-01.png")),
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-attack3-02.png")),
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-attack3-03.png")),
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-attack3-04.png")),
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-attack3-05.png"))
        }, 6, false);

        _dashAnimation = new Animation(new List<Image>
        {
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-slide-00.png")),
          Image.FromFile(EternalPath.Path.Character.Get("adventurer-slide-01.png")),
        }, 6, false);
      }
      catch (FileNotFoundException ex)
      {
        MessageBox.Show($"Sprite file not found: {ex.Message}", "Error");
        _idleAnimation = new Animation(new List<Image>(), 10);
        _runAnimation = new Animation(new List<Image>(), 8);
        _jumpAnimation = new Animation(new List<Image>(), 12, false);
        _attackAnimation = new Animation(new List<Image>(), 6, false);
        _dashAnimation = new Animation(new List<Image>(), 6, false);
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Error loading animations: {ex.Message}", "Error");
      }
    }

    public void Update(bool[] keys, List<Platform> platforms, List<Enemy> enemies, int screenHeight, Point mousePosition)
    {
      if (IsInvulnerable)
      {
        _invulnerabilityTimer--;
        if (_invulnerabilityTimer <= 0)
          IsInvulnerable = false;
      }

      if (IsAttacking)
      {
        _attackTimer--;
        if (_attackTimer <= 0)
        {
          IsAttacking = false;
          _attackAnimation.Reset();
          _currentAnimation = OnGround ? _idleAnimation : _jumpAnimation;
        }
      }

      FacingRight = mousePosition.X > X + Width / 2;

      if (IsDashing)
      {
        DashTimer--;
        VelocityX = DASH_SPEED * DashDirection;
        if (DashTimer <= 0)
        {
          IsDashing = false;
          _dashAnimation.Reset();
          _currentAnimation = OnGround ? _idleAnimation : _jumpAnimation;
        }
      }
      else
      {
        VelocityX = 0;
        if (keys[(int)Keys.Left] || keys[(int)Keys.A])
          VelocityX -= MOVE_SPEED;
        if (keys[(int)Keys.Right] || keys[(int)Keys.D])
          VelocityX += MOVE_SPEED;

        if (VelocityX != 0 && !IsAttacking)
          _currentAnimation = _runAnimation;
        else if (OnGround && !IsAttacking)
          _currentAnimation = _idleAnimation;
      }

      if (DashCooldown > 0)
        DashCooldown--;

      bool jumpKeyPressed = keys[(int)Keys.Space] || keys[(int)Keys.Up] || keys[(int)Keys.W];
      if (jumpKeyPressed && !_jumpKeyWasPressed && _jumpCount < MAX_JUMPS)
      {
        VelocityY = JUMP_POWER;
        _jumpCount++;
        OnGround = false;
        _currentAnimation = _jumpAnimation;
        _jumpAnimation.Reset();
      }
      _jumpKeyWasPressed = jumpKeyPressed;

      VelocityY += GRAVITY;
      if (VelocityY > MAX_FALL_SPEED)
        VelocityY = MAX_FALL_SPEED;

      X += VelocityX;
      Y += VelocityY;

      OnGround = false;
      foreach (var platform in platforms)
      {
        if (GetBounds().IntersectsWith(platform.GetBounds()))
        {
          if (VelocityY > 0 && Y < platform.Y)
          {
            Y = platform.Y - Height;
            VelocityY = 0;
            OnGround = true;
            _jumpCount = 0;
            if (!IsAttacking && !IsDashing)
              _currentAnimation = VelocityX == 0 ? _idleAnimation : _runAnimation;
          }
          else if (VelocityY < 0 && Y > platform.Y)
          {
            Y = platform.Y + platform.Height;
            VelocityY = 0;
          }
          else if (VelocityX != 0)
          {
            if (VelocityX > 0)
              X = platform.X - Width;
            else
              X = platform.X + platform.Width;
          }
        }
      }

      _currentAnimation.Update();
    }

    public void TakeDamage()
    {
      if (!IsInvulnerable)
      {
        Health--;
        IsInvulnerable = true;
        _invulnerabilityTimer = 120;
      }
    }

    public void StartAttack()
    {
      if (!IsAttacking)
      {
        IsAttacking = true;
        _attackTimer = 15;
        _currentAnimation = _attackAnimation;
        _attackAnimation.Reset();
      }
    }

    public void StartDash(bool[] keys)
    {
      if (!IsDashing && DashCooldown <= 0)
      {
        IsDashing = true;
        DashTimer = DASH_DURATION;
        DashCooldown = DASH_COOLDOWN;
        DashDirection = FacingRight ? 1 : -1;
        _currentAnimation = _dashAnimation;
        _dashAnimation.Reset();
      }
    }

    public Rectangle GetBounds()
    {
      return new Rectangle((int)X + HITBOX_HORIZONTAL_SHRINK, (int)Y + HITBOX_VERTICAL_SHRINK * 2, Width - HITBOX_HORIZONTAL_SHRINK * 2, Height - HITBOX_VERTICAL_SHRINK * 2);
    }

    public Rectangle GetAttackBounds()
    {
      int attackX = (int)X + (FacingRight ? Width : -40);
      return new Rectangle(attackX, (int)Y - 10, 40, Height + 20);
    }

    public Image GetCurrentSprite()
    {
      return _currentAnimation.GetCurrentFrame();
    }
  }
}
