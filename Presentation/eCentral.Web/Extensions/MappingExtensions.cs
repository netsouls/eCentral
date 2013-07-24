﻿using eCentral.Core.Domain.Clients;
using eCentral.Core.Domain.Common;
using eCentral.Core.Domain.Companies;
using eCentral.Core.Domain.Directory;
using eCentral.Core.Domain.Localization;
using eCentral.Core.Infrastructure;
using eCentral.Services.Directory;
using eCentral.Services.Localization;
using eCentral.Web.Models.Clients;
using eCentral.Web.Models.Common;
using eCentral.Web.Models.Companies;

namespace eCentral.Web.Extensions
{
    public static class MappingExtensions
    {
        //language
        public static LanguageModel ToModel(this Language entity)
        {
            if (entity == null)
                return null;

            var model = new LanguageModel()
            {
                RowId = entity.RowId,
                Name = entity.Name,
                FlagImageFileName = entity.FlagImageFileName,
            };
            return model;
        }

        //currency
        public static CurrencyModel ToModel(this Currency entity)
        {
            if (entity == null)
                return null;

            var model = new CurrencyModel()
            {
                RowId = entity.RowId,
                Name = entity.GetLocalized(x => x.Name),
            };
            return model;
        }

        //address
        public static AddressModel ToModel(this Address entity)
        {
            if (entity == null)
                return null;

            var model = new AddressModel()
            {
                RowId = entity.RowId,
                CountryId = entity.CountryId,
                CountryName = entity.Country != null ? entity.Country.Name : null,
                StateProvinceId = entity.StateProvinceId,
                StateProvinceName = entity.StateProvince != null ? entity.StateProvince.Name : null,
                City = entity.City,
                Address1 = entity.Address1,
                Address2 = entity.Address2,
                ZipPostalCode = entity.ZipPostalCode,
                PhoneNumber = entity.PhoneNumber,
                FaxNumber = entity.FaxNumber,
            };
            return model;
        }

        public static Address ToEntity(this AddressModel model)
        {
            if (model == null)
                return null;

            var entity = new Address();
            return ToEntity(model, entity);
        }

        public static Address ToEntity(this AddressModel model, Address destination)
        {
            if (model == null)
                return destination;

            destination.RowId = model.RowId;
            destination.CountryId = model.CountryId;
            destination.StateProvinceId = model.StateProvinceId;
            destination.City = model.City;
            destination.Address1 = model.Address1;
            destination.Address2 = model.Address2;
            destination.ZipPostalCode = model.ZipPostalCode;
            destination.PhoneNumber = model.PhoneNumber;
            destination.FaxNumber = model.FaxNumber;

            var countryService = EngineContext.Current.Resolve<ICountryService>();
            var stateService = EngineContext.Current.Resolve<IStateProvinceService>();

            destination.Country = countryService.GetById(destination.CountryId.Value);
            destination.StateProvince = stateService.GetById(destination.StateProvinceId.Value);
            return destination;
        }

        // clients
        public static ClientModel ToModel(this Client entity)
        {
            if (entity == null)
                return null;

            var model = new ClientModel()
            {
                RowId = entity.RowId, 
                ClientName = entity.ClientName, 
                Email = entity.Email, 
                Address = entity.Address.ToModel()
            };

            return model;
        }

        // branch office
        public static BranchOfficeModel ToModel(this BranchOffice entity)
        {
            if (entity == null)
                return null;

            var model = new BranchOfficeModel()
            {
                RowId = entity.RowId,
                BranchName = entity.BranchName,
                Abbreviation = entity.Abbreviation,
                Address = entity.Address.ToModel()
            };

            return model;
        }
    }
}