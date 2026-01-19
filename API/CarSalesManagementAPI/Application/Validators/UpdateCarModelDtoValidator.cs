using FluentValidation;
using CarSalesManagementAPI.Application.DTOs;

namespace CarSalesManagementAPI.Application.Validators;

public class UpdateCarModelDtoValidator : AbstractValidator<UpdateCarModelDto>
{
    public UpdateCarModelDtoValidator()
    {
        RuleFor(x => x.ModelID)
            .GreaterThan(0).WithMessage("Model ID is required.");

        RuleFor(x => x.BrandID)
            .GreaterThan(0).WithMessage("Brand is required.");

        RuleFor(x => x.ClassID)
            .GreaterThan(0).WithMessage("Class is required.");

        RuleFor(x => x.ModelName)
            .NotEmpty().WithMessage("Model Name is required.")
            .MaximumLength(100).WithMessage("Model Name cannot exceed 100 characters.");

        RuleFor(x => x.ModelCode)
            .NotEmpty().WithMessage("Model Code is required.")
            .Length(10).WithMessage("Model Code must be exactly 10 characters.")
            .Matches(@"^[A-Z0-9]{10}$").WithMessage("Model Code must be 10 alphanumeric characters only (uppercase).");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(x => x.Features)
            .NotEmpty().WithMessage("Features is required.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.DateOfManufacturing)
            .NotEmpty().WithMessage("Date of Manufacturing is required.")
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Date of Manufacturing cannot be in the future.");

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Sort Order must be 0 or greater.");
    }
}
