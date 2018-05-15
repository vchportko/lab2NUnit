using MenuOnWebFinal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MenuOnWebFinal.Controllers
{
    [Authorize(Roles ="Administrator")]
    public class AdminController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
      
        public ActionResult CreateUser()
        {
            return View();
        }

        public ActionResult AssignRole()
        {
            List<SelectListItem> roles = new List<SelectListItem>() {
                new SelectListItem { Value = "Administrator", Text="Administrator"},
                new SelectListItem {Value = "Moderator", Text="Moderator" }
            };

            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        public ActionResult AssignRole(FormCollection form)
        {
            try
            {
                string userName = form["txtUserName"];
                string role = form["RoleName"];

                User existingUser = context.Users.Where(u => u.UserName.Equals(userName)).FirstOrDefault();
                var userManager = new UserManager<User>(new UserStore<User>(context));
                userManager.AddToRole(existingUser.Id, role);
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return HttpNotFound();
            }
        }

        public ActionResult DeleteUser()
        {
            ViewBag.Users = context.Users.Select(r => new SelectListItem { Value = r.UserName, Text = r.UserName }).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult DeleteUser(FormCollection form)
        {
            string userName = form["Users"];

            var userManager = new UserManager<User>(new UserStore<User>(context));
            var user=userManager.FindByEmail(userName);
            var usersRoles = userManager.GetRoles(user.Id);

            foreach (var item in usersRoles)
            {
                userManager.RemoveFromRole(user.Id,item);
            }
            userManager.Delete(user);
            return RedirectToAction("Index", "Home");
        }
    }
}