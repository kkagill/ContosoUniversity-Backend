using ContosoUniversity.Model;
using ContosoUniversity.Model.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContosoUniversity.Data.Abstract
{
    public interface IAdminRepository : IEntityBaseRepository<ApplicationUser>
    {
        IEnumerable<ApplicationUser> GetUsers();
        void EnableLockOut(string userName);
        void DisableLockOut(string userName);
    }

    public interface IStudentsRepository : IEntityBaseRepository<Student>
    {
        Task<Student> GetStudentAsync(int? id);
    }

    public interface ICoursesRepository : IEntityBaseRepository<Course>
    {
        IEnumerable<Department> PopulateDepartmentDropdownList();
    }

    public interface IInstructorsRepository : IEntityBaseRepository<Instructor>
    {
        IEnumerable<Instructor> GetAllInstructors();
        Task<Instructor> GetInstructorCourses(int id);
        Task<Course> GetEnrolledStudents(int courseId);
        List<AssignedCourseViewModel> GetAssignedCourses(Instructor instructor);
        void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate);
        bool CheckOfficeExist(int id, string office);
    }

    public interface IDepartmentsRepository : IEntityBaseRepository<Department> { }

    public interface IEnrollmentRepository : IEntityBaseRepository<Enrollment> { }

    public interface IUserRepository : IEntityBaseRepository<ApplicationUser> { }
}