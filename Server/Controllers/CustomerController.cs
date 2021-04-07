using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipEase.Server.Data;
using RecipEase.Shared.Models.Api;
using Microsoft.AspNetCore.Authorization;
using RecipEase.Shared.Models;

namespace RecipEase.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class CustomerController : ControllerBase
    {
        private readonly RecipEaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomerController(RecipEaseContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Returns the Customer with the given username.
        /// </summary>
        /// <remarks>
        ///
        /// Retrieves the object with the given username value, in the username column, from the
        /// `Customer` table, if it exists.
        ///
        /// A 'select*' query with a 'where' clause to find the username
        /// and its associated attributes.
        /// </remarks>
        /// <param name="userId">The Username of the Customer to retrieve.</param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<ApiCustomer>> GetCustomer(string userId)
        {
            var customer = await _context.Customer.FindAsync(userId);

            if (customer == null)
            {
                return NotFound();
            }

            return new ApiCustomer()
            {
                Age = customer.Age, Weight = customer.Weight, CustomerName = customer.CustomerName,
                FavMeal = customer.FavMeal, UserId = customer.UserId
            };
        }

        /// <summary>
        /// Update the information of an existing customer
        /// </summary>
        /// <remarks>
        ///
        /// Updates information/attributes of an existing user of type customer,
        /// if the user exists in the Customer table of the database.
        /// The authenticated user must be the user to be updated.
        ///
        /// An Update operation is used to update the Customer in the database if
        /// the user exists.
        /// </remarks>
        ///<param name="id">The user id of the Customer to update.</param>
        ///<param name="apiCustomer">The Customer object to be updated.</param>
        [HttpPut("{id}")]
        [Consumes("application/json")]
        [Authorize]
        public async Task<IActionResult> PutCustomer(string id, ApiCustomer apiCustomer)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (id != currentUserId)
            {
                return Unauthorized();
            }
            
            if (id != apiCustomer.UserId)
            {
                return BadRequest();
            }

            var customer = new Customer()
            {
                Age = apiCustomer.Age,
                Weight = apiCustomer.Weight,
                FavMeal = apiCustomer.FavMeal,
                UserId = apiCustomer.UserId,
                CustomerName = apiCustomer.CustomerName
            };

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Add a new Customer
        /// </summary>
        /// <remarks>
        ///
        /// Add a new Customer to the Customer relation,
        /// if the username does not exist in the Customer relation of the database.
        /// 
        /// An Insert  operation to insert a new Customer is performed.
        /// </remarks>
        ///<param name="apiCustomer">The Customer object to be updated.</param>
        [HttpPost]
        [Consumes("application/json")]
        [Authorize]
        public async Task<ActionResult<ApiCustomer>> PostApiCustomer(ApiCustomer apiCustomer)
        {
            _context.ApiCustomer.Add(apiCustomer);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomerExists(apiCustomer.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCustomer", new {id = apiCustomer.UserId}, apiCustomer);
        }

        /// <summary>
        /// Delete a Customer user
        /// </summary>
        /// <remarks>
        ///
        /// Delete a Customer from the database,
        /// if the customer exists in the Customer relation of the database.
        /// The authenticated user must be the user to be deleted.
        /// 
        /// A Delete operation to delete a Customer is performed.
        /// </remarks>
        ///<param name="id">The username of the Customer to delete.</param>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteApiCustomer(string id)
        {
            var apiCustomer = await _context.ApiCustomer.FindAsync(id);
            if (apiCustomer == null)
            {
                return NotFound();
            }

            _context.ApiCustomer.Remove(apiCustomer);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool CustomerExists(string id)
        {
            return _context.Customer.Any(e => e.UserId == id);
        }
    }
}