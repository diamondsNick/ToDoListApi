using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListApi.Data;
using ToDoListApi.Entities;
using ToDoListApi.Models;

namespace ToDoListApi.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly ToDoDbContext _context;
        private readonly ILogger<PageController> _logger;
        private readonly IMapper _mapper;

        public PageController(ToDoDbContext context, ILogger<PageController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetPages
        (
            [FromQuery] long? pageId,
            [FromQuery] string? pageName,
            [FromQuery] long? userId,
            [FromQuery] DateTime? creationDate,
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            [FromQuery] string? sortBy = "id",
            [FromQuery] string? order = "asc"
        )
        {
            var res = _context.Pages.AsQueryable();

            if (pageId.HasValue)
            {
                res = res.Where(p => p.Id == pageId);
            }

            if (!string.IsNullOrWhiteSpace(pageName))
            {
                res = res.Where(p => p.PageName == pageName);
            }

            if (userId.HasValue) 
            { 
                res = res.Where(p => p.UserId == userId);
            }

            if (creationDate.HasValue)
            {
                res = res.Where(p => p.CreationDate == creationDate.Value.Date);
            }

            sortBy = string.IsNullOrEmpty(sortBy) ? sortBy : sortBy.ToLower();
            order = string.IsNullOrEmpty(order) ? order : order.ToLower();

            if (order != "asc" && order != "desc") order = "asc";

            switch (sortBy)
            {
                case "id":
                    res = order == "desc" ? res.OrderByDescending(p => p.Id) : res.OrderBy(p => p.Id);
                    break;

                case "name":
                    res = order == "desc" ? res = res.OrderByDescending(p => p.PageName) : res.OrderBy(p => p.PageName);
                    break;

                case "pageName":
                    res = order == "desc" ? res = res.OrderByDescending(p => p.PageName) : res.OrderBy(p => p.PageName);
                    break;

                case "date":
                    res = order == "desc" ? res = res.OrderByDescending(p => p.CreationDate) : res.OrderBy(p => p.CreationDate);
                    break;

                default:
                    return BadRequest("SortBy has incorrect value!");
            }

            var result = new List<Page>();

            if (page.HasValue && pageSize.HasValue)
            {
                result = await res
                .Skip((page.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToListAsync();
            }
            else result = await res.ToListAsync();

            if (!result.Any())
                return NotFound();

            var mappedRes = _mapper.Map<List<PageDTO>>(result);

            PagedResult<PageDTO> fin = new PagedResult<PageDTO> { TotalAmount = result.Count, Items = mappedRes };

            return Ok(fin);
        }

        [HttpPost]
        public async Task<ActionResult<PageDTO>> PostPage([FromBody] PageDTO clPage)
        {
            if (clPage.Id != 0) return BadRequest("Id was specified.");

            if (await PageExists(clPage)) return Conflict("Page already exists.");

            if (clPage.UserId < 1 || !await _context.Users.AnyAsync(u => u.Id == clPage.UserId)) return BadRequest("User with this Id does not exist.");

            if (await _context.Pages.AnyAsync(p => p.UserId == clPage.UserId && p.PageName == clPage.PageName)) return Conflict("Page with this name already exists!");

            var res = _mapper.Map<Page>(clPage);

            try
            {
                await _context.Pages.AddAsync(res);
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occured while saving new page.");
                return Problem("Internal DB error.", statusCode: 500);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occured while saving new page.");
                return Problem("Internal server error.", statusCode: 500);
            }

            clPage.Id = res.Id;

            return CreatedAtAction(nameof(PostPage), new { clPage });
        }

        [HttpPatch("{Id:long}")]
        public async Task<ActionResult> PatchPage([FromRoute] long Id, [FromBody] PageDTO page)
        {
            if (Id != page.Id) return BadRequest("Ids does not match.");

            var found = await _context.Pages.FirstOrDefaultAsync(p => p.Id == page.Id && p.UserId == page.UserId);

            if (found == null) return NotFound("Page does not exists.");

            if (found.PageName != page.PageName)
            {
                if (await _context.Pages.AnyAsync(p => p.PageName == page.PageName && p.UserId == page.UserId))
                    return Conflict("Page with this name already exists.");
                else
                    found.PageName = page.PageName;
            }

            var res = _mapper.Map<Page>(page);

            try
            {
                _context.Entry(res).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occured while saving page.");
                return Problem("Internal DB error.", statusCode: 500);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occured while saving page.");
                return Problem("Internal server error.", statusCode: 500);
            }

            return NoContent();
        }

        [HttpDelete("{Id:long}")]
        public async Task<ActionResult> DeletePage([FromRoute] long Id)
        {
            var deleted = await _context.Pages.FindAsync(Id);

            if (deleted == null) return NotFound("Page does not exist.");

            if (await _context.Assignments.AnyAsync(p => p.PageId == Id))
            {
                try
                {
                    _context.Assignments.RemoveRange(_context.Assignments.Where(p => p.PageId == Id));
                    await _context.SaveChangesAsync();
                }

                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error occured while deleting assignments for page.");
                    return Problem("Internal DB error.", statusCode: 500);
                }

                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error occured while deleting assignments from page.");
                    return Problem("Internal server error.", statusCode: 500);
                }
            }

            try
            {
                _context.Pages.Remove(deleted);
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occured while deleting page.");
                return Problem("Internal DB error.", statusCode: 500);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occured while deleting page.");
                return Problem("Internal server error.", statusCode: 500);
            }

            return NoContent();
        }

        private async Task<bool> PageExists(PageDTO page)
        {
            var res = await _context.Pages
                .AnyAsync(p => p.Id == page.Id ||
                    (p.PageName == page.PageName && p.UserId == page.UserId));
            return res;
        }

    }
}
