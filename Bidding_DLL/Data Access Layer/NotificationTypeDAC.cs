using Bidding_Models;
using Bidding_Models.DatabaseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Bidding_DLL.Data_Access_Layer
{
   public class NotificationTypeDAC
    {
        private readonly CHDBContext context;

        public NotificationTypeDAC(CHDBContext Context)
        {
            context = Context;
        }

        public NotificationType Add(NotificationType model) {
            try
            {
                using (context)
                {
                    context.NotificationTypes.Add(model);
                    context.SaveChanges();

                }
            }
            catch (Exception ex)
            {

                throw;
            }
            
          //  model.File = null;
            return model;

        }


        public NotificationType Update(NotificationType model)
        {

            using (context)
            {
              NotificationType NotificationType =  context.NotificationTypes.FirstOrDefault(t => t.Id == model.Id);
                if (NotificationType!=null)
                {
                    NotificationType.Name = model.Name;
                
                    context.Entry<NotificationType>(NotificationType).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                   // model.File = null;
                    return model;
                }
                context.NotificationTypes.Add(model);
               

            }
            return null;

        }

        public NotificationType GetById (int Id)
        {

            using (context)
            {
                return context.NotificationTypes.Include(t => t.NotificationTypeSettings).FirstOrDefault(t => t.Id == Id);

            }

        }

        public NotificationType GetbyNotificationTypeId(int NotificationTypeId)
        {

            using (context)
            {
                return context.NotificationTypes.Include(n=>n.NotificationTypeSettings).AsNoTracking().FirstOrDefault(t => t.NotificationTypeSettings.Id == NotificationTypeId);

            }

        }

        public Bidding_Models.PagedResult<NotificationType> GetAllWithPagging(Bidding_Models.PagedResult<NotificationType> model)
        {


            IOrderedQueryable<NotificationType> query = context.NotificationTypes.Include(t=>t.NotificationTypeSettings).OrderBy(c => c.Id);
            return ExecuteQuery(query,model);
       


          

        }

        public IOrderedQueryable<NotificationType> GetAll()
        {
            using (context)
            {
                return context.NotificationTypes.OrderBy(c => c.Id);
            }

        }


        public PagedResult<NotificationType> ExecuteQuery(IOrderedQueryable<NotificationType> query, PagedResultBase<NotificationType> model)
        {

            PagedResult<NotificationType> result = new PagedResult<NotificationType>();
            result.CurrentPage = model.CurrentPage;
            result.RowCount = query.Count();
            var pageCount = (double)result.RowCount / model.PageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);


            var skip = (model.CurrentPage - 1) * model.PageSize;
            result.Results = query.Skip(skip).Take(model.PageSize).ToList();

            return result;

        }


    }
}
