using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using BendeYaparim.Web.Models;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace BendeYaparim.Web.DAL
{
    public class CategoryRepository : ICategoryRepository
    {
        BendeyaparimContext context;

        public CategoryRepository(BendeyaparimContext context)
        {
            this.context = context;
        }

        public List<Category> AllFirstLevelCategories()
        {
            return context.Categories.Where(a => a.Level == 1).ToList();
        }

        public IQueryable<Category> AllIncluding(params Expression<Func<Category, object>>[] includeProperties)
        {
            IQueryable<Category> query = context.Categories;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Category Find(int id)
        {
            return context.Categories.Find(id);
        }

        public void InsertOrUpdate(Category category)
        {
            if (category.Id == default(int))
            {
                // New entity
                context.Categories.Add(category);
            }
            else
            {
                // Existing entity
                context.Entry(category).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var category = context.Categories.Find(id);
            context.Categories.Remove(category);
        }

        public void Save()
        {
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
        }

        public Category CategoryWithChildren(int id)
        {
            Category query = context.Categories.Include("Children.Children").Where(a => a.Id == id).FirstOrDefault();
            return query;
        }

        public Category CategoryWithParent(int id)
        {
            Category c = context.Categories.Include(a => a.Parent).Where(a => a.Id == id).FirstOrDefault();
            return c;
        }

        public List<Category> Top20JobOfferCategory()
        {
            return  context.Categories.Where(a => a.Level == 3).OrderBy(a => a.NumberOfJobOffers).Take(20).ToList();
        }

        public List<Category> Top20JobSeekCategory()
        {
            return context.Categories.Where(a => a.Level == 3).OrderBy(a => a.NumberOfJobSeeks).Take(20).ToList();
        }
    }

    public interface ICategoryRepository
    {
        List<Category> AllFirstLevelCategories();
        Category CategoryWithChildren(int id);
        IQueryable<Category> AllIncluding(params Expression<Func<Category, object>>[] includeProperties);
        Category Find(int id);
        Category CategoryWithParent(int id);
        void InsertOrUpdate(Category category);
        List<Category> Top20JobOfferCategory();
        List<Category> Top20JobSeekCategory();
        void Delete(int id);
        void Save();
    }
}