using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.PetPatients.Commands;
using Domain.PetPatients;
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
        var petPatient = await petPatientQueries.GetById(new PetPatientId(id), cancellationToken);

        return petPatient.Match<ActionResult<PetPatientDto>>(
            p => PetPatientDto.FromDomainModel(p),
            () => NotFound());
    }

    [HttpGet("microchip-id/{microchipId}")]
    public async Task<ActionResult<PetPatientDto>> GetPetPatientByMicrochipId(string microchipId,
        CancellationToken cancellationToken)
    {
        var petPatient = await petPatientQueries.GetByMicrochipId(microchipId, cancellationToken);

        return petPatient.Match<ActionResult<PetPatientDto>>(
            p => PetPatientDto.FromDomainModel(p),
            () => NotFound());
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

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<PetPatientDto>>(
            p => CreatedAtAction(nameof(GetById), new { id = p.Id.Value }, PetPatientDto.FromDomainModel(p)),
            e => e.ToObjectResult());
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

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<PetPatientDto>>(
            p => PetPatientDto.FromDomainModel(p),
            e => e.ToObjectResult());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePetPatient(Guid id,
        CancellationToken cancellationToken)
    {
        var input = new DeletePetPatientCommand { Id = id };
        var result = await sender.Send(input, cancellationToken);

        return result.Match<IActionResult>(
            _ => NoContent(),
            e => e.ToObjectResult());
    }
}
