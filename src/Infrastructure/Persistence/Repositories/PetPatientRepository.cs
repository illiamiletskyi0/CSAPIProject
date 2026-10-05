using Application.Common.Interfaces;
using Domain.PetPatients;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class PetPatientRepository(ApplicationDbContext context) : IPetPatientRepository
{
    public async Task<IReadOnlyList<PetPatient>> GetAll(CancellationToken cancellationToken)
    {
        return await context.PetPatients.ToListAsync(cancellationToken);
    }

    public Task<PetPatient?> GetById(Guid id, CancellationToken cancellationToken)
    {
        return context.PetPatients.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<PetPatient?> GetByMicrochipId(string microchipId, CancellationToken cancellationToken)
    {
        return context.PetPatients.FirstOrDefaultAsync(x => x.MicrochipId == microchipId, cancellationToken);
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
