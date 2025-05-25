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

        // Helper methods for mapping between DTO and Entity
        private MedicalExamination MapToEntity(MedicalExaminationDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            return new MedicalExamination
            {
                Id = dto.Id,
                Date = dto.Date,
                IsDisabled = dto.IsDisabled,
                ValidityDate = dto.ValidityDate,
                IdContract = dto.IdContract,
                IdDoctor = dto.IdDoctor,
                ExFilePrinted = dto.ExFilePrinted,
                ExIbys = dto.ExIbys,
                ExFileLocation = dto.ExFileLocation,
                ExFilePrintedUploaded = dto.ExFilePrintedUploaded,
                ExaminationType = dto.ExaminationType
            };
        }

        private MedicalExaminationDto MapToDto(MedicalExamination entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            return new MedicalExaminationDto
            {
                Id = entity.Id,
                Date = entity.Date,
                IsDisabled = entity.IsDisabled,
                ValidityDate = entity.ValidityDate,
                IdContract = entity.IdContract,
                IdDoctor = entity.IdDoctor,
                ExFilePrinted = entity.ExFilePrinted,
                ExIbys = entity.ExIbys,
                ExFileLocation = entity.ExFileLocation,
                ExFilePrintedUploaded = entity.ExFilePrintedUploaded,
                ExaminationType = entity.ExaminationType
            };
        }

        public async Task<MedicalExaminationDto> AddMedicalExaminationAsync(MedicalExaminationDto examination)
        {
            var entity = new MedicalExamination
            {
                Id = examination.Id,
                Date = examination.Date,
                IsDisabled = examination.IsDisabled,
                ValidityDate = examination.ValidityDate,
                IdContract = examination.IdContract,
                IdDoctor = examination.IdDoctor,
                ExFilePrinted = examination.ExFilePrinted,
                ExIbys = examination.ExIbys,
                ExFileLocation = examination.ExFileLocation,
                ExFilePrintedUploaded = examination.ExFilePrintedUploaded,
                ExaminationType = examination.ExaminationType
            };
            _context.MedicalExamination.Add(entity);
            await _context.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<List<MedicalExaminationDto>> GetAllMedicalExaminationsAsync()
        {
            var entities = await _context.MedicalExamination.ToListAsync();
            var dtos = new List<MedicalExaminationDto>();
            foreach (var entity in entities)
            {
                dtos.Add(MapToDto(entity));
            }
            return dtos;
        }

        public async Task<MedicalExaminationDto> GetMedicalExaminationByIdAsync(int id)
        {
            var entity = await _context.MedicalExamination.FindAsync(id);
            return MapToDto(entity);
        }

        public async Task<MedicalExaminationDto> UpdateMedicalExaminationAsync(MedicalExaminationDto examination)
        {
            var entity = await _context.MedicalExamination.FindAsync(examination.Id);
            if (entity == null) return null;
            entity.IdContract = examination.IdContract;
            entity.IdDoctor = examination.IdDoctor;
            
            _context.MedicalExamination.Update(entity);
            await _context.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<bool> DeleteMedicalExaminationAsync(int id)
        {
            var examination = await _context.MedicalExamination.FindAsync(id);
            if (examination == null) return false;
            _context.MedicalExamination.Remove(examination);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
