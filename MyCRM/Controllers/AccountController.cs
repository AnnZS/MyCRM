using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MyCRM.Models;
using MyCRM.ViewModels;
using System.Net;
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
                // Generate email confirmation token and send confirmation link
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebUtility.UrlEncode(token);
                var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = encodedToken }, protocol: Request.Scheme);

                try
                {
                    var subject = "Confirm your MyCRM account";
                    var body = $"Hello {user.FullName},<br/><br/>Thank you for registering. Please confirm your account by <a href=\"{callbackUrl}\">clicking here</a>.<br/><br/>Regards,<br/>MyCRM";
                    await _emailSender.SendEmailAsync(user.Email, subject, body);
                }
                catch
                {
                    // Do not block registration on email failure; consider logging the error
                }

                TempData["SuccessMessage"] = "Registration successful. Please check your email to confirm your account.";
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
        [HttpGet]
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["ErrorMessage"] = "No account found with that email.";
                return RedirectToAction("VerifyEmail");
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = encodedToken }, protocol: Request.Scheme);

            try
            {
                var subject = "Confirm your MyCRM account";
                var body = $"Hello {user.FullName},<br/><br/>Please confirm your account by <a href=\"{callbackUrl}\">clicking here</a>.<br/><br/>Regards,<br/>MyCRM";
                await _emailSender.SendEmailAsync(user.Email, subject, body);
            }
            catch
            {
                // Swallow email errors; consider logging
            }

            TempData["SuccessMessage"] = "Confirmation email sent. Please check your inbox.";
            return RedirectToAction("VerifyEmail");
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }

            var decodedToken = WebUtility.UrlDecode(token);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Thank you for confirming your email. You can now log in.";
                return RedirectToAction("Login");
            }

            TempData["ErrorMessage"] = "Error confirming your email.";
            return RedirectToAction("Login");
        }
    }
}
