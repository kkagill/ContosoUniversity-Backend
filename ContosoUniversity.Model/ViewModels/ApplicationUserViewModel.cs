using FluentValidation;
using Scheduler.Model.ViewModels.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ContosoUniversity.Model.ViewModels
{
    public class ApplicationUserViewModel
    {                
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? LockoutEnd { get; set; }
    }
}