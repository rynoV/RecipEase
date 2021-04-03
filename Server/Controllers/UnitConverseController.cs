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
    public class UnitConverseController : ControllerBase
    {
        private readonly RecipEaseContext _context;

        public UnitConverseController(RecipEaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all unit conversion
        /// </summary>
        /// <remarks>
        /// functionalities : retrieve all unitconversion in UnitConversion
        /// database: UnitConversion
        /// constraints: no constraints
        /// query: select * from UnitConversion
        /// </remarks>
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<IEnumerable<ApiUnitConversion>>> GetApiUnitConversion()
        {
            return await _context.ApiUnitConversion.ToListAsync();
        }

        /// <summary>
        /// Get  unit conversion from one unit to another
        /// </summary>
        /// <remarks>
        /// functionalities : retrieve unitconversion in UnitConversion with the specified units from both side
        /// database: UnitConversion, Unit
        /// constraints: two unit must be the same unit type
        /// query: select * from UnitConversion where ConvertsToUnitName = id1 and 
        /// ConvertsFromUnitName = id2
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<ApiUnitConversion>> GetApiUnitConversion(string id1, string id2)
        {
            var apiUnitConversion = _context.ApiUnitConversion.Find(id1, id2);

            if (apiUnitConversion == null)
            {
                return NotFound();
            }

            return apiUnitConversion;
        }



        private bool ApiUnitConversionExists(string id1)
        {
            return _context.ApiUnitConversion.Any(e => e.ConvertsFromUnitName == id1 || e.ConvertsToUnitName == id1);
        }
    }
}
