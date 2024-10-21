using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityApp.Entity;
using IdentityApp.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityApp.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View(_userManager.Users.ToList());
        }

       

         public async Task<IActionResult> Edit(string id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(id);

            if(user != null)
            {
                return View(new EditViewModel {
                    Id = user.Id,
                    UserName= user.UserName,
                    Email= user.Email
                });

            }

                return RedirectToAction("Index");

        }
        
        [HttpPost]
          public async Task<IActionResult> Edit(string id, EditViewModel model)
     {
        if(ModelState.IsValid)
         {
            var user = await _userManager.FindByIdAsync(id);

            if(user != null)
            {
                user.Email = model.Email;
                user.UserName = model.UserName;

                var result = await _userManager.UpdateAsync(user);   

                if(result.Succeeded && !string.IsNullOrEmpty(model.Password))
                {
                     var hasher = new PasswordHasher<ApplicationUser>();
                     user.PasswordHash = hasher.HashPassword(user, model.Password);
                     await _userManager.UpdateAsync(user);
                }


                if(result.Succeeded)
                {
                  return RedirectToAction("Index");

                }
           
            }

         }   
         return View(model);
     }

     [HttpPost]

     public async Task<IActionResult> Delete(string id)
     {
        var user = await _userManager.FindByIdAsync(id);
        if(user != null)
        {
            await _userManager.DeleteAsync(user);
        }

        return RedirectToAction("Index");

     }




    }
}