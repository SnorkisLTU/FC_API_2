using System;
using System.ComponentModel.DataAnnotations;
using FootballClubs.Entities;

namespace FootballClubs.DTO
{
    public class ClubDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Stadium { get; set; } = string.Empty;

        public ClubContacts ClubContacts { get; set; }
    }
}