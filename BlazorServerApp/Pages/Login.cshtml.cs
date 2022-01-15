using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlazorServerApp.Models;
namespace BlazorCookieAuth.Server.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {

        private IRecipeDataLoader _dataLoader;

        public LoginModel(IRecipeDataLoader dataLoader)
        {
            _dataLoader = dataLoader;
        }
        public async Task<IActionResult> OnGetAsync(string paramUsername, string paramPassword,string paramReturnURL)
        {
            string returnUrl;
            if (paramReturnURL != null && Url.IsLocalUrl(paramReturnURL))
            {
                returnUrl = paramReturnURL;
            }
            else
            {
                returnUrl = Url.Content("~/");
            }
           

            try
            {
                // Clear the existing external cookie
                await HttpContext
                    .SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
            
               User user = await _dataLoader.GetUserFromDatabase(paramUsername);

                string sha512 = BlazorServerApp.Models.User.CreateSHAHash(paramPassword);
                if (user != null && user.SHA512 == sha512)
                {

                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, paramUsername),
                new Claim(ClaimTypes.Role, user.Role),
            };
                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        IssuedUtc = DateTime.Now,
                        ExpiresUtc = DateTime.Now.AddMinutes(60),
                        RedirectUri = this.Request.Host.Value
                    };
 await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);
                }  
            }
            catch
            {

            }
            return LocalRedirect(returnUrl);
        }
    }
}