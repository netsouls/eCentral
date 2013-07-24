using System.Linq;
using SSI.Core;
using SSI.Core.Domain;
using SSI.Core.Domain.Users;
using SSI.Services.Users;

namespace SSI.Web.Framework.Themes
{
    /// <summary>
    /// Theme context
    /// </summary>
    public partial class ThemeContext : IThemeContext
    {
        private readonly IWorkContext _workContext;
        private readonly IUserService _userService;
        private readonly SiteInformationSettings _siteInformationSettings;
        private readonly IThemeProvider _themeProvider;

        private bool _desktopThemeIsCached;
        private string _cachedDesktopThemeName;

        private bool _mobileThemeIsCached;
        private string _cachedMobileThemeName;

        public ThemeContext(IWorkContext workContext, IUserService userService,
            SiteInformationSettings siteInformationSettings, IThemeProvider themeProvider)
        {
            this._workContext             = workContext;
            this._userService             = userService;
            this._siteInformationSettings = siteInformationSettings;
            this._themeProvider           = themeProvider;
        }

        /// <summary>
        /// Get or set current theme for desktops (e.g. darkOrange)
        /// </summary>
        public string WorkingDesktopTheme
        {
            get
            {
                if (_desktopThemeIsCached)
                    return _cachedDesktopThemeName;

                string theme = "";
                if (_siteInformationSettings.AllowUserToSelectTheme)
                {
                    if (_workContext.CurrentUser != null)
                        theme = _workContext.CurrentUser.GetAttribute<string>(SystemUserAttributeNames.WorkingDesktopThemeName);
                }

                //default store theme
                if (string.IsNullOrEmpty(theme))
                    theme = _siteInformationSettings.DefaultSiteThemeForDesktops;

                //ensure that theme exists
                if (!_themeProvider.ThemeConfigurationExists(theme))
                    theme = _themeProvider.GetThemeConfigurations()
                        .Where(x => !x.MobileTheme)
                        .FirstOrDefault()
                        .ThemeName;
                
                //cache theme
                this._cachedDesktopThemeName = theme;
                this._desktopThemeIsCached = true;
                return theme;
            }
            set
            {
                if (!_siteInformationSettings.AllowUserToSelectTheme)
                    return;

                if (_workContext.CurrentUser == null)
                    return;

                _userService.SaveUserAttribute(_workContext.CurrentUser, SystemUserAttributeNames.WorkingDesktopThemeName, value);

                //clear cache
                this._desktopThemeIsCached = false;
            }
        }

        /// <summary>
        /// Get current theme for mobile (e.g. Mobile)
        /// </summary>
        public string WorkingMobileTheme
        {
            get
            {
                if (_mobileThemeIsCached)
                    return _cachedMobileThemeName;

                //default store theme
                string theme = _siteInformationSettings.DefaultSiteThemeForMobileDevices;

                //ensure that theme exists
                if (!_themeProvider.ThemeConfigurationExists(theme))
                    theme = _themeProvider.GetThemeConfigurations()
                        .Where(x => x.MobileTheme)
                        .FirstOrDefault()
                        .ThemeName;

                //cache theme
                this._cachedMobileThemeName = theme;
                this._mobileThemeIsCached = true;
                return theme;
            }
        }
    }
}
