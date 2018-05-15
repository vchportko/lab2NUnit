using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MenuOnWebFinal.Controllers;
using MenuOnWebFinal.Tests.Fixtures;
using MenuOnWebFinal.Models;
using System.Web.Mvc;
using System.Web;
using Moq;
using System.Security.Principal;

namespace MenuOnWebFinal.Tests.Controllers
{
    [TestClass]
    public class RecipeControllerTest
    {
        InMemoryRecipeRepository db;
        Recipe recipe1;
        Recipe recipe2;
        Recipe recipe3;
        Comment comment1;
        Comment comment2;
        Like like1;
        RecipeController controller;

        public RecipeControllerTest()
        {
            db = new InMemoryRecipeRepository();
            recipe1 = new Recipe
            {
                Id = 1,
                UserId = "1",
                Text = "Some text1",
                CreateDate = DateTime.Now,
                ImageUrl = "Some url1",
                Name = "Recipe1",
                Tags = "first, second, third",
                User = new User() { Login = "Some ligsn", AvatarUrl = "some url" }
            };
            recipe2 = new Recipe
            {
                Id = 2,
                UserId = "1",
                Text = "Some text2",
                CreateDate = DateTime.Now,
                ImageUrl = "Some url2",
                Name = "Recipe2",
                Tags = "first, second, third2"
            };
            recipe3 = new Recipe
            {
                Id = 3,
                UserId = "2",
                Text = "Some text3",
                CreateDate = DateTime.Now,
                ImageUrl = "Some url3",
                Name = "Recipe3",
                Tags = "first, second, third3"
            };

            comment1 = new Comment
            {
                Id = 1,
                CreateDate = DateTime.Now,
                RecipeId = 1,
                Text = "comment1 text",
                UserId = "1"
            };
            comment2 = new Comment
            {
                Id = 2,
                CreateDate = DateTime.Now,
                RecipeId = 2,
                Text = "comment2 text",
                UserId = "2"
            };

            like1 = new Like
            {
                Id = 1,
                RecipeId = 1,
                UserId = "1",
                Value = 1
            };

            db.AddRecipe(recipe1);
            db.AddRecipe(recipe2);
            db.AddRecipe(recipe3);

            db.AddComment(comment1);
            db.AddComment(comment2);

            db.AddLike(like1);

            controller = new RecipeController(db);
        }

        [TestMethod]
        public void RecipeDetails()
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("Administrator")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("rr");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;

            string[] tags = { "first", "second", "third" };
            ViewRecipe expectedRecipe = new ViewRecipe
            {
                Id = 1,
                UserId = "1",
                Text = "Some text1",
                Likes = 0,
                ImageUrl = "Some url1",
                Name = "Recipe1",
                Tags = tags,
                IngridietsString = "first, second, third"
            };

            ViewResult result = controller.Details(1) as ViewResult;

            Assert.IsTrue(result.Model is ViewRecipe);
            Assert.AreEqual(expectedRecipe.Id, ((ViewRecipe)result.Model).Id);
        }

        [TestMethod]
        public void CreateRecipe()
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("Administrator")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("rr");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;

            RecipeAddModel recipe = new RecipeAddModel
            {
                Id = 10,
                UserId = "10",
                Text = "Some text10",
                ImageFile = null,
                Name = "Recipe10",
                IngridietsString = "first, second, third"
            };

            ViewResult result = controller.Create(recipe) as ViewResult;

            var added = db.FindRecipe(10);
            // Assert
            Assert.AreNotEqual(null, added);
        }

        [TestMethod]
        public void EditRecipe()
        {
            Recipe expectedRecipe = new Recipe
            {
                Id = 3,
                UserId = "2",
                Text = "Edited text",
                CreateDate = recipe3.CreateDate,
                ImageUrl = "Some url3",
                Name = "Recipe3",
                Tags = "first, second, third3"
            };

            ViewResult result = controller.Edit(new RecipeAddModel
            {
                Id = 3,
                UserId = "2",
                Text = "Edited text",
                Name = "Recipe3",
                IngridietsString = "first, second, third",
                ImageFile = null

            }) as ViewResult;

            var updated = db.FindRecipe(3);
            Assert.AreEqual(expectedRecipe.Text, updated.Text);
        }

        [TestMethod]
        public void DeleteRecipe()
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("Administrator")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("rr");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;

            ViewResult result = controller.Delete(3) as ViewResult;

            var deletedRecipe = db.FindRecipe(3);
            Assert.AreEqual(null, deletedRecipe);
        }

        [TestMethod]
        public void AddComment()
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("Administrator")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("rr");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;

            CommentModel toAdd = new CommentModel
            {
                AuthorLogin = "rr",
                CreateDate = DateTime.Now,
                RecipeId = 2,
                Text = "new comment",
                Id = 7
            };

            ViewResult result = controller.AddComment(toAdd) as ViewResult;

            var addedComment = db.FindComment(7);
            Assert.AreNotEqual(null, addedComment);
        }

        [TestMethod]
        public void DeleteComment()
        {

            ViewResult result = controller.DeleteComment(2) as ViewResult;

            var deletedComment = db.FindComment(2);
            Assert.AreEqual(null, deletedComment);
        }

        [TestMethod]
        public void AddLike()
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("Administrator")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("yulia.furta@gmail.com");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;

            LikeModel like = new LikeModel
            {
                RecipeId = 1
            };

            ViewResult result = controller.Like(like) as ViewResult;
            var added = db.FindLike("1", 1);
            Assert.AreNotEqual(null, added);
        }

        [TestMethod]
        public void GetFavourites()
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("Administrator")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("rr");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;

            ViewResult result = controller.GetFavourites() as ViewResult;

            Assert.AreNotEqual(null, result.ViewBag.Recipes);
        }

        [TestMethod]
        public void MyRecipes()
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("Administrator")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("rr");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;

            ViewResult result = controller.MyRecipes() as ViewResult;

            Assert.AreNotEqual(null, result.ViewBag.Recipes);
            Assert.AreEqual(0, result.ViewBag.Recipes.Count);
        }

        [TestMethod]
        public void AddToFavourite()
        {
            var controllerContext = new Mock<ControllerContext>();
            var principal = new Mock<IPrincipal>();
            principal.Setup(p => p.IsInRole("Administrator")).Returns(true);
            principal.SetupGet(x => x.Identity.Name).Returns("rr");
            controllerContext.SetupGet(x => x.HttpContext.User).Returns(principal.Object);
            controller.ControllerContext = controllerContext.Object;

            RedirectToRouteResult result = controller.AddToFavourite(1) as RedirectToRouteResult;
            Assert.IsTrue(result != null);
        }
    }
}
