using MudBlazor;
using Microsoft.AspNetCore.Components;

namespace FilterShop.UI.Pages
{
    public class MainLayoutBase : LayoutComponentBase
    {
        protected MudThemeProvider? MudThemeProvider { get; set; }
        protected DefaultTheme DefaultTheme { get; set; } = new();
        protected bool IsDrawerOpened { get; set; } = true;
        protected bool IsDarkMode { get; set; }

        protected void DrawerToggle()
            => IsDrawerOpened = !IsDrawerOpened;

        protected void DarkModeToggle()
            => IsDarkMode = !IsDarkMode;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                IsDarkMode = await MudThemeProvider!.GetSystemPreference();
                StateHasChanged();
            }
        }
    }
}
