using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.Abstraction;
using Pronia.ViewModels.UserViewModels;
using System.Threading.Tasks;

namespace Pronia.Controllers
{
    public class AccountController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager, RoleManager<IdentityRole> _roleManager, IConfiguration _configuration, IEmailService _emailService) : Controller
    {
      
       [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var existUser = await _userManager.FindByNameAsync(vm.UserName);

            if (existUser is { }) {
                ModelState.AddModelError("username", "username is already taken");
                return View(vm);
            }

            existUser = await _userManager.FindByNameAsync(vm.EmailAddress);

            if (existUser is { })
            {
                ModelState.AddModelError("email address", "email address is already taken");
                return View(vm);
            }

            AppUser user = new AppUser()
            {
                FullName = vm.UserName,
                Email = vm.EmailAddress,
            };
            var result =  await _userManager.CreateAsync(user, vm.Password);

            if (!result.Succeeded) {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(vm);
            }

            await _userManager.AddToRoleAsync(user, "Member");
            await _signInManager.SignInAsync(user, false);


            await SendConfirmationEmail(user);

            TempData["SuccessMessage"] = "Please confirm your email!";

            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var user = await _userManager.FindByEmailAsync(vm.EmailAddress);
            if (user is { })
            {
                ModelState.AddModelError("", "email or password is wrong");
            }

            var result = await _userManager.CheckPasswordAsync(user, vm.Password);

            if (!result)
            {
                ModelState.AddModelError("", "email or password is wrong");
                return View(vm);
             }

            if (user.EmailConfirmed is false)
            {
                ModelState.AddModelError("", "Please confirm your email!");
                await SendConfirmationEmail(user);
                return View(vm);
            }

            await _signInManager.SignInAsync(user, vm.isRemember);

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> CreateRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "Admin"
            });
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "Moderator"
            });
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "Member"
            });

            return Ok("Roles created successfully!");
        }

        public async Task<IActionResult> CreateAdminAndModerator()
        {
            var adminUserVM = _configuration.GetSection("AdminUser").Get<UserVM>();
            var moderatorUserVM = _configuration.GetSection("ModeratorUser").Get<UserVM>();

            if (adminUserVM is not null)
            {
                AppUser adminUser = new AppUser()
                {
                    Email = adminUserVM.Email,
                    UserName = adminUserVM.UserName,
                };
                await _userManager.CreateAsync(adminUser, adminUserVM.Password);
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }

            if (moderatorUserVM is not null)
            {
                AppUser moderatorUser = new AppUser()
                {
                    Email = moderatorUserVM.Email,
                    UserName = moderatorUserVM.UserName,
                };
                await _userManager.CreateAsync(moderatorUser, moderatorUserVM.Password);
                await _userManager.AddToRoleAsync(moderatorUser, "Moderator");
            }

            return Ok("Successfully!");
        }

        private async Task SendConfirmationEmail(AppUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string url = Url.Action("ConfirmEmail", "Account", new { token = token, userId = user.Id }, Request.Scheme) ?? string.Empty;

            string emailBody = $@"<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <title>Pronia Email Confirmation</title>
</head>
<body style=""font-family: Arial, sans-serif; background:#f4f4f4; padding:20px;"">
    <div style=""max-width:500px; margin:auto; background:#ffffff; padding:30px; text-align:center; border-radius:6px;"">
        <h2>Email Confirmation</h2>
        <p>Please confirm your email address by clicking the button below.</p>

        <a href=""{url}""
           style=""display:inline-block; margin-top:20px; padding:12px 24px;
                  background:#4f46e5; color:#ffffff; text-decoration:none;
                  border-radius:4px;"">
            Confirm Email
        </a>

        <p style=""margin-top:25px; font-size:12px; color:#777;"">
            If you didn’t create an account, you can ignore this email.
        </p>
    </div>
</body>
</html>
 ";
            await _emailService.SendEmailAsync(user.Email!, "Confirm your email", emailBody);
        }

        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return BadRequest();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("Index", "Home");
        }

    }
}
