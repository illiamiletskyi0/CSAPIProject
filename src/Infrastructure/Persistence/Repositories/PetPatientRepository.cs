using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.PetPatients;
using LanguageExt;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class PetPatientRepository(ApplicationDbContext context) : IPetPatientRepository, IPetPatientQueries
{
    public async Task<IReadOnlyList<PetPatient>> GetAll(CancellationToken cancellationToken)
    {
        return await context.PetPatients.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Option<PetPatient>> GetById(PetPatientId id, CancellationToken cancellationToken)
    {
        var entity = await context.PetPatients.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity ?? Option<PetPatient>.None;
    }

    public async Task<Option<PetPatient>> GetByMicrochipId(string microchipId, CancellationToken cancellationToken)
    {
        var entity = await context.PetPatients.FirstOrDefaultAsync(x => x.MicrochipId == microchipId, cancellationToken);
        return entity ?? Option<PetPatient>.None;
    }

    public async Task<PetPatient> Add(PetPatient petPatient, CancellationToken cancellationToken)
    {
        context.PetPatients.Add(petPatient);
        await context.SaveChangesAsync(cancellationToken);
        return petPatient;
    }

    public async Task<PetPatient> Update(PetPatient petPatient, CancellationToken cancellationToken)
    {
        context.PetPatients.Update(petPatient);
        await context.SaveChangesAsync(cancellationToken);
        return petPatient;
    }

    public async Task Delete(PetPatient petPatient, CancellationToken cancellationToken)
    {
        context.PetPatients.Remove(petPatient);
        await context.SaveChangesAsync(cancellationToken);
    }
}
