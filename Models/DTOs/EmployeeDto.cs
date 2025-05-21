namespace GuvenPortAPI.Models
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Ssn { get; set; }
        public string? Sex { get; set; }
        public string? BloodT { get; set; }
        public DateOnly? Dob { get; set; }
        public bool? Disable { get; set; }
        public bool? Active { get; set; }
        public DateOnly? EntryDate { get; set; }
        public bool? Chronic { get; set; }
        // Add navigation properties if needed, but without [JsonIgnore]
    }
}
