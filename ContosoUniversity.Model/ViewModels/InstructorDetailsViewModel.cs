using System;
using System.Collections.Generic;

namespace ContosoUniversity.Model.ViewModels
{
    public class InstructorDetailsViewModel
    {
        public int ID { get; set; }

        public IEnumerable<CourseViewModel> Courses { get; set; }
        public IEnumerable<EnrollmentViewModel> Enrollments { get; set; }
    }
}