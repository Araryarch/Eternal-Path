using System;
using System.Drawing;

namespace EternalPath
{
  public class Enemy
  {
    public PointF Position { get; set; }
    public PointF Velocity { get; set; }
    public SizeF Size { get; set; }
    public bool IsOnGround { get; set; }
    public bool IsFacingRight { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public EnemyType Type { get; set; }
    public EnemyState State { get; set; }

    // AI Properties
    public float PatrolDistance { get; set; }
    public PointF StartPosition { get; set; }
    public float DetectionRange { get; set; }
    public float AttackRange { get; set; }
    public int AttackDamage { get; set; }
    public int AttackCooldown { get; set; }
    public int LastAttackTime { get; set; }

    // Animation
    public int CurrentFrame { get; set; }
    public int AnimationTimer { get; set; }

    // Constants
    public const float MaxSpeed = 2f;
    public const float Friction = 0.8f;

    public Enemy(EnemyType type, float x, float y, float width = 24, float height = 24)
    {
      Type = type;
      Position = new PointF(x, y);
      StartPosition = new PointF(x, y);
      Velocity = new PointF(0, 0);
      Size = new SizeF(width, height);
      IsOnGround = false;
      IsFacingRight = true;
      State = EnemyState.Patrolling;

      // Set properties based on enemy type
      switch (type)
      {
        case EnemyType.Slime:
          Health = MaxHealth = 30;
          PatrolDistance = 100f;
          DetectionRange = 80f;
          AttackRange = 35f;
          AttackDamage = 10;
          AttackCooldown = 60; // 1 second at 60fps
          break;

        case EnemyType.Goblin:
          Health = MaxHealth = 50;
          PatrolDistance = 150f;
          DetectionRange = 120f;
          AttackRange = 40f;
          AttackDamage = 15;
          AttackCooldown = 45;
          break;

        case EnemyType.Skeleton:
          Health = MaxHealth = 80;
          PatrolDistance = 80f;
          DetectionRange = 150f;
          AttackRange = 60f;
          AttackDamage = 20;
          AttackCooldown = 90;
          break;
      }

      CurrentFrame = 0;
      AnimationTimer = 0;
      LastAttackTime = 0;
    }

    public void Update(Player player, int gameTime)
    {
      LastAttackTime++;

      // Update animation
      AnimationTimer++;
      if (AnimationTimer >= 15)
      {
        CurrentFrame = (CurrentFrame + 1) % 3;
        AnimationTimer = 0;
      }

      // AI Behavior
      float distanceToPlayer = GetDistanceToPlayer(player);

      switch (State)
      {
        case EnemyState.Patrolling:
          Patrol();
          if (distanceToPlayer <= DetectionRange)
          {
            State = EnemyState.Chasing;
          }
          break;

        case EnemyState.Chasing:
          ChasePlayer(player);
          if (distanceToPlayer > DetectionRange * 1.5f)
          {
            State = EnemyState.Returning;
          }
          else if (distanceToPlayer <= AttackRange && CanAttack())
          {
            State = EnemyState.Attacking;
            Attack(player);
          }
          break;

        case EnemyState.Attacking:
          // Attack animation/cooldown
          if (LastAttackTime >= AttackCooldown / 2)
          {
            State = EnemyState.Chasing;
          }
          break;

        case EnemyState.Returning:
          ReturnToStart();
          if (GetDistanceToStart() <= 10f)
          {
            State = EnemyState.Patrolling;
          }
          if (distanceToPlayer <= DetectionRange * 0.8f)
          {
            State = EnemyState.Chasing;
          }
          break;
      }

      // Apply friction when on ground
      if (IsOnGround)
      {
        Velocity = new PointF(Velocity.X * Friction, Velocity.Y);
      }

      // Update facing direction
      if (Math.Abs(Velocity.X) > 0.1f)
      {
        IsFacingRight = Velocity.X > 0;
      }
    }

    private void Patrol()
    {
      float distanceFromStart = Math.Abs(Position.X - StartPosition.X);

      if (distanceFromStart >= PatrolDistance)
      {
        // Change direction
        IsFacingRight = Position.X < StartPosition.X;
      }

      // Move in patrol direction
      float moveDirection = IsFacingRight ? 1f : -1f;
      Velocity = new PointF(moveDirection * MaxSpeed * 0.5f, Velocity.Y);
    }

    private void ChasePlayer(Player player)
    {
      float direction = player.Position.X > Position.X ? 1f : -1f;
      Velocity = new PointF(direction * MaxSpeed, Velocity.Y);
    }

    private void ReturnToStart()
    {
      float direction = StartPosition.X > Position.X ? 1f : -1f;
      Velocity = new PointF(direction * MaxSpeed * 0.7f, Velocity.Y);
    }

    private bool CanAttack()
    {
      return LastAttackTime >= AttackCooldown;
    }

    private void Attack(Player player)
    {
      if (CanAttack())
      {
        player.TakeDamage(AttackDamage);
        LastAttackTime = 0;
      }
    }

    private float GetDistanceToPlayer(Player player)
    {
      float dx = player.Position.X - Position.X;
      float dy = player.Position.Y - Position.Y;
      return (float)Math.Sqrt(dx * dx + dy * dy);
    }

    private float GetDistanceToStart()
    {
      float dx = StartPosition.X - Position.X;
      float dy = StartPosition.Y - Position.Y;
      return (float)Math.Sqrt(dx * dx + dy * dy);
    }

    public RectangleF GetBounds()
    {
      return new RectangleF(Position.X, Position.Y, Size.Width, Size.Height);
    }

    public void TakeDamage(int damage)
    {
      Health = Math.Max(0, Health - damage);
    }

    public bool IsDead()
    {
      return Health <= 0;
    }

    public void Draw(Graphics g)
    {
      Color enemyColor = GetEnemyColor();

      // Draw enemy body
      using (Brush brush = new SolidBrush(enemyColor))
      {
        g.FillRectangle(brush, Position.X, Position.Y, Size.Width, Size.Height);
      }

      // Draw eyes
      using (Brush eyeBrush = new SolidBrush(Color.Red))
      {
        float eyeSize = 3;
        float eyeY = Position.Y + 4;

        if (IsFacingRight)
        {
          g.FillEllipse(eyeBrush, Position.X + Size.Width - 8, eyeY, eyeSize, eyeSize);
        }
        else
        {
          g.FillEllipse(eyeBrush, Position.X + 5, eyeY, eyeSize, eyeSize);
        }
      }

      // Draw state indicator (for debugging)
      DrawStateIndicator(g);

      // Draw health bar
      DrawHealthBar(g);
    }

    private Color GetEnemyColor()
    {
      Color baseColor = Type switch
      {
        EnemyType.Slime => Color.Green,
        EnemyType.Goblin => Color.Orange,
        EnemyType.Skeleton => Color.Gray,
        _ => Color.Purple
      };

      // Flash red when attacking
      if (State == EnemyState.Attacking && LastAttackTime < AttackCooldown / 4)
      {
        return Color.Red;
      }

      return baseColor;
    }

    private void DrawStateIndicator(Graphics g)
    {
      string stateText = State switch
      {
        EnemyState.Patrolling => "P",
        EnemyState.Chasing => "C",
        EnemyState.Attacking => "A",
        EnemyState.Returning => "R",
        _ => "?"
      };

      using (Font font = new Font("Arial", 8))
      {
        g.DrawString(stateText, font, Brushes.White, Position.X, Position.Y - 15);
      }
    }

    private void DrawHealthBar(Graphics g)
    {
      if (Health < MaxHealth)
      {
        float barWidth = Size.Width;
        float barHeight = 3;
        float barX = Position.X;
        float barY = Position.Y - 6;

        // Background
        g.FillRectangle(Brushes.Red, barX, barY, barWidth, barHeight);

        // Health
        float healthPercent = (float)Health / MaxHealth;
        g.FillRectangle(Brushes.Green, barX, barY, barWidth * healthPercent, barHeight);
      }
    }
  }

  public enum EnemyType
  {
    Slime,
    Goblin,
    Skeleton
  }

  public enum EnemyState
  {
    Patrolling,
    Chasing,
    Attacking,
    Returning,
    Dead
  }
}