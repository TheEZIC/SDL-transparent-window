using Win32Interop.Enums;
using Win32Interop.Methods;

namespace SDLTest.Window.Transparency.Creator;

public class WinApiTransparencyCreator : TransparencyCreator
{
    public override bool IsEnabled()
    {
        return true;
    }

    public override void MakeTransparent(Window window)
    {
        var hWnd = window.GetHandle();
        
        int GWL_STYLE = -16;
        int GWL_EXSTYLE = -20;
        
        var WS_EX_LAYERED = 0x00080000;
        var WS_EX_NOREDIRECTIONBITMAP = 0x00200000;

        var currentWindowFlags = User32.GetWindowLong(hWnd, GWL_EXSTYLE);
        var extendedWindowFlags = currentWindowFlags | WS_EX_LAYERED | WS_EX_NOREDIRECTIONBITMAP;
        
        User32.SetWindowLong(hWnd, GWL_EXSTYLE, extendedWindowFlags);
        User32.SetLayeredWindowAttributes(hWnd, 0x000000, 0, LWA.LWA_COLORKEY);
    }
}