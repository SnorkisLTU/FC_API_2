using System;
using System.Collections.Generic;
using FootballClubs.Entities;
using FootballClubs.Repositories;
using Microsoft.AspNetCore.Mvc;
using FootballClubs.DTO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace FootballClubs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClubController : ControllerBase
    {
        private readonly string uri = "http://contacts:5000/contacts/";

        static readonly HttpClient client = new HttpClient();
        private readonly ClubRepositoryInterface repository;

        public ClubController(ClubRepositoryInterface repositoryInterface)
        {
            repository = repositoryInterface;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClubGetDTO>>> GetClubs()
        {
            var clubsEntities = await repository.GetClubsAsync();
            ICollection<ClubGetDTO> ListOfClubs = new List<ClubGetDTO>();
            bool isServiceActive = true;

            foreach (var club in clubsEntities)
            {
                string contactId;

                if(club.ClubContacts != string.Empty)
                {
                    contactId = club.ClubContacts;
                }
                else
                {
                    ClubGetDTO clubWithNoContact = new ClubGetDTO()
                    {
                        Id = club.Id,
                        Name = club.Name,
                        City = club.City,
                        Stadium = club.Stadium,
                        ClubContacts = null
                    };
                    ListOfClubs.Add(clubWithNoContact);
                    continue;
                }

                if (isServiceActive)
                {
                    try
                    {
                        string json = await client.GetStringAsync(uri + contactId);
                        var contact = JsonConvert.DeserializeObject<ClubContacts>(json);
                        
                        if (contact != null)
                        {
                            ClubGetDTO clubGetDTO = new ClubGetDTO()
                            {
                                Id = club.Id,
                                Name = club.Name,
                                City = club.City,
                                Stadium = club.Stadium,
                                ClubContacts = contact
                            };
                            ListOfClubs.Add(clubGetDTO);
                        }  
                    }
                    catch (HttpRequestException e)
                    {
                        Console.WriteLine("Message :{0} ", e.Message);
                        if (e.StatusCode == null)
                        {
                            isServiceActive = false;
                            ICollection<ClubGetDTO> ListOfClubsWithNoContactsAPI = new List<ClubGetDTO>();
                            foreach (var club2 in clubsEntities)
                            {
                                ClubGetDTO clubGetDTO2 = new ClubGetDTO()
                                {
                                    Id = club2.Id,
                                    Name = club2.Name,
                                    City = club2.City,
                                    Stadium = club2.Stadium,
                                    ClubContacts = null
                                };
                                ListOfClubsWithNoContactsAPI.Add(clubGetDTO2);
                            }
                            return Ok(ListOfClubsWithNoContactsAPI);
                        }
                        else if ((int)e.StatusCode == 404)
                        {
                            continue;
                        }

                        break;
                    }
                }
            }

            return Ok(ListOfClubs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Club>> GetClubAsync(Guid id)
        {
            var club = await repository.GetClubAsync(id);
            
            if (club == null)
            {
                return NotFound();
            }

            string contactId = "";
            ClubGetDTO clubGetDTO = new ClubGetDTO();

            if (club.ClubContacts != string.Empty)
            {
                contactId = club.ClubContacts;
            }

            string json = string.Empty;

            try
            {
                json = await client.GetStringAsync(uri + contactId);
                var contact = JsonConvert.DeserializeObject<ClubContacts>(json);
                if (contact != null)
                {
                    clubGetDTO = new ClubGetDTO()
                    {
                        Id = club.Id,
                        Name = club.Name,
                        City = club.City,
                        Stadium = club.Stadium,
                        ClubContacts = contact
                    };
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Message :{0} ", e.Message);
                clubGetDTO = new ClubGetDTO()
                {
                    Id = club.Id,
                    Name = club.Name,
                    City = club.City,
                    Stadium = club.Stadium,
                    ClubContacts = null
                };
                return Ok(clubGetDTO);
            }

            return Ok(clubGetDTO);
        }
        
        [HttpPost]
        public async Task<ActionResult<Club>> CreateClubAsync(ClubDTO clubDTO)
        {
            Club newClub = new Club(){
                Id = Guid.NewGuid(),
                Name = clubDTO.Name,
                City = clubDTO.City,
                Stadium = clubDTO.Stadium
            };

            if(clubDTO.ClubContacts.Id == 0 || await repository.FindClubContactsAsync(clubDTO.ClubContacts.Id) == null)
            {
                if (clubDTO.ClubContacts.Id == 0)
                {
                    int randomId;
                    Random rand = new Random();
                    randomId = rand.Next(1, Int16.MaxValue);
                    clubDTO.ClubContacts.Id = randomId;
                }

                HttpResponseMessage response = null!;

                try
                {
                    response = await client.PostAsJsonAsync(uri, clubDTO.ClubContacts);
                    if (response.IsSuccessStatusCode)
                        newClub.ClubContacts = clubDTO.ClubContacts.Id.ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                newClub.ClubContacts = clubDTO.ClubContacts.Id.ToString();
            }

            repository.CreateClub(newClub);

            //return CreatedAtAction(nameof(GetClubAsync), new { id = newClub.Id }, newClub);
            return Ok(newClub);
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateClub(Guid id, ClubDTO clubDTO)
        {
            var currentClub = await repository.GetClubAsync(id);

            if(currentClub == null)
            {
                return NotFound();
            }

            currentClub.Name = clubDTO.Name;
            currentClub.City = clubDTO.City;
            currentClub.Stadium = clubDTO.Stadium;

            if(clubDTO.ClubContacts.Id == 0 || await repository.FindClubContactsAsync(clubDTO.ClubContacts.Id) == null)
            {
                if (clubDTO.ClubContacts.Id == 0)
                {
                    int randomId;
                    Random rand = new Random();
                    randomId = rand.Next(1, Int16.MaxValue);
                    clubDTO.ClubContacts.Id = randomId;
                }

                HttpResponseMessage response = null!;

                try
                {
                    response = await client.PostAsJsonAsync(uri, clubDTO.ClubContacts);
                    if (response.IsSuccessStatusCode)
                        currentClub.ClubContacts = clubDTO.ClubContacts.Id.ToString();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            else
            {
                HttpResponseMessage response = null!;
                string contactId = clubDTO.ClubContacts.Id.ToString(); 

                try
                {
                    ClubPutContacts clubPutContacts = new ClubPutContacts()
                    {
                        Surname = clubDTO.ClubContacts.Surname,
                        Name = clubDTO.ClubContacts.Name,
                        Number = clubDTO.ClubContacts.Number,
                        Email = clubDTO.ClubContacts.Email
                    };

                    response = await client.PutAsJsonAsync(uri + contactId, clubPutContacts);
                    if (response.IsSuccessStatusCode)
                        currentClub.ClubContacts = contactId;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            repository.UpdateClub(currentClub);

            return Ok(currentClub);
        }
        
        [HttpDelete("{id}")]
        public async Task DeleteClubAsync(Guid id)
        {
            var currentClub = await repository.GetClubAsync(id);

            if (currentClub != null)
            {
                repository.DeleteClub(id);
            }
        }
    }
}