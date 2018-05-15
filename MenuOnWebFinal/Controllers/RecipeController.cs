using MenuOnWebFinal.Models;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using MenuOnWebFinal.Repository;

namespace MenuOnWebFinal.Controllers
{
    [Authorize]
    public class RecipeController : Controller
    {
        IRecipeRepository db;
        public RecipeController()
        {
            db = new RecipeRepository();
        }

        public RecipeController(IRecipeRepository repository)
        {
            db = repository;
        }

        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            var recipe = db.FindRecipe(id);
            if (recipe == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var commentList = db.FindRecipeComments(id);
            var commentModelList = new List<CommentModel>();
            foreach (var item in commentList)
            {
                commentModelList.Add((CommentModel)item);
            }
            ViewBag.Comments = commentModelList;
            bool alreadyInFavorite = false;
            bool isModerator = false;
            var userId = User.Identity.GetUserId();
            if(userId != null)
            {
                isModerator = User.IsInRole("Moderator");
                var user = db.FindUser(userId);
                var result = user.FavouriteRecipes.FirstOrDefault(i => i.UserId == userId && i.Id == id);
                if (result == null)
                {
                    alreadyInFavorite = false;
                }
                else
                {
                    alreadyInFavorite = true;
                }
            }
            bool alreadyLiked = false;
            var l = db.FindLike(userId, id);
            if (l != null && l.Value != 0)
            {
                alreadyLiked = true;
            }

            ViewBag.AlreadyInFavorite = alreadyInFavorite;
            ViewBag.AlreadyLiked = alreadyLiked;
            ViewBag.Editable = userId == recipe.UserId || isModerator;
            ViewBag.IsModerator = isModerator;
            return View((ViewRecipe)recipe);
        }

        public ActionResult Create()
        {
            bool isAdmin = User.IsInRole("Administrator");
            if (isAdmin == true)
            {
                return HttpNotFound();
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(RecipeAddModel model)
        {
            if (ModelState.IsValid)
            {
                Recipe recipe = new Recipe
                {
                    Id = model.Id,
                    Name = model.Name,
                    Text = model.Text,
                    CreateDate = DateTime.Now,
                    Tags = model.IngridietsString,
                    UserId = User.Identity.GetUserId(),
                };
                string imagePath = UploadFile(model.ImageFile, "images/thumbnails");
                recipe.ImageUrl = imagePath;

                db.AddRecipe(recipe);
                db.Save();

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }

        public string UploadFile(HttpPostedFileBase file, string pathPart)
        {
            if (file == null) return string.Empty;

            var fileName = Path.GetFileName(file.FileName);
            var random = Guid.NewGuid() + fileName;
            var path = Path.Combine(HttpContext.Server.MapPath("~/Content/" + pathPart), random);
            if (!Directory.Exists(HttpContext.Server.MapPath("~/Content/" + pathPart)))
            {
                Directory.CreateDirectory(HttpContext.Server.MapPath("~/Content/" + pathPart));
            }
            file.SaveAs(path);

            return random;
        }

        public ActionResult Edit(int id)
        {
            var recipe = db.FindRecipe(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            var userId = User.Identity.GetUserId();
            var isModerator = User.IsInRole("Moderator");
            if (userId != recipe.UserId && isModerator == false)
            {
                return HttpNotFound();
            }

            return View(new RecipeAddModel
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Text = recipe.Text,
                UserId = User.Identity.GetUserId(),
                IngridietsString = recipe.Tags,
                ImageFile = null
            });
        }

        [HttpPost]
        public ActionResult Edit(RecipeAddModel editedRecipe)
        {
            try
            {
                if (editedRecipe.IngridietsString == null)
                {
                    throw new Exception();
                }

                var recipeToUpdate = db.FindRecipe(editedRecipe.Id);

                string imagePath = "";
                string imageToDelete = "";
                if (editedRecipe.ImageFile != null)
                {
                    imagePath = UploadFile(editedRecipe.ImageFile, "images/thumbnails");
                    imageToDelete = recipeToUpdate.ImageUrl;
                }


                recipeToUpdate.Name = editedRecipe.Name;
                recipeToUpdate.Text = editedRecipe.Text;
                recipeToUpdate.Tags = editedRecipe.IngridietsString;
                if (imagePath != "")
                {
                    recipeToUpdate.ImageUrl = imagePath;
                }

                db.UpdateRecipe(recipeToUpdate);
                db.Save();

                if (imageToDelete != "")
                {
                    System.IO.File.Delete(HttpContext.Server.MapPath("~/Content/images/thumbnails/" + imageToDelete));
                }

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            var recipeToDelite = db.FindRecipe(id);
            if (recipeToDelite == null)
            {
                return HttpNotFound();
            }
            var userId = User.Identity.GetUserId();
            var isModerator = User.IsInRole("Moderator");
            if (userId != null && userId != recipeToDelite.UserId && isModerator == false)
            {
                return HttpNotFound();
            }

            if (recipeToDelite.ImageUrl != null)
            {
                try
                {
                    System.IO.File.Delete(HttpContext.Server.MapPath("~/Content/images/thumbnails/" + recipeToDelite.ImageUrl));
                }
                catch { }
            }

            db.DeleteRecipe(recipeToDelite);
            db.Save();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult AddComment(CommentModel comment)
        {
            try
            {
                var commentToAdd = (Comment)comment;
                commentToAdd.Id = comment.Id;
                commentToAdd.UserId = User.Identity.GetUserId();
                db.AddComment(commentToAdd);
                db.Save();
                return RedirectToAction("Details", "Recipe", new { id = comment.RecipeId });
            }
            catch
            {
                return RedirectToAction("Details", "Recipe", new { id = comment.RecipeId });
            }
        }

        [HttpGet]
        public ActionResult DeleteComment(int id)
        {
            try
            {
                bool isModerator = User.IsInRole("Moderator");
                if (isModerator == false)
                {
                    return HttpNotFound();
                }
                var commentToDelete = db.FindComment(id);
                if (commentToDelete == null)
                {
                    throw new Exception();
                }

                db.DeleteComment(commentToDelete);
                db.Save();
                return RedirectToAction("Details", "Recipe", new { id = commentToDelete.RecipeId });
            }
            catch
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult Like(LikeModel like)
        {
            var id = User.Identity.GetUserId();
            var l = db.FindLike(id, like.RecipeId);
            if (l != null)
            {
                if (l.Value == 0)
                {
                    l.Value = 1;
                }
                else
                {
                    l.Value = 0;
                }
                var val = l.Value;

                db.UpdateLike(l);
                db.Save();

                return RedirectToAction("Details", "Recipe", new { id = like.RecipeId });
            }
            else
            {
                var likeToAdd = new Like()
                {
                    Id = 1,
                    UserId = User.Identity.GetUserId(),
                    RecipeId = like.RecipeId,
                    Value = 1
                };
                db.AddLike(likeToAdd);

                db.Save();

                return RedirectToAction("Details", "Recipe", new { id = like.RecipeId });
            }
        }

        [HttpGet]
        public ActionResult AddToFavourite(int recipeId)
        {
            var id = User.Identity.GetUserId();
            var user = db.FindUser(id);
            var recipe = db.FindRecipe(recipeId);

            var result = user.FavouriteRecipes.FirstOrDefault(i => i.UserId == id && i.Id == recipeId);
            if (result == null)
            {
                user.FavouriteRecipes.Add(recipe);
            }
            else
            {
                user.FavouriteRecipes.Remove(recipe);
            }

            db.Save();

            return RedirectToAction("Details", "Recipe", new { id = recipeId });
        }

        [HttpGet]
        public ActionResult GetFavourites()
        {
            var id = User.Identity.GetUserId();
            var user = db.FindUser(id);
            var favRecipes = user.FavouriteRecipes.ToList();
            List<ViewRecipe> recipes = new List<ViewRecipe>();

            foreach (var item in favRecipes)
            {
                recipes.Add((ViewRecipe)item);
            }

            ViewBag.Recipes = recipes;

            return View();
        }

        [HttpGet]
        public ActionResult MyRecipes()
        {
            var id = User.Identity.GetUserId();
            var user = db.FindUser(id);
            var myRecipes = user.Recipes.ToList();
            List<ViewRecipe> recipes = new List<ViewRecipe>();

            foreach (var item in myRecipes)
            {
                recipes.Add((ViewRecipe)item);
            }

            ViewBag.Recipes = recipes;

            return View();
        }

        [HttpPost]
        public ActionResult Find(SearchModel model)
        {
            throw new NotImplementedException();
        }
    }
}