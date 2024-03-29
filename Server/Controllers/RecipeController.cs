using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipEase.Server.Data;
using RecipEase.Shared.Models.Api;
using RecipEase.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;




namespace RecipEase.Server.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [ApiController]
  
    public class RecipeController : ControllerBase
    {
        private readonly RecipEaseContext _context;

        private readonly IHttpContextAccessor _httpContextAccessor;


        public RecipeController(RecipEaseContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get a recipe.
        /// </summary>
        /// <remarks>
        ///
        /// Returns the recipe with the given `id`, or gives a 404 status code
        /// if it doesn't exist in the database. The recipe will include it's
        /// average rating if it has been rated.
        /// 
        /// This endpoint interacts with all attributes from the `recipe` table,
        /// and with the `RecipeId` and `Rating` attributes from the
        /// `reciperating` table.
        /// 
        /// The endpoint performs a `select *` query with a `where` clause
        /// to find the specified recipe. If the recipe was found, the
        /// `reciperating` table is queried for rows with `RecipeId` matching
        /// the found id, and the `Rating` attribute is collected.
        ///
        /// </remarks>
        /// <param name="id">The id of the recipe to return.</param>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiRecipe>> GetRecipe(int id)
        {
            var recipe = await _context.Recipe.FindAsync(id);

            if (recipe == null)
            {
                return NotFound();
            }

            return recipe.ToApiRecipe();
        }

        /// <summary>
        /// Get a list of recipes.
        /// </summary>
        /// <remarks>
        ///
        /// Returns a list of recipes from the database, optionally filtered by
        /// title, category, and/or user id.
        ///
        /// This endpoint interacts with all attributes in the `recipe` and
        /// `recipeincategory` tables, and the `UserId` attribute in the
        /// `Customer` table.
        ///
        /// The endpoint performs a `select *` query, with a `where` clause
        /// included when necessary to apply the specified filters. The `recipe`
        /// table is optionally joined with `recipeincategory` to filter by
        /// category. The `recipe` table is optionally joined with the
        /// `customer` table to filter by customer.
        ///
        /// </remarks>
        /// <param name="titleMatch">String to filter recipes by title. If
        /// provided, only recipes with the filter string in their title (case
        /// insensitive) will be returned.</param>
        /// <param name="categoryName">String to filter recipes by category. If
        /// provided, only recipes in the given category will be returned. If
        /// there is no category with the given name in the database, no recipes
        /// will be returned.</param>
        /// <param name="userId">Get only recipes authored by the customer with this
        /// id.</param>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApiRecipe>>> GetRecipes(string titleMatch, string categoryName,
            string userId)
        {
            var query = _context.Recipe.AsQueryable();
            if (userId != null)
            {
                query = query.Where(recipe => recipe.AuthorId == userId);
            }

            if (titleMatch != null)
            {
                query = query.Where(recipe => recipe.Name.ToLower().Contains(titleMatch.ToLower()));
            }

            if (categoryName != null)
            {
                query = from recipe in query
                        join cat in _context.RecipeInCategory on recipe.Id equals cat.RecipeId
                        where cat.CategoryName == categoryName
                        select recipe;
            }

            return await query.Select(recipe => recipe.ToApiRecipe()).ToListAsync();
        }

        /// <summary>
        /// Edit a recipe.
        /// </summary>
        /// <remarks>
        ///
        /// Updates the given recipe in the database. The `id` in the url must
        /// match the `id` in the recipe.
        ///
        /// The customer specified by `authorId` must be the authenticated user
        /// making this request.
        ///
        /// This endpoint interacts with the `recipe` and `customer` tables. The
        /// `UserId` attribute on the `customer` table will be checked against
        /// the `authorId`.
        ///
        /// The endpoint will perform an `update` command on the `recipe` table
        /// to update the recipe, and foreign key constraints will be relied
        /// upon to validate the `authorId`.
        ///
        /// </remarks>
        /// <param name="id">The id of the recipe to update.</param>
        /// <param name="apiRecipe"></param>
        [HttpPut("{id}")]
        [Consumes("application/json")]
        [Authorize]
        public async Task<IActionResult> PutRecipe(int id, Recipe apiRecipe)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != apiRecipe.AuthorId)
            {
                return Unauthorized();
            }

            if (id != apiRecipe.Id)
            {
                return BadRequest();
            }

            _context.Entry(apiRecipe).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RecipeExists(id))
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
        /// Create a recipe.
        /// </summary>
        /// <remarks>
        ///
        /// Adds the given recipe to the database, and returns it on success. If
        /// the `id` is specified for the recipe, the endpoint will attempt to
        /// add the recipe to the database with that `id`. If a recipe with the
        /// given `id` already exists, an error code is returned. If the `id` is
        /// not specified, the `id` is automatically generated for the given
        /// recipe.
        ///
        /// The customer specified by `authorId` must be the authenticated user
        /// making this request.
        ///
        /// This endpoint interacts with the `recipe` and `customer` tables. The
        /// `UserId` attribute on the `customer` table will be checked against
        /// the `authorId`.
        ///
        /// The endpoint will perform an `insert` command on the `recipe` table
        /// to add the recipe, and foreign key constraints will be relied upon
        /// to validate the `authorId`.
        ///
        /// </remarks>
        [HttpPost]
        [Authorize]
        [Consumes("application/json")]
        public async Task<ActionResult<Recipe>> PostRecipe(Recipe apiRecipe)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != apiRecipe.AuthorId)
            {
                return Unauthorized();
            }

            _context.Recipe.Add(apiRecipe);


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RecipeExists(apiRecipe.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetRecipes", new { id = apiRecipe.Id }, apiRecipe);
        }

        /// <summary>
        /// Delete a recipe.
        /// </summary>
        /// <remarks>
        ///
        /// Deletes the given recipe in the database.
        ///
        /// If the recipe does not exist in the database, a 404 status code is
        /// returned. The customer specified by `AuthorId` in the found recipe
        /// must be the authenticated user making this request.
        ///
        /// This endpoint interacts with the `recipe` and `customer` tables. The
        /// `UserId` attribute on the `customer` table will be checked against
        /// the `AuthorId` from the `recipe` table.
        ///
        /// The endpoint will perform a `select` query on the `customer` table
        /// to validate the `authorId`, and a `delete` command on the `recipe`
        /// table to delete the recipe.
        ///
        /// </remarks>
        /// <param name="id">The id of the recipe to update.</param>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var apiRecipe = await _context.Recipe.FindAsync(id);
            if (apiRecipe == null)
            {
                return NotFound();
            }

            if (userId != apiRecipe.AuthorId)
            {
                return Unauthorized();
            }


            _context.Recipe.Remove(apiRecipe);
            await _context.SaveChangesAsync();

            return Ok();

        }

        private bool RecipeExists(int id)
        {
            return _context.Recipe.Any(e => e.Id == id);
        }
    }
}