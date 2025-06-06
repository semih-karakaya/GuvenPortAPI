using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;

namespace GuvenPortAPI.Service
{
    public class StaffService : IStaffService
    {
        private readonly isgportalContext _context;

        public StaffService(isgportalContext context)
        {
            _context = context;
        }

        public async Task<Staff> AddStaffAsync(Staff staff)
        {
            _context.Staff.Add(staff); 
            await _context.SaveChangesAsync();
            return staff;
        }

        public async Task<List<Staff>> GetAllStaffAsync()
        {
            return await _context.Staff
                .Include(s => s.Doctor)
                .ToListAsync();
        }
        

        public async Task<Staff> GetStaffByIdAsync(int id)
        {
            return await _context.Staff.FindAsync(id); 
        }

        public async Task<Staff> UpdateStaffAsync(Staff staff)
        {
            _context.Staff.Update(staff); 
            await _context.SaveChangesAsync();
            return staff;
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null) return false;
            staff.Active = false;
            _context.Staff.Update(staff);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
