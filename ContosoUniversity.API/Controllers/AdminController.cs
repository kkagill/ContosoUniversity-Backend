using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ContosoUniversity.Data.Abstract;
using ContosoUniversity.Model;
using ContosoUniversity.Model.ViewModels;
using AutoMapper;

namespace ContosoUniversity.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class AdminController : Controller
    {
        private readonly IAdminRepository _repository;

        public AdminController(IAdminRepository repository)
        {
            _repository = repository;
        }
       
        [HttpGet(Name = "GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public IActionResult Get()
        {  
            IEnumerable<ApplicationUser> users = _repository
                .GetUsers()
                .OrderBy(ap => ap.UserName)
                .ToList();         

            IEnumerable<ApplicationUserViewModel> userVM = Mapper.Map<IEnumerable<ApplicationUser>, 
                                                                      IEnumerable<ApplicationUserViewModel>>(users);

            return new OkObjectResult(userVM);
        }

        [HttpPost("EnableLockOut")]
        [Authorize(Roles = "Admin")]
        public IActionResult EnableLockOut([FromBody]string userName)
        {
            try
            {
                _repository.EnableLockOut(userName);

                return CreatedAtRoute("GetAllUsers", new { controller = "Admin" });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("DisableLockOut")]
        [Authorize(Roles = "Admin")]
        public IActionResult DisableLockOut([FromBody]string userName)
        {
            try
            {
                _repository.DisableLockOut(userName);

                return CreatedAtRoute("GetAllUsers", new { controller = "Admin" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}