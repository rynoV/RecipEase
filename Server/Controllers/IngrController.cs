using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipEase.Server.Data;
using RecipEase.Shared.Models;
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
        ///
        /// functionalities : Retrieves all ingredients. 
        /// 
        /// database: Ingredient
        /// 
        /// constraints: no constraints
        /// 
        /// query: select * in Ingredient
        /// 
        /// </remarks>

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiIngredient>>> GetApiIngredient()
        {

            return await _context.Ingredient.Select(ing => ing.ToApiIngredient()).ToListAsync();
        }

        /// <summary>
        /// Returns the ingredients in Ingredient with the given id
        /// </summary>
        /// <remarks>
        ///
        /// functionalities : Retrieves 1 row of ingredient
        /// 
        /// database: Ingredient
        /// 
        /// constraints: no constraints
        /// 
        /// query: select * in Ingredient where name=id
        /// 
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiIngredient>> GetIngredient(string id)
        {
            var apiIngredient = await _context.Ingredient.FindAsync(id);

            if (apiIngredient == null)
            {
                return NotFound();
            }
            return apiIngredient.ToApiIngredient();
        }


        private bool IngredientExists(string id)
        {
            return _context.Ingredient.Any(e => e.Name == id);
        }
    }
}
