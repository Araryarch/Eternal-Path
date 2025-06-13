using System.Drawing.Drawing2D;

namespace EternalPath
{
  public class Renderer
  {
    public void Render(Graphics g, GameManager gameManager, int screenWidth, int screenHeight)
    {
      g.SmoothingMode = SmoothingMode.AntiAlias;
      g.TranslateTransform(-gameManager.CameraX, 0);

      foreach (var platform in gameManager.Platforms.Where(p => p.X > gameManager.CameraX - 100 && p.X < gameManager.CameraX + screenWidth + 100))
      {
        g.FillRectangle(Brushes.SaddleBrown, platform.X, platform.Y, platform.Width, platform.Height);
        g.DrawRectangle(new Pen(Color.DarkRed, 2), platform.X, platform.Y, platform.Width, platform.Height);
        for (int i = 0; i < platform.Width; i += 20)
        {
          g.DrawLine(Pens.Brown, platform.X + i, platform.Y, platform.X + i, platform.Y + platform.Height);
        }
      }

      foreach (var coin in gameManager.Coins.Where(c => c.X > gameManager.CameraX - 100 && c.X < gameManager.CameraX + screenWidth + 100 && c.IsActive))
      {
        g.FillEllipse(Brushes.Gold, coin.X, coin.Y, coin.Width, coin.Height);
        g.DrawEllipse(new Pen(Color.Orange, 2), coin.X, coin.Y, coin.Width, coin.Height);
        g.DrawString("$", new Font("Arial", 10, FontStyle.Bold), Brushes.DarkGoldenrod, coin.X + 5, coin.Y + 2);
      }

      foreach (var enemy in gameManager.Enemies.Where(e => e.X > gameManager.CameraX - 100 && e.X < gameManager.CameraX + screenWidth + 100))
      {
        if (enemy.IsActive)
        {
          g.FillEllipse(Brushes.Red, enemy.X, enemy.Y, enemy.Width, enemy.Height);
          g.DrawEllipse(new Pen(Color.DarkRed, 2), enemy.X, enemy.Y, enemy.Width, enemy.Height);
          g.FillEllipse(Brushes.Yellow, enemy.X + 5, enemy.Y + 5, 5, 5);
          g.FillEllipse(Brushes.Yellow, enemy.X + 15, enemy.Y + 5, 5, 5);

          float healthRatio = (float)enemy.Health / enemy.MaxHealth;
          int healthBarWidth = (int)(30 * healthRatio);
          g.FillRectangle(Brushes.Red, enemy.X - 2, enemy.Y - 10, healthBarWidth, 5);
          g.DrawRectangle(Pens.Black, enemy.X - 2, enemy.Y - 10, 30, 5);
        }
      }

      Image playerSprite = gameManager.Player.GetCurrentSprite();
      if (playerSprite != null)
      {
        if (gameManager.Player.FacingRight)
        {
          g.DrawImage(playerSprite, gameManager.Player.X, gameManager.Player.Y, gameManager.Player.Width, gameManager.Player.Height);
        }
        else
        {
          g.DrawImage(playerSprite, gameManager.Player.X + gameManager.Player.Width, gameManager.Player.Y, -gameManager.Player.Width, gameManager.Player.Height);
        }
      }
      else
      {
        Color playerColor = gameManager.Player.IsInvulnerable ? Color.FromArgb(150, 135, 206, 250) : Color.Blue;
        g.FillRectangle(new SolidBrush(playerColor), gameManager.Player.X, gameManager.Player.Y, gameManager.Player.Width, gameManager.Player.Height);
        g.DrawRectangle(new Pen(Color.DarkBlue, 2), gameManager.Player.X, gameManager.Player.Y, gameManager.Player.Width, gameManager.Player.Height);
      }

      if (gameManager.Player.IsAttacking)
      {
        var attackBounds = gameManager.Player.GetAttackBounds();
        g.FillEllipse(new SolidBrush(Color.FromArgb(100, 255, 255, 0)), attackBounds.X, attackBounds.Y, attackBounds.Width, attackBounds.Height);
      }

      if (gameManager.Player.IsDashing)
      {
        for (int i = 0; i < 3; i++)
        {
          g.FillRectangle(new SolidBrush(Color.FromArgb(50 - i * 10, 135, 206, 250)),
              gameManager.Player.X - (i * 5 * (gameManager.Player.DashDirection > 0 ? 1 : -1)), gameManager.Player.Y, gameManager.Player.Width, gameManager.Player.Height);
        }
      }

      g.ResetTransform();

      // HUD: Score, High Score, Coins, Health, Dash
      using (var font = new Font("Arial", 12, FontStyle.Bold))
      using (var smallFont = new Font("Arial", 10, FontStyle.Bold))
      using (var tinyFont = new Font("Arial", 8))
      using (var format = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near })
      {
        // HUD Background
        g.FillRectangle(new SolidBrush(Color.FromArgb(180, 0, 0, 0)), 10, 10, 300, 140);

        // Score, High Score, Coins
        g.DrawString($"Score: {gameManager.Score}", font, Brushes.White, 15, 15, format);
        g.DrawString($"High Score: {gameManager.HighScore}", font, Brushes.White, 15, 35, format);
        g.DrawString($"Coins: {gameManager.CoinsCollected}", font, Brushes.Gold, 15, 55, format);

        // Health
        g.DrawString("Health:", smallFont, Brushes.White, 15, 75, format);
        for (int i = 0; i < gameManager.Player.MaxHealth; i++)
        {
          Color heartColor = i < gameManager.Player.Health ? Color.Red : Color.Gray;
          g.FillEllipse(new SolidBrush(heartColor), 70 + (i * 25), 75, 20, 20);
          g.DrawString("♥", new Font("Arial", 12), Brushes.White, 72 + (i * 25), 75);
        }

        // Dash Cooldown
        float dashCooldownRatio = gameManager.Player.DashCooldown <= 0 ? 1f : (float)gameManager.Player.DashTimer / gameManager.Player.DashCooldown;
        int dashBarWidth = (int)(100 * dashCooldownRatio);
        g.DrawString("Dash:", smallFont, Brushes.White, 15, 100, format);
        g.FillRectangle(Brushes.Cyan, 70, 100, dashBarWidth, 10);
        g.DrawRectangle(Pens.White, 70, 100, 100, 10);

        // Controls Hint
        g.DrawString("Arrow/WASD: Move, Space: Jump, Left Click: Attack, Right Click: Dash", tinyFont, Brushes.White, 15, 125, format);
      }

      // Game Over Screen
      if (gameManager.GameOver)
      {
        using (var largeFont = new Font("Arial", 36, FontStyle.Bold))
        using (var mediumFont = new Font("Arial", 18))
        using (var smallFont = new Font("Arial", 14))
        using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
        {
          g.FillRectangle(new SolidBrush(Color.FromArgb(200, 0, 0, 0)), 0, 0, screenWidth, screenHeight);

          string gameOverText = "GAME OVER";
          SizeF gameOverSize = g.MeasureString(gameOverText, largeFont);
          g.DrawString(gameOverText, largeFont, Brushes.Red, screenWidth / 2, screenHeight / 2 - 100, format);

          string scoreText = $"Final Score: {gameManager.Score}";
          g.DrawString(scoreText, mediumFont, Brushes.White, screenWidth / 2, screenHeight / 2 - 50, format);

          string coinsText = $"Coins Collected: {gameManager.CoinsCollected}";
          g.DrawString(coinsText, mediumFont, Brushes.Gold, screenWidth / 2, screenHeight / 2 - 20, format);

          string restartText = "Press R to Restart or ESC to Exit";
          g.DrawString(restartText, smallFont, Brushes.White, screenWidth / 2, screenHeight / 2 + 20, format);
        }
      }
    }
  }
}