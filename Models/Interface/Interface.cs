
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GuvenPortAPI.Models.Interface
{
    public interface IOfficeService
    {
        Task<Office> AddOfficeService(Office office);
        Task<List<vmOfficeDetails>> ListOfficeService();
        Task<vmOfficeDetails> GetOneOfficeFromID(int id);
        Task<Office> EditOfficeService(Office office);
        Task<bool> DeleteOfficeService(int id);
        Task<bool> AddStaffToOffice(int officeId, int staffId);
    }
    public interface IWorkplaceService
    {
        Task<Workplace> AddWorkplaceService(Workplace workplace);
        Task<List<Workplace>> ListWorkplaceService();
        Task<Workplace> GetOneWorkplaceFromID(int id);
        Task<Workplace> EditWorkplaceService(Workplace workplace);
        Task<bool> DeleteWorkplaceService(int id);
        Task<bool> AddStaffToWorkplace(int workplaceId, int staffId);
        Task<bool> RemoveStaffFromWorkplace(int workplaceId, int staffId);
        Task<Workplace> FindWorkplaceFromOfficeId(int officeId);

    }
   


}