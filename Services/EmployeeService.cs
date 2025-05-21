using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GuvenPortAPI.Models;
using GuvenPortAPI.Models.Interface;

namespace GuvenPortAPI.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly isgportalContext _context;

        public EmployeeService(isgportalContext context)
        {
            _context = context;
        }

        public async Task<EmployeeDto> AddEmployeeAsync(EmployeeDto employee)
        {
            // Map EmployeeDto to Employee
            var employeeEntity = new Employee
            {
                Id = employee.Id,
                Name = employee.Name,
                Ssn = employee.Ssn,
                Sex = employee.Sex,
                BloodT = employee.BloodT,
                Dob = employee.Dob,
                Disable = employee.Disable,
                Active = employee.Active,
                EntryDate = employee.EntryDate,
                Chronic = employee.Chronic
            };

            _context.Employee.Add(employeeEntity);
            await _context.SaveChangesAsync();

            // Map back to EmployeeDto if needed
            return new EmployeeDto
            {
                Id = employeeEntity.Id,
                Name = employeeEntity.Name,
                Ssn = employeeEntity.Ssn,
                Sex = employeeEntity.Sex,
                BloodT = employeeEntity.BloodT,
                Dob = employeeEntity.Dob,
                Disable = employeeEntity.Disable,
                Active = employeeEntity.Active,
                EntryDate = employeeEntity.EntryDate,
                Chronic = employeeEntity.Chronic
            };
        }

        public async Task<List<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _context.Employee.ToListAsync();
            var employeeDtos = new List<EmployeeDto>();
            foreach (var employee in employees)
            {
                employeeDtos.Add(new EmployeeDto
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Ssn = employee.Ssn,
                    Sex = employee.Sex,
                    BloodT = employee.BloodT,
                    Dob = employee.Dob,
                    Disable = employee.Disable,
                    Active = employee.Active,
                    EntryDate = employee.EntryDate,
                    Chronic = employee.Chronic
                });
            }
            return employeeDtos;
        }

        public async Task<EmployeeDto> GetEmployeeByIdAsync(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee == null) return null;
            return new EmployeeDto
            {
                Id = employee.Id,
                Name = employee.Name,
                Ssn = employee.Ssn,
                Sex = employee.Sex,
                BloodT = employee.BloodT,
                Dob = employee.Dob,
                Disable = employee.Disable,
                Active = employee.Active,
                EntryDate = employee.EntryDate,
                Chronic = employee.Chronic
            };
        }

        public async Task<EmployeeDto> UpdateEmployeeAsync(EmployeeDto employee)
        {
            // Map EmployeeDto to Employee
            var employeeEntity = new Employee
            {
                Id = employee.Id,
                Name = employee.Name,
                Ssn = employee.Ssn,
                Sex = employee.Sex,
                BloodT = employee.BloodT,
                Dob = employee.Dob,
                Disable = employee.Disable,
                Active = employee.Active,
                EntryDate = employee.EntryDate,
                Chronic = employee.Chronic
            };

            _context.Employee.Update(employeeEntity);
            await _context.SaveChangesAsync();

            // Map back to EmployeeDto if needed
            return new EmployeeDto
            {
                Id = employeeEntity.Id,
                Name = employeeEntity.Name,
                Ssn = employeeEntity.Ssn,
                Sex = employeeEntity.Sex,
                BloodT = employeeEntity.BloodT,
                Dob = employeeEntity.Dob,
                Disable = employeeEntity.Disable,
                Active = employeeEntity.Active,
                EntryDate = employeeEntity.EntryDate,
                Chronic = employeeEntity.Chronic
            };
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee == null) return false;
            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
