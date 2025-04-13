using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRepositoryService
{
   public interface ISqlProcedureService<T>
    {

        Task<IEnumerable<T>>? UseOurSql(string procedureName, string id);
    }
}
