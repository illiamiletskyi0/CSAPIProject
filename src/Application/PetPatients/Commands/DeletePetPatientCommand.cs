using Application.Common.Interfaces.Repositories;
using Application.PetPatients.Exceptions;
using Domain.PetPatients;
using LanguageExt;
using MediatR;

namespace Application.PetPatients.Commands;

public record DeletePetPatientCommand : IRequest<Either<PetPatientException, PetPatient>>
{
    public required Guid Id { get; init; }
}

public class DeletePetPatientCommandHandler(IPetPatientRepository petPatientRepository)
    : IRequestHandler<DeletePetPatientCommand, Either<PetPatientException, PetPatient>>
{
    public async Task<Either<PetPatientException, PetPatient>> Handle(
        DeletePetPatientCommand request,
        CancellationToken cancellationToken)
    {
        var petPatientId = new PetPatientId(request.Id);
        var petPatient = await petPatientRepository.GetById(petPatientId, cancellationToken);

        return await petPatient.MatchAsync(
            p => DeleteEntity(p, cancellationToken),
            () => new PetPatientNotFoundException(petPatientId));
    }

    private async Task<Either<PetPatientException, PetPatient>> DeleteEntity(
        PetPatient petPatient,
        CancellationToken cancellationToken)
    {
        try
        {
            await petPatientRepository.Delete(petPatient, cancellationToken);
            return petPatient;
        }
        catch (Exception exception)
        {
            return new PetPatientUnhandledException(petPatient.Id, exception);
        }
    }
}
