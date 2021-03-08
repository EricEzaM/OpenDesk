using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenDesk.Domain.Entities;
using OpenDesk.Infrastructure.Persistence;

namespace OpenDesk.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesksController : ControllerBase
    {
        private readonly OpenDeskDbContext _context;

        public DesksController(OpenDeskDbContext context)
        {
            _context = context;
        }

        // GET: api/Desks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Desk>>> GetDesks()
        {
            return await _context.Desks.Include(d => d.OfficeLocation).ToListAsync();
        }

        // GET: api/Desks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Desk>> GetDesk(string id)
        {
            var desk = await _context.Desks.FindAsync(id);

            if (desk == null)
            {
                return NotFound();
            }

            return desk;
        }

        // PUT: api/Desks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDesk(string id, Desk desk)
        {
            if (id != desk.Id)
            {
                return BadRequest();
            }

            _context.Entry(desk).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeskExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Desks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Desk>> PostDesk(Desk desk)
        {
            _context.Desks.Add(desk);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDesk", new { id = desk.Id }, desk);
        }

        // DELETE: api/Desks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDesk(string id)
        {
            var desk = await _context.Desks.FindAsync(id);
            if (desk == null)
            {
                return NotFound();
            }

            _context.Desks.Remove(desk);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeskExists(string id)
        {
            return _context.Desks.Any(e => e.Id == id);
        }
    }
}
