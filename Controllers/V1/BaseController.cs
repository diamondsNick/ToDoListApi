using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoListApi.Data;

namespace ToDoListApi.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<TEntity, TDTO, TKey> : ControllerBase
        where TEntity : class
    {
        private readonly ToDoDbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        private readonly IMapper _mapper;

        public BaseController(
            ToDoDbContext context, 
            DbSet<TEntity> dbSet,
            IMapper mapper)
        {
            _context = context;
            _dbSet = dbSet;
            _mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult<TEntity>> GetAllRecords()
        {
            var res = await _dbSet.ToListAsync();

            if (!res.Any()) return NotFound("No records were found");

            var converted = _mapper.Map<TDTO>(res);

            return Ok(converted);
        }
    }
}
