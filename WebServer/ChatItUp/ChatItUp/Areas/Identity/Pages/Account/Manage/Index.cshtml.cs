﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChatItUp.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly Context.CIUDataDbContext _context;
        private readonly HtmlEncoder _htmlEncoder;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            Context.CIUDataDbContext context,
            HtmlEncoder htmlEncoder)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _htmlEncoder = htmlEncoder;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; } = string.Empty;

            [Display(Name = "Display Name")]
            public string? DisplayName { get; set; } = string.Empty;
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user) ?? string.Empty;
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var dataUser = _context.Users.FirstOrDefault(u => u.Id == Guid.Parse(user.Id));

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber ?? string.Empty,
                DisplayName = dataUser != null? dataUser.DisplayName : null
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var dataUser = user != null? _context.Users.First(u => u.Id == Guid.Parse(user.Id)) : null;

            if (user == null || dataUser == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            string? newDisplayName = null;
            if (!string.IsNullOrEmpty(Input.DisplayName) && !string.IsNullOrEmpty(Input.DisplayName.Trim()))
                newDisplayName = _htmlEncoder.Encode(Input.DisplayName.Trim());
            

            var displayName = dataUser.DisplayName;
            
            if(Input.DisplayName != displayName)
            {
                if (!string.IsNullOrEmpty(newDisplayName))
                {
                    var conflict = _context.Users.FirstOrDefault(u => (u.DisplayName != null? u.DisplayName.ToLower() : u.EmailAddress.ToLower()) == newDisplayName.ToLower());
                    if (conflict != null)
                    {
                        StatusMessage = "That display name is already taken.";
                        return RedirectToPage();
                    }
                }
                if (!string.IsNullOrEmpty(newDisplayName))
                {
                    dataUser.DisplayName = newDisplayName;
                }
                else
                {
                    dataUser.DisplayName = null;
                }
                _context.Users.Update(dataUser);
                await _context.SaveChangesAsync();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
