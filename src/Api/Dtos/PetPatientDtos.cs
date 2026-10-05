using System.ComponentModel.DataAnnotations;
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
            petPatient.Id,
            petPatient.Name,
            petPatient.Species,
            petPatient.AgeYears,
            petPatient.IsVaccinated,
            petPatient.MicrochipId,
            petPatient.CreatedAt);
}

public record CreatePetPatientDto(
    [Required, MaxLength(255)] string MicrochipId,
    [MaxLength(255)] string Name,
    Species Species,
    [Range(1, 100)] int AgeYears,
    [Required] bool IsVaccinated);
    
    
public record UpdatePetPatientDto(
    [Required, MaxLength(255)] string MicrochipId,
    [MaxLength(255)] string Name,
    Species Species,
    [Range(1, 100)] int AgeYears,
    [Required] bool IsVaccinated);