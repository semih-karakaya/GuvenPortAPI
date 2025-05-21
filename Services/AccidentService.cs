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
            _context.Accident.Add(accident);
            await _context.SaveChangesAsync();
            return accident;
        }

        public async Task<List<Accident>> GetAllAccidentsAsync()
        {
            return await _context.Accident.ToListAsync();
        }

        public async Task<Accident> GetAccidentByIdAsync(int id)
        {
            return await _context.Accident.FindAsync(id);
        }

        public async Task<Accident> UpdateAccidentAsync(Accident accident)
        {
            _context.Accident.Update(accident);
            await _context.SaveChangesAsync();
            return accident;
        }
        public async Task<bool> AddStaffToAccidentAsync(int accidentId, int staffId)
        {
            var exists = await _context.AccidentReportStaff
                .AnyAsync(x => x.IdAccident == accidentId && x.IdStaff == staffId);
            if (exists) return false;

            var entity = new AccidentReportStaff
            {
                IdAccident = accidentId,
                IdStaff = staffId
            };
            _context.AccidentReportStaff.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveStaffFromAccidentAsync(int accidentId, int staffId)
        {
            var entity = await _context.AccidentReportStaff
                .FirstOrDefaultAsync(x => x.IdAccident == accidentId && x.IdStaff == staffId);
            if (entity == null) return false;

            _context.AccidentReportStaff.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Staff>> GetStaffByAccidentAsync(int accidentId)
        {
            return await _context.AccidentReportStaff
                .Where(x => x.IdAccident == accidentId)
                .Select(x => x.IdStaffNavigation)
                .ToListAsync();
        }

        public async Task<List<Accident>> GetAccidentsByStaffAsync(int staffId)
        {
            return await _context.AccidentReportStaff
                .Where(x => x.IdStaff == staffId)
                .Select(x => x.IdAccidentNavigation)
                .ToListAsync();
        }
        public async Task<bool> AddContractToAccidentAsync(int accidentId, int contractId)
        {
            var exists = await _context.ContractAccident
                .AnyAsync(x => x.IdAccident == accidentId && x.IdContract == contractId);
            if (exists) return false;

            var entity = new ContractAccident
            {
                IdAccident = accidentId,
                IdContract = contractId
            };
            _context.ContractAccident.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveContractFromAccidentAsync(int accidentId, int contractId)
        {
            var entity = await _context.ContractAccident
                .FirstOrDefaultAsync(x => x.IdAccident == accidentId && x.IdContract == contractId);
            if (entity == null) return false;

            _context.ContractAccident.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Contract>> GetContractsByAccidentAsync(int accidentId)
        {
            return await _context.ContractAccident
                .Where(x => x.IdAccident == accidentId)
                .Select(x => x.IdContractNavigation)
                .ToListAsync();
        }

        public async Task<List<Accident>> GetAccidentsByContractAsync(int contractId)
        {
            return await _context.ContractAccident
                .Where(x => x.IdContract == contractId)
                .Select(x => x.IdAccidentNavigation)
                .ToListAsync();
        }



        public async Task<bool> DeleteAccidentAsync(int id)
        {
            var accident = await _context.Accident.FindAsync(id);
            if (accident == null) return false;
            _context.Accident.Remove(accident);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
