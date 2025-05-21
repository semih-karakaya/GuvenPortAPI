namespace GuvenPortAPI.Models.DTOs
{
    public class ContractWithEmployeesDto
    {
        public int Id { get; set; }
        public List<EmployeeDto2> Employees { get; set; }
    }

    public class EmployeeDto2
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
