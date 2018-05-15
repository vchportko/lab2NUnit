using MenuOnWebFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MenuOnWebFinal.Repository
{
    public interface IRecipeRepository
    {
        Recipe FindRecipe(int id);
        IEnumerable<Comment> FindRecipeComments(int recipeId);
        void AddRecipe(Recipe recipe);
        void Save();
        void UpdateRecipe(Recipe recipe);
        void DeleteRecipe(Recipe recipe);
        void AddComment(Comment comment);
        void DeleteComment(Comment comment);
        Like FindLike(string userId, int recipeId);
        Comment FindComment(int id);
        void UpdateLike(Like like);
        void AddLike(Like like);
        User FindUser(string id);
    }
}
