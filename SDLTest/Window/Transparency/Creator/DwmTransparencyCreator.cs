using System.Runtime.InteropServices;
using Win32Interop.Methods;
using Win32Interop.Structs;

namespace SDLTest.Window.Transparency.Creator;

public class DwmTransparencyCreator : TransparencyCreator
{
    public override bool IsEnabled()
    {
        var windowsVersion = Environment.OSVersion.Version.Major;
        
        bool dwmEnable;
        Dwmapi.DwmIsCompositionEnabled(out dwmEnable);

        return windowsVersion is > 6 and < 8 && dwmEnable;
    }

    public override void MakeTransparent(Window window)
    {
        var windowHandle = window.GetHandle();
        var blurRect = CreateRectRgn(-1, -1, -1, -1);

        var blurProperties = new DWM_BLURBEHIND()
        {
            hRgnBlur = blurRect,
            dwFlags = 0x0001 | 0x0002,
            fEnable = true,
        };

        Dwmapi.DwmEnableBlurBehindWindow(windowHandle, ref blurProperties);
    }
    
    [DllImport("Gdi32.dll")]
    private static extern IntPtr CreateRectRgn(int x1, int y1, int x2, int y2);
}