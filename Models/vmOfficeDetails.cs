using GuvenPortAPI.Models;
using Newtonsoft.Json;
namespace GuvenPortAPI.Models
{
    public class vmOfficeDetails
    {
        public int Id { get; set; }
        public string OfficeName { get; set; }
        public string Address { get; set; }
        public string Crm { get; set; }
        public string ManagerName { get; set; }
        public int NumberOfStaff { get; set; }
        public List<Staff> StaffInOffice { get; set; }
        public List<Workplace> WorkplacesInOffice { get; set; }
    }
}
