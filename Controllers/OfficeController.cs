using guveporrtapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class OfficesController : ControllerBase
{
    private readonly isgportalContext _context;

    public OfficesController(isgportalContext context)
    {
        _context = context;
    }

    // GET: api/Offices
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OfficeSimpleResponse>>> GetOffices()
    {
        return await _context.Offices
            .Select(o => new OfficeSimpleResponse
            {
                Id = o.Id,
                Address = o.Address,
                OName = o.OName,
                Crm = o.Crm,
                Active = o.Active,
                ManagerStaffId = o.IdManagerstaff
            })
            .ToListAsync();
    }

    // GET: api/Offices/5
    [HttpGet("{id}")]
    public async Task<ActionResult<OfficeSimpleResponse>> GetOffice(int id)
    {
        var office = await _context.Offices
            .Where(o => o.Id == id)
            .Select(o => new OfficeSimpleResponse
            {
                Id = o.Id,
                Address = o.Address,
                OName = o.OName,
                Crm = o.Crm,
                Active = o.Active,
                ManagerStaffId = o.IdManagerstaff
            })
            .FirstOrDefaultAsync();

        if (office == null)
        {
            return NotFound();
        }

        return office;
    }

    public class OfficeSimpleResponse
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string OName { get; set; }
        public string Crm { get; set; }
        public bool? Active { get; set; }
        public int? ManagerStaffId { get; set; }
    }
    [HttpPost]
    public async Task<ActionResult<Office>> PostOffice(
    [FromBody] OfficeCreateRequest request)
    {
        var office = new Office
        {
            Address = request.Address,
            OName = request.OName,
            Crm = request.Crm,
            Active = request.Active,
            IdManagerstaff = request.ManagerStaffId
        };

        _context.Offices.Add(office);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetOffice", new { id = office.Id }, office);
    }

    public class OfficeCreateRequest
    {
        public string Address { get; set; }
        public string OName { get; set; }
        public string Crm { get; set; }
        public bool? Active { get; set; }
        public int? ManagerStaffId { get; set; }
    }
}
