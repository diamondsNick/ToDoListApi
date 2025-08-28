using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListApi.Data;
using ToDoListApi.Entities;
using ToDoListApi.Models;

namespace ToDoListApi.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentPageController : ControllerBase
    {
        private readonly ToDoDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AssignmentPageController> _logger;

        public AssignmentPageController(
            ToDoDbContext context,
            IMapper mapper,
            ILogger<AssignmentPageController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAssignedPages([FromHeader] long? pageId)
        {
            var query = _context.AssigmentPages.AsQueryable();

            if (pageId is not null)
            {
                query = query.Where(ap => ap.PageId == pageId);
            }

            var res = await query.ToListAsync();

            if (!res.Any()) return NotFound("No records in DB were found");

            var compRes = _mapper.Map<List<AssignmentPageDTO>>(res);

            return Ok(compRes);
        }

        [HttpPost]
        public async Task<ActionResult> PostAssingedPage([FromBody] AssignmentPageDTO assignmentPage)
        {
            if (!await _context.Assignments.AnyAsync(a => a.Id == assignmentPage.AssignmentId))
            {
                return BadRequest("Assignment with this Id does not exist.");
            }

            if (!await _context.Pages.AnyAsync(p => p.Id == assignmentPage.PageId))
            {
                return BadRequest("Page with this Id does not exist.");
            }

            if (await _context.AssigmentPages.FirstOrDefaultAsync(ap =>
                ap.AssignmentId == assignmentPage.AssignmentId
                && ap.PageId == assignmentPage.PageId) != null)
            {
                return Conflict("Object already exists!");
            }

            try
            {
                var res = _mapper.Map<AssigmentPage>(assignmentPage);
                _context.AssigmentPages.Add(res);
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occured while saving new assignment page.");
                return Problem("Internal DB error.", statusCode:500);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured while processing new assignment page.");
                return Problem("Internal server error.", statusCode: 500);
            }

            return CreatedAtAction(nameof(PostAssingedPage), assignmentPage);
        }

    }
}
