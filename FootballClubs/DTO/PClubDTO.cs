using System;
using System.ComponentModel.DataAnnotations;

namespace FootballClubs.DTO
{
    public class PClubDTO
    {
        public string Name { get; set; }

        public string City { get; set; }

        public string Stadium { get; set; }

        public string ClubContacts { get; set; }
    }
}