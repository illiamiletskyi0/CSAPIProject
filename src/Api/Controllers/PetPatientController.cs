using Api.Dtos;
using Application.Common.Interfaces.Queries;
using Application.PetPatients.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("pet-patients")]
[ApiController]
public class PetPatientController(
    ISender sender,
    IPetPatientQueries petPatientQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PetPatientDto>>> GetPetPatients(
        CancellationToken cancellationToken)
    {
        var petPatients = await petPatientQueries.GetAll(cancellationToken);
        return petPatients.Select(PetPatientDto.FromDomainModel).ToList();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PetPatientDto>> GetById(Guid id,
        CancellationToken cancellationToken)
    {
        var petPatient = await petPatientQueries.GetById(id, cancellationToken);
        if (petPatient is null)
        {
            return NotFound();
        }
        return PetPatientDto.FromDomainModel(petPatient);
    }

    [HttpGet("microchip-id/{microchipId}")]
    public async Task<ActionResult<PetPatientDto>> GetPetPatientByMicrochipId(string microchipId,
        CancellationToken cancellationToken)
    {
        var petPatient = await petPatientQueries.GetByMicrochipId(microchipId, cancellationToken);
        if (petPatient is null)
        {
            return NotFound();
        }
        return PetPatientDto.FromDomainModel(petPatient);
    }

    [HttpPost]
    public async Task<ActionResult<PetPatientDto>> CreatePetPatient(
        [FromBody] CreatePetPatientDto request,
        CancellationToken cancellationToken)
    {
        var input = new CreatePetPatientCommand
        {
            MicrochipId = request.MicrochipId,
            Name = request.Name,
            Species = request.Species,
            AgeYears = request.AgeYears,
            IsVaccinated = request.IsVaccinated
        };
        try
        {
            var petPatient = await sender.Send(input, cancellationToken);
            var dto = PetPatientDto.FromDomainModel(petPatient);
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (ArgumentException exception)
        {
            return Conflict(exception.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PetPatientDto>> UpdatePetPatient(Guid id,
        [FromBody] UpdatePetPatientDto request,
        CancellationToken cancellationToken)
    {
        var input = new UpdatePetPatientCommand
        {
            Id = id,
            MicrochipId = request.MicrochipId,
            Name = request.Name,
            Species = request.Species,
            AgeYears = request.AgeYears,
            IsVaccinated = request.IsVaccinated
        };
        try
        {
            var petPatient = await sender.Send(input, cancellationToken);
            if (petPatient is null)
            {
                return NotFound();
            }
            return PetPatientDto.FromDomainModel(petPatient);
        }
        catch (ArgumentException exception)
        {
            return Conflict(exception.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> DeletePetPatient(Guid id,
        CancellationToken cancellationToken)
    {
        var input = new DeletePetPatientCommand { Id = id };
        var result = await sender.Send(input, cancellationToken);
        if (!result)
        {
            return NotFound();
        }
        return Ok(result);
    }
}
