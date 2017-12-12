using ContosoUniversity.Model.ViewModels;
using FluentValidation;
using System;

namespace Scheduler.Model.ViewModels.Validations
{
    public class StudentViewModelValidator : AbstractValidator<StudentViewModel>
    {
        public StudentViewModelValidator()
        {
            RuleFor(s => s.LastName).Length(1, 50).WithMessage("Last Name cannot be longer than 50 characters.");
            RuleFor(s => s.FirstMidName).Length(1, 50).WithMessage("First Name cannot be longer than 50 characters.");

            RuleFor(s => s.LastName).NotEmpty().WithMessage("Last Name cannot be empty.");
            RuleFor(s => s.FirstMidName).NotEmpty().WithMessage("First Name cannot be empty.");
        }
    }
}