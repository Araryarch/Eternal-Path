using System;
using System.Drawing;
using System.Windows.Forms;

namespace EternalPath.InputHandlers
{
  public static class KeyHandler
  {
    public static Point PlayerLocation { get; set; }
    public static Size PlayerSize { get; set; }
    public static Eternal Form { get; set; } // Ganti ke class Eternal, bukan Form umum

    private static int MoveSpeed = 50;
    private static bool isMoving = false;
    public static bool FacingLeft { get; private set; } = false;

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
          PlayerLocation = new Point(
              PlayerLocation.X,
              Math.Max(0, PlayerLocation.Y - MoveSpeed));
          moved = true;
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
    }

    public static void HandleKeyUp(KeyEventArgs e)
    {
      if (Form == null) return;
      isMoving = false;
      Form.SetPlayerImage(Path.Characters.Get("Idle/Idle.gif"));
    }

    public static void HandleKeyPress(KeyPressEventArgs e)
    {
      // Optional: logika tambahan jika ingin
    }
  }
}
