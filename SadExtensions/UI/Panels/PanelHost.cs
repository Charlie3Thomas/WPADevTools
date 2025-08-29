// SadExtensions/UI/Panels/PanelHost.cs
using System.Collections.Generic;
using SadConsole;

namespace WPADevTools.SadExtensions.UI.Panels
{
    /// <summary>Container that owns panels and their lifecycle.</summary>
    public sealed class PanelHost : ScreenObject
    {
        private readonly List<PanelViewBase> _panels = [];
        public PanelHostMode Mode { get; }

        public PanelHost(PanelHostMode mode = PanelHostMode.Multiple)
        {
            Mode = mode;
        }

        public T Add<T>(T panel) where T : PanelViewBase
        {
            if (Mode == PanelHostMode.Single)
                Clear();

            panel.AttachTo(this);
            _panels.Add(panel);
            return panel;
        }

        public void Remove(PanelViewBase panel)
        {
            panel.DetachFrom(this);
            _panels.Remove(panel);
            panel.Dispose();
        }

        public void Clear()
        {
            foreach (var p in _panels)
            {
                p.DetachFrom(this);
                p.Dispose();
            }
            _panels.Clear();
        }
    }
}
