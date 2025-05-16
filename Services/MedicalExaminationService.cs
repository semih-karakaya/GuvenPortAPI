using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;

namespace GuvenPortAPI.Service
{
    public class MedicalExaminationService : IMedicalExaminationService
    {
        private readonly isgportalContext _context;

        public MedicalExaminationService(isgportalContext context)
        {
            _context = context;
        }

        public async Task<MedicalExamination> AddMedicalExaminationAsync(MedicalExamination examination)
        {
            _context.MedicalExaminations.Add(examination);
            await _context.SaveChangesAsync();
            return examination;
        }

        public async Task<List<MedicalExamination>> GetAllMedicalExaminationsAsync()
        {
            return await _context.MedicalExaminations.ToListAsync();
        }

        public async Task<MedicalExamination> GetMedicalExaminationByIdAsync(int id)
        {
            return await _context.MedicalExaminations.FindAsync(id);
        }

        public async Task<MedicalExamination> UpdateMedicalExaminationAsync(MedicalExamination examination)
        {
            _context.MedicalExaminations.Update(examination);
            await _context.SaveChangesAsync();
            return examination;
        }

        public async Task<bool> DeleteMedicalExaminationAsync(int id)
        {
            var examination = await _context.MedicalExaminations.FindAsync(id);
            if (examination == null) return false;
            _context.MedicalExaminations.Remove(examination);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
