using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RecipEase.Shared;
using RecipEase.Shared.Models;
using RecipEase.Shared.Models.Api;

namespace RecipEase.Server.Data
{
    public static class DbInitializer
    {
        private static string _testCustomerId;
        private static string _testCustomerId2;
        private static string _testCustomerId3;
        private static string _testSupplierId;
        private static int _recipe0Id;
        private static int _recipe1Id;
        private static int _recipe2Id;
        private const string TestCustomerUsername = "c@c";
        private const string TestCustomerPassword = "c";
        private const string TestSupplierUsername = "s@s";
        private const string TestSupplierPassword = "s";

        public static async Task Initialize(RecipEaseContext context, UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await context.Database.EnsureCreatedAsync();

            await InitializeRoles(roleManager);
            await InitializeUsers(context, userManager);

            await InitializeSupplies(context);

            await InitializeRecipes(context);
            await InitializeRecipeRatings(context);

            await context.SaveChangesAsync();
        }

        private static async Task InitializeRecipes(RecipEaseContext context)
        {
            if (!context.Recipe.Any())
            {
                var recipes = new[]
                {
                    new Recipe
                    {
                        Name = "Mac N' Cheese",
                        AuthorId = _testCustomerId,
                        Calories = 1,
                        Carbs = 2,
                        Cholesterol = 3,
                        Fat = 4,
                        Protein = 5,
                        Sodium = 6,
                        Steps = "The steps to make the recipe..."
                    },
                    new Recipe
                    {
                        Name = "Pork Cutlets",
                        AuthorId = _testCustomerId,
                        Calories = 1,
                        Carbs = 2,
                        Cholesterol = 3,
                        Fat = 4,
                        Protein = 5,
                        Sodium = 6,
                        Steps = "The steps to make the recipe..."
                    },
                    new Recipe
                    {
                        Name = "Sushi",
                        AuthorId = _testCustomerId2,
                        Calories = 1,
                        Carbs = 2,
                        Cholesterol = 3,
                        Fat = 4,
                        Protein = 5,
                        Sodium = 6,
                        Steps = "The steps to make the recipe..."
                    },
                };

                await context.Recipe.AddRangeAsync(recipes);
                await context.SaveChangesAsync();
            }
            var allRecipes = await context.Recipe.ToListAsync();
            if (allRecipes.Count == 3)
            {
                _recipe0Id = allRecipes[0].Id;
                _recipe1Id = allRecipes[1].Id;
                _recipe2Id = allRecipes[2].Id;
            }
        }

        private static async Task InitializeRecipeRatings(RecipEaseContext context)
        {
            if (!context.RecipeRating.Any())
            {
                var recipeRatings = new[]
                {
                    new RecipeRating
                    {
                        Rating = 5,
                        RecipeId = _recipe0Id,
                        UserId = _testCustomerId
                    },
                    new RecipeRating
                    {
                        Rating = 4,
                        RecipeId = _recipe1Id,
                        UserId = _testCustomerId
                    },
                    new RecipeRating
                    {
                        Rating = 3,
                        RecipeId = _recipe2Id,
                        UserId = _testCustomerId
                    },
                    new RecipeRating
                    {
                        Rating = 2,
                        RecipeId = _recipe0Id,
                        UserId = _testCustomerId2
                    },
                    new RecipeRating
                    {
                        Rating = 1,
                        RecipeId = _recipe1Id,
                        UserId = _testCustomerId2
                    },
                    new RecipeRating
                    {
                        Rating = 5,
                        RecipeId = _recipe2Id,
                        UserId = _testCustomerId2
                    },
                    new RecipeRating
                    {
                        Rating = 4,
                        RecipeId = _recipe0Id,
                        UserId = _testCustomerId3
                    },
                    new RecipeRating
                    {
                        Rating = 3,
                        RecipeId = _recipe1Id,
                        UserId = _testCustomerId3
                    },
                    new RecipeRating
                    {
                        Rating = 2,
                        RecipeId = _recipe2Id,
                        UserId = _testCustomerId3
                    },

                };
                
                await context.RecipeRating.AddRangeAsync(recipeRatings);
            }
        }

        private static async Task InitializeSupplies(RecipEaseContext context)
        {
            const string flour = "Flour";
            const string grams = "Grams";
            const string cups = "Cups";
            const string rice = "Rice";
            const string goatMilk = "Goat Milk";

            if (!context.Ingredient.Any())
            {
                var ingredients = new[]
                {
                    new Ingredient()
                    {
                        Name = flour,
                        Rarity = Rarity.Common,
                        WeightToVolRatio = 0.5
                    },
                    new Ingredient()
                    {
                        Name = rice,
                        Rarity = Rarity.Common,
                        WeightToVolRatio = 0.9
                    },
                    new Ingredient()
                    {
                        Name = goatMilk,
                        Rarity = Rarity.Rare,
                        WeightToVolRatio = 0.6
                    },
                };

                await context.Ingredient.AddRangeAsync(ingredients);
            }

            if (!context.Unit.Any())
            {
                var units = new[]
                {
                    new Unit()
                    {
                        Name = grams,
                        Symbol = "g",
                        UnitType = UnitType.Mass
                    },
                    new Unit()
                    {
                        Name = cups,
                        Symbol = "cups",
                        UnitType = UnitType.Volume
                    },
                };

                await context.Unit.AddRangeAsync(units);
            }

            if (!context.Supplies.Any())
            {
                var supplies = new[]
                {
                    new Supplies()
                    {
                        UserId = _testSupplierId,
                        Quantity = 5,
                        IngrName = flour,
                        UnitName = grams,
                    },
                    new Supplies()
                    {
                        UserId = _testSupplierId,
                        Quantity = 100,
                        IngrName = rice,
                        UnitName = grams,
                    },
                    new Supplies()
                    {
                        UserId = _testSupplierId,
                        Quantity = 10,
                        IngrName = goatMilk,
                        UnitName = cups,
                    }
                };

                await context.Supplies.AddRangeAsync(supplies);
            }
        }

        private static async Task InitializeUsers(RecipEaseContext context, UserManager<User> userManager)
        {
            var existingCustomer = await context.Customer.FirstOrDefaultAsync();
            if (existingCustomer == null)
            {
                var (customer, _) = await 
                    Users.CreateUser(userManager, context, Users.AccountType.Customer,
                        TestCustomerUsername, TestCustomerPassword);     
                _testCustomerId = customer.Id;

                (customer, _) = await 
                    Users.CreateUser(userManager, context, Users.AccountType.Customer,
                        "c2@c2", "c2");     
                _testCustomerId2 = customer.Id;

                (customer, _) = await 
                    Users.CreateUser(userManager, context, Users.AccountType.Customer,
                        "c3@c3", "c3");     
                _testCustomerId3 = customer.Id;
            }
            else
            {
                _testCustomerId = existingCustomer.UserId;

                var (customer, _) = await 
                    Users.CreateUser(userManager, context, Users.AccountType.Customer,
                        "c2@c2", "c2");     
                _testCustomerId2 = customer.Id;

                (customer, _) = await 
                    Users.CreateUser(userManager, context, Users.AccountType.Customer,
                        "c3@c3", "c3");     
                _testCustomerId3 = customer.Id;
            }

            var existingSupplier = await context.Supplier.FirstOrDefaultAsync();
            if (existingSupplier == null)
            {
                var (supplier, _) = await Users.CreateUser(userManager, context, Users.AccountType.Supplier,
                    TestSupplierUsername, TestSupplierPassword);
                _testSupplierId = supplier.Id;
            }
            else
            {
                _testSupplierId = existingSupplier.UserId;
            }
        }

        private static async Task InitializeRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Enum.GetNames(typeof(Role)))
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}