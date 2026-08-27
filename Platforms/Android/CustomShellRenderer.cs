using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;

namespace Packo.Platforms.Android
{
    public class CustomShellRenderer : ShellRenderer
    {
        protected override IShellToolbarTracker CreateTrackerForToolbar(AndroidX.AppCompat.Widget.Toolbar toolbar)
        {
            toolbar.ContentInsetStartWithNavigation = 0;
            toolbar.SetContentInsetsAbsolute(0, 0);
            return base.CreateTrackerForToolbar(toolbar);
        }
    }
}
