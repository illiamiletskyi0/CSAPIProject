using Application.PetPatients.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class PetPatientErrorFactory
{
    public static ObjectResult ToObjectResult(this PetPatientException error)
    {
        return new ObjectResult(error.Message)
        {
            StatusCode = error switch
            {
                PetPatientAlreadyExistsException => StatusCodes.Status409Conflict,
                PetPatientNotFoundException => StatusCodes.Status404NotFound,
                PetPatientUnhandledException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Pet patient error handler is not implemented.")
            }
        };
    }
}
