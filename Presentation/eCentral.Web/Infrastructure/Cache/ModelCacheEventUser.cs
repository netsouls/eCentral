using eCentral.Core.Caching;
using eCentral.Core.Domain.Clients;
using eCentral.Core.Domain.Companies;
using eCentral.Core.Domain.Configuration;
using eCentral.Core.Domain.Directory;
using eCentral.Core.Domain.Localization;
using eCentral.Core.Domain.Users;
using eCentral.Core.Events;
using eCentral.Core.Infrastructure;
using eCentral.Services.Events;

namespace eCentral.Web.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event (used for caching presentation layer models)
    /// </summary>
    public partial class ModelCacheEventUser :

        //languages
        IUser<EntityInserted<Language>>,
        IUser<EntityUpdated<Language>>,
        IUser<EntityDeleted<Language>>,

        //settings
        IUser<EntityInserted<Setting>>,
        IUser<EntityUpdated<Setting>>,
        IUser<EntityDeleted<Setting>>,

        //states/province
        IUser<EntityInserted<StateProvince>>,
        IUser<EntityUpdated<StateProvince>>,
        IUser<EntityDeleted<StateProvince>>,

        //company 
        IUser<EntityInserted<Company>>,
        IUser<EntityUpdated<Company>>,
        IUser<EntityDeleted<Company>>,

        //client
        IUser<EntityInserted<Client>>,
        IUser<EntityUpdated<Client>>,
        IUser<EntityDeleted<Client>>,

        // offices
        IUser<EntityInserted<BranchOffice>>,
        IUser<EntityUpdated<BranchOffice>>,
        IUser<EntityDeleted<BranchOffice>>,

        // users
        IUser<EntityInserted<User>>,
        IUser<EntityUpdated<User>>,
        IUser<EntityDeleted<User>>

    {
        /// <summary>
        /// Key for NavigationLinkModel caching
        /// </summary>
        /// <remarks>
        /// {0} : User id
        /// {1} : Language Id
        /// </remarks>
        public const string NAVIGATION_MODEL_KEY = "ecentral.web.navigationlinks-{0}-{1}";
        public const string NAVIGATION_PATTERN_KEY = "ecentral.web.navigationlinks-{0}";

        /// <summary>
        /// Key for states by country id
        /// </summary>
        /// <remarks>
        /// {0} : country ID
        /// {0} : addEmptyStateIfRequired value
        /// {0} : language ID
        /// </remarks>
        public const string STATEPROVINCES_BY_COUNTRY_MODEL_KEY = "ecentral.web.stateprovinces.bycountry-{0}-{1}-{2}";
        public const string STATEPROVINCES_PATTERN_KEY = "ecentral.web.stateprovinces.";

        /// <summary>
        /// Key for company
        /// </summary>
        /// <remarks>
        /// {0} : Filter values
        /// </remarks>
        public const string COMPANY_MODEL_KEY = "ecentral.web.company-{0}";
        public const string COMPANY_PATTERN_KEY = "ecentral.web.company-";

        /// <summary>
        /// Key for branch offices
        /// </summary>
        /// <remarks>
        /// {0} : Filter values
        /// </remarks>
        public const string OFFICE_MODEL_KEY = "ecentral.web.office-{0}";
        public const string OFFICE_PATTERN_KEY = "ecentral.web.office-";

        /// <summary>
        /// Key for client
        /// </summary>
        /// <remarks>
        /// {0} : Filter values
        /// </remarks>
        public const string CLIENT_MODEL_KEY = "ecentral.web.client-{0}";
        public const string CLIENT_PATTERN_KEY = "ecentral.web.client-";

        /// <summary>
        /// Key for users
        /// </summary>
        /// <remarks>
        /// {0} : Filter values
        /// </remarks>
        public const string USERS_MODEL_KEY = "ecentral.web.users-{0}";
        public const string USERS_PATTERN_KEY = "ecentral.web.users-";

        private readonly ICacheManager cacheManager;

        public ModelCacheEventUser()
        {
            //TODO inject static cache manager using constructor
            this.cacheManager = EngineContext.Current.ContainerManager.Resolve<ICacheManager>("site_cache_static");
        }

        //languages
        public void HandleEvent(EntityInserted<Language> eventMessage)
        {
            //clear all localizable models
            cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<Language> eventMessage)
        {
            //clear all localizable models
            cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<Language> eventMessage)
        {
            //clear all localizable models
            cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }

        //settings
        public void HandleEvent(EntityInserted<Setting> eventMessage)
        {
            //clear models which depend on settings
        }
        public void HandleEvent(EntityUpdated<Setting> eventMessage)
        {
            //clear models which depend on settings
        }
        public void HandleEvent(EntityDeleted<Setting> eventMessage)
        {
            //clear models which depend on settings
        }

        //State/province
        public void HandleEvent(EntityInserted<StateProvince> eventMessage)
        {
            cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void HandleEvent(EntityUpdated<StateProvince> eventMessage)
        {
            cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }
        public void HandleEvent(EntityDeleted<StateProvince> eventMessage)
        {
            cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }

        //company
        public void HandleEvent(EntityInserted<Company> eventMessage)
        {
            RemoveApplicationData();
        }
        public void HandleEvent(EntityUpdated<Company> eventMessage)
        {
            RemoveApplicationData();
        }
        public void HandleEvent(EntityDeleted<Company> eventMessage)
        {
            RemoveApplicationData();
        }

        // branch office
        public void HandleEvent(EntityInserted<BranchOffice> eventMessage)
        {
            RemoveApplicationData();
        }
        public void HandleEvent(EntityUpdated<BranchOffice> eventMessage)
        {
            RemoveApplicationData();
        }
        public void HandleEvent(EntityDeleted<BranchOffice> eventMessage)
        {
            RemoveApplicationData();
        }

        // client
        public void HandleEvent(EntityInserted<Client> eventMessage)
        {
            RemoveApplicationData();
        }
        public void HandleEvent(EntityUpdated<Client> eventMessage)
        {
            RemoveApplicationData();
        }
        public void HandleEvent(EntityDeleted<Client> eventMessage)
        {
            RemoveApplicationData();
        }

        // users
        public void HandleEvent(EntityInserted<User> eventMessage)
        {
            RemoveApplicationData();
        }
        public void HandleEvent(EntityUpdated<User> eventMessage)
        {
            RemoveApplicationData();
        }
        public void HandleEvent(EntityDeleted<User> eventMessage)
        {
            RemoveApplicationData();
        }

        private void RemoveApplicationData ()
        {
            cacheManager.RemoveByPattern(COMPANY_PATTERN_KEY);
            cacheManager.RemoveByPattern(CLIENT_PATTERN_KEY);
            cacheManager.RemoveByPattern(OFFICE_PATTERN_KEY);
            cacheManager.RemoveByPattern(USERS_PATTERN_KEY);
        }
    }
}
