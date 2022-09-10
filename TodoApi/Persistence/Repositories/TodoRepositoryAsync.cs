using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class TodoRepositoryAsync : GenericRepositoryAsync<Todo>, ITodoRepositoryAsync
    {
        private readonly DbSet<Todo> _todo;
        public TodoRepositoryAsync(ApplicationDbContext dbContext) : base(dbContext)
        {
            _todo = dbContext.Set<Todo>();
        }


    }
}
