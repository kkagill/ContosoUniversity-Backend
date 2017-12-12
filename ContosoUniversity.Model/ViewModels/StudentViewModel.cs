using Scheduler.Model.ViewModels.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ContosoUniversity.Model.ViewModels
{
    public class StudentViewModel : IValidatableObject
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }       
        public DateTime EnrollmentDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validator = new StudentViewModelValidator();
            var result = validator.Validate(this);
            return result.Errors.Select(item => new ValidationResult(item.ErrorMessage, new[] { item.PropertyName }));
        }
    }
}