using AutoMapper;
using Scheduler.Model.ViewModels.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ContosoUniversity.Model.ViewModels
{
    public class EnrollmentViewModel
    {
        public string Title { get; set; }     
        public string Grade { get; set; }
        public string FullName { get; set; }
    }
}