using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [ApiConventionType(typeof(DefaultApiConventions))]
    [ApiController]
    public class RecipeCollectionController : ControllerBase
    {
        private readonly RecipEaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RecipeCollectionController(RecipEaseContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get recipe collections.
        /// </summary>
        /// <remarks>
        ///
        /// Returns the recipe collections of the user with given `userId`.
        ///
        /// This endpoint interacts with the `recipecollection` table.
        ///
        /// The endpoint performs a `select *` query with a `where` clause to
        /// find the specified recipe collections.
        ///
        /// </remarks>
        /// <param name="userId">The id of the customer whose recipe collections
        /// should be returned.</param>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiRecipeCollection>>> GetRecipeCollection(string userId)
        {
            var query = from c in _context.RecipeCollection where c.UserId == userId select c;
            return await query.Select(c => c.ToApiRecipeCollection()).ToListAsync();
        }

        /// <summary>
        /// Create a recipe collection.
        /// </summary>
        /// <remarks>
        ///
        /// Adds the given recipe collection to the database, and returns it on
        /// success. The `title` must be unique across all recipe collections
        /// for the given user; if it isn't an error code will be returned.
        ///
        /// The customer specified by `userId` must be the authenticated user
        /// making this request.
        ///
        /// This endpoint interacts with the `recipecollection` and `customer`
        /// tables. The `UserId` attribute on the `customer` table will be
        /// checked against the `userId`.
        ///
        /// The endpoint will perform an `insert` command on the
        /// `recipecollection` table to add the recipe collection, and foreign
        /// key constraints will be relied upon to validate the `userId`.
        ///
        /// </remarks>
        [HttpPost]
        [Authorize]
        [Consumes("application/json")]
        public async Task<ActionResult<ApiRecipeCollection>> PostRecipeCollection(ApiRecipeCollection apiRecipeCollection)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (currentUserId != apiRecipeCollection.UserId)
            {
                return Unauthorized();
            }

            var collection = new RecipeCollection
            {
                UserId = apiRecipeCollection.UserId,
                Title = apiRecipeCollection.Title,
                Description = apiRecipeCollection.Description,
                Visibility = apiRecipeCollection.Visibility
            };
            _context.RecipeCollection.Add(collection);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RecipeCollectionExists(collection))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRecipeCollection", new { id = apiRecipeCollection.UserId }, apiRecipeCollection);
        }

        /// <summary>
        /// Delete a recipe collection.
        /// </summary>
        /// <remarks>
        ///
        /// Deletes the given recipe collection in the database.
        ///
        /// If the recipe collection does not exist in the database, a 404
        /// status code is returned. The customer specified by `UserId` in the
        /// found recipe collection must be the authenticated user making this
        /// request.
        ///
        /// This endpoint interacts with the `recipecollection` and `customer`
        /// tables. The `UserId` attribute on the `customer` table will be
        /// checked against the `UserId` from the `recipecollection` table.
        ///
        /// The endpoint will perform a `select` query on the `customer` table
        /// to validate the `UserId`, and a `delete` command on the
        /// `recipecollection` table to delete the recipe collection.
        ///
        /// </remarks>
        /// <param name="title">The title of the recipe collection to delete.</param>
        [HttpDelete]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteRecipeCollection(string title)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var query = from c in _context.RecipeCollection
                where c.UserId == currentUserId && c.Title == title
                select c;
            var recipeCollection = await query.FirstOrDefaultAsync();
            
            if (recipeCollection == null)
            {
                return NotFound();
            }

            _context.RecipeCollection.Remove(recipeCollection);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool RecipeCollectionExists(RecipeCollection c)
        {
            return _context.RecipeCollection.Any(e => e.UserId == c.UserId && e.Title == c.Title);
        }
    }
}

