using DatabaseConnection;
using IRepositoryService;
using IRepositoryService.Consts;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Linq;
using System.Linq.Expressions;

namespace RepositoryFactory
{
    public class DatabaseRepository<T> : IDatabaseRepository<T> where T:class
    {
            protected readonly ApplicationDbContext context;

            public DatabaseRepository(ApplicationDbContext context)
            {
                this.context = context;
            }

            public T GetById(int id)
            {
                return context.Set<T>().Find(id);
            }
            public T? GetByStringId(string id = null, Expression<Func<T, bool>> predicate = null)
            {
                if (id != null)
                {
                    return context.Set<T>().Find(id);

                }
                else if (predicate != null)
                {
                    return context.Set<T>().FirstOrDefault(predicate);
                }
                else
                {
                    return null;
                }

            }
            public T Findme(Expression<Func<T, bool>> match, string[] includes = null)
            {
                if (includes == null)
                {
                    return context.Set<T>().FirstOrDefault(match);
                }
                IQueryable<T> query = context.Set<T>();
                foreach (var item in includes)
                {
                    query = query.Include(item);
                }
                return query.FirstOrDefault(match);
            }

            public IQueryable<T> FindAll(Expression<Func<T, bool>> match, string[] includes = null)
            {
                if (includes == null)
                {
                    return context.Set<T>().Where(match);
                }
                else
                {
                    IQueryable<T> query = context.Set<T>();
                    foreach (var item in includes)
                    {
                        query = query.Include(item);
                    }
                    return query.Where(match);
                }

            }


            public IQueryable<T> MatchOrder(Expression<Func<T, bool>> match = null, string[] includes = null, int? take = null, int? skip = null,
                Expression<Func<T, object>> orderby = null, string orderbydirection = Orderby.Ascending
                )
            {
                IQueryable<T> query = context.Set<T>();

                if (match != null)
                { query = query.Where(match); }


                if (skip != null)
                {
                    query = query.Skip(skip ?? 0);
                }
                if (take != null)
                {
                    query = query.Take(take ?? 0);
                }


                if (orderby != null)
                {
                    if (orderbydirection == Orderby.Ascending)
                    { query = query.OrderBy(orderby); }
                    else
                    {
                        query = query.OrderByDescending(orderby);
                    }

                }


                if (includes == null)
                {
                    return query;
                }
                else
                {
                    foreach (var item in includes)
                        query = query.Include(item);
                    return query;
                }



            }


            public void AddRange(IEnumerable<T> entities)
            {
                var query = context.Set<T>();
                foreach (var entity in entities)
                {
                    query.Add(entity);
                }
            }


            public IQueryable<T> GetAll()
            {
                var query = context.Set<T>();

                return query;
            }

        public  IQueryable<IGrouping<TKey,T>>?
            GetWithGroup<TKey>(Expression<Func<T, TKey>> keyselector)
        {
            var query = context.Set<T>();
            var groupedquery = query.GroupBy(keyselector);

            return groupedquery;
        }
        public IQueryable<T> GetAllWith(string[] includes=null,int? take=null,int? skip=null)
            {
                IQueryable<T> query = context.Set<T>();
                  if(skip !=null && take != null)
            {
                query = query.Skip(skip ?? 0).Take(take ?? 0);
            }
                  if(includes != null)
            {
                foreach (string entity in includes)
                {
                    query = query.Include(entity);
                }
            }
               
                return query;
            }
            public T? GetdetailWith(string[] includes = null, Expression<Func<T, bool>> predicate = null)
            {
                IQueryable<T> query = context.Set<T>();
                foreach (string entity in includes)
                {
                    query = query.Include(entity);
                }
                var response = query.FirstOrDefault(predicate);
                return response;
            }

            public void Addone(T entitiy)
            {
                var query = context.Set<T>();
                query.Add(entitiy);

            }

            public void Update(T entity)
            {
                context.Set<T>().Update(entity);
            }


        public void UpdateRange(List<T> entities)
            {

                context.Set<T>().UpdateRange(entities);


            }

            public void Delete(T entity)
            {
                context.Set<T>().Remove(entity);

            }
            public void DeleteRange(IEnumerable<T> entity)
            {
                context.Set<T>().RemoveRange(entity);

            }
            public void Attach(T entity)
            {
                context.Set<T>().Attach(entity);
            }

            public int Count()
            {
                return context.Set<T>().Count();
            }

            public int Count(Expression<Func<T, bool>> match)
            {
                return context.Set<T>().Where(match).Count();
            }


        public async Task<T> GetByStringIdAsync(string id)
        {
            return await context.Set<T>().FindAsync(id);
        }















    }
}

