using SDLTest.Window.Transparency.Creator;

namespace SDLTest.Window.Transparency;

public class WindowTransparency
{
    private Window _window;
    
    private List<TransparencyCreator> _transparencyCreators = new()
    {
        new WinApiTransparencyCreator(),
        new DwmTransparencyCreator(),
        new CompositionTransparencyCreator(),
    };
    
    public WindowTransparency(Window window)
    {
        _window = window;
    }

    public void Initialize()
    {
        foreach (var transparencyCreator in _transparencyCreators)
        {
            if (!transparencyCreator.IsEnabled())
            {
                continue;
            }
            
            transparencyCreator.MakeTransparent(_window);
        }
    }
}