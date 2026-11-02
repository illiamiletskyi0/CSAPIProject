using Application.Common.Interfaces.Repositories;
using Domain.PetPatients;
using MediatR;

namespace Application.PetPatients.Commands;

public record CreatePetPatientCommand : IRequest<PetPatient>
{
    public required string MicrochipId { get; init; }
    public required string Name { get; init; }
    public required Species Species { get; init; }
    public required int AgeYears { get; init; }
    public required bool IsVaccinated { get; init; }
}

public class CreatePetPatientCommandHandler(IPetPatientRepository petPatientRepository)
    : IRequestHandler<CreatePetPatientCommand, PetPatient>
{
    public async Task<PetPatient> Handle(CreatePetPatientCommand request, CancellationToken cancellationToken)
    {
        var existingPetPatient = await petPatientRepository.GetByMicrochipId(request.MicrochipId,
            cancellationToken);
        if (existingPetPatient is not null)
        {
            throw new ArgumentException($"Pet patient with microchip id '{request.MicrochipId}' already exists");
        }
        var petPatient = PetPatient.New(
            Guid.NewGuid(),
            request.Name,
            request.Species,
            request.AgeYears,
            request.IsVaccinated,
            request.MicrochipId);
        return await petPatientRepository.Add(petPatient, cancellationToken);
    }
}