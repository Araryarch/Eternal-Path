using System;
using System.Drawing;
using System.Windows.Forms;

namespace EternalPath.InputHandlers
{
  public static class KeyHandler
  {
    public static Point PlayerLocation { get; set; }
    public static Size PlayerSize { get; set; }
    public static Eternal Form { get; set; }
    public static Player player { get; set; }

    private static int MoveSpeed = 50;
    private static bool isMoving = false;
    public static bool FacingLeft { get; private set; } = false;

    // Lompat manual
    private static float verticalVelocity = 0;
    private static bool isJumping = false;
    private const float Gravity = 5f;
    private const float JumpStrength = -30f;
    private const float MaxFallSpeed = 20f;

    public static void HandleKeyDown(KeyEventArgs e)
    {
      if (Form == null) return;

      bool moved = false;

      switch (e.KeyCode)
      {
        case Keys.Escape:
          Application.Exit();
          break;

        case Keys.A:
          FacingLeft = true;
          PlayerLocation = new Point(
              Math.Max(0, PlayerLocation.X - MoveSpeed),
              PlayerLocation.Y);
          moved = true;
          break;

        case Keys.D:
          FacingLeft = false;
          PlayerLocation = new Point(
              Math.Min(Form.ClientSize.Width - PlayerSize.Width, PlayerLocation.X + MoveSpeed),
              PlayerLocation.Y);
          moved = true;
          break;

        case Keys.Space:
          if (!isJumping)
          {
            verticalVelocity = JumpStrength;
            isJumping = true;
          }
          break;

        case Keys.S:
          PlayerLocation = new Point(
              PlayerLocation.X,
              Math.Min(Form.ClientSize.Height - PlayerSize.Height, PlayerLocation.Y + MoveSpeed));
          moved = true;
          break;
      }

      if (moved && !isMoving)
      {
        isMoving = true;
        Form.SetPlayerImage(Path.Characters.Get("Run/Run.gif"));
      }

      SyncPlayerPosition();
    }

    public static void HandleKeyUp(KeyEventArgs e)
    {
      if (Form == null) return;
      isMoving = false;
      Form.SetPlayerImage(Path.Characters.Get("Idle/Idle.gif"));
    }

    public static void HandleKeyPress(KeyPressEventArgs e)
    {
      // Optional, if you want keypress behavior
    }

    public static void UpdatePhysics()
    {
      if (isJumping)
      {
        verticalVelocity += Gravity;
        if (verticalVelocity > MaxFallSpeed)
          verticalVelocity = MaxFallSpeed;

        int newY = PlayerLocation.Y + (int)verticalVelocity;
        int groundLevel = Form.ClientSize.Height - PlayerSize.Height;

        if (newY >= groundLevel)
        {
          PlayerLocation = new Point(PlayerLocation.X, groundLevel);
          verticalVelocity = 0;
          isJumping = false;
        }
        else
        {
          PlayerLocation = new Point(PlayerLocation.X, newY);
        }

        SyncPlayerPosition();
      }
    }

    private static void SyncPlayerPosition()
    {
      if (player != null)
      {
        player.SetPosition(PlayerLocation);
      }
    }
  }
}
