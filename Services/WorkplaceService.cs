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
                _db.Workplaces.Remove(workplace);
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
