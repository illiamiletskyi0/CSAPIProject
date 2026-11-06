using Domain.PetPatients;

namespace Application.PetPatients.Exceptions;

public abstract class PetPatientException(PetPatientId petPatientId, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public PetPatientId PetPatientId { get; } = petPatientId;
}

public class PetPatientAlreadyExistsException(PetPatientId petPatientId)
    : PetPatientException(petPatientId, $"Pet patient already exists under id {petPatientId}");

public class PetPatientNotFoundException(PetPatientId petPatientId)
    : PetPatientException(petPatientId, $"Pet patient not found under id {petPatientId}");

public class PetPatientUnhandledException(PetPatientId petPatientId, Exception? innerException = null)
    : PetPatientException(petPatientId, $"Unhandled exception under id {petPatientId}", innerException);
