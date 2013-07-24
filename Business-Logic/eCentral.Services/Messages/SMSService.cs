using System;
using System.Collections.Generic;
using System.Linq;
using eCentral.Core.Domain.Messages;
using eCentral.Core.Plugins;

namespace eCentral.Services.Messages
{
    /// <summary>
    /// Represents the SMS service
    /// </summary>
    public partial class SmsService:ISmsService
    {
        private readonly IPluginFinder pluginFinder;
        private readonly SmsSettings smsSettings;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pluginFinder">Plugin finder</param>
        /// <param name="smsSettings">SmsSettings instance</param>
        public SmsService(IPluginFinder pluginFinder, SmsSettings smsSettings)
        {
            this.pluginFinder = pluginFinder;
            this.smsSettings  = smsSettings;
        }

        /// <summary>
        /// Load all SMS providers
        /// </summary>
        /// <returns>SMS provider list</returns>
        public virtual IList<ISmsProvider> LoadAll()
        {
            return pluginFinder.GetPlugins<ISmsProvider>().ToList();
        }

        /// <summary>
        /// Load SMS provider by system name
        /// </summary>
        /// <param name="systemName">SMS provider system name</param>
        /// <returns>SMS provider</returns>
        public virtual ISmsProvider LoadBySystemName(string systemName)
        {
            var descriptor = pluginFinder.GetPluginDescriptorBySystemName<ISmsProvider>(systemName);
            if (descriptor != null)
                return descriptor.Instance<ISmsProvider>();

            return null;
        }

        /// <summary>
        /// Load active SMS providers
        /// </summary>
        /// <returns>Active SMS provider list</returns>
        public virtual IList<ISmsProvider> LoadActive()
        {
            return LoadAll()
                .Where(provider => smsSettings.ActiveSmsProviderSystemNames.Contains(provider.PluginDescriptor.SystemName, StringComparer.InvariantCultureIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Number of sent messages</returns>
        public virtual int SendSms(string text)
        {
            int i = 0;

            foreach (var smsProvider in LoadActive())
            {
                if (smsProvider.SendSms(text))
                {
                    i++;
                }
            }
            return i;
        } 
    }
}
