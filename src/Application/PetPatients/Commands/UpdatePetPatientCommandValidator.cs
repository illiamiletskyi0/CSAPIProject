using FluentValidation;

namespace Application.PetPatients.Commands;

public class UpdatePetPatientCommandValidator : AbstractValidator<UpdatePetPatientCommand>
{
    public UpdatePetPatientCommandValidator()
    {
        RuleFor(x => x.MicrochipId).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Species).IsInEnum();
        RuleFor(x => x.AgeYears).InclusiveBetween(1, 100);
        RuleFor(x => x.IsVaccinated).NotNull();
    }
}
