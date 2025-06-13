namespace EternalPath
{
  public static class Path
  {
    private const string RootRelative = "../EternalPath/";

    public static class Assets
    {
      public static string Get(string relativePath)
      {
        return System.IO.Path.Combine(RootRelative, "Assets", relativePath);
      }
    }

    public static class Sound
    {
      public static string Get(string relativePath)
      {
        return System.IO.Path.Combine(RootRelative, "Assets", "Sounds", relativePath);
      }
    }

    public static class Map
    {
      public static string Get(string relativePath)
      {
        return System.IO.Path.Combine(RootRelative, "Assets", "Map", relativePath);
      }
    }
    public static class Sprite
    {
      public static string Get(string relativePath)
      {
        return System.IO.Path.Combine(RootRelative, "Assets", "Sprites", relativePath);
      }
    }
    public static class Character
    {
      public static string Get(string relativePath)
      {
        return System.IO.Path.Combine(RootRelative, "Assets", "Sprites", "Adventurer", "ListSprites", relativePath);
      }
    }
    public static class Icon
    {
      public static string Get(string relativePath)
      {
        return System.IO.Path.Combine(RootRelative, "Assets", "Icons", relativePath);
      }
    }
  }
}