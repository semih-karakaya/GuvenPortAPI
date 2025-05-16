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
            _context.Staff.Add(staff); // Fixed: Staffs -> Staff
            await _context.SaveChangesAsync();
            return staff;
        }

        public async Task<List<Staff>> GetAllStaffAsync()
        {
            return await _context.Staff.ToListAsync(); // Fixed: Staffs -> Staff
        }

        public async Task<Staff> GetStaffByIdAsync(int id)
        {
            return await _context.Staff.FindAsync(id); // Fixed: Staffs -> Staff
        }

        public async Task<Staff> UpdateStaffAsync(Staff staff)
        {
            _context.Staff.Update(staff); // Fixed: Staffs -> Staff
            await _context.SaveChangesAsync();
            return staff;
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            var staff = await _context.Staff.FindAsync(id); // Fixed: Staffs -> Staff
            if (staff == null) return false;
            _context.Staff.Remove(staff); // Fixed: Staffs -> Staff
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
