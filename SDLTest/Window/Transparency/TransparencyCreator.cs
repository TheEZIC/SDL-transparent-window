namespace SDLTest.Window.Transparency.Creator;

public abstract class TransparencyCreator
{
    public abstract bool IsEnabled();

    public abstract void MakeTransparent(Window window);
}