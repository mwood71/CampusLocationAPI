using CampusLocation.Contracts;
using CampusLocation.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampusLocation.Services
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext _db;

        public LocationRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> Create(Location entity)
        {
            await _db.Locations.AddAsync(entity);
            return await Save();
        }

        public async Task<bool> Delete(Location entity)
        {
            _db.Locations.Remove(entity);
            return await Save();
            
        }

        public async Task<bool> doesExist(int id)
        {
            return await _db.Locations.AnyAsync(u => u.Id == id);
            
        }

        public async Task<Location> FindById(int id)
        {
            return await _db.Locations.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IList<Location>> GetAll()
        {
            return await _db.Locations.ToListAsync();
            
        }

        public async Task<bool> Save()
        {
            var changes = await _db.SaveChangesAsync();
            return changes > 0;
        }

        public async Task<bool> Update(Location entity)
        {
            _db.Locations.Update(entity);

            return await Save();
            
        }
    }
}
