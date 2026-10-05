using Domain.PetPatients;

namespace Application.PetPatients.Services.Abstract;

public interface IPetPatientService
{
    Task<IReadOnlyList<PetPatient>> GetPetPatients(CancellationToken cancellationToken);
    Task<PetPatient?> GetPetPatient(string microchipId, CancellationToken cancellationToken);
    Task<PetPatient?> GetById(Guid id, CancellationToken cancellationToken);
    Task<PetPatient> Add(
        string microchipId,
        string name,
        Species species,
        int ageYears,
        bool isVaccinated,
        CancellationToken cancellationToken);
    Task<PetPatient?> Update(
        Guid id,
        string microchipId,
        string name,
        Species species,
        int ageYears,
        bool isVaccinated,
        CancellationToken cancellationToken);
    Task<bool> Delete(Guid id, CancellationToken cancellationToken);
}
