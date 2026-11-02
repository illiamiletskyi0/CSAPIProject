using FluentValidation;

namespace Application.PetPatients.Commands;

public class CreatePetPatientCommandValidator : AbstractValidator<CreatePetPatientCommand>
{
    public CreatePetPatientCommandValidator()
    {
        RuleFor(x => x.MicrochipId).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Species).IsInEnum();
        RuleFor(x => x.AgeYears).InclusiveBetween(1, 100);
        RuleFor(x => x.IsVaccinated).NotNull();
    }
}
