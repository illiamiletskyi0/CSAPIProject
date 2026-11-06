using Domain.PetPatients;
using LanguageExt;

namespace Application.Common.Interfaces.Repositories;

public interface IPetPatientRepository
{
    Task<Option<PetPatient>> GetById(PetPatientId id, CancellationToken cancellationToken);
    Task<Option<PetPatient>> GetByMicrochipId(string microchipId, CancellationToken cancellationToken);
    Task<PetPatient> Add(PetPatient petPatient, CancellationToken cancellationToken);
    Task<PetPatient> Update(PetPatient petPatient, CancellationToken cancellationToken);
    Task Delete(PetPatient petPatient, CancellationToken cancellationToken);
}
