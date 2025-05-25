// File: Services/StaffExaminationSummaryService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;

namespace GuvenPortAPI.Service
{
    public class StaffExaminationSummaryService : IStaffExaminationSummaryService
    {
        private readonly isgportalContext _context;

        public StaffExaminationSummaryService(isgportalContext context)
        {
            _context = context;
        }

        public async Task<List<StaffExaminationSummary>> GetAllAsync()
        {
            return await _context.StaffExaminationSummary.ToListAsync();
        }
    }
}
