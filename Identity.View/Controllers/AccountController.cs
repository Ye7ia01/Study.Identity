using Identity.Domain.DTOs;
using Identity.Domain.Entities.Identity;
using Identity.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Identity.View.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly RoleManager<ApplicationRole> _roleManager; 
        public AccountController(UserManager<ApplicationUser> user, SignInManager<ApplicationUser> manager
            , RoleManager<ApplicationRole> roleManager) { 
            _userManager = user;
            _signinManager = manager;
            _roleManager = roleManager;
            
        }

        [HttpGet]
        public async Task<IActionResult> IsEmailValid(string Email)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                return Json(true);
            }
            return Json(false);
        }
        public IActionResult Register()
        {
            return View("Register");


        }
        public IActionResult Login()
        {
            return View("Login");


        }




        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO, string? returnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error =  ModelState.Values.SelectMany(temp => temp.Errors).Select(e => e.ErrorMessage);
                return View(loginDTO);
            }

            

           var result = await _signinManager.PasswordSignInAsync(loginDTO.UserName, loginDTO.UserPassword, false, false);
            if (result.Succeeded)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(loginDTO.UserName);

                if (await _userManager.IsInRoleAsync(user, AccountTypes.Admin.ToString()))
                {
                    //return RedirectToAction("Index", "Merchant", new { area = "Merchant" });
                    return RedirectToAction("Index", "Admin");
                }

                else if (await _userManager.IsInRoleAsync(user, "Merchant"))
                {

                }

                else if (await _userManager.IsInRoleAsync(user, "User"))
                {

                }


                if (!returnUrl.IsNullOrEmpty())
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction("Index", "Home");
            }

            else
            {
                List<String> errors = new List<String>();
                if (result.IsNotAllowed) errors.Add("Not Allowed");
                else if (result.IsLockedOut) errors.Add("Locked Out");
                else if (result.RequiresTwoFactor) errors.Add("regquires 2 factor Authentication");
                else errors.Add("Invalid Credentials");

                ViewBag.Errors = errors;
                return View(loginDTO);
            }


        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO RegDTO)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(temp => temp.ErrorMessage);
                return View(RegDTO);
            }

            ApplicationUser user = new ApplicationUser()
            {
                UserName = RegDTO.UserEmail,
                NormalizedUserName = RegDTO.UserName,
                Email = RegDTO.UserEmail,
                PhoneNumber = RegDTO.UserPhone
            };
            var result = await _userManager.CreateAsync(user,RegDTO.UserPassword);

            // Login
            if(result.Succeeded)
            {
                if (await _roleManager.FindByNameAsync(RegDTO.AccountType.ToString()) is null)
                {
                    ApplicationRole role = new ApplicationRole()
                    {
                        Name = RegDTO.AccountType.ToString()
                    };
                    await _roleManager.CreateAsync(role);
                    await _userManager.AddToRoleAsync(user, RegDTO.AccountType.ToString());

                }

                await _signinManager.SignInAsync(user,isPersistent:false);
                return RedirectToAction("Index", "Home");
            }
            // return Errors
            else
            {
                ViewBag.Errors = result.Errors.Select(tmp=>tmp.Description);
                
                return View(RegDTO);
            }

        }

        public async Task<IActionResult> Logout(RegisterDTO RegDTO)
        {
            await _signinManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

       



    }
}
