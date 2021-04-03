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
    public class ShoppingListController : ControllerBase
    {
        private readonly RecipEaseContext _context;

        public ShoppingListController(RecipEaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns an user's shopping list
        /// </summary>
        /// <remarks>
        /// Functionalities : Retrieves an user's shopping list with according id.
        /// Database: ShoppingList, User.
        /// Constraints: The authenticated user making this request must be the owner of the shopping list.
        /// Query: select * ShoppingList with UserId = userId.
        /// </remarks>
        /// <param name="userId">id of the user who have the shopping list.</param>
        [HttpGet("{userId}")]
        public async Task<ActionResult<ApiShoppingList>> GetApiShoppingList(string userId)
        {
            var apiShoppingList = await _context.ApiShoppingList.FindAsync(userId);

            if (apiShoppingList == null)
            {
                return NotFound();
            }

            return apiShoppingList;
        }

        /// <summary>
        /// Edit an user's shopping list
        /// </summary>
        /// <remarks>
        /// Functionalities : Edit an user's shopping list with accordinging id.
        /// Database: ShoppingList, User.
        /// Constraints: The authenticated user making this request must be the owner of the shopping list.
        /// Query: update ShoppingList Set (some values) where UserId = userId
        /// </remarks>
        /// <param name="userId">id of the user who have the shopping list.</param>
        [HttpPut("{userId}")]
        [Consumes("application/json")]
        public async Task<IActionResult> PutApiShoppingList(string userId, ApiShoppingList apiShoppingList)
        {
            if (userId != apiShoppingList.UserId)
            {
                return BadRequest();
            }

            _context.Entry(apiShoppingList).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApiShoppingListExists(userId))
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

        private bool ApiShoppingListExists(string id)
        {
            return _context.ApiShoppingList.Any(e => e.UserId == id);
        }
    }
}
