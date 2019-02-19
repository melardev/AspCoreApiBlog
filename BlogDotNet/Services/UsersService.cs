using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using BlogDotNet.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlogDotNet.Services
{
    public class UsersService
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IPasswordValidator<ApplicationUser> _passwordValidator;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserValidator<ApplicationUser> _userValidator;

        private readonly IConfigurationService _configurationService;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsersService(UserManager<ApplicationUser> userManager,
            IConfigurationService configurationService, IUserValidator<ApplicationUser> userValidator,
            SignInManager<ApplicationUser> signInManager, IPasswordValidator<ApplicationUser> passwordValidator,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _configurationService = configurationService;
            _userValidator = userValidator;
            _signInManager = signInManager;
            _passwordValidator = passwordValidator;
            _passwordHasher = passwordHasher;

            _httpContextAccessor = httpContextAccessor;
        }


        public string GetCurrentUserId()
        {
            string currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return currentUserId;
        }

        public string GetCurrentUserName()
        {
            // _httpContextAccessor.HttpContext.User.FindFirst(JwtRegisteredClaimNames.Sub).Value
            return _httpContextAccessor.HttpContext.User.FindFirst(cl => cl.Type=="sub").Value;
        }

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            //Returns a list of the names of the roles of which the user is a member
            return await _userManager.GetRolesAsync(user);
        }

        private async Task<bool> IsUserInRoleAsync(ApplicationUser user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<bool> IsUserInRole(int userId, string roleName)
        {
            var user = await GetCurrentUserAsync();
            return await IsUserInRoleAsync(user, roleName);
        }

        public bool IsUserInRole(ClaimsPrincipal user, string roleName)
        {
            return user.IsInRole(roleName);
        }

        public string GetUserId(ClaimsPrincipal user)
        {
            // ControllerBase has User, so you can use from any controller ( GetUserId(User) )
            return user.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        }

        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            //return await _userManager.Users.Where(user => user.Id == id).FirstOrDefaultAsync();
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUser> GetByPrincipal(ClaimsPrincipal principal)
        {
            return await _userManager.GetUserAsync(principal);
        }

        protected bool IsCurrentUserLoggedIn()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity.IsAuthenticated == true;
        }

        protected bool IsUserLoggedIn(ClaimsPrincipal user)
        {
            return user.Identity.IsAuthenticated;
        }

        protected bool IsUserLoggedIn(IIdentity user)
        {
            return user.IsAuthenticated;
        }


        public async Task<IdentityResult> AddUserToRolesAsync(ApplicationUser user, string roleName)
        {
            //AddToRoleAsync(user, name) Adds the user ID to the role with the specified name
            //RemoveFromRoleAsync(user, name) 
            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<List<ApplicationUser>> GetAll()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        {
            IdentityResult result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded && (user.Roles == null || user.Roles.Count == 0))
                result = await _userManager.AddToRoleAsync(user, "ROLE_USER");
            return result;
        }

        public async Task<IdentityResult> DeleteUserAsync(ApplicationUser user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public Task<bool> CheckPasswordAgainstPolicies(string password)
        {
            // Check if password complies our defined password policy
            //var task = _userManager.PasswordValidators[0].ValidateAsync(password);
            return Task.FromResult(true);
        }

        public async Task<bool> CheckPasswordValid(string password, ApplicationUser user)
        {
            foreach (var userManagerPasswordValidator in _userManager.PasswordValidators)
            {
                IdentityResult res = await userManagerPasswordValidator.ValidateAsync(_userManager, user, password);
                if (res.Succeeded)
                    return true;
            }

            return false;
        }


        public async Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword,
            string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<bool> IsAdmin()
        {
            ApplicationUser user = await GetCurrentUserAsync();
            return await IsAdmin(user);
        }

        public async Task<bool> IsAdmin(ApplicationUser user)
        {
            if (user == null)
                return false;
            return await IsUserInRoleAsync(user, _configurationService.GetAdminRoleName());
        }

        public async Task<bool> IsAuthor()
        {
            ApplicationUser user = await GetCurrentUserAsync();
            return await IsAuthor(user);
        }

        public async Task<bool> IsAuthor(ApplicationUser user)
        {
            if (user == null)
                return false;
            return await IsUserInRoleAsync(user, _configurationService.GetAuthorRoleName());
        }


        public async Task<ApplicationUser> GetByUserNameAsync(string username)
        {
            //_userManager.Users.SingleOrDefault(u => u.Email == username)
            ApplicationUser user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName.Equals(username));
            return user;
        }

        public async Task SignOut()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string roleUser)
        {
            return await _userManager.AddToRoleAsync(user, "ROLE_USER");
        }
    }
}