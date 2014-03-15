using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BendeYaparim.Web.Models;

namespace BendeYaparim.Web.DAL
{ 
    public class CityRepository : ICityRepository
    {
        BendeyaparimContext context;

        public CityRepository(BendeyaparimContext con)
        {
            context = con;
        }

        public IQueryable<City> All
        {
            get { return context.Cities; }
        }

        public IQueryable<City> AllIncluding(params Expression<Func<City, object>>[] includeProperties)
        {
            IQueryable<City> query = context.Cities;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public City Find(int id)
        {
            return context.Cities.Find(id);
        }

        public void InsertOrUpdate(City city)
        {
            if (city.Id == default(int)) {
                // New entity
                context.Cities.Add(city);
            } else {
                // Existing entity
                context.Entry(city).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var city = context.Cities.Find(id);
            context.Cities.Remove(city);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface ICityRepository
    {
        IQueryable<City> All { get; }
        IQueryable<City> AllIncluding(params Expression<Func<City, object>>[] includeProperties);
        City Find(int id);
        void InsertOrUpdate(City city);
        void Delete(int id);
        void Save();
    }
}