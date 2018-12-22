﻿namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Models.ViewModels.Restaurants;

    public interface IRestaurantsService : ICrudDataService
    {
        Task<RestaurantViewModel[]> GetAllAsync();

        Task Review(int id, string username, params object[] parameters);
    }
}
