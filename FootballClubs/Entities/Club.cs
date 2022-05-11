using System;
using System.ComponentModel.DataAnnotations;

namespace FootballClubs.Entities
{
    public class Club
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Stadium { get; set; } = string.Empty;

        public string ClubContacts { get; set; } = string.Empty;
    }
}