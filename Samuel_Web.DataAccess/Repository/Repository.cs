using Microsoft.EntityFrameworkCore;
using Samuel_Web.DataAccess.Data;
using Samuel_Web.DataAccess.Repository.IRepository;
using Samuel_Web.Models;
using System.Linq.Expressions;
namespace Samuel_Web.DataAccess.Repository
{
    public class Repository <T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _db;
        internal  DbSet<T> dbSet;
        public Repository(AppDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);  
        }
        public void AddRange(IEnumerable<T> entity)
        {
            dbSet.AddRange(entity); 
        }
        public IEnumerable<T> GetAll(string includepro = null! , Expression<Func<T, bool>> filter = null!)
        {
            IQueryable<T> query;

            if (filter == null)
                query = dbSet; 
            else
              query = dbSet.Where(filter);
            if (!string.IsNullOrEmpty(includepro))
            {
                foreach (var includeProp in includepro
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }

            }
            return query.ToList();  
        }

        public T GetFirst(Expression<Func<T, bool>> filter , string includepro = null! , bool tracked = false)
        {
            // this will also work but less efficient because we may put some other linq operations where , include , orderby etc
            //  return dbSet.FirstOrDefault(filter)!;

            // so we create a queryable object from dbset 
            IQueryable<T> query;
            if (tracked)
                query = dbSet;
            else
                query = dbSet.AsNoTracking(); 

            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includepro))
            {
                foreach (var includeProp in includepro
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }

            }
            // means 
            return query.FirstOrDefault()!;    
        }

        public void Remove(T entity)
        {
            
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entity)
        {
            dbSet.RemoveRange(entity);  
        }
    }
}
