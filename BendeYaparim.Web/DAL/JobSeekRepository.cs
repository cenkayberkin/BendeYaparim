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
    public class JobSeekRepository : IJobSeekRepository
    {
        BendeyaparimContext context;

        public JobSeekRepository(BendeyaparimContext context)
        {
            this.context = context;
        }

        public IQueryable<JobSeek> All
        {
            get { return context.JobSeeks; }
        }

        public IQueryable<JobSeek> AllIncluding(params Expression<Func<JobSeek, object>>[] includeProperties)
        {
            IQueryable<JobSeek> query = context.JobSeeks;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public JobSeek Find(int id)
        {
            return context.JobSeeks
                .Include(a => a.Owner)
                .Include(a => a.Category)
                .Include(a => a.City)
                .Where(a => a.Id == id).Single();
        }

        public void InsertOrUpdate(JobSeek jobseek)
        {
            if (jobseek.Id == default(int))
            {
                // New entity
                context.JobSeeks.Add(jobseek);
            }
            else
            {
                // Existing entity
                context.Entry(jobseek).State = EntityState.Modified;
            }
        }

        public void Delete(int id, int UserId)
        {
            var jobseek = context.JobSeeks.Find(id);
            if (jobseek.UserId == UserId)
            {
                Category cat = context.Categories.Where(a => a.Id == jobseek.CategoryId).First();
                if (cat.NumberOfJobSeeks != 0)
                {
                    cat.NumberOfJobSeeks--;    
                }
                context.JobSeeks.Remove(jobseek);    
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Insert(JobSeek jobseek)
        {
            context.JobSeeks.Add(jobseek);
            
        }
    }

    public interface IJobSeekRepository
    {
        IQueryable<JobSeek> All { get; }
        IQueryable<JobSeek> AllIncluding(params Expression<Func<JobSeek, object>>[] includeProperties);
        JobSeek Find(int id);
        void InsertOrUpdate(JobSeek jobseek);
        void Insert(JobSeek jobseek);
        void Delete(int id,int UserId);
        void Save();
    }
}