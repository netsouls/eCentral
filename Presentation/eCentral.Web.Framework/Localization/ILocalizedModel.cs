using System.Collections.Generic;

namespace eCentral.Web.Framework.Localization
{
    public interface ILocalizedModel
    {

    }

    public interface ILocalizedModel<TLocalizedModel> : ILocalizedModel
    {
        #region Data Members

        IList<TLocalizedModel> Locales { get; set; }

        #endregion Data Members
    }
}
