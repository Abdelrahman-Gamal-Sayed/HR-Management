using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Temp.Models;
using Temp.ViewModels;

namespace Temp.Controllers
{

    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
   


        public AccountController(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager ) {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Logout() {
            await signInManager.SignOutAsync();
            //signInManager.();
            return RedirectToAction("index", "home");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login() {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl) {
            if(ModelState.IsValid) {

                
                var result = await signInManager.PasswordSignInAsync(model.Email,
                    model.Password, model.RememberMe, false);

                if(result.Succeeded) {
                    if(!string.IsNullOrEmpty(returnUrl)&& Url.IsLocalUrl(returnUrl)) {
                        return Redirect(returnUrl);
                    } else {
                        return RedirectToAction("index", "home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register() {
            return View();
        }
        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email) {
            var user = await userManager.FindByEmailAsync(email);

            if(user == null) {
                return Json(true);
            } else {
                return Json($"Email {email} is already in use.");
            }
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model) {
            if(ModelState.IsValid) {
                // Copy data from RegisterViewModel to IdentityUser
                var user = new ApplicationUser {
                    UserName = model.Email,
                    Email = model.Email,
                    City=model.City
                };

                // Store user data in AspNetUsers database table
                var result = await userManager.CreateAsync(user, model.Password);

                // If user is successfully created, sign-in the user using
                // SignInManager and redirect to index action of HomeController
                if(result.Succeeded) {

                    // If the user is signed in and in the Admin role, then it is
                    // the Admin user that is creating a new user. So redirect the
                    // Admin user to ListRoles action
                    if(signInManager.IsSignedIn(User) && User.IsInRole("Admin")) {
                        return RedirectToAction("ListUsers", "Administration");
                    }

                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }

                // If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach(var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied() {
            return View();
        }
    }
}