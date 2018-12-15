﻿namespace UnravelTravel.Web.Areas.Administrator.InputModels.Activities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class ActivityCreateInputModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 symbols")]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        [Display(Name = "Image")]
        public IFormFile Image { get; set; }

        [Required]
        [Display(Name = "Date and time of the activity")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Location")]
        public int LocationId { get; set; }

        public string Location { get; set; }
    }
}
