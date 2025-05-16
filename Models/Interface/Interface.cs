
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
        public interface IAccidentService
        {
            Task<Accident> AddAccidentAsync(Accident accident);
            Task<List<Accident>> GetAllAccidentsAsync();
            Task<Accident> GetAccidentByIdAsync(int id);
            Task<Accident> UpdateAccidentAsync(Accident accident);
            Task<bool> DeleteAccidentAsync(int id);
        }
    public interface IContractService
    {
        Task<Contract> AddContractAsync(Contract contract);
        Task<List<Contract>> GetAllContractsAsync();
        Task<Contract> GetContractByIdAsync(int id);
        Task<Contract> UpdateContractAsync(Contract contract);
        Task<bool> DeleteContractAsync(int id);
    }
    public interface IEmployeeService
    {
        Task<Employee> AddEmployeeAsync(Employee employee);
        Task<List<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<Employee> UpdateEmployeeAsync(Employee employee);
        Task<bool> DeleteEmployeeAsync(int id);
    }
    public interface IStaffService
    {
        Task<Staff> AddStaffAsync(Staff staff);
        Task<List<Staff>> GetAllStaffAsync();
        Task<Staff> GetStaffByIdAsync(int id);
        Task<Staff> UpdateStaffAsync(Staff staff);
        Task<bool> DeleteStaffAsync(int id);
    }
    public interface IMedicalExaminationService
    {
        Task<MedicalExamination> AddMedicalExaminationAsync(MedicalExamination examination);
        Task<List<MedicalExamination>> GetAllMedicalExaminationsAsync();
        Task<MedicalExamination> GetMedicalExaminationByIdAsync(int id);
        Task<MedicalExamination> UpdateMedicalExaminationAsync(MedicalExamination examination);
        Task<bool> DeleteMedicalExaminationAsync(int id);
    }
}




