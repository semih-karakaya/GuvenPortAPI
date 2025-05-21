using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GuvenPortAPI.Models;
using GuvenPortAPI.Models.DTOs;
using GuvenPortAPI.Models.Interface;

namespace GuvenPortAPI.Service
{
    public class ContractService : IContractService
    {
        private readonly isgportalContext _context;

        public ContractService(isgportalContext context)
        {
            _context = context;
        }

        public async Task<Contract> AddContractAsync(Contract contract)
        {
            _context.Contract.Add(contract);
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task<List<Contract>> GetAllContractsAsync()
        {
            return await _context.Contract.ToListAsync();
        }

        public async Task<Contract> GetContractByIdAsync(int id)
        {
            return await _context.Contract.FindAsync(id);
        }

        public async Task<Contract> UpdateContractAsync(Contract contract)
        {
            _context.Contract.Update(contract);
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            var contract = await _context.Contract.FindAsync(id);
            if (contract == null) return false;
            _context.Contract.Remove(contract);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<ContractWithEmployeesDto>> GetActiveContractsWithEmployeesByWorkplaceIdAsync(int workplaceId)
        {
            return await _context.Contract
             .Where(c => c.IdWorkplace == workplaceId
                         && (c.EndDate == null || c.EndDate >= DateOnly.FromDateTime(DateTime.Now)))
             .Select(c => new ContractWithEmployeesDto
             {
                 Id = c.Id,
                 Employees = c.IdEmployeeNavigation == null ? new List<EmployeeDto2>() : new List<EmployeeDto2>
                 {
                    new EmployeeDto2
                    {
                        Id = c.IdEmployeeNavigation.Id,
                        Name = c.IdEmployeeNavigation.Name
                    }
                 } // Eğer ilişki varsa, çalışan bilgilerinin alınması
             }).ToListAsync();
        }
    }


}
