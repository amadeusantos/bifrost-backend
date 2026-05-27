using Bifrost.Core.Port.Repository;
using Bifrost.Infrastructure.Persistence.Entity;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Infrastructure.Persistence.Repository;

public abstract class RepositoryBase<TE, TD>(ApplicationDbContext applicationDbContext): IRepository<TD> where TE : EntityBase where TD : class
{
    
    protected DbSet<TE> dbSet { get; set; } = applicationDbContext.Set<TE>();
    
    public async Task<List<TD>> GetAll()
    {
        return (await dbSet.ToListAsync()).Select(EntityToDomain).ToList();
    }

    public async Task<TD> Add(TD domain)
    {
        var entity = DomainToEntity(domain);
        entity.Id = Guid.NewGuid();
        entity = (await dbSet.AddAsync(entity)).Entity;
        await applicationDbContext.SaveChangesAsync();
        return EntityToDomain(entity);
    }

    public async Task<TD> Update(TD domain)
    {
        var entity = DomainToEntity(domain);
        entity = dbSet.Update(entity).Entity;
        await applicationDbContext.SaveChangesAsync();
        return EntityToDomain(entity);
    }

    public async Task<TD?> FindById(Guid id)
    {
        var entity = await dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        return entity is not null ? EntityToDomain(entity) : null;
    }

    public Task<bool> IdExists(Guid id)
    {
        return dbSet.AnyAsync(e => e.Id == id);
    }

    public async Task DeleteById(Guid id)
    {
        await dbSet.Where(e => e.Id == id).ExecuteDeleteAsync();
        await applicationDbContext.SaveChangesAsync();
    }
    
    protected abstract TD EntityToDomain(TE entity);
    
    protected abstract TE DomainToEntity(TD domain);
    
}