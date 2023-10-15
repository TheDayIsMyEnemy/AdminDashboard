using MudBlazor;

namespace BlazorTemplate.UserInterface.Themes
{
    public class DefaultTheme : MudTheme
    {
        private readonly string[] fontFamily = { "Roboto", "Montserrat", "Arial", "sans-serif" };

        public DefaultTheme()
        {
            Palette = new()
            {
                Primary = Colors.Blue.Darken1,
                Secondary = Colors.DeepPurple.Accent2,
                Background = Colors.Grey.Lighten5,
                AppbarBackground = Colors.Blue.Darken1,
                DrawerBackground = "#FFF",
                DrawerText = "rgba(0,0,0, 0.7)",
                Success = "#06d79c"
            };

            PaletteDark = new()
            {
                Primary = "#7e6fff",
                Surface = "#1e1e2d",
                Background = "#1a1a27",
                BackgroundGrey = "#151521",
                AppbarText = "#92929f",
                AppbarBackground = "rgba(26,26,39,0.8)",
                DrawerBackground = "#1a1a27",
                ActionDefault = "#74718e",
                ActionDisabled = "#9999994d",
                ActionDisabledBackground = "#605f6d4d",
                TextPrimary = "#b2b0bf",
                TextSecondary = "#92929f",
                TextDisabled = "#ffffff33",
                DrawerIcon = "#92929f",
                DrawerText = "#92929f",
                GrayLight = "#2a2833",
                GrayLighter = "#1e1e2d",
                Info = "#4a86ff",
                Success = "#3dcb6c",
                Warning = "#ffb545",
                Error = "#ff3f5f",
                LinesDefault = "#33323e",
                TableLines = "#33323e",
                Divider = "#292838",
                OverlayLight = "#1e1e2d80"
            };

            LayoutProperties = new()
            {
                DefaultBorderRadius = "6px",
                DrawerWidthLeft = "200px"
            };


            Typography = new()
            {
                Default = new Default()
                {
                    FontFamily = fontFamily,
                    FontSize = ".875rem",
                    FontWeight = 400,
                    LineHeight = 1.43,
                    LetterSpacing = ".01071em"
                },
                H1 = new H1()
                {
                    FontFamily = fontFamily,
                    FontSize = "6rem",
                    FontWeight = 300,
                    LineHeight = 1.167,
                    LetterSpacing = "-.01562em"
                },
                H2 = new H2()
                {
                    FontFamily = fontFamily,
                    FontSize = "3.75rem",
                    FontWeight = 300,
                    LineHeight = 1.2,
                    LetterSpacing = "-.00833em"
                },
                H3 = new H3()
                {
                    FontFamily = fontFamily,
                    FontSize = "3rem",
                    FontWeight = 400,
                    LineHeight = 1.167,
                    LetterSpacing = "0"
                },
                H4 = new H4()
                {
                    FontFamily = fontFamily,
                    FontSize = "2.125rem",
                    FontWeight = 400,
                    LineHeight = 1.235,
                    LetterSpacing = ".00735em"
                },
                H5 = new H5()
                {
                    FontFamily = fontFamily,
                    FontSize = "1.5rem",
                    FontWeight = 400,
                    LineHeight = 1.334,
                    LetterSpacing = "0"
                },
                H6 = new H6()
                {
                    FontFamily = fontFamily,
                    FontSize = "1.25rem",
                    FontWeight = 400,
                    LineHeight = 1.6,
                    LetterSpacing = ".0075em"
                },
                Button = new Button()
                {
                    FontFamily = fontFamily,
                    FontSize = ".875rem",
                    FontWeight = 500,
                    LineHeight = 1.75,
                    LetterSpacing = ".02857em"
                },
                Body1 = new Body1()
                {
                    FontFamily = fontFamily,
                    FontSize = "1rem",
                    FontWeight = 400,
                    LineHeight = 1.5,
                    LetterSpacing = ".00938em"
                },
                Body2 = new Body2()
                {
                    FontFamily = fontFamily,
                    FontSize = ".875rem",
                    FontWeight = 400,
                    LineHeight = 1.43,
                    LetterSpacing = ".01071em"
                },
                Caption = new Caption()
                {
                    FontFamily = fontFamily,
                    FontSize = ".75rem",
                    FontWeight = 400,
                    LineHeight = 1.66,
                    LetterSpacing = ".03333em"
                },
                Subtitle2 = new Subtitle2()
                {
                    FontFamily = fontFamily,
                    FontSize = ".875rem",
                    FontWeight = 500,
                    LineHeight = 1.57,
                    LetterSpacing = ".00714em"
                }
            };
            Shadows = new Shadow();
            ZIndex = new ZIndex();
        }
    }
}
