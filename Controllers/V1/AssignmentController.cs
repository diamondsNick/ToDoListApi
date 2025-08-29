using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using ToDoListApi.Data;
using ToDoListApi.Entities;

namespace ToDoListApi.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssignmentController : ControllerBase
    {
        private readonly ToDoDbContext _context;
        private readonly ILogger<AssignmentController> _logger;
        private readonly IMapper _mapper;

        public AssignmentController(
            ToDoDbContext context,
            IMapper mapper,
            ILogger<AssignmentController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAssignments(
            [FromQuery] long? id,
            [FromQuery] string? text,
            [FromQuery] long? statusId,
            [FromQuery] long? pageId,
            [FromQuery] DateTime? creationDate,
            [FromQuery] DateTime? completionDate,
            [FromQuery] DateTime? completionDeadline,
            [FromQuery] int? page,
            [FromQuery] int? pageAmount,
            [FromQuery] string? sortBy = "id",
            [FromQuery] string? order = "asc")
        {
            var query = _context.Assignments.AsQueryable();

            if (id.HasValue)
            {
                query = query.Where(a => a.Id == id);
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                query = query.Where(a => a.Text == text);
            }

            if (statusId.HasValue)
            {
                query = query.Where(a => a.StatusId == statusId);
            }

            if (pageId.HasValue)
            {
                query = query.Where(a => a.PageId == pageId);
            }

            if (creationDate.HasValue)
            {
                query = query.Where(a => a.CreationDate == creationDate);
            }

            if (completionDate.HasValue)
            {
                query = query.Where(a => a.CompletionDate == completionDate);
            }

            if (completionDeadline.HasValue)
            {
                query = query.Where(a => a.CompletionDeadline == completionDeadline);
            }

            sortBy = sortBy?.ToLower();
            order = order?.ToLower();

            if (order != "" && order != "asc" && order != "desc")
            {
                return BadRequest("Order param must be asc or desc.");
            }

            if (sortBy != "" && order != "")
            {
                switch (sortBy)
                {
                    case "id":
                        {
                            query = order == "desc" ? query.OrderByDescending(a => a.Id) : query.OrderBy(a => a.Id);
                            break;
                        }

                    case "statusid":
                        {
                            query = order == "desc" ? query.OrderByDescending(a => a.StatusId) : query.OrderBy(a => a.StatusId);
                            break;
                        }

                    case "pageid":
                        {
                            query = order == "desc" ? query.OrderByDescending(a => a.PageId) : query.OrderBy(a => a.PageId);
                            break;
                        }

                    case "creationDate":
                        {
                            query = order == "desc" ? query.OrderByDescending(a => a.CreationDate) : query.OrderBy(a => a.CreationDate);
                            break;
                        }

                    default:
                        {
                            return BadRequest("OrderBy was not specified correctly.");
                        }
                }
            }

            var res = await query.ToListAsync();

            if (id.HasValue && !res.Any()) return NotFound();

            var converted = _mapper.Map<List<Assignment>>(res);

            return Ok(converted);
        }
    }
}
