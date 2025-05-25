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
using Newtonsoft.Json; 



namespace GuvenPortAPI.Service
{
  
    public class OfficeService : IOfficeService
    {
        private readonly isgportalContext _db;

        public OfficeService(isgportalContext db)
        {
            _db = db;
        }

        public async Task<Office> AddOfficeService(Office office)
        {
            if (office != null)
            {
                _db.Office.Add(office);
                await _db.SaveChangesAsync();
                return office;
            }
            else
            {
                return new Office(); 
            }
        }
        public async Task<int> GetTotalActiveCompaniesAsync()
        {
            using var context = new isgportalContext(); 
            return await context.Office.CountAsync(o => o.Active == true);
        }

        public async Task<List<Workplace>> GetActiveWorkplacesByOfficeIdAsync(int officeId)
        {
            using var context = new isgportalContext(); 
            return await context.Workplace
                .Where(w => w.IdOffice == officeId && w.Active == true)
                .ToListAsync();
        }
            
        public async Task<int> GetTotalActiveWorkplacesAsync()
        {
            using var context = new isgportalContext(); 
            return await context.Workplace.CountAsync(w => w.Active == true);
        }

        public async Task<List<CompanyWorkplaceCountDto>> GetCompanyWorkplaceCountsAsync()
        {
            using var context = new isgportalContext(); 
            var result = await (from o in context.Office
                                where o.Active == true
                                join w in context.Workplace.Where(x => x.Active == true)
                                    on o.Id equals w.IdOffice into g
                                from w in g.DefaultIfEmpty()
                                group w by new { o.OName } into grp
                                select new CompanyWorkplaceCountDto
                                {
                                    CompanyName = grp.Key.OName,
                                    TotalWorkplaces = grp.Count(x => x != null)
                                })
                                .OrderByDescending(x => x.CompanyName)
                                .ToListAsync();
            return result;
        }




        public async Task<List<vmOfficeDetails>> ListOfficeService()
        {
            List<vmOfficeDetails> officeList = new List<vmOfficeDetails>();

            var offices = await _db.Office
                                 .Include(o => o.IdManagerstaffNavigation)
                                 .Where(o => o.Active != false)
                                 .ToListAsync();

            foreach (var office in offices)
            {
                vmOfficeDetails officeDetails = new vmOfficeDetails
                {
                    Id = office.Id,
                    OfficeName = office.OName ?? string.Empty,
                    Address = office.Address ?? string.Empty,
                    ManagerName = office.IdManagerstaffNavigation?.Name ?? string.Empty,
                };
                officeList.Add(officeDetails);
            }

            return officeList;
        }
        public async Task<List<EmployeeMedicalStatusDto>> GetEmployeesWithExpiredOrMissingMedicalAsync(int officeId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var query = from e in _db.Employee
                        join c in _db.Contract on e.Id equals c.IdEmployee
                        join w in _db.Workplace on c.IdWorkplace equals w.Id
                        join o in _db.Office on w.IdOffice equals o.Id
                        where o.Id == officeId
                        join m in _db.MedicalExamination on c.Id equals m.IdContract into medGroup
                        from m in medGroup.DefaultIfEmpty()
                        group m by new { e.Id, e.Name } into g
                        let lastValidExam = g.Max(x => x != null ? x.ValidityDate : null)
                        where lastValidExam == null || lastValidExam < today
                        select new EmployeeMedicalStatusDto
                        {
                            EmployeeId = g.Key.Id,
                            EmployeeName = g.Key.Name,
                            LastValidExam = lastValidExam,
                            Status = lastValidExam == null ? "No medical examination" : "expired"
                        };

            return await query.OrderBy(x => x.LastValidExam).ToListAsync();
        }


        public async Task<vmOfficeDetails> GetOneOfficeFromID(int id)
        {
            var office = await _db.Office
                .FirstOrDefaultAsync(o => o.Id == id && o.Active == true);

            if (office == null)
            {
                return new vmOfficeDetails();
            }

            
            var manager = await _db.Staff
                .FirstOrDefaultAsync(s => s.Id == office.IdManagerstaff);

            vmOfficeDetails officeDetails = new vmOfficeDetails
            {
                Id = office.Id,
                OfficeName = office.OName ?? string.Empty,
                Address = office.Address ?? string.Empty,
                Crm = office.Crm ?? string.Empty,
                ManagerName = manager?.Name ?? string.Empty, 
            };

            return officeDetails;
        }

        public async Task<Office> EditOfficeService(Office office)
        {
            if (office != null)
            {
                var existingOffice = await _db.Office.FirstOrDefaultAsync(o => o.Id == office.Id);
                if (existingOffice == null)
                {
                    return new Office(); 
                }

                existingOffice.Address = office.Address ?? existingOffice.Address;
                existingOffice.OName = office.OName ?? existingOffice.OName;
                existingOffice.Crm = office.Crm ?? existingOffice.Crm;
                existingOffice.IdManagerstaff = office.IdManagerstaff;

                _db.Office.Update(existingOffice);
                await _db.SaveChangesAsync();
                return existingOffice;
            }
            else
            {
                return new Office(); 
            }
        }

        public async Task<List<vmOfficeDetails>> GetActiveOfficesWithManagerAsync()
        {
            using var context = new isgportalContext(); 
            var result = await (from o in context.Office
                                where o.Active == true
                                join s in context.Staff on o.IdManagerstaff equals s.Id into mgr
                                from manager in mgr.DefaultIfEmpty()
                                select new vmOfficeDetails
                                {
                                    Id = o.Id,
                                    OfficeName = o.OName,
                                    Address = o.Address,
                                    Crm = o.Crm,
                                    ManagerName = manager != null ? manager.Name : null
                                }).ToListAsync();

            return result;
        }
        public async Task<bool> DeleteOfficeService(int id)
        {
            var office = await _db.Office.FirstOrDefaultAsync(o => o.Id == id);
            if (office != null)
            {
                office.Active = false;
                _db.Office.Update(office);
                await _db.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> AddStaffToOffice(int officeId, int staffId)
        {
            var office = await _db.Office.FindAsync(officeId);
            var staff = await _db.Staff.FindAsync(staffId);

            if (office != null && staff != null)
            {
                var staffOffice = new StaffOffice
                {
                    IdOffice = officeId,
                    IdStaff = staffId
                };
                _db.StaffOffice.Add(staffOffice);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }

}
