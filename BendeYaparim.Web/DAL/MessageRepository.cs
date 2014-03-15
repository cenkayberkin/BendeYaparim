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
    public class MessageRepository : IMessageRepository
    {
        BendeyaparimContext context;

        public MessageRepository(BendeyaparimContext context)
        {
            this.context = context;
        }

        public List<Message> AllMessagesForUser(int UserID)
        {
            var messages = context.Messages.Include(a => a.From).OrderByDescending(a => a.SentAt).Where(a => a.ToUserId == UserID).ToList();
            return messages;
        }

        public IQueryable<Message> AllIncluding(params Expression<Func<Message, object>>[] includeProperties)
        {
            IQueryable<Message> query = context.Messages;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Message Find(int id)
        {
            return context.Messages.Find(id);
        }

        public void Insert(Message message)
        {
            context.Messages.Add(message);
        }

        public void Delete(int id, int UserId)
        {
            var message = context.Messages.Find(id);

            if (message.ToUserId == UserId)
            {
                context.Messages.Remove(message);
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }

    public interface IMessageRepository
    {
        List<Message> AllMessagesForUser(int UserID);
        IQueryable<Message> AllIncluding(params Expression<Func<Message, object>>[] includeProperties);
        Message Find(int id);
        void Insert(Message message);
        void Delete(int id, int UserId);
        void Save();
    }
}