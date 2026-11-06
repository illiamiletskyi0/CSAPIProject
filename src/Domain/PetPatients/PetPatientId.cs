namespace Domain.PetPatients;

public record PetPatientId(Guid Value)
{
    public static PetPatientId Empty() => new(Guid.Empty);
    public static PetPatientId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
