using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;

namespace GuvenPortAPI.Service
{
    public class AccidentService : IAccidentService
    {
        private readonly isgportalContext _context;

        public AccidentService(isgportalContext context)
        {
            _context = context;
        }

        public async Task<Accident> AddAccidentAsync(Accident accident)
        {
            _context.Accidents.Add(accident);
            await _context.SaveChangesAsync();
            return accident;
        }

        public async Task<List<Accident>> GetAllAccidentsAsync()
        {
            return await _context.Accidents.ToListAsync();
        }

        public async Task<Accident> GetAccidentByIdAsync(int id)
        {
            return await _context.Accidents.FindAsync(id);
        }

        public async Task<Accident> UpdateAccidentAsync(Accident accident)
        {
            _context.Accidents.Update(accident);
            await _context.SaveChangesAsync();
            return accident;
        }

        public async Task<bool> DeleteAccidentAsync(int id)
        {
            var accident = await _context.Accidents.FindAsync(id);
            if (accident == null) return false;
            _context.Accidents.Remove(accident);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
