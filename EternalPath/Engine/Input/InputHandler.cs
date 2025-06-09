using System;
using System.Drawing;
using System.Windows.Forms;

namespace EternalPath.InputHandlers
{
  public static class KeyHandler
  {
    public static Point PlayerLocation { get; set; }
    public static Size PlayerSize { get; set; }
    public static Form Form { get; set; }

    private static int MoveSpeed = 50;

    public static void HandleKeyDown(KeyEventArgs e)
    {
      if (Form == null) return;

      switch (e.KeyCode)
      {
        case Keys.Escape:
          Application.Exit();
          break;
        case Keys.Left:
          PlayerLocation = new Point(
              Math.Max(0, PlayerLocation.X - MoveSpeed),
              PlayerLocation.Y);
          break;
        case Keys.Right:
          PlayerLocation = new Point(
              Math.Min(Form.ClientSize.Width - PlayerSize.Width, PlayerLocation.X + MoveSpeed),
              PlayerLocation.Y);
          break;
        case Keys.Up:
          PlayerLocation = new Point(
              PlayerLocation.X,
              Math.Max(0, PlayerLocation.Y - MoveSpeed));
          break;
        case Keys.Down:
          PlayerLocation = new Point(
              PlayerLocation.X,
              Math.Min(Form.ClientSize.Height - PlayerSize.Height, PlayerLocation.Y + MoveSpeed));
          break;
      }
    }

    public static void HandleKeyPress(KeyPressEventArgs e)
    {
      Console.WriteLine("KeyPress: " + e.KeyChar);
    }

    public static void HandleKeyUp(KeyEventArgs e)
    {
      Console.WriteLine("KeyUp: " + e.KeyCode);
    }
  }
}