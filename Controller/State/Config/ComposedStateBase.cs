using SadConsole.UI;
using SadConsole.UI.Controls;
using WPADevTools.Controller.State.Config;
using WPADevTools.Controller.State;
using WPADevTools.Controller;
using WPADevTools.Messaging;
using WPADevTools.SadExtensions.Common.Components;
using WPADevTools.SadExtensions.UI.Adornments;
using WPADevTools.SadExtensions.UI.Buttons;
using WPADevTools.SadExtensions.UI.Labels;
using WPADevTools.SadExtensions.UI.Layout;
using WPADevTools.SadExtensions.UI.Panels;
using SadRogue.Primitives;

public abstract class ComposedStateBase : AppStateBase
{
    const int headerRows = 3;
    const int footerRows = 3;
    const int topGap = 1;
    const int bottomGap = 1;

    protected ChromeSurface Chrome { get; }
    protected ControlsConsole MenuHeader { get; }
    protected ControlsConsole MenuList { get; }
    protected PanelHost ContentHost { get; }
    protected PanelHost ToastHost { get; }

    private ControlBase? _menuFocusBeforeToast;
    private ControlListScroller? _menuScroll;

    private readonly ComposedStateSpec _spec;

    private readonly int menuX = 3;
    private readonly int menuY = headerRows + topGap;
    private readonly int menuW = 40;
    private readonly int menuH;

    private Button? _firstButton;

    protected ComposedStateBase(string name, ComposedStateSpec spec, Dimensions? chromeSize = null)
        : base(name)
    {
        _spec = spec;

        Chrome = new ChromeSurface(chromeSize ?? new Dimensions(90, 30));
        Root.Children.Add(Chrome);

        menuH = Chrome.Height - (headerRows + footerRows + topGap + bottomGap);

        int headerH = 1;
        MenuHeader = new ControlsConsole(menuW, headerH)
        {
            Position = new Point(menuX, menuY),
            UseKeyboard = false,
            UseMouse = false
        };
        Chrome.Children.Add(MenuHeader);

        MenuList = new ControlsConsole(menuW, menuH - headerH)
        {
            Position = new Point(menuX, menuY + headerH),
            UseKeyboard = true,
            UseMouse = true,
            FocusOnMouseClick = true
        };
        Chrome.Children.Add(MenuList);

        ContentHost = new PanelHost(PanelHostMode.Single);
        Chrome.Children.Add(ContentHost);

        ToastHost = new PanelHost(PanelHostMode.Multiple);
        Chrome.Children.Add(ToastHost); // draw on top

        new ChromeHeaderBox(_spec.Title).Draw(Chrome);
        new ChromeFooterBox(_spec.Footer).Draw(Chrome);

        BuildFromSpec();
    }

    public override Task OnEnterAsync(Controller controller, CancellationToken ct)
    {
        MenuHeader.IsVisible = MenuList.IsVisible = true;
        MenuHeader.IsEnabled = MenuList.IsEnabled = true;
        MenuList.IsFocused = true;

        EventHub.Message += OnComposedAppMessage;

        _menuScroll?.Dispose();
        _menuScroll = new ControlListScroller(MenuList, Controller.Instance.Host!, stickyTopCount: 0);
        _menuScroll.Rebuild();

        return Task.CompletedTask;
    }

    public override Task OnExitAsync(Controller controller, CancellationToken ct)
    {
        _menuScroll?.Dispose();
        _menuScroll = null;

        EventHub.Message -= OnComposedAppMessage;

        ToastHost.Clear();
        ContentHost.Clear();
        return Task.CompletedTask;
    }

    private void BuildFromSpec()
    {
        MenuHeader.Controls.Clear();
        MenuList.Controls.Clear();

        MenuHeader.Controls.Add(new TitleLabel(_spec.Title) { Position = new Point(0, 0) });

        var stack = new ButtonStack(MenuList, x: 1, startY: 1, gapY: 2);
        var built = _spec.ButtonsFactory(stack)?.ToList() ?? [];
        if (built.Count > 0)
        {
            _firstButton = built[0];
            _firstButton.IsFocused = true;
            MenuList.Controls.FocusedControl = _firstButton;
        }

        foreach (var p in _spec.PanelsFactory?.Invoke(ContentHost) ?? [])
            ContentHost.Add(p);

        _menuScroll?.Rebuild();
    }

    protected void FocusFirstButton()
    {
        if (_firstButton != null)
        {
            _firstButton.IsFocused = true;
            MenuList.Controls.FocusedControl = _firstButton;
        }
    }

    public override void Update(TimeSpan delta)
    {
        var host = Controller.Instance.Host;
        if (host?.Keyboard.IsKeyReleased(SadConsole.Input.Keys.Escape) == true)
        {
            if (ToastHost.Children.Count > 0)
            {
                EventHub.Publish(AppMessage.ToastDismiss());
                return;
            }

            if (_spec.BackIntent is not null && (_spec.BackEnabled?.Invoke() ?? true))
            {
                EventHub.Publish(_spec.BackIntent());
            }
        }
    }

    protected void SetHeader(string text) => ChromeHeaderBox.WriteText(Chrome, text);

    protected void ShowConfirmToast(string message)
    {
        _menuFocusBeforeToast = MenuList.Controls.FocusedControl;

        var toast = new ConfirmToast(new Point(0, 0), message);
        toast.CenterTo(Chrome);
        ToastHost.Clear();
        ToastHost.Add(toast);
    }

    private void RestoreMenuFocus()
    {
        if (!MenuList.IsVisible) return;

        if (_menuFocusBeforeToast != null && MenuList.Controls.Contains(_menuFocusBeforeToast))
        {
            MenuList.Controls.FocusedControl = _menuFocusBeforeToast;
            _menuFocusBeforeToast.IsFocused = true;
        }
        else
        {
            FocusFirstButton();
        }

        MenuList.IsFocused = true;
    }

    private void OnComposedAppMessage(AppMessage msg) =>
        (msg.Type switch
        {
            AppMessageType.QuitRequested => (Action)(() =>
            {
                ShowConfirmToast("Really quit?");
                HandleAppMessage(msg);
            }),

            AppMessageType.ToastDismiss => (Action)(() =>
            {
                ToastHost.Clear();
                RestoreMenuFocus();
                HandleAppMessage(msg);
            }),

            _ => (Action)(() => HandleAppMessage(msg))
        })();


    protected virtual void HandleAppMessage(AppMessage msg) { }

    protected void ShowMenu(bool focus = true)
    {
        MenuHeader.IsVisible = true;
        MenuHeader.IsEnabled = true;

        MenuList.IsVisible = true;
        MenuList.IsEnabled = true;

        if (focus)
        {
            MenuList.IsFocused = true;
            FocusFirstButton();
        }
    }

    protected void ShowMenu(string headerText, bool focus = true)
    {
        SetHeader(headerText);
        ShowMenu(focus);
    }

    protected void HideMenu()
    {
        MenuHeader.IsVisible = false;
        MenuHeader.IsEnabled = false;

        MenuList.IsVisible = false;
        MenuList.IsEnabled = false;
    }

    protected void ResetMenuScrollTop() => _menuScroll?.ResetToTop();
}
