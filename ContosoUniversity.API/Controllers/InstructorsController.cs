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
using ContosoUniversity.Data;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    public class InstructorsController : Controller
    {
        private readonly IInstructorsRepository _instructorsRepository;
        private readonly IDepartmentsRepository _departmentsRepository;
        int page = 1;
        int pageSize = 4;

        public InstructorsController(IInstructorsRepository instructorsRepository,
                                     IDepartmentsRepository departmentsRepository)
        {
            _instructorsRepository = instructorsRepository;
            _departmentsRepository = departmentsRepository;
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
            var totalInstructors = await _instructorsRepository.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalInstructors / pageSize);

            IEnumerable<Instructor> instructors = _instructorsRepository
                .GetAllInstructors()
                .Skip((currentPage - 1) * currentPageSize)
                .Take(currentPageSize)
                .ToList();

            Response.AddPagination(page, pageSize, totalInstructors, totalPages);

            IEnumerable<InstructorViewModel> coursesVM = Mapper.Map<IEnumerable<Instructor>, IEnumerable<InstructorViewModel>>(instructors);

            return new OkObjectResult(coursesVM);
        }

        [HttpGet("{id}", Name = "GetInstructor")]
        public IActionResult Get(int id)
        {
            Instructor instructor = _instructorsRepository
                .GetSingle(i => i.ID == id, i => i.CourseAssignments, i => i.OfficeAssignment);

            if (instructor != null)
            {
                InstructorEditViewModel instructorDetailsVM = Mapper.Map<Instructor, InstructorEditViewModel>(instructor);
                instructorDetailsVM.AssignedCourses = _instructorsRepository.GetAssignedCourses(instructor);

                return new OkObjectResult(instructorDetailsVM);
            }
            else
                return NotFound();
        }

        [HttpGet("{id}/courses", Name = "GetInstructorCourses")]
        public async Task<IActionResult> GetInstCrs(int id)
        {
            Instructor instructor = await _instructorsRepository.GetInstructorCourses(id);

            if (instructor != null)
            {
                InstructorDetailsViewModel instructorDetailsVM = Mapper.Map<Instructor, InstructorDetailsViewModel>(instructor);

                return new OkObjectResult(instructorDetailsVM);
            }
            else
                return NotFound();
        }

        [HttpGet("{id}/students", Name = "GetInstructorCourseStudents")]
        public async Task<IActionResult> GetInstCrStds(int id)
        {
            Course course = await _instructorsRepository.GetEnrolledStudents(id);

            if (course != null)
            {
                InstructorDetailsViewModel instructorDetailsVM = Mapper.Map<Course, InstructorDetailsViewModel>(course);

                return new OkObjectResult(instructorDetailsVM);
            }
            else
                return NotFound();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody]InstructorViewModel instructor)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Instructor newInstructor = new Instructor
            {
                LastName = instructor.LastName,
                FirstMidName = instructor.FirstMidName,
                HireDate = instructor.HireDate
            };

            // One-to-one/zero 
            if (instructor.Office != null)
                newInstructor.OfficeAssignment = new OfficeAssignment { InstructorID = newInstructor.ID, Location = instructor.Office };

            // One-to-Many
            if (instructor.SelectedCourses != null)
            {
                newInstructor.CourseAssignments = new List<CourseAssignment>();
                foreach (var course in instructor.SelectedCourses)
                {
                    var courseToAdd = new CourseAssignment { InstructorID = instructor.ID, CourseID = int.Parse(course) };
                    newInstructor.CourseAssignments.Add(courseToAdd);
                }
            }

            await _instructorsRepository.AddAsync(newInstructor);
            await _instructorsRepository.CommitAsync();

            // Need to grab Course (.ThenInclude(ca => ca.Course)) since newInstructor does not retrieve Course entity after CommitAsync() 
            // when InstructorViewModel maps (Title = ca.Course.Title)          
            Instructor InstructorWithCourse = await _instructorsRepository.GetInstructorCourses(newInstructor.ID);

            instructor = Mapper.Map<Instructor, InstructorViewModel>(InstructorWithCourse);

            return CreatedAtRoute("GetInstructor", new { controller = "Instructors", id = instructor.ID }, instructor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]InstructorViewModel instructor)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Instructor updateInstructor = _instructorsRepository
                .GetSingle(i => i.ID == id, i => i.CourseAssignments, i => i.OfficeAssignment);

            if (updateInstructor == null)
                return NotFound();
            else
            {
                // Need this b/c of the error: The instance of entity type 'OfficeAssignment' cannot be tracked ...
                bool hasOffice = _instructorsRepository.CheckOfficeExist(updateInstructor.ID, instructor.Office);

                updateInstructor.LastName = instructor.LastName;
                updateInstructor.FirstMidName = instructor.FirstMidName;
                updateInstructor.HireDate = instructor.HireDate;
                                
                if (hasOffice == false)
                {
                    OfficeAssignment officeAssignment = new OfficeAssignment { InstructorID = id, Location = instructor.Office };
                    updateInstructor.OfficeAssignment = officeAssignment;
                }

                _instructorsRepository.UpdateInstructorCourses(instructor.SelectedCourses, updateInstructor);
                
                await _instructorsRepository.CommitAsync();
            }

            //student = Mapper.Map<Student, StudentViewModel>(updateStudent);

            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            Instructor deleteInstructor = _instructorsRepository
                .GetSingle(i => i.ID == id, i => i.CourseAssignments);
            var departments = _departmentsRepository
                .FindBy(d => d.InstructorID == id)
                .ToList();

            departments.ForEach(d => d.InstructorID = null); // Delete instructor(s) in Departments

            if (deleteInstructor == null)
                return new NotFoundResult();
            else
            {              
                _instructorsRepository.Delete(deleteInstructor);

                await _instructorsRepository.CommitAsync();

                return new NoContentResult();
            }
        }
    }
}