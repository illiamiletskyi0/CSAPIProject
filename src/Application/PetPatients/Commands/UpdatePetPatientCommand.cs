using Application.Common.Interfaces.Repositories;
using Application.PetPatients.Exceptions;
using Domain.PetPatients;
using LanguageExt;
using MediatR;
using Unit = LanguageExt.Unit;

namespace Application.PetPatients.Commands;

public record UpdatePetPatientCommand : IRequest<Either<PetPatientException, PetPatient>>
{
    public required Guid Id { get; init; }
    public required string MicrochipId { get; init; }
    public required string Name { get; init; }
    public required Species Species { get; init; }
    public required int AgeYears { get; init; }
    public required bool IsVaccinated { get; init; }
}

public class UpdatePetPatientCommandHandler(IPetPatientRepository petPatientRepository)
    : IRequestHandler<UpdatePetPatientCommand, Either<PetPatientException, PetPatient>>
{
    public async Task<Either<PetPatientException, PetPatient>> Handle(
        UpdatePetPatientCommand request,
        CancellationToken cancellationToken)
    {
        var petPatientId = new PetPatientId(request.Id);
        var petPatient = await petPatientRepository.GetById(petPatientId, cancellationToken);

        return await petPatient.MatchAsync(
            p => CheckDuplicates(p.Id, request.MicrochipId, cancellationToken)
                .BindAsync(_ => UpdateEntity(p, request, cancellationToken)),
            () => new PetPatientNotFoundException(petPatientId));
    }

    private async Task<Either<PetPatientException, Unit>> CheckDuplicates(
        PetPatientId currentPetPatientId,
        string microchipId,
        CancellationToken cancellationToken)
    {
        var petPatient = await petPatientRepository.GetByMicrochipId(microchipId, cancellationToken);

        return petPatient.Match<Either<PetPatientException, Unit>>(
            p => p.Id == currentPetPatientId ? Unit.Default : new PetPatientAlreadyExistsException(p.Id),
            () => Unit.Default);
    }

    private async Task<Either<PetPatientException, PetPatient>> UpdateEntity(
        PetPatient petPatient,
        UpdatePetPatientCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            petPatient.UpdateDetails(
                request.Name,
                request.Species,
                request.AgeYears,
                request.IsVaccinated,
                request.MicrochipId);

            return await petPatientRepository.Update(petPatient, cancellationToken);
        }
        catch (Exception exception)
        {
            return new PetPatientUnhandledException(petPatient.Id, exception);
        }
    }
}
