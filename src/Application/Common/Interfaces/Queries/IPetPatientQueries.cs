using Domain.PetPatients;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IPetPatientQueries
{
    Task<IReadOnlyList<PetPatient>> GetAll(CancellationToken cancellationToken);
    Task<Option<PetPatient>> GetById(PetPatientId id, CancellationToken cancellationToken);
    Task<Option<PetPatient>> GetByMicrochipId(string microchipId, CancellationToken cancellationToken);
}
