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
    public class IngrInShoppingListController : ControllerBase
    {
        private readonly RecipEaseContext _context;

        public IngrInShoppingListController(RecipEaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all the items in an user's shopping list
        /// </summary>
        /// <remarks>
        /// Functionalities : Retrieves ingredients in a user's shopping list.
        /// Database: IngrInShoppingList, Customer.
        /// Constraints: The authenticated user making this request must be the owner of the shopping list.
        /// Query: select * IngrInShoppingList with UserId = id.
        /// </remarks>
        /// <param name="id">id of the user of the shopping list</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ApiIngrInShoppingList>>> GetApiIngrInShoppingList(string id)
        {
            return await _context.ApiIngrInShoppingList.ToListAsync();
        }

        /// <summary>
        /// Returns the specific item in the IngrInShoppingList.
        /// </summary>
        /// <remarks>
        /// Functionalities : Retrieves an item ingredient with a specific userid ,ingredient and unit.
        /// Database: IngrInShoppingList, Customer.
        /// Constraints: The authenticated user making this request must be the owner of the shopping list.
        /// Query: select * IngrInShoppingList with UserId = id and IngrName = iname and UnitName = uname.
        /// </remarks>
        /// <param name="id">id of the user of the shopping list</param>
        /// <param name="uname">specify unit</param>
        /// <param name="iname">specify ingredient</param>

        [HttpGet]
        public async Task<ActionResult<ApiIngrInShoppingList>> GetApiIngrInShoppingList(string id, string uname, string iname)
        {
            var apiIngrInShoppingList = await _context.ApiIngrInShoppingList.FindAsync(id);

            if (apiIngrInShoppingList == null)
            {
                return NotFound();
            }

            return apiIngrInShoppingList;
        }

        /// <summary>
        /// Edit the specific item in the IngrInShoppingList
        /// </summary>
        /// <remarks>
        /// Functionalities : Edit an item ingredient with a specific userid ,ingredient and unit.
        /// Database: IngrInShoppingList, Customer.
        /// Constraints: The authenticated user making this request must be the owner of the shopping list.
        /// query: update * IngrInShoppingList set (some instance) where UserId = id and IngrName = iname and UnitName = uname.
        /// </remarks>
        /// <param name="id">id of the user of the shopping list</param>
        [HttpPut]
        [Consumes("application/json")]
        public async Task<IActionResult> PutApiIngrInShoppingList(string id, ApiIngrInShoppingList apiIngrInShoppingList)
        {
            if (id != apiIngrInShoppingList.UserId)
            {
                return BadRequest();
            }

            _context.Entry(apiIngrInShoppingList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApiIngrInShoppingListExists(id))
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

        /// <summary>
        /// Create new row in IngrInShoppingList
        /// </summary>
        /// <remarks>
        /// Functionalities : Insert a row with a specific userid ,ingredient and unit.
        /// Database: IngrInShoppingList, Customer.
        /// Constraints: The authenticated user making this request must be the owner of the shopping list.
        /// Query: insert into IngrInShoppingList.
        /// </remarks>
        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<ApiIngrInShoppingList>> PostApiIngrInShoppingList(ApiIngrInShoppingList apiIngrInShoppingList)
        {
            _context.ApiIngrInShoppingList.Add(apiIngrInShoppingList);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ApiIngrInShoppingListExists(apiIngrInShoppingList.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetApiIngrInShoppingList", new { id = apiIngrInShoppingList.UserId }, apiIngrInShoppingList);
        }

        /// <summary>
        /// Delete an ingredient in an user's shopping list
        /// </summary>
        /// <remarks>
        /// Functionalities :  Delete a row with a specific userid ,ingredient and unit.
        /// Database: IngrInShoppingList, Customer.
        /// Constraints: The authenticated user making this request must be the owner of the shopping list.
        /// Query: Delete from IngrInShoppingList where userId = userId, ingrName = ingrName, unitName = unitName.
        /// </remarks>

        [HttpDelete]
        [Consumes("application/json")]
        public async Task<IActionResult> DeleteApiIngrInShoppingList(ApiIngrInShoppingList apiIngrInShoppingList)
        {
            if (apiIngrInShoppingList == null)
            {
                return NotFound();
            }

            _context.ApiIngrInShoppingList.Remove(apiIngrInShoppingList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApiIngrInShoppingListExists(string id)
        {
            return _context.ApiIngrInShoppingList.Any(e => e.UserId == id);
        }
    }
}
