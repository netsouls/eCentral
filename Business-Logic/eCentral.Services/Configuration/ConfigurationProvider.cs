using System;
using System.Linq;
using eCentral.Core;
using eCentral.Core.Configuration;

namespace eCentral.Services.Configuration
{
    public class ConfigurationProvider<TSettings> : IConfigurationProvider<TSettings> where TSettings : ISettings, new()
    {
        readonly ISettingService settingService;

        public ConfigurationProvider(ISettingService settingService) 
        {
            this.settingService = settingService;
            this.BuildConfiguration();
        }

        public TSettings Settings { get; protected set; }

        [System.Diagnostics.DebuggerStepThrough]
        private void BuildConfiguration() 
        {
            Settings = Activator.CreateInstance<TSettings>();

            // get properties we can write to
            var properties = from prop in typeof(TSettings).GetProperties()
                             where prop.CanWrite && prop.CanRead
                             let setting = this.settingService.GetByKey<string>(typeof(TSettings).Name + "." + prop.Name)
                             where setting != null
                             where CommonHelper.GetCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string))
                             let value = CommonHelper.GetCustomTypeConverter(prop.PropertyType).ConvertFromInvariantString(setting)
                             select new { prop, value };

            // assign properties
            properties.ToList().ForEach(p => p.prop.SetValue(Settings, p.value, null));
        }

        public void SaveSettings(TSettings settings)
        {
            var properties = from prop in typeof(TSettings).GetProperties()
                             where prop.CanWrite && prop.CanRead
                             where CommonHelper.GetCustomTypeConverter(prop.PropertyType).CanConvertFrom(typeof(string))
                             select prop;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in properties)
            {
                string key = typeof(TSettings).Name + "." + prop.Name;
                //Duck typing is not supported in C#. That's why we're using dynamic type
                dynamic value = prop.GetValue(settings, null);
                if (value != null)
                    this.settingService.Set(key, value, false);
                else
                    this.settingService.Set(key, "", false);
            }

            //and now clear cache
            this.settingService.ClearCache();

            this.Settings = settings;
        }
    }
}
