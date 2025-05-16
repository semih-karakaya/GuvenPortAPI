// DTOs/AccidentDto.cs
public class AccidentDto
{
    public int Id { get; set; }
    public DateOnly? AccDate { get; set; }
    public TimeOnly? AccTime { get; set; }
    public bool? Fatality { get; set; }
    public bool? Injury { get; set; }
    public bool? PropertyDamage { get; set; }
    public bool? NearMiss { get; set; }
    public string? StoryOfAccident { get; set; }
    public int? IdWorkplace { get; set; }
    public DateOnly? SgkInfoDate { get; set; }
    public bool? SgkInfoCheck { get; set; }
}
