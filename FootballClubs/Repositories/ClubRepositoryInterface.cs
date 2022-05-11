using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FootballClubs.Entities;
using FootballClubs.DTO;

namespace FootballClubs.Repositories
{
    public interface ClubRepositoryInterface
    {
        Task<IEnumerable<Club>> GetClubsAsync();
        Task<Club> GetClubAsync(Guid id);
        void CreateClub(Club club);/*
        public void UpdateClubField(Club club);*/
        public void UpdateClub(Club club);
        public void DeleteClub(Guid id);
        Task<ClubContacts> FindUserAsync(int contactId);
    }
}