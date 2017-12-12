using ContosoUniversity.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace ContosoUniversity.Model.ViewModels
{
    public class InstructorEditViewModel : IInstructor
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }
        public DateTime HireDate { get; set; }
        public string Office { get; set; }
   
        public IEnumerable<AssignedCourseViewModel> AssignedCourses { get; set; }
    }
}