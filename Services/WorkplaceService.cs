using GuvenPortAPI.Models; // Use the correct namespace for your models
using GuvenPortAPI.Models.Interface; // Use the correct namespace for your models
using System;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using Newtonsoft.Json; // Keep if you use JsonConvert

// You might need a specific interface for this service (e.g., IOfficeService)
// namespace demo.webapi.Models.Interface
// {
//     public interface IOfficeService
//     {
//         Task<Office> AddOfficeService(Office office);
//         Task<List<vmOfficeDetails>> ListOfficeService(); // You'll need a VM for listing offices
//         Task<vmOfficeDetails> GetOneOfficeFromID(int id); // You'll need a VM for getting details
//         Task<Office> EditOfficeService(Office office);
//         Task<bool> DeleteOfficeService(int id);
//         // Add methods for managing related entities like StaffOffice and Workplaces if needed
//     }
// }


namespace GuvenPortAPI.Service
{
    // You'll need to implement an interface like IOfficeService
    public class WorkplaceService : IWorkplaceService
    {
        private readonly isgportalContext _db;

        public WorkplaceService(isgportalContext db)
        {
            _db = db;
        }

        public async Task<Workplace> AddWorkplaceService(Workplace workplace)
        {
            if (workplace != null)
            {
                _db.Workplaces.Add(workplace);
                await _db.SaveChangesAsync();
                return workplace;
            }
            else
            {
                return new Workplace(); // Avoid returning null
            }
        }
        public async Task<List<EmployeeMedicalStatusDto>> GetOfficeEmployeesMedicalStatusAsync(int officeId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var query = from e in _db.Employees
                        join c in _db.Contracts on e.Id equals c.IdEmployee
                        join w in _db.Workplaces on c.IdWorkplace equals w.Id
                        join o in _db.Offices on w.IdOffice equals o.Id
                        where o.Id == officeId
                        select new
                        {
                            e.Id,
                            e.Name,
                            ValidityDate = _db.MedicalExaminations
                                .Where(m => m.IdContract == c.Id)
                                .Max(m => (DateOnly?)m.ValidityDate)
                        };

            var result = await query
                .GroupBy(x => new { x.Id, x.Name })
                .Select(g => new EmployeeMedicalStatusDto
                {
                    EmployeeId = g.Key.Id,
                    EmployeeName = g.Key.Name,
                    LastValidExam = g.Max(x => x.ValidityDate)
                })
                .Where(x => x.LastValidExam == null || x.LastValidExam < today)
                .OrderBy(x => x.LastValidExam)
                .ToListAsync();

            foreach (var item in result)
            {
                item.Status = item.LastValidExam == null ? "No medical examination" : "expired";
            }

            return result;
        }
        public async Task<List<MedicalExaminationDto>> GetMedicalExaminationsByEmployeeIdAsync(int employeeId)
        {
            var result = await (from m in _db.MedicalExaminations
                                join c in _db.Contracts on m.IdContract equals c.Id
                                where c.IdEmployee == employeeId
                                select new MedicalExaminationDto
                                {
                                    Id = m.Id,
                                    IdContract = m.IdContract,
                                    IdDoctor = m.IdDoctor,
                                    ExamType = m.ExaminationType,
                                    ValidityDate = m.ValidityDate
                                    // Map other properties as needed
                                }).ToListAsync();

            return result;
        }
        public async Task<List<WorkplaceExamCountDto>> GetWorkplaceExamCountsAsync()
        {
            var result = await (from w in _db.Workplaces
                                join c in _db.Contracts on w.Id equals c.IdWorkplace into wc
                                from c in wc.DefaultIfEmpty()
                                join m in _db.MedicalExaminations on c.Id equals m.IdContract into cm
                                from m in cm.DefaultIfEmpty()
                                group m by w.Name into g
                                select new WorkplaceExamCountDto
                                {
                                    WorkplaceName = g.Key,
                                    ExamCount = g.Count(x => x != null)
                                }).ToListAsync();

            return result;
        }


        public async Task<int> GetMedicalExaminationCountByEmployeeIdAsync(int employeeId)
        {
            return await (from m in _db.MedicalExaminations
                          join c in _db.Contracts on m.IdContract equals c.Id
                          where c.IdEmployee == employeeId
                          select m).CountAsync();
        }

        public async Task<List<MedicalExaminationWithDoctorDto>> GetMedicalExaminationsByDoctorIdAsync(int doctorId)
        {
            var result = await (from m in _db.MedicalExaminations join s in _db.Staff on m.IdDoctor equals s.Id where s.Id == doctorId select new MedicalExaminationWithDoctorDto { Id = m.Id, IdContract = m.IdContract, IdDoctor = m.IdDoctor, ExaminationType = m.ExaminationType, DoctorName = s.Name }).ToListAsync();
            return result;
        }

        public async Task<int> GetMedicalExaminationCountByDoctorIdAsync(int doctorId) { 
            return await _db.MedicalExaminations.CountAsync(m => m.IdDoctor == doctorId); 
        }


        public async Task<EmployeeDemographicsDto> GetEmployeeDemographicsAsync()
        {
            var result = await _db.Employees
                .Where(e => e.Active == true)
                .GroupBy(e => 1)
                .Select(g => new EmployeeDemographicsDto
                {
                    DisabledCount = g.Count(e => e.Disable == true),
                    FemaleCount = g.Count(e => e.Sex == "Kadın"),
                    MaleCount = g.Count(e => e.Sex == "Erkek")
                })
                .FirstOrDefaultAsync();

            return result ?? new EmployeeDemographicsDto();
        }

        public async Task<EmployeeDemographicsDto> GetActiveContractEmployeeDemographicsAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var result = await (from e in _db.Employees
                                join c in _db.Contracts on e.Id equals c.IdEmployee
                                where e.Active == true
                                      && c.StartDate <= today
                                      && (c.EndDate == null || c.EndDate >= today)
                                select e)
                .Distinct()
                .GroupBy(e => 1)
                .Select(g => new EmployeeDemographicsDto
                {
                    DisabledCount = g.Count(e => e.Disable == true),
                    FemaleCount = g.Count(e => e.Sex == "Kadın"),
                    MaleCount = g.Count(e => e.Sex == "Erkek")
                })
                .FirstOrDefaultAsync();

            return result ?? new EmployeeDemographicsDto();
        }
        public async Task<WorkplaceAccidentCountDto> GetWorkplaceAccidentCountAsync(int workplaceId, DateOnly startDate, DateOnly endDate)
        {
            var result = await (from w in _db.Workplaces
                                join a in _db.Accidents on w.Id equals a.IdWorkplace
                                where w.Id == workplaceId
                                      && a.AccDate >= startDate
                                      && a.AccDate <= endDate
                                group a by w.Name into g
                                select new WorkplaceAccidentCountDto
                                {
                                    WorkplaceName = g.Key,
                                    TotalAccidents = g.Count()
                                }).FirstOrDefaultAsync();

            return result ?? new WorkplaceAccidentCountDto { WorkplaceName = "", TotalAccidents = 0 };
        }
        public async Task<EmployeeAccidentCountDto> GetEmployeeAccidentCountDtoAsync(int employeeId)
        {
            var count = await (from a in _db.Accidents
                               join ca in _db.ContractAccidents on a.Id equals ca.IdAccident
                               join c in _db.Contracts on ca.IdContract equals c.Id
                               where c.IdEmployee == employeeId
                               select a.Id)
                              .CountAsync();

            return new EmployeeAccidentCountDto
            {
                EmployeeId = employeeId,
                AccidentCount = count
            };
        }

        public async Task<List<EmployeeWithoutEntryExamDto>> GetEmployeesWithoutEntryExamAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var query = from c in _db.Contracts
                        join e in _db.Employees on c.IdEmployee equals e.Id
                        join w in _db.Workplaces on c.IdWorkplace equals w.Id
                        where (c.EndDate == null || c.EndDate > today)
                        && !_db.MedicalExaminations.Any(me =>
                            me.IdContract == c.Id &&
                            me.ExaminationType == ExamType.IseGiris)
                        orderby c.StartDate descending
                        select new EmployeeWithoutEntryExamDto
                        {
                            EmployeeName = e.Name,
                            WorkplaceName = w.Name
                        };

            return await query.ToListAsync();
        }

        public async Task<List<Workplace>> ListWorkplaceService()
        {
            return await _db.Workplaces
                            .Include(w => w.IdOfficeNavigation)
                            .ToListAsync();
        }

        public async Task<Workplace> GetOneWorkplaceFromID(int id)
        {
            var workplace = await _db.Workplaces
                                      .Include(w => w.IdOfficeNavigation)
                                      .FirstOrDefaultAsync(w => w.Id == id);

            return workplace ?? new Workplace(); // Avoid returning null
        }

        public async Task<Workplace> EditWorkplaceService(Workplace workplace)
        {
            if (workplace != null)
            {
                var existingWorkplace = await _db.Workplaces.FirstOrDefaultAsync(w => w.Id == workplace.Id);
                if (existingWorkplace == null)
                {
                    return new Workplace(); // Avoid returning null
                }

                existingWorkplace.Name = workplace.Name ?? existingWorkplace.Name;
                existingWorkplace.SocialSecurityNumber = workplace.SocialSecurityNumber ?? existingWorkplace.SocialSecurityNumber;
                existingWorkplace.IdOffice = workplace.IdOffice;

                _db.Workplaces.Update(existingWorkplace);
                await _db.SaveChangesAsync();
                return existingWorkplace;
            }
            else
            {
                return new Workplace(); // Avoid returning null
            }
        }

        public async Task<bool> DeleteWorkplaceService(int id)
        {
            var workplace = await _db.Workplaces.FirstOrDefaultAsync(w => w.Id == id);
            if (workplace != null)
            {
                workplace.Active = false;
                _db.Workplaces.Update(workplace);
                await _db.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> AddStaffToWorkplace(int workplaceId, int staffId)
        {
            var workplace = await _db.Workplaces.FindAsync(workplaceId);
            var staff = await _db.Staff.FindAsync(staffId);

            if (workplace != null && staff != null)
            {
                var staffWorkplace = new StaffWorkplace
                {
                    IdWorkplace = workplaceId,
                    IdStaff = staffId
                };
                _db.StaffWorkplaces.Add(staffWorkplace);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveStaffFromWorkplace(int workplaceId, int staffId)
        {
            var staffWorkplace = await _db.StaffWorkplaces
                                          .FirstOrDefaultAsync(sw => sw.IdWorkplace == workplaceId && sw.IdStaff == staffId);

            if (staffWorkplace != null)
            {
                staffWorkplace.IdStaffNavigation.Active = false;
                _db.StaffWorkplaces.Update(staffWorkplace);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Workplace> FindWorkplaceFromOfficeId(int officeId)
        {
            var workplace = await _db.Workplaces
                                     .Include(w => w.IdOfficeNavigation)
                                     .FirstOrDefaultAsync(w => w.IdOffice == officeId);

            return workplace ?? new Workplace(); // Avoid returning null
        }
    }

}
