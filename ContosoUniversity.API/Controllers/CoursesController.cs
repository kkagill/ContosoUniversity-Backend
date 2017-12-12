using AutoMapper;
using System.Threading.Tasks;
using ContosoUniversity.Model;
using Microsoft.AspNetCore.Mvc;
using ContosoUniversity.Data.Abstract;
using ContosoUniversity.Model.ViewModels;
using System.Collections.Generic;
using System;
using System.Linq;
using ContosoUniversity.API.Core;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    public class CoursesController : Controller
    {
        private readonly ICoursesRepository _coursesRepository;
        int page = 1;
        int pageSize = 4;

        public CoursesController(ICoursesRepository coursesRepository)
        {
            _coursesRepository = coursesRepository;
        }

        public async Task<IActionResult> Get()
        {
            var pagination = Request.Headers["Pagination"];

            if (!string.IsNullOrEmpty(pagination))
            {
                string[] vals = pagination.ToString().Split(',');
                int.TryParse(vals[0], out page);
                int.TryParse(vals[1], out pageSize);
            }

            int currentPage = page;
            int currentPageSize = pageSize;
            var totalCourses = await _coursesRepository.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCourses / pageSize);

            IEnumerable<Course> courses = _coursesRepository
                .AllIncluding(c => c.Department)
                .OrderBy(c => c.CourseID)
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            Response.AddPagination(page, pageSize, totalCourses, totalPages);

            IEnumerable<CourseViewModel> coursesVM = Mapper.Map<IEnumerable<Course>, IEnumerable<CourseViewModel>>(courses);

            return new OkObjectResult(coursesVM);
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            IEnumerable<Course> courses = _coursesRepository.GetAll();
            IEnumerable<CourseViewModel> coursesVM = Mapper.Map<IEnumerable<Course>, IEnumerable<CourseViewModel>>(courses);

            return new OkObjectResult(coursesVM);
        }

        [HttpGet("{id}", Name = "GetCourse")]
        public IActionResult Get(int? id)
        {
            if (id == null)
                return NotFound();

            Course course = _coursesRepository.GetSingle(c => c.CourseID == id, c => c.Department);

            if (course != null)
            {
                CourseViewModel courseVM = Mapper.Map<Course, CourseViewModel>(course);

                return new OkObjectResult(courseVM);
            }
            else
                return NotFound();
        }

        [HttpGet("departments")]
        public IActionResult GetDepartments()
        {          
            IEnumerable<Department> department = _coursesRepository.PopulateDepartmentDropdownList();

            if (department != null)
            {
                return new OkObjectResult(department);
            }
            else
                return StatusCode(500);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody]CourseViewModel course)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Course newCourse = new Course
            {
                CourseID = course.CourseID,
                Title = course.Title,
                Credits = course.Credits,
                DepartmentID = course.DepartmentID
            };

            await _coursesRepository.AddAsync(newCourse);
            await _coursesRepository.CommitAsync();

            course = Mapper.Map<Course, CourseViewModel>(newCourse);

            return CreatedAtRoute("GetCourse", new { controller = "Courses", id = course.CourseID }, course);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int? id, [FromBody]CourseViewModel course)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id == null)
                return NotFound();

            Course updateCourse = _coursesRepository.GetSingle(s => s.CourseID == id);

            if (updateCourse == null)
                return NotFound();
            else
            {
                updateCourse.Title = course.Title;
                updateCourse.Credits = course.Credits;
                updateCourse.DepartmentID = course.DepartmentID;

                await _coursesRepository.CommitAsync();
            }

            //student = Mapper.Map<Student, StudentViewModel>(updateStudent);

            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            var deleteCourse = _coursesRepository.GetSingle(c => c.CourseID == id, c => c.Department);

            if (deleteCourse == null)
                return new NotFoundResult();
            else
            {
                _coursesRepository.Delete(deleteCourse);

                await _coursesRepository.CommitAsync();

                return new NoContentResult();
            }
        }
    }
}