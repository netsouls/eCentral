using AutoMapper;
using eCentral.Core.Domain.Directory;
using eCentral.Core.Domain.Logging;
using eCentral.Core.Domain.Messages;
using eCentral.Core.Infrastructure;
using eCentral.Web.Models.Directory;
using eCentral.Web.Models.Logging;
using eCentral.Web.Models.Messages;

namespace eCentral.Web.Infrastructure
{
    public class AutoMapperStartupTask : IStartupTask
    {
        public void Execute()
        {
            //email account
            Mapper.CreateMap<EmailAccount, EmailAccountModel>()
                .ForMember(dest => dest.IsDefaultEmailAccount, mo => mo.Ignore())
                .ForMember(dest => dest.Password, mo=>mo.Ignore())
                .ForMember(dest => dest.SendTestEmailTo, mo => mo.Ignore());
            Mapper.CreateMap<EmailAccountModel, EmailAccount>();

            //message template
            Mapper.CreateMap<MessageTemplate, MessageTemplateModel>()
                .ForMember(dest => dest.AllowedTokens, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableEmailAccounts, mo => mo.Ignore());
            Mapper.CreateMap<MessageTemplateModel, MessageTemplate>();

            //countries
            Mapper.CreateMap<CountryModel, Country>()
                .ForMember(dest => dest.StateProvinces, mo => mo.Ignore());
            Mapper.CreateMap<Country, CountryModel>()
                .ForMember(dest => dest.NumberOfStates, mo => mo.MapFrom(src => src.StateProvinces != null ? src.StateProvinces.Count : 0));

            //state/provinces
            Mapper.CreateMap<StateProvince, StateProvinceModel>()
                .ForMember(dest => dest.DisplayOrder, mo => mo.MapFrom(src => src.DisplayOrder));
            Mapper.CreateMap<StateProvinceModel, StateProvince>()
                .ForMember(dest => dest.Abbreviation, mo => mo.NullSubstitute(string.Empty))
                .ForMember(dest => dest.Country, mo => mo.Ignore());

            //ports
            Mapper.CreateMap<Port, PortModel>();
            Mapper.CreateMap<PortModel, Port>()
                .ForMember(dest => dest.Abbreviation, mo => mo.NullSubstitute(string.Empty))
                .ForMember(dest => dest.Country, mo => mo.Ignore());

            //logs
            Mapper.CreateMap<Log, LogModel>()
                .ForMember(dest => dest.UserName, mo => mo.Ignore())
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore());
            Mapper.CreateMap<LogModel, Log>()
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.LogLevelId, mo => mo.Ignore())
                .ForMember(dest => dest.User, mo => mo.Ignore());
            //ActivityLogType
            Mapper.CreateMap<ActivityLogTypeModel, ActivityLogType>()
                .ForMember(dest => dest.SystemKeyword, mo => mo.Ignore())
                .ForMember(dest => dest.ActivityLog, mo => mo.Ignore());
            Mapper.CreateMap<ActivityLogType, ActivityLogTypeModel>();
            Mapper.CreateMap<ActivityLog, ActivityLogModel>()
                .ForMember(dest => dest.ActivityLogTypeName, mo => mo.MapFrom(src => src.ActivityLogType.Name))
                .ForMember(dest => dest.UserName, mo => mo.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore());
            /*
            //locale resource
            Mapper.CreateMap<LocaleStringResource, LanguageResourceModel>()
                .ForMember(dest => dest.Name, mo => mo.MapFrom(src => src.ResourceName))
                .ForMember(dest => dest.Value, mo => mo.MapFrom(src => src.ResourceValue))
                .ForMember(dest => dest.LanguageName, mo => mo.MapFrom(src => src.Language != null ? src.Language.Name : string.Empty));
            Mapper.CreateMap<LanguageResourceModel, LocaleStringResource>()
                .ForMember(dest => dest.ResourceName, mo => mo.MapFrom(src => src.Name))
                .ForMember(dest => dest.ResourceValue, mo => mo.MapFrom(src => src.Value))
                .ForMember(dest => dest.Language, mo => mo.Ignore());
             
            //queued email
            Mapper.CreateMap<QueuedEmail, QueuedEmailModel>()
                .ForMember(dest => dest.EmailAccountName, mo => mo.MapFrom(src => src.EmailAccount != null ? src.EmailAccount.FriendlyName : string.Empty))
                .ForMember(dest => dest.CreatedOn, mo => mo.Ignore())
                .ForMember(dest => dest.SentOn, mo => mo.Ignore());
            Mapper.CreateMap<QueuedEmailModel, QueuedEmail>()
                .ForMember(dest => dest.CreatedOn, dt => dt.Ignore())
                .ForMember(dest => dest.SentOn, mo => mo.Ignore())
                .ForMember(dest => dest.EmailAccount, mo => mo.Ignore())
                .ForMember(dest => dest.EmailAccountId, mo => mo.Ignore());
            
            //look up codes
            Mapper.CreateMap<LookUpCodeModel, LookUpCode>();
            Mapper.CreateMap<LookUpCode, LookUpCodeModel>()
                .ForMember(dest => dest.Locales, mo => mo.Ignore());

            //Settings
            Mapper.CreateMap<MediaSettings, MediaSettingsModel>();
            Mapper.CreateMap<MediaSettingsModel, MediaSettings>();
            Mapper.CreateMap<UserSettings, UserSettingsModel.UserSettingModel>();
            Mapper.CreateMap<UserSettingsModel.UserSettingModel, UserSettings>()
                .ForMember(dest => dest.PasswordMinLength, mo => mo.Ignore())
                .ForMember(dest => dest.CryptphraseMinLength, mo => mo.Ignore())
                .ForMember(dest => dest.OnlineUserMinutes, mo => mo.Ignore());

            
            */
        }

        public int Order
        {
            get { return 0; }
        }
    }
}