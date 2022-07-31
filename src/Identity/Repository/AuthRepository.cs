using Identity.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Repository
{
    public class AuthRepository: IAuthRepository
    {
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly OptionsJWT _optionsJWT;

        public AuthRepository(SignInManager<IdentityUser<Guid>> signInManager, UserManager<IdentityUser<Guid>> userManager, IOptions<OptionsJWT> options)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            this._optionsJWT = options.Value;
        }

        public IAsyncEnumerable<UserViewModel> GetUsers()
        {
            return _userManager.Users.Select(s => new UserViewModel
            {
                Id = s.Id,
                Name = s.UserName
            }).AsAsyncEnumerable();
        }

        public async Task<LoginResponse> Login(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, lockoutOnFailure: false);
            LoginResponse response = new LoginResponse { };

            if (result.IsNotAllowed) response.Status= LoginStatus.IsNotAllowed;

            else if (result.IsLockedOut) response.Status = LoginStatus.IsLockedOut;

            else if (result.RequiresTwoFactor) response.Status = LoginStatus.RequiresTwoFactor;

            else if (result.Succeeded)
            {
                response.Status = LoginStatus.Succeeded;
                var user = await _userManager.FindByNameAsync(model.UserName);

                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypeCustom.SID.ToString(),user.Id.ToString()),
                    new Claim(ClaimTypeCustom.UserName.ToString(),user.UserName),
                    new Claim(ClaimTypeCustom.Email.ToString(),user.Email),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypeCustom.Roles.ToString(), userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_optionsJWT.Secret));
                var token = new JwtSecurityToken(
                    issuer: _optionsJWT.ValidIssuer,
                    audience: _optionsJWT.ValidAudience,
                    expires: DateTime.UtcNow.AddDays(_optionsJWT.TokenExpireTime),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                await _userManager.AddClaimsAsync(user, authClaims);

                response.Data= new JwtSecurityTokenHandler().WriteToken(token);

            }

            return response;
        }
    }
}
