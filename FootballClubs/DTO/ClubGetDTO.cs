using System;
using System.ComponentModel.DataAnnotations;
using FootballClubs.Entities;

namespace FootballClubs.DTO
{
    public class ClubGetDTO
    {
        public Guid Id { get; init; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Stadium { get; set; } = string.Empty;

        public ClubContacts ClubContacts { get; set; } = new ClubContacts();
    }
}