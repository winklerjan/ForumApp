using ForumAppScratch.Dto;
using ForumAppScratch.Entities;
using ForumAppScratch.Services;
using ForumAppScratch.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CoreIdentity = Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;


namespace InvoiceApp.Api.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> logger;
        private readonly CoreIdentity.SignInManager<User> signInManager;
        private readonly CoreIdentity.UserManager<User> userManager;
        private readonly IConfiguration config;
        private readonly ForumService service;

        public AccountController(ILogger<AccountController> logger,
            CoreIdentity.SignInManager<User> signInManager,
            CoreIdentity.UserManager<User> userManager,
            IConfiguration config,
            ForumService service)
        {
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.config = config;
            this.service = service;
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await this.userManager.FindByEmailAsync(model.Email);
                if (existingUser == null)
                {
                    var user = new User { UserName = model.Email, Email = model.Email }; 
                    //TODO add username to the registration formular
                    
                    var result = await userManager.CreateAsync(user, model.Password);
                    //var result = userManager.CreateAsync(user, model.Password).Result; - waits for the result, await pointless in that case

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                        var loginModel = new LoginDto { Email = model.Email, Password = model.Password };
                        return await Login(loginModel);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View(model);
                    }
                }
                ModelState.AddModelError("", "User with this email already exists.");
                return View(model);
            }
            return View(model);
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var model = new LoginDto
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
        };
            return View(model);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("index", "forum");
                }
                ModelState.AddModelError("", "Wrong email or password."); //TODO target single errors
                #region JWT token
                //var user = await this.userManager.FindByEmailAsync(model.Email);
                //if (user != null)
                //{

                //var passwordCheck = await this.signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                //if (passwordCheck.Succeeded)
                //{
                //    var claims = new List<Claim>
                //    {
                //        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
                //    };

                //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.config["Tokens:Key"]));
                //    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
                //    var tokenDescriptor = new SecurityTokenDescriptor
                //    {
                //        Subject = new ClaimsIdentity(claims),
                //        Expires = DateTime.UtcNow.AddHours(3),
                //        SigningCredentials = credentials
                //    };

                //    var tokenHandler = new JwtSecurityTokenHandler();
                //    var token = tokenHandler.CreateToken(tokenDescriptor);
                //    return RedirectToAction("index", "forum");
                //    //return Ok(new
                //    //{
                //    //    token = tokenHandler.WriteToken(token)
                //    //});
                //}
                //}
                //else
                //{
                //    return Unauthorized();
                //}
                #endregion
            }
            
            return View(model);
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var model = new LoginDto
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", model);
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information.");
                return View("login", model);
            }

            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    var user = await userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        user = new User
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };
                        await userManager.CreateAsync(user);
                    }

                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }
                ModelState.AddModelError("", "Email claim not received.");
                return View(model);
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "forum");
        }

        [HttpGet("userProfile/{userId}")]
        public IActionResult UserProfile([FromRoute]string userId)
        {
            var user = service.GetUserById(userId); //TODO change to one service using Include
            var posts = service.GetPostsByUserId(userId);
            var topics = service.GetTopicsByUserId(userId);
            var recentPosts = service.GetRecentPosts(userId);
            var model = new UserProfileViewModel { User = user, Posts = posts, Topics = topics, RecentPosts = recentPosts };
            return View(model);
        }

        [HttpGet("admin")]
        public IActionResult Admin()
        {
            var user = service.GetUserById(User.Identity.GetUserId());
            var model = new AdminViewModel { User = user };
            return View(model);
        }

        [HttpGet("changeUsername")]
        public IActionResult ChangeUsername()
        {
            return View();
        }

        [HttpGet("changeAvatar")]
        public IActionResult ChangeAvatar()
        {
            var user = service.GetUserById(User.Identity.GetUserId());
            var model = new AdminViewModel { User = user };
            return View(model);
        }

        [HttpPost("changeUsername")]
        public IActionResult ChangeUsername(string username)
        {
            var user = service.GetUserById(User.Identity.GetUserId());
            service.ChangeUsername(username, user);

            return RedirectToAction("admin");
        }

        [HttpPost("changeAvatar")]
        public IActionResult ChangeAvatar(string avatarUrl)
        {
            var user = service.GetUserById(User.Identity.GetUserId());
            service.ChangeAvatar(avatarUrl, user);

            return RedirectToAction("admin");
        }
    }
}