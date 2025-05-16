public class EmployeeMedicalStatusDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public DateOnly? LastValidExam { get; set; }
    public string Status { get; set; }
}
