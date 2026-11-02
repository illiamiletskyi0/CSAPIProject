using Application.Common.Interfaces.Repositories;
using Domain.PetPatients;
using MediatR;

namespace Application.PetPatients.Commands;

public record UpdatePetPatientCommand : IRequest<PetPatient?>
{
    public required Guid Id { get; init; }
    public required string MicrochipId { get; init; }
    public required string Name { get; init; }
    public required Species Species { get; init; }
    public required int AgeYears { get; init; }
    public required bool IsVaccinated { get; init; }
}

public class UpdatePetPatientCommandHandler(IPetPatientRepository petPatientRepository)
    : IRequestHandler<UpdatePetPatientCommand, PetPatient?>
{
    public async Task<PetPatient?> Handle(UpdatePetPatientCommand request, CancellationToken cancellationToken)
    {
        var petPatient = await petPatientRepository.GetById(request.Id, cancellationToken);
        if (petPatient is null)
        {
            return null;
        }

        var petPatientWithSameMicrochipId = await petPatientRepository.GetByMicrochipId(
            request.MicrochipId, cancellationToken);
        if (petPatientWithSameMicrochipId is not null && petPatientWithSameMicrochipId.Id != request.Id)
        {
            throw new ArgumentException($"PetPatient with microchip id '{request.MicrochipId}' already exists");
        }

        petPatient.UpdateDetails(
            request.Name,
            request.Species,
            request.AgeYears,
            request.IsVaccinated,
            request.MicrochipId);

        return await petPatientRepository.Update(petPatient, cancellationToken);
    }
}