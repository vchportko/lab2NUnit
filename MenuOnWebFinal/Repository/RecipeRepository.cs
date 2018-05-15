using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MenuOnWebFinal.Models;
using System.Data.Entity;

namespace MenuOnWebFinal.Repository
{
    public class RecipeRepository : IRecipeRepository
    {
        private ApplicationDbContext db;

        public RecipeRepository()
        {
            db = new ApplicationDbContext();
        }

        public IEnumerable<Recipe> GetRecipes()
        {
            return db.Recipes.ToList();
        }

        public void AddComment(Comment comment)
        {
            db.Comments.Add(comment);
        }

        public void AddLike(Like like)
        {
            db.Likes.Add(like);
        }

        public void AddRecipe(Recipe recipe)
        {
            db.Recipes.Add(recipe);
        }

        public void DeleteComment(Comment comment)
        {
            db.Comments.Remove(comment);
        }

        public void DeleteRecipe(Recipe recipe)
        {
            db.Recipes.Remove(recipe);
        }

        public Like FindLike(string userId, int recipeId)
        {
            var l = db.Likes.FirstOrDefault(i => i.UserId == userId && i.RecipeId == recipeId);
            return l;
        }

        public Recipe FindRecipe(int id)
        {
            var recipe = db.Recipes.Find(id);
            return recipe;
        }

        public IEnumerable<Comment> FindRecipeComments(int recipeId)
        {
            var comments = db.Comments.Where(i => i.RecipeId == recipeId);
            return comments;
        }

        public User FindUser(string id)
        {
            var user = db.Users.Find(id);
            return user;
        }

        public Comment FindComment(int id)
        {
            var comment = db.Comments.Find(id);
            return comment;
        }

        public void UpdateLike(Like like)
        {
            db.Entry(like).State = EntityState.Modified;
        }

        public void UpdateRecipe(Recipe recipe)
        {
            db.Entry(recipe).State = EntityState.Modified;
        }

        public void Save()
        {
            db.SaveChanges();
        }
    }
}