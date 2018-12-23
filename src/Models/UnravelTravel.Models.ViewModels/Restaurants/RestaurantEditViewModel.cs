﻿namespace UnravelTravel.Models.ViewModels.Restaurants
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    public class RestaurantEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression("^[A-Z]\\D+[a-z]$")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Restaurant name must be between 3 and 50 symbols")]
        public string Name { get; set; }

        [Display(Name = "Current image")]
        public string ImageUrl { get; set; }

        [Display(Name = "New Image")]
        [DataType(DataType.Upload)]
        public IFormFile NewImage { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Address must be between 3 and 50 symbols")]
        public string Address { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Seats { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        [Display(Name = "Destination")]
        public int DestinationId { get; set; }

        public string DestinationName { get; set; }
    }
}