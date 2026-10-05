using Domain.PetPatients;
using Application.Common.Interfaces;
using Application.PetPatients.Services.Abstract;

namespace Application.PetPatients.Services.Implementation;

public class PetPatientService(IPetPatientRepository petPatientRepository) : IPetPatientService
{
    public async Task<IReadOnlyList<PetPatient>> GetPetPatients(CancellationToken cancellationToken)
    {
        return await petPatientRepository.GetAll(cancellationToken);
    }

    public async Task<PetPatient?> GetPetPatient(string microchipId, CancellationToken cancellationToken)
    {
        return await petPatientRepository.GetByMicrochipId(microchipId, cancellationToken);
    }
    
    public async Task<PetPatient?> GetById(Guid id, CancellationToken cancellationToken)
    {
        return await petPatientRepository.GetById(id, cancellationToken);
    }
    
    public async Task<PetPatient> Add(
        string microchipId,
        string name,
        Species species,
        int ageYears,
        bool isVaccinated,
        CancellationToken cancellationToken)
    {
        var existingPetPatient = await petPatientRepository.GetByMicrochipId(microchipId, cancellationToken);
        if (existingPetPatient is not null)
        {
            throw new ArgumentException($"PetPatient with microchip id '{microchipId}' already exists");
        }
        var petPatient = PetPatient.New(Guid.NewGuid(), name, species, ageYears, isVaccinated, microchipId);
        return await petPatientRepository.Add(petPatient, cancellationToken);
    }
    
    public async Task<PetPatient?> Update(
        Guid id,
        string microchipId,
        string name,
        Species species,
        int ageYears,
        bool isVaccinated,
        CancellationToken cancellationToken)
    {
        var petPatient = await petPatientRepository.GetById(id, cancellationToken);
        if (petPatient is null)
        {
            return null;
        }
        var petPatientWithSameMicrochipId = await petPatientRepository.GetByMicrochipId(microchipId,
            cancellationToken);
        if (petPatientWithSameMicrochipId is not null && petPatientWithSameMicrochipId.Id != id)
        {
            throw new ArgumentException($"PetPatient with microchip id '{microchipId}' already exists");
        }
        petPatient.UpdateDetails(name, species, ageYears, isVaccinated, microchipId);
        return await petPatientRepository.Update(petPatient, cancellationToken);
    }
    
    public async Task<bool> Delete(Guid id, CancellationToken cancellationToken)
    {
        var petPatient = await petPatientRepository.GetById(id, cancellationToken);
        if (petPatient is null)
        {
            return false;
        }
        await petPatientRepository.Delete(petPatient, cancellationToken);
        return true;
    }
}