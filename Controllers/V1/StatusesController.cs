using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Data.Common;
using ToDoListApi.Data;
using ToDoListApi.Entities;
using ToDoListApi.Models;

namespace ToDoListApi.Controllers.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StatusesController : ControllerBase
    {
        private readonly ToDoDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<StatusesController> _logger;

        public StatusesController(ToDoDbContext context, IMapper mapper, ILogger<StatusesController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatuses()
        {
            var res = await _context.Statuses.ToListAsync();

            if (!res.Any())
            {
                return NotFound();
            }

            var dto = _mapper.Map<List<StatusDTO>>(res);

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<StatusDTO>> PostStatus([FromBody] StatusDTO status)
        {
            var newStat = _mapper.Map<Status>(status);

            if (await IsStatusExists(status))
                return Conflict("Object already exists!");

            try
            {
                await _context.Statuses.AddAsync(newStat);
                await _context.SaveChangesAsync();
            }

            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to save new status");
                return BadRequest("Error occured while saving");
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occured");
                return BadRequest("Error occured while processing");
            }

            return Created(nameof(PostStatus), new { Id = newStat.Id, newStat });
        }

        [HttpPut("{Id:long}")]
        public async Task<IActionResult> PutStatus(long Id, [FromBody] StatusDTO status)
        {
            return Ok();
        }

        private async Task<bool> IsStatusExists(StatusDTO stat)
        {
            bool res = await _context.Statuses.AnyAsync(s => s.Id == stat.Id || s.Name == stat.Name);
            return res;
        }
    }
}
