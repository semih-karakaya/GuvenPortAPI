// DTOs/MedicalExaminationDto.cs
public class MedicalExaminationDto
{
    public int Id { get; set; }
    public DateOnly? Date { get; set; }
    public bool? IsDisabled { get; set; }
    public DateOnly? ValidityDate { get; set; }
    public int? IdContract { get; set; }
    public int? IdDoctor { get; set; }
    public bool? ExFilePrinted { get; set; }
    public bool? ExIbys { get; set; }
    public string? ExFileLocation { get; set; }
    public bool? ExFilePrintedUploaded { get; set; }
    public ExamType? ExamType { get; set; }
}
