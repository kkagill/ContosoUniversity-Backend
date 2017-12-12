using AutoMapper;
using Scheduler.Model.ViewModels.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ContosoUniversity.Model.ViewModels
{
    public class StudentDetailsViewModel
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }    
        public DateTime EnrollmentDate { get; set; }

        public ICollection<EnrollmentViewModel> Enrollments { get; set; }
    }
}