using DatabaseConnection;
using IRepositoryService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryFactory
{
    public class SqlProcedureService<T> : ISqlProcedureService<T> where T :class
    {
        protected readonly ApplicationDbContext context;

        public SqlProcedureService(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<IEnumerable<T>>? UseOurSql(string procedureName, string id)
        {
          return  await context.Set<T>().FromSqlInterpolated($"EXEC {procedureName} @tripid = {id}").ToListAsync();
        }
    }
}
