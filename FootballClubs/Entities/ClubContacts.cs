using MongoDB.Bson.Serialization.Attributes;

namespace FootballClubs.Entities
{
    public class ClubContacts
    {
        public int Id {get; set;}
        public string Surname { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}