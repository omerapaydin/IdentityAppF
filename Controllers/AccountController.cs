using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Entity;
using IdentityApp.Models;
using IdentityApp.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    public class AccountController: Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();

                   if(!await _userManager.IsEmailConfirmedAsync(user))
                   {
                    ModelState.AddModelError("", "Hesabınızı onaylayınız.");
                    return View(model);
                   }


                    var result = await _signInManager.PasswordSignInAsync(user,model.Password,model.RememberMe,false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index","Home");
                    }else{
                         ModelState.AddModelError("", "hatalı parola ya da password");
                    }
                }


            }else {
                ModelState.AddModelError("", "hatalı parola ya da password");
            }

            return View(model);
        }


         public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser {
                    UserName = model.UserName,
                    Email = model.Email
                };
                var hasher = new PasswordHasher<ApplicationUser>();
                user.PasswordHash = hasher.HashPassword(user, model.Password);
               IdentityResult result= await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                      var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail", "Account", new{user.Id,token});

                  
                await _emailSender.SendEmailAsync(user.Email, "Hesap Onayı",$"Lütfen email hesabınızı onaylamak için linke <a href='http://localhost:5041{url}'> tıklayınız. <a/>");



                TempData["message"] = "Email hesabınızdaki onay mailine tıkla.";
                return RedirectToAction("Login", "Account");
                }
              

            }
            return View(model);
        }


        public async Task<IActionResult> ConfirmEmail(string Id, string token)
        {
            if(Id == null || token == null)
            {
                TempData["message"] = "Geçersiz token bilgisi";
                return View();
            }

             var user = await _userManager.FindByIdAsync(Id);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user,token);


                if (result.Succeeded)
                {
                    TempData["message"] = "Hesabınız onaylandı";
                    return RedirectToAction("Login","Account");
                }
            }

            TempData["message"] = "Kullanıcı bulunamadı onaylandı";
                    return View();
        }

          public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }





          public IActionResult ForgotPassword()
        {

            return View();
        }
        [HttpPost]
          public async Task<IActionResult> ForgotPassword(string Email)
        {
            if(string.IsNullOrEmpty(Email))
            {
                 TempData["message"] = "Eposta giriniz";
                 return View();

            }

            var user = await _userManager.FindByEmailAsync(Email);
            
            if (user == null)
            {
                 TempData["message"] = "Eposta yok";

                return View();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
             var url = Url.Action("ResetPassword", "Account", new{user.Id,token});

              await _emailSender.SendEmailAsync(user.Email, "Şifre Sıfırlama",$"Lütfen şifre değiştirmek için linke <a href='http://localhost:5041{url}'> tıklayınız. <a/>");

              TempData["message"] = "Epostanıza gönderilen link ile şifrenizi sıfırlayabilirsiniz.";

             return View();

        }


            public IActionResult ResetPassword(string Id,string token)
            {
                if(Id==null || token == null)
                {
                     return RedirectToAction("Login");
                }
                var model = new ResetPasswordModel{Token = token };
                return View(model);
            }

            [HttpPost]
            public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
            {
                if(ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);

                 if(user == null)
                 {
                     TempData["message"] = "Email adresiyle eşleşen kullanıcı yok.";
                    return RedirectToAction("Login");
                 }

                    var result = await _userManager.ResetPasswordAsync(user,model.Token,model.Password);

                    if(result.Succeeded)

                    {
                          TempData["message"] = "Şifre değiştirildi.";
                        return RedirectToAction("Login");
                        
                    }


                }
                return View(model);

            }




    }
}