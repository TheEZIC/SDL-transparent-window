using SDL2;
using SDLTest.Window.Transparency;
using Win32Interop.Methods;
using Win32Interop.Structs;

namespace SDLTest.Window;

public class Window
{
    private bool _running;
    
    public IntPtr MainWindow { get; private set; }
    public IntPtr Renderer { get; private set; }
    public IntPtr GlContext { get; private set; }

    private WindowTransparency _windowTransparency;

    public Window()
    {
        CheckSDLSupport();

        MainWindow = CreateMainWindow();
        Renderer = CreateRenderer();
        GlContext = CreateGlContext();
        
        _windowTransparency = new WindowTransparency(this);

        Initialize();
        
        ShowMainWindow();
    }

    public Window Start()
    {
        _running = true;
        
        while (_running)
        {
            while (SDL.SDL_PollEvent(out var e) > 0)
            {
                ProcessSDLEvent(e);
            }
    
            SDL.SDL_Delay(1000 / 60);
        }

        return this;
    }
    
    public Window Stop()
    {
        _running = false;
        
        SDL.SDL_DestroyRenderer(Renderer);
        SDL.SDL_DestroyWindow(MainWindow);
        SDL.SDL_Quit();

        return this;
    }

    private void Initialize()
    {
        SetWindowBlackBackground();
        // ClipWindowBorder();
        ApplyWindowTransparency();
    }

    private void ProcessSDLEvent(SDL.SDL_Event e)
    {
        switch (e.type)
        {
            case SDL.SDL_EventType.SDL_QUIT:
                Stop();
                break;
            case SDL.SDL_EventType.SDL_KEYDOWN:
            {
                switch (e.key.keysym.sym)
                {
                    case SDL.SDL_Keycode.SDLK_ESCAPE:
                        Stop();
                        break;
                }

                break;
            }
        }
    }
    
    private IntPtr CreateMainWindow()
    {
        SDL.SDL_WindowFlags flags = SDL.SDL_WindowFlags.SDL_WINDOW_BORDERLESS |
                                    SDL.SDL_WindowFlags.SDL_WINDOW_ALLOW_HIGHDPI |
                                    SDL.SDL_WindowFlags.SDL_WINDOW_POPUP_MENU |
                                    SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN; // prevent blinking on initialization stage
        
        var window = SDL.SDL_CreateWindow(
            "SDL window", 
            100,
            100,
            800,
            600,
            flags
        );

        if (window == IntPtr.Zero)
        {
            throw new Exception($"There was an issue while creating the window. {SDL.SDL_GetError()}");
        }

        return window;
    }

    private IntPtr CreateRenderer()
    {
        var renderer = SDL.SDL_CreateRenderer(
            MainWindow,
            -1,
            SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL.SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC
        );
    
        if (renderer == IntPtr.Zero)
        {
            throw new Exception($"There was an issue while creating the renderer. {SDL.SDL_GetError()}");
        }

        return renderer;
    }

    private IntPtr CreateGlContext()
    {
        var context = SDL.SDL_GL_CreateContext(MainWindow);

        return context;
    }

    private void CheckSDLSupport()
    {
        if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0)
        {
            throw new Exception($"There was an issue while initializing SDL. {SDL.SDL_GetError()}");
        }
    }
    
    private void ShowMainWindow() => SDL.SDL_ShowWindow(MainWindow);

    private void SetWindowBlackBackground()
    {
        SDL.SDL_SetRenderDrawColor(Renderer, 0, 0, 0, 0);
        SDL.SDL_RenderClear(Renderer);
        SDL.SDL_RenderPresent(Renderer);
    }

    private void ApplyWindowTransparency() => _windowTransparency.Initialize();

    // TODO: Fix this method
    private void ClipWindowBorder()
    {
        var hWnd = GetHandle();
        var margins = new MARGINS()
        {
            cyTopHeight = 1,
            cyBottomHeight = 1,
            cxLeftWidth = 1,
            cxRightWidth = 1,
        };
        
        Dwmapi.DwmExtendFrameIntoClientArea(hWnd, ref margins);
    }
    
    public SDL.SDL_SysWMinfo GetInfo()
    {
        SDL.SDL_SysWMinfo wmInfo = new SDL.SDL_SysWMinfo();
        SDL.SDL_VERSION(out wmInfo.version);
        SDL.SDL_GetWindowWMInfo(MainWindow, ref wmInfo);

        return wmInfo;
    }

    public IntPtr GetHandle()
    {
        var windowInfo = GetInfo();
        
        return windowInfo.info.win.window;
    }
}
