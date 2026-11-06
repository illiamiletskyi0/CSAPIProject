using Domain.PetPatients;

namespace Api.Dtos;

public record PetPatientDto(
    Guid Id,
    string Name,
    Species Species,
    int AgeYears,
    bool IsVaccinated,
    string MicrochipId,
    DateTime CreatedAt)
{
    public static PetPatientDto FromDomainModel(PetPatient petPatient)
        => new(
            petPatient.Id.Value,
            petPatient.Name,
            petPatient.Species,
            petPatient.AgeYears,
            petPatient.IsVaccinated,
            petPatient.MicrochipId,
            petPatient.CreatedAt);
}

public record CreatePetPatientDto(
    string MicrochipId,
    string Name,
    Species Species,
    int AgeYears,
    bool IsVaccinated);
    
    
public record UpdatePetPatientDto(
    string MicrochipId,
    string Name,
    Species Species,
    int AgeYears,
    bool IsVaccinated);