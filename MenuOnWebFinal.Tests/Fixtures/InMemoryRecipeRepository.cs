using MenuOnWebFinal.Models;
using MenuOnWebFinal.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuOnWebFinal.Tests.Fixtures
{
    class InMemoryRecipeRepository : IRecipeRepository
    {
        private List<Like> _likes;
        private List<Comment> _comments;
        private List<User> _users;
        private List<Recipe> _recipes;

        public InMemoryRecipeRepository()
        {
            _likes = new List<Like>();
            _comments = new List<Comment>();
            _users = new List<User>();
            _recipes = new List<Recipe>();
        }

        public IEnumerable<Recipe> GetRecipes()
        {
            return _recipes;
        }

        public void AddComment(Comment comment)
        {
            comment.UserId = comment.UserId == null ? "1" : comment.UserId;
            _comments.Add(comment);
        }

        public void AddLike(Like like)
        {
            like.UserId = like.UserId == null ? "1" : like.UserId;
            _likes.Add(like);
        }

        public void AddRecipe(Recipe recipe)
        {
            recipe.UserId = recipe.UserId == null ? "1" : recipe.UserId;
            _recipes.Add(recipe);
        }

        public void DeleteComment(Comment comment)
        {
            _comments.Remove(comment);
        }

        public void DeleteRecipe(Recipe recipe)
        {
            _recipes.Remove(recipe);
        }

        public Like FindLike(string userId, int recipeId)
        {
            var l = _likes.FirstOrDefault(i => i.UserId == userId && i.RecipeId == recipeId);
            return l;
        }

        public Recipe FindRecipe(int id)
        {
            var recipe = _recipes.FirstOrDefault(i => i.Id == id);
            return recipe;
        }

        public IEnumerable<Comment> FindRecipeComments(int recipeId)
        {
            var comments = _comments.Where(i => i.RecipeId == recipeId);
            foreach(var item in comments)
            {
                item.User = new User { Login = "some login", AvatarUrl = "some url" };
            }
            return comments;
        }

        public User FindUser(string id)
        {
            List<Recipe> fav = new List<Recipe>();
            fav.Add(new Recipe()
            {
                Id = 100,
                UserId = null,
                CreateDate = DateTime.Now,
                ImageUrl = "...",
                Name = "fav1",
                Tags = "first, second",
                Text = "description"
            });

            return new User() { Login = "user", FavouriteRecipes = fav };
        }

        public Comment FindComment(int id)
        {
            var comment = _comments.FirstOrDefault(i => i.Id == id);
            return comment;
        }

        public void UpdateLike(Like like)
        {
            var l = _likes.FirstOrDefault(i => i.Id == like.Id);
            l.RecipeId = like.RecipeId;
            l.UserId = like.UserId;
            l.Value = like.Value;
        }

        public void UpdateRecipe(Recipe recipe)
        {
        }

        public void Save()
        {
        }
    }
}
