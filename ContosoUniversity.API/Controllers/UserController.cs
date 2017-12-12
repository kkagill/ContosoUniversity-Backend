using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ContosoUniversity.Data.Abstract;
using ContosoUniversity.Model;
using ContosoUniversity.Model.ViewModels;
using AutoMapper;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ContosoUniversity.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class UserController : Controller
    {
        private readonly IAdminRepository _repository; // TODO: IUserRepository

        public UserController(IAdminRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{userName}")]
        [Authorize(Roles = "User")]
        public IActionResult Get(string userName)
        {   
            ApplicationUser user = _repository.GetSingle(au => au.UserName == userName);

            if (user != null)
            {
                ApplicationUserViewModel userVM = Mapper.Map<ApplicationUser,ApplicationUserViewModel>(user);

                return new OkObjectResult(userVM);
            }
            else
                return NotFound();
        }

        [HttpPut("{userId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Put(string userId, [FromBody]ApplicationUserViewModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (userId == null)
                return NotFound();

            ApplicationUser updateUser = _repository.GetSingle(s => s.Id == userId);

            if (updateUser == null)
                return NotFound();
            else
            {
                updateUser.LastName = user.LastName;
                updateUser.FirstName = user.FirstName;

                await _repository.CommitAsync();
            }

            return new NoContentResult();
        }
    }
}