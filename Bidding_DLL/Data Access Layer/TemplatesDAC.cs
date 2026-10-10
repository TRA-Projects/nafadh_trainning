using Bidding_Models;
using Bidding_Models.DatabaseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bidding_DLL.Data_Access_Layer
{
   public class TemplateDAC
    {
        private readonly CHDBContext context;

        public TemplateDAC(CHDBContext Context)
        {
            context = Context;
        }

        public Template Add(Template model) {
            try
            {
                using (context)
                {
                    context.Templates.Add(model);
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


        public Template Update(Template model)
        {

            using (context)
            {
              Template template =  context.Templates.FirstOrDefault(t => t.Id == model.Id);
                if (template!=null)
                {
                    template.Text = model.Text;
                    template.Type = model.Type;
                    template.NotificationTypeId = model.NotificationTypeId;
                    template.ForAdmin = model.ForAdmin;
                    template.ForUser = model.ForUser;
                    template.ForVendor = model.ForVendor;
                    template.UpdatedById = model.UpdatedById;
                    template.UpdatedDate = model.UpdatedDate;
                    template.EmailSubject = model.EmailSubject;
                    template.Language = model.Language;
                    template.IsActive = model.IsActive;
                    context.Entry<Template>(template).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                   // model.File = null;
                    return model;
                }
                context.Templates.Add(model);
               

            }
            return null;

        }

        public Template GetById (int Id)
        {

            using (context)
            {
                return context.Templates.FirstOrDefault(t => t.Id == Id);

            }

        }


        public bool IsTemplateExist(int TypeId, int CarrierId, bool ForAdmin, bool ForClient, bool ForVendor)
        {
            bool isExist = false;

            if (ForAdmin)
            {
                isExist = context.Templates.AsNoTracking().Any(t => t.Type == CarrierId && t.NotificationTypeId == TypeId && t.ForAdmin && t.IsActive);
            }
            else if (ForClient)
            {
                isExist = context.Templates.AsNoTracking().Any(t => t.Type == CarrierId && t.NotificationTypeId == TypeId && t.ForUser && t.IsActive);
            }
            else if (ForVendor)
            {
                isExist = context.Templates.AsNoTracking().Any(t => t.Type == CarrierId && t.NotificationTypeId == TypeId && t.ForVendor && t.IsActive);
            }



            return isExist;

        }


        public  Template GetbyTypeandCarrieranduserType(int TypeId, int CarrierId, bool ForAdmin, bool ForUser, bool ForVendor)
        {
                Template tem = new Template();
            
                if (ForAdmin)
                {
                    tem= context.Templates.AsNoTracking().Where(t => t.Type == CarrierId  && t.NotificationTypeId == TypeId && t.ForAdmin).FirstOrDefault();
                }
                else if (ForUser)
                {
                    tem =  context.Templates.AsNoTracking().Where(t => t.Type == CarrierId && t.NotificationTypeId == TypeId && t.ForUser).FirstOrDefault();
                }
                else if (ForVendor)
                {
                    tem =  context.Templates.AsNoTracking().Where(t => t.Type == CarrierId && t.NotificationTypeId == TypeId && t.ForVendor).FirstOrDefault();
                }

                
            
            return tem;

        }

        public Bidding_Models.PagedResult<Template> GetAllWithPagging(Bidding_Models.PagedResult<Template> model)
        {


            IQueryable<Template> query = context.Templates.Include(t=>t.NotificationType).OrderBy(c => c.Id);

            query = ApplyFilter(query, model.SearchParametersObject);
            return ExecuteQuery(query,model);
       


          

        }

        public IOrderedQueryable<Template> GetAll()
        {
            using (context)
            {
                return context.Templates.OrderBy(c => c.Id);
            }

        }


        public PagedResult<Template> ExecuteQuery(IQueryable<Template> query, PagedResultBase<Template> model)
        {

            PagedResult<Template> result = new PagedResult<Template>();
            result.CurrentPage = model.CurrentPage;
            result.RowCount = query.Count();
            var pageCount = (double)result.RowCount / model.PageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);


            var skip = (model.CurrentPage - 1) * model.PageSize;
            result.Results = query.Skip(skip).Take(model.PageSize).ToList();

            return result;

        }



        private IQueryable<Template> ApplyFilter(IQueryable<Template> query, Template Template)
        {
            if (Template != null)
            {
                if (Template.NotificationTypeId > 0)
                {
                    query = query.Where(t => t.NotificationTypeId == Template.NotificationTypeId);
                }
                if (Template.Type > 0)
                {
                    query = query.Where(t => t.Type == Template.Type);
                }
            }
            return query;
        }


    }
}
