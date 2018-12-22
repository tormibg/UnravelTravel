﻿namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Models.ViewModels.Activities;

    public interface IActivitiesService : IBaseDataService
    {
        Task<ActivityViewModel[]> GetAllAsync();

        Task Review(int id, string username, params object[] parameters);
    }
}
