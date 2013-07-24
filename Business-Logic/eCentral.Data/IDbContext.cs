using System.Collections.Generic;
using System.Data.Entity;
using eCentral.Core;

namespace eCentral.Data
{
    public interface IDbContext 
    {
        IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity;

        int SaveChanges();

        IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters)
            where TEntity : BaseEntity, new();
    }
}
