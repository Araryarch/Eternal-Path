using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace EternalPath
{
  public class Renderer
  {
    // Cache fonts dan brushes untuk menghindari pembuatan berulang
    private readonly Font hudFont;
    private readonly Font smallFont;
    private readonly Font tinyFont;
    private readonly Font coinFont;
    private readonly Font heartFont;
    private readonly Font gameOverFont;
    private readonly Font mediumFont;
    private readonly Font restartFont;
    private readonly StringFormat centerFormat;
    private readonly StringFormat leftFormat;
    private readonly Pen orangePen;
    private readonly Pen blackPen;
    private readonly Pen whitePen;
    private readonly Pen darkBluePen;
    private readonly SolidBrush hudBackgroundBrush;
    private readonly SolidBrush gameOverBackgroundBrush;
    private readonly SolidBrush attackBrush;

    // Cache untuk objek yang sering digunakan
    private readonly List<Platform> visiblePlatforms;
    private readonly List<Coin> visibleCoins;
    private readonly List<Enemy> visibleEnemies;

    public Renderer()
    {
      // Inisialisasi font dan brush sekali saja
      hudFont = new Font("Arial", 12, FontStyle.Bold);
      smallFont = new Font("Arial", 10, FontStyle.Bold);
      tinyFont = new Font("Arial", 8);
      coinFont = new Font("Arial", 10, FontStyle.Bold);
      heartFont = new Font("Arial", 12);
      gameOverFont = new Font("Arial", 36, FontStyle.Bold);
      mediumFont = new Font("Arial", 18);
      restartFont = new Font("Arial", 14);

      centerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
      leftFormat = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };

      orangePen = new Pen(Color.Orange, 2);
      blackPen = new Pen(Color.Black, 1);
      whitePen = new Pen(Color.White, 1);
      darkBluePen = new Pen(Color.DarkBlue, 2);

      hudBackgroundBrush = new SolidBrush(Color.FromArgb(180, 0, 0, 0));
      gameOverBackgroundBrush = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
      attackBrush = new SolidBrush(Color.FromArgb(100, 255, 255, 0));

      // Inisialisasi lists untuk caching
      visiblePlatforms = new List<Platform>();
      visibleCoins = new List<Coin>();
      visibleEnemies = new List<Enemy>();
    }

    public void Render(Graphics g, GameManager gameManager, int screenWidth, int screenHeight)
    {
      g.SmoothingMode = SmoothingMode.AntiAlias;
      g.TranslateTransform(-gameManager.CameraX, 0);

      float viewLeft = gameManager.CameraX;
      float viewRight = gameManager.CameraX + screenWidth;

      GetVisiblePlatforms(gameManager.Platforms);
      RenderPlatforms(g);

      GetVisibleCoins(gameManager.Coins, viewLeft, viewRight);
      RenderCoins(g);

      GetVisibleEnemies(gameManager.Enemies, viewLeft, viewRight);
      RenderEnemies(g);

      RenderPlayer(g, gameManager.Player);

      RenderEffects(g, gameManager.Player);

      g.ResetTransform();

      RenderHUD(g, gameManager);

      if (gameManager.GameOver)
      {
        RenderGameOver(g, gameManager, screenWidth, screenHeight);
      }
    }

    private void GetVisiblePlatforms(List<Platform> allPlatforms)
    {
      visiblePlatforms.Clear();
      foreach (var platform in allPlatforms)
      {
        visiblePlatforms.Add(platform);
      }
    }

    private void GetVisibleCoins(List<Coin> allCoins, float viewLeft, float viewRight)
    {
      visibleCoins.Clear();
      foreach (var coin in allCoins)
      {
        if (coin.IsActive && coin.X >= viewLeft && coin.X <= viewRight)
        {
          visibleCoins.Add(coin);
        }
      }
    }

    private void GetVisibleEnemies(List<Enemy> allEnemies, float viewLeft, float viewRight)
    {
      visibleEnemies.Clear();
      foreach (var enemy in allEnemies)
      {
        if (enemy.IsActive && enemy.X >= viewLeft && enemy.X <= viewRight)
        {
          visibleEnemies.Add(enemy);
        }
      }
    }

    private void RenderPlatforms(Graphics g)
    {
      foreach (var platform in visiblePlatforms)
      {
        // Draw dirt for the entire platform
        using (var dirtBrush = new SolidBrush(Color.FromArgb(139, 69, 19))) // SaddleBrown-like color
        {
          g.FillRectangle(dirtBrush, platform.X, platform.Y, platform.Width, platform.Height);
        }

        // Draw base grass layer (12 pixels thick) with varied height
        int baseGrassHeight = 12;
        using (var grassBrush = new SolidBrush(Color.FromArgb(34, 139, 34))) // ForestGreen-like color
        {
          for (int x = platform.X; x < platform.X + platform.Width; x += 3)
          {
            // Vary grass height for organic look
            int varyHeight = (x % 3) switch
            {
              0 => 2,
              1 => 0,
              _ => 1
            };
            g.FillRectangle(grassBrush, x, platform.Y - varyHeight, 3, baseGrassHeight + varyHeight);
          }
        }

        // Draw lighter grass patches for depth
        using (var lightGrassBrush = new SolidBrush(Color.FromArgb(50, 205, 50))) // LimeGreen-like color
        {
          for (int x = platform.X; x < platform.X + platform.Width; x += 4)
          {
            if ((x % 5) == 0 || (x % 7) == 0) // Pseudo-random placement
            {
              int patchHeight = 10 + (x % 4); // Vary height (10-13 pixels)
              g.FillRectangle(lightGrassBrush, x, platform.Y - (x % 3), 3, patchHeight);
            }
          }
        }

        // Draw darker grass tufts extending downward for more green
        using (var darkGrassBrush = new SolidBrush(Color.FromArgb(20, 100, 20))) // Darker green
        {
          for (int x = platform.X; x < platform.X + platform.Width; x += 3)
          {
            if ((x % 4) == 0 || (x % 6) == 0) // More frequent patches
            {
              int tuftHeight = 12 + (x % 5); // Extend into dirt (12-16 pixels)
              g.FillRectangle(darkGrassBrush, x, platform.Y, 2, tuftHeight);
            }
          }
        }

        // Draw dirt speckles on dirt portion (below grass)
        using (var speckleBrush = new SolidBrush(Color.FromArgb(92, 46, 13))) // Darker brown
        {
          int speckleSize = 4;
          for (int x = platform.X; x < platform.X + platform.Width; x += speckleSize * 2)
          {
            for (int y = platform.Y + baseGrassHeight; y < platform.Y + platform.Height; y += speckleSize * 2)
            {
              if ((x + y) % 3 == 0) // Simple pseudo-random pattern
              {
                g.FillRectangle(speckleBrush, x, y, speckleSize, speckleSize);
              }
            }
          }
        }

        // Draw grid lines to emphasize blocky Minecraft style
        using (var gridPen = new Pen(Color.FromArgb(80, 40, 10), 1)) // Very dark brown for grid
        {
          int blockSize = 16; // Mimic Minecraft's 16x16 pixel block size
          for (int x = platform.X; x < platform.X + platform.Width; x += blockSize)
          {
            g.DrawLine(gridPen, x, platform.Y, x, platform.Y + platform.Height);
          }
          for (int y = platform.Y; y < platform.Y + platform.Height; y += blockSize)
          {
            g.DrawLine(gridPen, platform.X, y, platform.X + platform.Width, y);
          }
        }
      }
    }

    private void RenderCoins(Graphics g)
    {
      foreach (var coin in visibleCoins)
      {
        g.FillEllipse(Brushes.Gold, coin.X, coin.Y, coin.Width, coin.Height);
        g.DrawEllipse(orangePen, coin.X, coin.Y, coin.Width, coin.Height);
        g.DrawString("$", coinFont, Brushes.DarkGoldenrod, coin.X + 5, coin.Y + 2);
      }
    }

    private void RenderEnemies(Graphics g)
    {
      foreach (var enemy in visibleEnemies)
      {
        g.FillEllipse(Brushes.Red, enemy.X, enemy.Y, enemy.Width, enemy.Height);
        g.DrawEllipse(blackPen, enemy.X, enemy.Y, enemy.Width, enemy.Height);
        g.FillEllipse(Brushes.Yellow, enemy.X + 5, enemy.Y + 5, 5, 5);
        g.FillEllipse(Brushes.Yellow, enemy.X + 15, enemy.Y + 5, 5, 5);

        // Health bar
        float healthRatio = (float)enemy.Health / enemy.MaxHealth;
        int healthBarWidth = (int)(30 * healthRatio);
        g.FillRectangle(Brushes.Red, enemy.X - 2, enemy.Y - 10, healthBarWidth, 5);
        g.DrawRectangle(blackPen, enemy.X - 2, enemy.Y - 10, 30, 5);
      }
    }

    private void RenderPlayer(Graphics g, Player player)
    {
      Image playerSprite = player.GetCurrentSprite();
      if (playerSprite != null)
      {
        if (player.FacingRight)
        {
          g.DrawImage(playerSprite, player.X, player.Y, player.Width, player.Height);
        }
        else
        {
          g.DrawImage(playerSprite, player.X + player.Width, player.Y, -player.Width, player.Height);
        }
      }
      else
      {
        Color playerColor = player.IsInvulnerable ? Color.FromArgb(150, 135, 206, 250) : Color.Blue;
        using (var playerBrush = new SolidBrush(playerColor))
        {
          g.FillRectangle(playerBrush, player.X, player.Y, player.Width, player.Height);
        }
        g.DrawRectangle(darkBluePen, player.X, player.Y, player.Width, player.Height);
      }
    }

    private void RenderEffects(Graphics g, Player player)
    {
      if (player.IsAttacking)
      {
        var attackBounds = player.GetAttackBounds();
        g.FillEllipse(attackBrush, attackBounds.X, attackBounds.Y, attackBounds.Width, attackBounds.Height);
      }

      if (player.IsDashing)
      {
        for (int i = 0; i < 3; i++)
        {
          using (var dashBrush = new SolidBrush(Color.FromArgb(50 - i * 10, 135, 206, 250)))
          {
            g.FillRectangle(dashBrush,
                player.X - (i * 5 * (player.DashDirection > 0 ? 1 : -1)),
                player.Y, player.Width, player.Height);
          }
        }
      }
    }

    private void RenderHUD(Graphics g, GameManager gameManager)
    {
      g.FillRectangle(hudBackgroundBrush, 10, 10, 300, 140);

      g.DrawString($"Score: {gameManager.Score}", hudFont, Brushes.White, 15, 15, leftFormat);
      g.DrawString($"High Score: {gameManager.HighScore}", hudFont, Brushes.White, 15, 35, leftFormat);
      g.DrawString($"Coins: {gameManager.CoinsCollected}", hudFont, Brushes.Gold, 15, 55, leftFormat);

      g.DrawString("Health:", smallFont, Brushes.White, 15, 75, leftFormat);
      for (int i = 0; i < gameManager.Player.MaxHealth; i++)
      {
        Color heartColor = i < gameManager.Player.Health ? Color.Red : Color.Gray;
        using (var heartBrush = new SolidBrush(heartColor))
        {
          g.FillEllipse(heartBrush, 70 + (i * 25), 75, 20, 20);
        }
        g.DrawString("♥", heartFont, Brushes.White, 72 + (i * 25), 75);
      }

      float dashCooldownRatio = gameManager.Player.DashCooldown <= 0 ? 1f :
          (float)gameManager.Player.DashTimer / gameManager.Player.DashCooldown;
      int dashBarWidth = (int)(100 * dashCooldownRatio);
      g.DrawString("Dash:", smallFont, Brushes.White, 15, 100, leftFormat);
      g.FillRectangle(Brushes.Cyan, 70, 100, dashBarWidth, 10);
      g.DrawRectangle(whitePen, 70, 100, 100, 10);

      g.DrawString("Arrow/WASD: Move, Space: Jump, Left Click: Attack, Right Click: Dash",
          tinyFont, Brushes.White, 15, 125, leftFormat);
    }

    private void RenderGameOver(Graphics g, GameManager gameManager, int screenWidth, int screenHeight)
    {
      g.FillRectangle(gameOverBackgroundBrush, 0, 0, screenWidth, screenHeight);

      g.DrawString("GAME OVER", gameOverFont, Brushes.Red, screenWidth / 2, screenHeight / 2 - 100, centerFormat);
      g.DrawString($"Final Score: {gameManager.Score}", mediumFont, Brushes.White, screenWidth / 2, screenHeight / 2 - 50, centerFormat);
      g.DrawString($"Coins Collected: {gameManager.CoinsCollected}", mediumFont, Brushes.Gold, screenWidth / 2, screenHeight / 2 - 20, centerFormat);
      g.DrawString("Press R to Restart or ESC to Exit", restartFont, Brushes.White, screenWidth / 2, screenHeight / 2 + 20, centerFormat);
    }

    // Dispose resources
    public void Dispose()
    {
      hudFont?.Dispose();
      smallFont?.Dispose();
      tinyFont?.Dispose();
      coinFont?.Dispose();
      heartFont?.Dispose();
      gameOverFont?.Dispose();
      mediumFont?.Dispose();
      restartFont?.Dispose();
      centerFormat?.Dispose();
      leftFormat?.Dispose();
      orangePen?.Dispose();
      blackPen?.Dispose();
      whitePen?.Dispose();
      darkBluePen?.Dispose();
      hudBackgroundBrush?.Dispose();
      gameOverBackgroundBrush?.Dispose();
      attackBrush?.Dispose();
    }
  }
}