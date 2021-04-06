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
    [ApiConventionType(typeof(DefaultApiConventions))]
    [ApiController]
    public class RecipeRatingController : ControllerBase
    {
        private readonly RecipEaseContext _context;

        public RecipeRatingController(RecipEaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns the (user) rating of the recipe under consideration
        /// </summary>
        /// <remarks>
        ///
        /// In the case of a specific customers rating, it
        /// retrieves the object with the given recipeID value and customer userID,
        /// from the recipeID and userID columns, from the
        /// `RecipeRating` table, if the entry exists. In this case the following query applies:
        /// A 'select rating' query with a 'where userId=x and recipeId=y' clause  to find a users
        /// rating from the RecipeRating table
        ///
        /// In the case of a recipe's rating, it
        /// retrieves all objects with the given recipeID value
        /// from the recipeID column, from the
        /// `RecipeRating` table, if the entry exists. In this case the following query applies:
        ///
        /// A 'select rating' query with a 'where' clause to find the receipeID's ratings from the
        /// RecipeRating table
        /// </remarks>
        /// <param name="userId">The Username of the Customer to retrieve.</param>
        /// <param name="recipeId">The recipeID of the recipe to retrieve.</param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<IEnumerable<ApiRecipeRating>>> GetRecipeRating(string userId, int recipeId)
        {
            if (userId == null)
            {
                //get rows that match the recipeID
                var ratings = 
                    from c in _context.RecipeRating
                    where c.RecipeId == recipeId
                    select c;
                return await ratings.Select(x => x.ToApiRecipeRating() ).ToListAsync();
            }

            //else return a recipe rating of the current customer user
            var userRating =
                from c in _context.RecipeRating
                where c.UserId == userId && c.RecipeId == recipeId
                select c;
            return await userRating.Select(x => x.ToApiRecipeRating() ).ToListAsync();
        }
        
        /// <summary>
        /// Create a recipe rating.
        /// </summary>
        /// <remarks>
        ///
        /// Adds the given recipe rating to the database, and returns it on
        /// success.
        ///
        /// The customer specified by `userId` must be the authenticated user
        /// making this request. The recipe specified by `recipeId` must exist
        /// in the database.
        ///
        /// This endpoint interacts with the `reciperating`, `recipe`, and
        /// `customer` tables. The `UserId` attribute on the `customer` table
        /// will be checked against the `userId`. The `Id` attribute on the
        /// `recipe` table will be checked against the `recipeId`.
        ///
        /// The endpoint will perform an `insert` command on the `reciperating`
        /// table to add the recipe rating, and the foreign key constraints will
        /// be relied upon to validate the `recipeId` and `userId`.
        ///
        /// </remarks>
        [HttpPost]
        [Consumes("application/json")]
        public async Task<ActionResult<ApiRecipeRating>> PostRecipeRating(ApiRecipeRating apiRecipeRating)
        {
            _context.ApiRecipeRating.Add(apiRecipeRating);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ApiRecipeRatingExists(apiRecipeRating.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetApiRecipeRating", new { id = apiRecipeRating.UserId }, apiRecipeRating);
        }

        private bool ApiRecipeRatingExists(string id)
        {
            return _context.ApiRecipeRating.Any(e => e.UserId == id);
        }
    }
}
