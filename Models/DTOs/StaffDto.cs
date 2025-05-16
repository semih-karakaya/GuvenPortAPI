// DTOs/StaffDto.cs
public class StaffDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Ssn { get; set; }
    public string? Mail { get; set; }
    public DateOnly? Dob { get; set; }
    public bool? Active { get; set; }
}
