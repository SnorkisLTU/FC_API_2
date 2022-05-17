using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FootballClubs.DTO;
using FootballClubs.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace FootballClubs.Repositories
{
    public class MongoDBClubRepository : ClubRepositoryInterface
    {
        private const string databaseName = "football";
        private const string collectionName = "clubs";

        private readonly string uri = "http://contacts:5000/contacts/";
        static readonly HttpClient client = new HttpClient();

        private readonly List<Club> defaultClubs = new()
        {
            new Club
            {
                Id = Guid.NewGuid(),
                Name = "Real Madrid",
                City = "Madrid",
                Stadium = "Santiago Bernabeu",
                ClubContacts = "74638"
            },
            new Club
            {
                Id = Guid.NewGuid(),
                Name = "Chelsea",
                City = "London",
                Stadium = "Stamford Bridge"
            },
            new Club
            {
                Id = Guid.NewGuid(),
                Name = "Juventus",
                City = "Turin",
                Stadium = "Allianz Stadium",
                ClubContacts = "12345"
            }
        };

        private readonly IMongoCollection<Club> clubsCollection;

        private readonly FilterDefinitionBuilder<Club> filterBuilder = Builders<Club>.Filter;

        public MongoDBClubRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(databaseName);
            clubsCollection = database.GetCollection<Club>(collectionName);
            clubsCollection.InsertMany(defaultClubs);
        }

        public void CreateClub(Club club)
        {
            clubsCollection.InsertOne(club);
        }
        
        public void DeleteClub(Guid id)
        {
            var filter = filterBuilder.Eq(club => club.Id, id);
            clubsCollection.DeleteOne(filter);
        }
        
        public async Task<Club> GetClubAsync(Guid id)
        {
            var filter = filterBuilder.Eq(club => club.Id, id);
            return await clubsCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Club>> GetClubsAsync()
        {
            return await clubsCollection.Find(new BsonDocument()).ToListAsync();
        }

        public void UpdateClub(Club club)
        {
            var filter = filterBuilder.Eq(currentClub => currentClub.Id, club.Id);
            clubsCollection.ReplaceOne(filter, club);
        }

        public async Task<ClubContacts> FindClubContactsAsync(int contactId)
        {
            ClubContacts contacts = null;
            try
            {
                string json = await client.GetStringAsync(uri + contactId);
                contacts = JsonConvert.DeserializeObject<ClubContacts>(json);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Message :{0} ", e.Message);
                return null;
            }

            return contacts;
        }
    }
}