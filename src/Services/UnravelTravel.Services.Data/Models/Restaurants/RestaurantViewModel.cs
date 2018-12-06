﻿using AutoMapper;

namespace UnravelTravel.Services.Data.Models.Restaurants
{
    using UnravelTravel.Common.Mapping;
    using UnravelTravel.Data.Models;

    public class RestaurantViewModel : IMapFrom<Restaurant>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public int DestinationId { get; set; }

        public string DestinationName { get; set; }
    }
}