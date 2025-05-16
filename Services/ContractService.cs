using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GuvenPortAPI.Models;
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
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task<List<Contract>> GetAllContractsAsync()
        {
            return await _context.Contracts.ToListAsync();
        }

        public async Task<Contract> GetContractByIdAsync(int id)
        {
            return await _context.Contracts.FindAsync(id);
        }

        public async Task<Contract> UpdateContractAsync(Contract contract)
        {
            _context.Contracts.Update(contract);
            await _context.SaveChangesAsync();
            return contract;
        }

        public async Task<bool> DeleteContractAsync(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return false;
            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
