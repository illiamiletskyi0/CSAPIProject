using Api.Dtos;
using Application.PetPatients.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("pet-patients")]
[ApiController]
public class PetPatientController(IPetPatientService petPatientService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PetPatientDto>> GetById(Guid id,
        CancellationToken cancellationToken)
    {
        var petPatient = await petPatientService.GetById(id, cancellationToken);
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
        try
        {
            var petPatient = await petPatientService.Add(
                request.MicrochipId,
                request.Name,
                request.Species,
                request.AgeYears,
                request.IsVaccinated,
                cancellationToken);

            var dto = PetPatientDto.FromDomainModel(petPatient);

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }
        catch (ArgumentException exception)
        {
            return Conflict(exception.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PetPatientDto>> UpdatePetPatient(Guid id, [FromBody] UpdatePetPatientDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var petPatient = await petPatientService.Update(
                id,
                request.MicrochipId,
                request.Name,
                request.Species,
                request.AgeYears,
                request.IsVaccinated,
                cancellationToken);

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
    
    [HttpGet("microchip-id/{microchipId}")]
    public async Task<ActionResult<PetPatientDto>> GetPetPatientByMicrochipId(string microchipId,
        CancellationToken cancellationToken)
    {
        var petPatient = await petPatientService.GetPetPatient(microchipId, cancellationToken);
        if (petPatient is null)
        {
            return NotFound();
        }
        return PetPatientDto.FromDomainModel(petPatient);
    }
    
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PetPatientDto>>> GetPetPatients(CancellationToken cancellationToken)
    {
        var petPatients = await petPatientService.GetPetPatients(cancellationToken);
        return Ok(petPatients.Select(PetPatientDto.FromDomainModel));
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> DeletePetPatient(Guid id, CancellationToken cancellationToken)
    {
        var result = await petPatientService.Delete(id, cancellationToken);
        if (!result)
        {
            return NotFound();
        }
        return Ok(result);
    }
}