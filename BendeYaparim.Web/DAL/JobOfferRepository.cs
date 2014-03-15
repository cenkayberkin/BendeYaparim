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
    public class JobOfferRepository : IJobOfferRepository
    {
        BendeyaparimContext context;

        public JobOfferRepository(BendeyaparimContext context)
        {
            this.context = context;
        }

        public IQueryable<JobOffer> All
        {
            get { return context.JobOffers; }
        }

        public IQueryable<JobOffer> AllIncluding(params Expression<Func<JobOffer, object>>[] includeProperties)
        {
            IQueryable<JobOffer> query = context.JobOffers;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public JobOffer Find(int id)
        {
            return context.JobOffers
                 .Include(a => a.Owner)
                 .Include(a => a.Category)
                 .Include(a => a.City)
                 .Where(a => a.Id == id).Single();
        }

        public void InsertOrUpdate(JobOffer joboffer)
        {
            if (joboffer.Id == default(int))
            {
                // New entity
                context.JobOffers.Add(joboffer);
            }
            else
            {
                // Existing entity
                context.Entry(joboffer).State = EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var joboffer = context.JobOffers.Find(id);
            context.JobOffers.Remove(joboffer);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Insert(JobOffer jobOffer)
        {
            context.JobOffers.Add(jobOffer);
        }

        public void Delete(int id, int UserId)
        {
            var jobOffer = context.JobOffers.Find(id);
            if (jobOffer.UserId == UserId)
            {
                Category cat = context.Categories.Where(a => a.Id == jobOffer.CategoryId).First();
                if (cat.NumberOfJobOffers != 0)
                {
                    cat.NumberOfJobOffers--;
                }
                context.JobOffers.Remove(jobOffer);
            }
        }
    }

    public interface IJobOfferRepository
    {
        IQueryable<JobOffer> All { get; }
        IQueryable<JobOffer> AllIncluding(params Expression<Func<JobOffer, object>>[] includeProperties);
        JobOffer Find(int id);
        void InsertOrUpdate(JobOffer joboffer);
        void Insert(JobOffer jobOffer);
        void Delete(int id);
        void Delete(int id, int UserId);
        void Save();
    }
}