using Application.Common.Interfaces.Repositories;
using Domain.PetPatients;
using MediatR;

namespace Application.PetPatients.Commands;

public record DeletePetPatientCommand : IRequest<bool>
{
    public required Guid Id { get; init; }
}

public class DeletePetPatientCommandHandler(IPetPatientRepository petPatientRepository)
    : IRequestHandler<DeletePetPatientCommand, bool>
{
    public async Task<bool> Handle(DeletePetPatientCommand request, CancellationToken cancellationToken)
    {
        var petPatient = await petPatientRepository.GetById(request.Id, cancellationToken);
        if (petPatient is null)
        {
            return false;
        }

        await petPatientRepository.Delete(petPatient, cancellationToken);
        return true;
    }
}