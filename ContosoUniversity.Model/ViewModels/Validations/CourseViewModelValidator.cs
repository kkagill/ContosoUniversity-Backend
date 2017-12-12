using ContosoUniversity.Model.ViewModels;
using FluentValidation;
using System;

namespace Scheduler.Model.ViewModels.Validations
{
    public class CourseViewModelValidator : AbstractValidator<CourseViewModel>
    {
        public CourseViewModelValidator()
        {
            RuleFor(s => s.Title).Length(3, 50).WithMessage("Title must be longer than 3 characters and less than 50 characters.");
            RuleFor(s => s.Credits).InclusiveBetween(0, 50).WithMessage("Value must be number Beetween 0, 50");
        }
    }
}