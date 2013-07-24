using System.Collections.Generic;
using eCentral.Web.Framework.Mvc;

namespace eCentral.Web.Models.Common
{
    public class CurrencySelectorModel : BaseModel
    {
        public CurrencySelectorModel()
        {
            AvailableCurrencies = new List<CurrencyModel>();
        }

        public IList<CurrencyModel> AvailableCurrencies { get; set; }

        public CurrencyModel CurrentCurrency { get; set; }
    }
}