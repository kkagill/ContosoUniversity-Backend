using ContosoUniversity.Data;
using ContosoUniversity.Data.Abstract;
using ContosoUniversity.Data.Repositories;
using ContosoUniversity.Model;
using ContosoUniversity.Model.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Data.Repositories
{
    public class InstructorsRepository : EntityBaseRepository<Instructor>, IInstructorsRepository
    {
        private ContosoContext _context;

        public InstructorsRepository(ContosoContext context)
            : base(context)
        {
            _context = context;
        }

        public IEnumerable<Instructor> GetAllInstructors()
        {
            IEnumerable<Instructor> result = _context.Instructors
                  .Include(i => i.OfficeAssignment)
                  .Include(i => i.CourseAssignments)
                    .ThenInclude(i => i.Course)                            
                  .AsNoTracking()
                  .OrderBy(i => i.LastName);

            return result;
        }

        public async Task<Instructor> GetInstructorCourses(int id)
        {
            var result = await _context.Instructors
                  .Include(i => i.OfficeAssignment)
                  .Include(i => i.CourseAssignments)
                    .ThenInclude(ca => ca.Course)
                        .ThenInclude(c => c.Department)
                  .Where(i => i.ID == id)
                  .AsNoTracking()
                  .OrderBy(i => i.CourseAssignments.Select(c => c.CourseID))
                  .SingleOrDefaultAsync();

            return result;
        }

        public async Task<Course> GetEnrolledStudents(int courseId)
        {
            var result = await _context.Courses
                   .Include(i => i.Enrollments)
                     .ThenInclude(e => e.Student)
                   .Where(i => i.CourseID == courseId)
                   .AsNoTracking()
                   .OrderBy(i => i.CourseID)
                   .SingleOrDefaultAsync();

            return result;
        }

        public List<AssignedCourseViewModel> GetAssignedCourses(Instructor instructor)
        {
            var allCourses = _context.Courses;
            var instructorCourses = new HashSet<int>(instructor.CourseAssignments.Select(c => c.CourseID));
            var viewModel = new List<AssignedCourseViewModel>();

            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseViewModel
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)
                });
            }

            return viewModel;
        }

        public void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
        {
            if (selectedCourses == null)
            {
                instructorToUpdate.CourseAssignments = new List<CourseAssignment>();
                return;
            }

            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>
                (instructorToUpdate.CourseAssignments.Select(c => c.CourseID));

            foreach (var course in _context.Courses)
            {
                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    if (!instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.CourseAssignments.Add(new CourseAssignment { InstructorID = instructorToUpdate.ID, CourseID = course.CourseID });
                    }
                }
                else
                {
                    if (instructorCourses.Contains(course.CourseID))
                    {
                        CourseAssignment courseToRemove = instructorToUpdate.CourseAssignments.SingleOrDefault(i => i.CourseID == course.CourseID);
                        _context.Remove(courseToRemove);
                    }
                }
            }
        }

        public bool CheckOfficeExist(int id, string office)
        {
            OfficeAssignment checkOfficeExist = _context.OfficeAssignments
                  .Where(x => x.InstructorID == id)
                  .SingleOrDefault();

            // Add new office if instructor didn't have one and instructor.Office has value
            if (checkOfficeExist == null && !string.IsNullOrWhiteSpace(office))
            {
                return false;                
            }
            // Change office name if instructor already had one and instructor.Office has value
            else if (checkOfficeExist != null && !string.IsNullOrWhiteSpace(office))
                checkOfficeExist.Location = office;
            // Remove Office if instructor.Office is null and instructor had office before
            else if (checkOfficeExist != null && string.IsNullOrWhiteSpace(office))
                _context.OfficeAssignments.Remove(checkOfficeExist);

            return true;
        }
    }
}