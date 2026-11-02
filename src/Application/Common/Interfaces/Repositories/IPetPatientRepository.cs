using Domain.PetPatients;

namespace Application.Common.Interfaces.Repositories;

public interface IPetPatientRepository
{
    Task<PetPatient?> GetById(Guid id, CancellationToken cancellationToken);
    Task<PetPatient?> GetByMicrochipId(string microchipId, CancellationToken cancellationToken);
    Task<PetPatient> Add(PetPatient petPatient, CancellationToken cancellationToken);
    Task<PetPatient> Update(PetPatient petPatient, CancellationToken cancellationToken);
    Task Delete(PetPatient petPatient, CancellationToken cancellationToken);
}