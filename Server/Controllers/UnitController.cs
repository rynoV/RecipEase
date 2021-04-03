using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipEase.Server.Data;
using RecipEase.Shared.Models.Api;

namespace RecipEase.Server.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        private readonly RecipEaseContext _context;

        public UnitController(RecipEaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves every unit
        /// </summary>
        /// <remarks>
        /// functionalities : retrieve all unit in Unit table
        /// database: Unit
        /// constraints: no constraints
        /// query: Select * from Unit
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiUnit>>> GetApiUnit()
        {
            return await _context.ApiUnit.ToListAsync();
        }

        /// <summary>
        /// Get a unit with the specified name
        /// </summary>
        /// <remarks>
        /// functionalities : retrieve the unit with the specified id
        /// database: Unit
        /// constraints: no constraints
        /// query: select * from Unit where Name = id
        /// </remarks>
        /// <param name="id">name of the unit to be retrieved.</param>

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiUnit>> GetApiUnit(string id)
        {
            var apiUnit = await _context.ApiUnit.FindAsync(id);

            if (apiUnit == null)
            {
                return NotFound();
            }

            return apiUnit;
        }


        private bool ApiUnitExists(string id)
        {
            return _context.ApiUnit.Any(e => e.Name == id);
        }
    }
}
