using Domain.PetPatients;

namespace Tests.Data.PetPatients;

public static class PetPatientsTestData
{
    public static PetPatient FirstTestPetPatient() => PetPatient.New(
        PetPatientId.New(),
        "Fluffy",
        Species.Dog,
        3,
        true,
        "111111111111111");

    public static PetPatient SecondTestPetPatient() => PetPatient.New(
        PetPatientId.New(),
        "Whiskers",
        Species.Cat,
        2,
        false,
        "987654321");

    public static PetPatient ThirdTestPetPatient() => PetPatient.New(
        PetPatientId.New(),
        "Rex",
        Species.Dog,
        5,
        true,
        "222222222222222");
}
