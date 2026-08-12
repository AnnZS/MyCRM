using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MyCRM.Models;
using MyCRM.ViewModels;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using MyCRM.Email;

namespace MyCRM.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<Users> _signInManager;
        private readonly UserManager<Users> _userManager;
        private readonly IEmailSender _emailSender;

        public AccountController(SignInManager<Users> signInManager, UserManager<Users> userManager, IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;  //Remember and return to the page after login
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");   //After suceed, redirect to the homepage
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");   //Redirect
        }

        //Registration--------------------------------------------------------

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            //New user
            var user = new Users
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.Name
                //For security reasons, the password is not stored here (UserManager!!)
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Send welcome / verification email to the newly created user (recipient = user.Email)
                try
                {
                    var subject = "Welcome to MyCRM";
                    var body = $"Hello {user.FullName},<br/><br/>Thank you for registering. You can now log in with your email.<br/><br/>Regards,<br/>MyCRM";
                    await _emailSender.SendEmailAsync(user.Email, subject, body);
                }
                catch
                {
                    // Swallow email errors to avoid blocking registration flow; consider logging.
                }

                // After registration redirect to Login so the user can sign in
                TempData["SuccessMessage"] = "Registration successful. Please check your email and log in.";
                return RedirectToAction("Login");
            }

            //If registration fails, add model error
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, "Something went wrong");//error.Description);
            }

            return View(model);
        }

        //Verify----------------------------------------------------------

        public IActionResult VerifyEmail()
        {
            return View();
        }
    }
}
