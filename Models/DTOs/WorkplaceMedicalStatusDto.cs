public class WorkplaceMedicalStatusDto
{
    public int WorkplaceId { get; set; }
    public string WorkplaceName { get; set; }
    public DateOnly? LastValidExam { get; set; }
    public string Status { get; set; }
}
