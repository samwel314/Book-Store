using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Samuel_Web.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class 
    {
        // T any class type Category, Product, ApplicationUser etc 

        IEnumerable<T> GetAll( string includepro = null! , Expression<Func<T ,bool>> filter = null!);

        // why Expression not Func ? because we want to convert it to sql query 
        // if we use Func it will be in memory filtering means after getting all data from db we will filter it
        T GetFirst(Expression<Func<T , bool>> filter  , string includepro = null! , bool tracked = false); 
        void Add (T entity);
        void AddRange(IEnumerable< T> entity);

        //    void Update (T entity);  i remove update method from generic repository because update logic is different for different entities
        void Remove (T entity);
        void RemoveRange (IEnumerable<T> entity);
    }
}
