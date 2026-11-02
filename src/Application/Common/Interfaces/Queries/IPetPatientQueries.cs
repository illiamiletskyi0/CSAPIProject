using Domain.PetPatients;

namespace Application.Common.Interfaces.Queries;

public interface IPetPatientQueries
{
    Task<IReadOnlyList<PetPatient>> GetAll(CancellationToken cancellationToken);
    Task<PetPatient?> GetById(Guid id, CancellationToken cancellationToken);
    Task<PetPatient?> GetByMicrochipId(string microchipId, CancellationToken cancellationToken);
}