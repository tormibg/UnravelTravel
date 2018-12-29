﻿namespace UnravelTravel.Models.Common
{
    public class ModelConstants
    {
        public const int AddressMinLength = 3;
        public const int AddressMaxLength = 50;

        public const string NameLengthError = "Name must be between {2} and {1} symbols";
        public const string AddressLengthError = "Address must be between {2} and {1} symbols";
        public const string NameRegex = "^[A-Z]\\D+[a-z]$";
        public const string NameRegexError = "Name must start with upper case and end with lower case";
        public const string ImageUrlDisplay = "Current image";
        public const string NewImageDisplay = "New Image";
        public const string UserFullNameDisplay = "Full name";

        public class Activity
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 50;
            public const int MinMinutesTillStart = 15;

            public const string StartingHourError = "Activity starting hour must be at least 15 minutes from now";
            public const string AdminDateDisplay = "Activity date and starting hour";
            public const string NameDisplay = "Activity name";
            public const string DateDisplay = "Activity date";
            public const string StartingHourDisplay = "Activity starting hour";
        }

        public class Destination
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 50;

            public const int InformationMinLength = 10;
            public const int InformationMaxLength = 550;

            public const string InformationError = "Information must be between {2} and {1} symbols";
        }

        public class Location
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 50;
        }

        public class Restaurant
        {
            public const int NameMinLength = 3;
            public const int NameMaxLength = 50;
        }

        public class Reservation
        {
            public const int MinMinutesFromNow = 30;

            public const string HourError = "Reservation time must be at least 30 minutes from now";
            public const string DateDisplay = "Reservation date and time";
            public const string PeopleCountDisplay = "Table for";
        }

        public class Search
        {
            public const string StartDateDisplay = "From";
            public const string EndDateDisplay = "To";
            public const string EndDateError = "End date cannot be before start date";
        }

        public class Review
        {
            public const int RatingMin = 1;
            public const int RatingMax = 5;
            public const int ContentMinLength = 5;
            public const int ContentMaxLength = 500;

            public const string ContentError = "Content must be between {2} and {1} symbols";
        }
    }
}