using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ContosoUniversity.Model;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using ContosoUniversity.Model.ViewModels;

namespace ContosoUniversity.API.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {     
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private JwtIssuerOptions _jwtOptions;
        private readonly JsonSerializerSettings _serializerSettings;        

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 IOptions<JwtIssuerOptions> jwtOptions)
        {         
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtOptions = jwtOptions.Value;

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
        }

    //    private async Task<List<Claim>> GetValidClaims(ApplicationUser user)
    //    {
    //        IdentityOptions _options = new IdentityOptions();
    //        var claims = new List<Claim>
    //    {
    //        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
    //        new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),         
    //        new Claim(_options.ClaimsIdentity.UserIdClaimType, user.Id.ToString()),
    //        new Claim(_options.ClaimsIdentity.UserNameClaimType, user.UserName)
    //    };
    //        var userClaims = await _userManager.GetClaimsAsync(user);
    //        var userRoles = await _userManager.GetRolesAsync(user);
    //        claims.AddRange(userClaims);
    //        foreach (var userRole in userRoles)
    //        {
    //            claims.Add(new Claim(ClaimTypes.Role, userRole));
    //            var role = await _userManager.FindByNameAsync(userRole);
    //            if (role != null)
    //            {
    //                var roleClaims = await _userManager.GetClaimsAsync(role);
    //                foreach (Claim roleClaim in roleClaims)
    //                {
    //                    claims.Add(roleClaim);
    //                }
    //            }
    //        }
    //        return claims;
    //    }

    //    [HttpPost("token")]
    //    public async Task<IActionResult> Token([FromBody] LoginViewModel model)
    //    {
    //        if (!ModelState.IsValid)
    //        {
    //            return BadRequest();
    //        }

    //        var user = await _userManager.FindByNameAsync(model.Email);
            
    //        var token = await GetJwtSecurityToken(user);

    //        return Ok(new
    //        {
    //            token = new JwtSecurityTokenHandler().WriteToken(token),
    //            expiration = token.ValidTo
    //        });
    //    }

    //    private async Task<JwtSecurityToken> GetJwtSecurityToken(ApplicationUser user)
    //    {
    //        var userClaims = await _userManager.GetClaimsAsync(user);

    //        return new JwtSecurityToken(
    //             issuer: _jwtOptions.Issuer,
    //            audience: _jwtOptions.Audience,
    //            claims: GetTokenClaims(user).Union(userClaims),
    //            expires: DateTime.UtcNow.AddMinutes(10),
    //            signingCredentials: _jwtOptions.SigningCredentials
    //        );
    //    }

    //    private static IEnumerable<Claim> GetTokenClaims(ApplicationUser user)
    //    {
    //        return new List<Claim>
    //{
    //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
    //    new Claim(JwtRegisteredClaimNames.Sub, user.UserName)
    //};
    //    }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Wrong username or password");
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByNameAsync(model.Email);
            // Get valid claims and pass them into JWT
            //var claims = await GetValidClaims(user);

            //// Create the JWT security token and encode it.
            //var jwt = new JwtSecurityToken(
            //    issuer: _jwtOptions.Issuer,
            //    audience: _jwtOptions.Audience,
            //    claims: claims,
            //    notBefore: _jwtOptions.NotBefore,
            //    expires: _jwtOptions.Expiration,
            //    signingCredentials: _jwtOptions.SigningCredentials);

            //var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Serialize and return the response
            //var response = new
            //{
            //    access_token = encodedJwt,
            //    expires_in = (int)_jwtOptions.ValidFor.TotalSeconds
            //};

            //var json = JsonConvert.SerializeObject(response, _serializerSettings);
            //return new OkObjectResult(json);
            return new OkResult();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody]RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                string errorMsg = null;

                foreach (var test in ModelState.Values)
                {
                    foreach (var msg in test.Errors)
                    {
                        errorMsg = msg.ErrorMessage;
                    }
                }
                return BadRequest(errorMsg);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var userAccount = await _userManager.FindByEmailAsync(model.Email);
                // This code can be deleted when the user must activate their account via email.
                userAccount.EmailConfirmed = true;

                // Create user role                
                var findUserRole = await _roleManager.FindByNameAsync("User");
                var userRole = new IdentityRole("User");

                //If user role does not exists, create it
                if (findUserRole == null)
                {
                    await _roleManager.CreateAsync(userRole);
                }

                // Add userAccount to a user role
                if (!await _userManager.IsInRoleAsync(userAccount, userRole.Name))
                {
                    await _userManager.AddToRoleAsync(userAccount, userRole.Name);
                }

                return new OkResult();
            }

            // If result is not successful, add error message(s)
            AddErrors(result);

            return new BadRequestObjectResult(result.Errors);
        }

        #region Helpers
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        #endregion
    }
}