namespace Domain.PetPatients;

public class PetPatient
{
    public PetPatientId Id { get; }
    public string MicrochipId { get; private set; }
    public string Name { get; private set; }
    public Species Species { get; private set; }
    public int AgeYears { get; private set; }
    public bool IsVaccinated { get; private set; }
    public DateTime CreatedAt { get; }

    private PetPatient(PetPatientId id, string name, Species species, int ageYears, bool isVaccinated, string microchipId, DateTime createdAt)
        => (Id, Name, Species, AgeYears, IsVaccinated, MicrochipId, CreatedAt) = (id, name, species, ageYears, isVaccinated, microchipId, createdAt);
    
    public static PetPatient New(PetPatientId id, string name, Species species, int ageYears, bool isVaccinated, string microchipId)
        => new(id, name, species, ageYears, isVaccinated, microchipId, DateTime.UtcNow);

    public void UpdateDetails(string name, Species species, int ageYears, bool isVaccinated, string microchipId)
        => (Name, Species, AgeYears, IsVaccinated, MicrochipId) = (name, species, ageYears, isVaccinated, microchipId);
}
