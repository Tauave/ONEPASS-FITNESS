#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ONEPASS_FITNESS.Areas.Identity.Pages;
using ONEPASS_FITNESS.Data;
using ONEPASS_FITNESS.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static ONEPASS_FITNESS.Areas.Identity.Pages.AppUser;

namespace ONEPASS_FITNESS.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IUserEmailStore<AppUser> _emailStore;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<AppUser> userManager,
            IUserStore<AppUser> userStore,
            SignInManager<AppUser> signInManager,
            ApplicationDbContext context,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {


            [Required]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters long.")]
            [RegularExpression(@"^[A-Za-z\s'-]+$", ErrorMessage = "Name can only contain letters, spaces, hyphens, and apostrophes.")]
            public string Name { get; set; }


            [Required]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "Lastname must be between 2 and 50 characters long.")]
            [RegularExpression(@"^[A-Za-z\s'-]+$", ErrorMessage = "Lastname can only contain letters, spaces, hyphens, and apostrophes.")]
            public string Lastname { get; set; }

            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]{2,}$", ErrorMessage = "Invalid email address.")]

            public string Email { get; set; }

              
            [Required]
            [Phone]
            public string PhoneNumber { get; set; }

            [Required (ErrorMessage = "Your Date of birth is required.") ]
            [AgeValidation]
            public DateOnly DOB { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            //Normalize the names before validation to ensure consistent formatting
            Input.Name = NormalizeName(Input.Name);
            Input.Lastname = NormalizeName(Input.Lastname);

            //Names werre changed, so we need to revalidate them. Remove the old validation state and revalidate the model.
            ModelState.Remove("Input.Name");
            ModelState.Remove("Input.Lastname");
            TryValidateModel(Input, "Input");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = new AppUser
            {
                UserName = Input.Email,
                Name = Input.Name,
                Lastname = Input.Lastname,
                PhoneNumber = Input.PhoneNumber,
                DOB = Input.DOB,
                Email = Input.Email,
                EmailConfirmed = true
            };


            var result = await _userManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            
            await _context.SaveChangesAsync();

            await _userManager.AddToRoleAsync(user, "Member");

            _logger.LogInformation("User created a new account with profile.");
            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl);
        }

        //Capitalizes the first letter of each word in a name and lowecases the rest
        private static string NormalizeName(string s)
        {
            //Leaves empty names so rquired validation can handle them, but trims whitespace and capitalizes words otherwise
            if (string.IsNullOrWhiteSpace(s)) return s;
            var parts = s.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                //Capitalize the first letter and lowercase the rest of each part
                var p = parts[i];
                parts[i] = char.ToUpperInvariant(p[0]) + (p.Length > 1 ? p.Substring(1).ToLowerInvariant() : string.Empty);
            }
            return string.Join(' ', parts);
        }

        private IUserEmailStore<AppUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AppUser>)_userStore;
        }
    }
}
