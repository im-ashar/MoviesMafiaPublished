﻿using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Net.Mail;
using System.Net;

namespace MoviesMafia.Models.Repo
{

    public class UserRepo : IUserRepo
    {
        private readonly UserManager<ExtendedIdentityUser> _userManager;
        private readonly SignInManager<ExtendedIdentityUser> _signInManager;



        public UserRepo(UserManager<ExtendedIdentityUser> userManager, SignInManager<ExtendedIdentityUser> sManager)
        {
            _userManager = userManager;
            _signInManager = sManager;
        }
        public async Task<IdentityResult> SignUp(UserSignUpModel model)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", model.ProfilePicture.FileName);
            var extension = Path.GetExtension(model.ProfilePicture.FileName);
            var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProfilePictures", model.Username + extension);
            var user = new ExtendedIdentityUser
            {
                UserName = model.Username,
                Email = model.Email,
                EmailConfirmed = false,
                LockoutEnabled = false,
                ProfilePicturePath = dbPath
            };
            var checkEmail = await _userManager.FindByEmailAsync(model.Email);
            if (checkEmail != null)
            {
                var result = IdentityResult.Failed(new IdentityError { Code = "0001", Description = "Email Already Exists" });
                return result;
            }
            else
            {
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        model.ProfilePicture.CopyTo(stream);

                    }
                    File.Move(path, dbPath, true);
                    await _userManager.AddToRoleAsync(user, "User");
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var verificationUrl = $"http://moviesmafia.ga/Account/VerifyEmail?email={Uri.EscapeDataString(model.Email)}&token={Uri.EscapeDataString(token)}";
                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress("moviesmafiaaa@gmail.com");
                        mail.To.Add(model.Email);
                        mail.Subject = "Verify your email address";
                        mail.Body = $"Please click the following link to verify your email address: {verificationUrl}";
                        
                        

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.Credentials = new NetworkCredential("moviesmafiaaa@gmail.com", "jmhswshiiyubnjxk");
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }
                    }
                    return result;
                }
                else
                {
                    return result;
                }

            }
        }

        public async Task<Microsoft.AspNetCore.Identity.SignInResult> LogIn(UserLogInModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            return result;
        }
        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
            return;
        }
        public string GetUserEmail(string userName)
        {
            var email = _userManager.FindByNameAsync(userName).Result.Email;
            return email;
        }
        public string GetUserProfilePicture(string userName)
        {
            var profilePicture = _userManager.FindByNameAsync(userName).Result.ProfilePicturePath;
            if (Path.DirectorySeparatorChar == '\\')
            {
                profilePicture = profilePicture.Replace('\\', '/');
            }
            return profilePicture;
        }
        public async Task<ExtendedIdentityUser> GetUser(ClaimsPrincipal userName)
        {
            var user = await _userManager.GetUserAsync(userName);
            return user;
        }

        public async Task<IdentityResult> UpdatePassword(ExtendedIdentityUser user, string currentPassword, string newPassword)
        {
            var update = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return update;
        }
        public async Task<IList<ExtendedIdentityUser>> GetAllUsers()
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync("User");

            return usersInRole;
        }
        public async Task<bool> DeleteUser(ExtendedIdentityUser user)
        {
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<ExtendedIdentityUser> GetUserById(string id)
        {
            var result = await _userManager.FindByIdAsync(id);
            return result;
        }
        public async Task<bool> UpdateEmail(string id, string email)
        {
            var result = await GetUserById(id);
            result.Email = email;
            var update = await _userManager.UpdateAsync(result);
            if (update.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> VerifyEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);

                if (result.Succeeded)
                {
                    return true;
                }
            }

            return false;
        }
        
    }

}