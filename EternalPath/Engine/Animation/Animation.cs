namespace EternalPath
{
  public class Animation
  {
    private List<Image> frames;
    private int currentFrame;
    private int frameDelay;
    private int frameCounter;
    private bool loop;

    public Animation(List<Image> frames, int frameDelay, bool loop = true)
    {
      this.frames = frames;
      this.frameDelay = frameDelay;
      this.loop = loop;
      currentFrame = 0;
      frameCounter = 0;
    }

    public void Update()
    {
      frameCounter++;
      if (frameCounter >= frameDelay)
      {
        frameCounter = 0;
        currentFrame++;
        if (currentFrame >= frames.Count)
        {
          currentFrame = loop ? 0 : frames.Count - 1;
        }
      }
    }

    public Image GetCurrentFrame()
    {
      return frames.Count > 0 ? frames[currentFrame] : null!;
    }

    public void Reset()
    {
      currentFrame = 0;
      frameCounter = 0;
    }
  }

}