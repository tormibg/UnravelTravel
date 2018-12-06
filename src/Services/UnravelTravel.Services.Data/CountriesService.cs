﻿namespace UnravelTravel.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Common.Mapping;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.Countries;

    public class CountriesService : ICountriesService
    {
        private readonly IRepository<Country> countriesRepository;

        public CountriesService(IRepository<Country> countriesRepository)
        {
            this.countriesRepository = countriesRepository;
        }

        Task<CountryViewModel[]> ICountriesService.GetAllAsync()
        {
            var countries = this.countriesRepository
                .All()
                .OrderBy(c => c.Name)
                .To<CountryViewModel>()
                .ToArrayAsync();

            return countries;
        }
    }
}