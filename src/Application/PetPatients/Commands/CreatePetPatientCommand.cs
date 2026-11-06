using Application.Common.Interfaces.Repositories;
using Application.PetPatients.Exceptions;
using Domain.PetPatients;
using LanguageExt;
using MediatR;

namespace Application.PetPatients.Commands;

public record CreatePetPatientCommand : IRequest<Either<PetPatientException, PetPatient>>
{
    public required string MicrochipId { get; init; }
    public required string Name { get; init; }
    public required Species Species { get; init; }
    public required int AgeYears { get; init; }
    public required bool IsVaccinated { get; init; }
}

public class CreatePetPatientCommandHandler(IPetPatientRepository petPatientRepository)
    : IRequestHandler<CreatePetPatientCommand, Either<PetPatientException, PetPatient>>
{
    public async Task<Either<PetPatientException, PetPatient>> Handle(
        CreatePetPatientCommand request,
        CancellationToken cancellationToken)
    {
        var existingPetPatient = await petPatientRepository.GetByMicrochipId(request.MicrochipId,
            cancellationToken);

        return await existingPetPatient.MatchAsync(
            p => new PetPatientAlreadyExistsException(p.Id),
            () => CreateEntity(request, cancellationToken));
    }

    private async Task<Either<PetPatientException, PetPatient>> CreateEntity(
        CreatePetPatientCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var petPatient = PetPatient.New(
                PetPatientId.New(),
                request.Name,
                request.Species,
                request.AgeYears,
                request.IsVaccinated,
                request.MicrochipId);
            return await petPatientRepository.Add(petPatient, cancellationToken);
        }
        catch (Exception exception)
        {
            return new PetPatientUnhandledException(PetPatientId.Empty(), exception);
        }
    }
}
