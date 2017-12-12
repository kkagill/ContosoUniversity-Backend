using Scheduler.Model.ViewModels.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ContosoUniversity.Model.ViewModels
{
    public class OfficeAssignmentViewModel
    {
        [StringLength(50)]
        public string Location { get; set; }
    }
}