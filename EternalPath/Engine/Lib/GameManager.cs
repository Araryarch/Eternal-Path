namespace EternalPath
{
  public class GameManager
  {
    public Player Player { get; private set; }
    public List<Platform> Platforms { get; private set; }
    public List<Enemy> Enemies { get; private set; }
    public List<Coin> Coins { get; private set; }
    public int Score { get; set; }
    public int HighScore { get; set; }
    public int CoinsCollected { get; set; }
    public float CameraX { get; set; }
    public bool GameOver { get; set; }
    private readonly Random random;
    private int worldWidth;
    private readonly int screenHeight;
    private const int CHUNK_SIZE = 1000;

    public GameManager(int screenHeight)
    {
      this.screenHeight = screenHeight;
      random = new Random();
      InitializeGame();
    }

    private void InitializeGame()
    {
      Platforms = new List<Platform>();
      Enemies = new List<Enemy>();
      Coins = new List<Coin>();
      Player = new Player(100, 400);
      Score = 0;
      CoinsCollected = 0;
      CameraX = 0;
      GameOver = false;
      worldWidth = 0;
      GenerateInitialWorld();
    }

    public void GenerateInitialWorld()
    {
      for (int x = 0; x < CHUNK_SIZE * 3; x += 120)
      {
        Platforms.Add(new Platform(x, screenHeight - 80, 120, Platform.PLATFORM_HEIGHT));
      }

      GenerateChunk(0);
      GenerateChunk(CHUNK_SIZE);
      GenerateChunk(CHUNK_SIZE * 2);

      worldWidth = CHUNK_SIZE * 3;
    }

    public void GenerateChunk(int startX)
    {
      int platformCount = random.Next(8, 12);
      List<Rectangle> usedSpaces = new List<Rectangle>();

      int[] layerHeights = {
                screenHeight - 200,
                screenHeight - 320,
                screenHeight - 440,
                screenHeight - 560
            };

      for (int i = 0; i < platformCount; i++)
      {
        bool placed = false;
        int attempts = 0;

        while (!placed && attempts < 30)
        {
          int x = startX + random.Next(80, CHUNK_SIZE - 180);
          int layerIndex = random.Next(0, layerHeights.Length);
          int y = layerHeights[layerIndex] + random.Next(-30, 30);
          int width = random.Next(100, 180);

          Rectangle newPlatform = new Rectangle(x, y, width, Platform.PLATFORM_HEIGHT);

          bool tooClose = usedSpaces.Any(rect =>
          {
            int horizontalDistance = Math.Abs(rect.X - x);
            int verticalDistance = Math.Abs(rect.Y - y);
            if (verticalDistance > 80)
              return horizontalDistance < 100;
            else
              return horizontalDistance < 180 || verticalDistance < 60;
          });

          if (!tooClose)
          {
            Platforms.Add(new Platform(x, y, width, Platform.PLATFORM_HEIGHT));
            usedSpaces.Add(newPlatform);
            placed = true;

            if (random.Next(0, 3) == 0)
            {
              Coins.Add(new Coin(x + width / 2 - 10, y - 25));
            }
          }
          attempts++;
        }
      }

      var chunkPlatforms = Platforms.Where(p => p.X >= startX && p.X < startX + CHUNK_SIZE).OrderBy(p => p.X).ToList();

      for (int i = 0; i < chunkPlatforms.Count - 1; i++)
      {
        var currentPlatform = chunkPlatforms[i];
        var nextPlatform = chunkPlatforms[i + 1];

        int horizontalGap = nextPlatform.X - (currentPlatform.X + currentPlatform.Width);
        int verticalGap = Math.Abs(nextPlatform.Y - currentPlatform.Y);

        if (horizontalGap > 180 && verticalGap > 80 && random.Next(0, 2) == 0)
        {
          int bridgeX = currentPlatform.X + currentPlatform.Width + horizontalGap / 2 - 40;
          int bridgeY = Math.Min(currentPlatform.Y, nextPlatform.Y) + Math.Abs(currentPlatform.Y - nextPlatform.Y) / 2;

          Platforms.Add(new Platform(bridgeX, bridgeY, 80, Platform.PLATFORM_HEIGHT));
        }
      }

      var enemyPlatforms = Platforms.Where(p => p.X >= startX && p.X < startX + CHUNK_SIZE && p.Y < screenHeight - 100).ToList();
      int enemyCount = random.Next(2, 4);

      for (int i = 0; i < enemyCount && i < enemyPlatforms.Count; i++)
      {
        var platform = enemyPlatforms[random.Next(enemyPlatforms.Count)];
        Enemies.Add(new Enemy(platform.X + 30, platform.Y - 35, platform.X, platform.X + platform.Width - 60));
      }
    }

    public void Update(bool[] keys, Point mousePosition)
    {
      if (GameOver) return;

      Point worldMousePosition = new Point(mousePosition.X + (int)CameraX, mousePosition.Y);
      Player.Update(keys, Platforms, Enemies, screenHeight, worldMousePosition);

      CameraX = Player.X - screenHeight / 2;

      if (Player.X > worldWidth - CHUNK_SIZE)
      {
        GenerateChunk(worldWidth);
        worldWidth += CHUNK_SIZE;

        Platforms.RemoveAll(p => p.X < CameraX - 300);
        Enemies.RemoveAll(e => e.X < CameraX - 300);
        Coins.RemoveAll(c => c.X < CameraX - 300);
      }

      foreach (var enemy in Enemies.ToList())
      {
        enemy.Update();

        if (Player.IsAttacking && Player.GetAttackBounds().IntersectsWith(enemy.GetBounds()))
        {
          if (enemy.IsActive)
          {
            enemy.TakeDamage();
            if (enemy.Health <= 0)
            {
              enemy.IsActive = false;
              Score += 50;
            }
          }
        }

        if (Player.GetBounds().IntersectsWith(enemy.GetBounds()) && enemy.IsActive)
        {
          if (Player.VelocityY > 0 && Player.Y < enemy.Y && !Player.IsAttacking)
          {
            enemy.TakeDamage();
            if (enemy.Health <= 0)
            {
              enemy.IsActive = false;
              Score += 25;
            }
            Player.VelocityY = -8;
          }
          else if (!Player.IsInvulnerable)
          {
            Player.TakeDamage();
            if (Player.Health <= 0)
            {
              GameOver = true;
              if (Score > HighScore)
                HighScore = Score;
            }
          }
        }
      }

      Enemies.RemoveAll(e => !e.IsActive);

      foreach (var coin in Coins.ToList())
      {
        if (Player.GetBounds().IntersectsWith(coin.GetBounds()) && coin.IsActive)
        {
          coin.IsActive = false;
          CoinsCollected++;
          Score += 10;
        }
      }

      Coins.RemoveAll(c => !c.IsActive);

      int distanceScore = (int)(Player.X / 20);
      if (distanceScore > Score - (CoinsCollected * 10))
      {
        Score = distanceScore + (CoinsCollected * 10);
      }

      if (Player.Y > screenHeight + 100)
      {
        GameOver = true;
        if (Score > HighScore)
          HighScore = Score;
      }
    }

    public void Restart()
    {
      InitializeGame();
    }
  }

}