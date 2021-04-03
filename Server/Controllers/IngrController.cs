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
    public class IngrController : ControllerBase
    {
        private readonly RecipEaseContext _context;

        public IngrController(RecipEaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all the ingredients in Ingredient
        /// </summary>
        /// <remarks>
        /// Functionalities : Retrieves all ingredients.
        /// Database: Ingredient.
        /// Constraints: no constraints.
        /// Query: Select * in Ingredient.
        /// </remarks>

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiIngredient>>> GetApiIngredient()
        {
            return await _context.ApiIngredient.ToListAsync();
        }

        /// <summary>
        /// Returns the ingredients in Ingredient with the given id
        /// </summary>
        /// <remarks>
        /// Functionalities : Retrieves 1 row of ingredient.
        /// Database: Ingredient.
        /// Constraints: No constraints.
        /// Query: Select * in Ingredient where name=id.
        /// </remarks>
        /// <param name="id">id of the specific ingredient</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiIngredient>> GetApiIngredient(string id)
        {
            var apiIngredient = await _context.ApiIngredient.FindAsync(id);

            if (apiIngredient == null)
            {
                return NotFound();
            }

            return apiIngredient;
        }


        private bool ApiIngredientExists(string id)
        {
            return _context.ApiIngredient.Any(e => e.Name == id);
        }
    }
}
