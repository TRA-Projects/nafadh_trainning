using Bidding_Core.Interfaces;
using Bidding_DLL.Data_Access_Layer;
using Bidding_Models.DatabaseModel;
using Bidding_DLL.Data_Access_Layer;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bidding_Models;

namespace Bidding_Core.Repository
{
    public class NotificationTypeRepository : INotificationType
    {
        private readonly NotificationTypeDAC NotificationTypeDAC;

        public NotificationTypeRepository(NotificationTypeDAC NotificationTypeDAC)
        {
            this.NotificationTypeDAC = NotificationTypeDAC;
        }
        public NotificationType Save(NotificationType model)
        {
            if (model.Id > 0)
            {
              return  NotificationTypeDAC.Update(model);
            }
            else {

                return NotificationTypeDAC.Add(model);

            }
            
        }


        public IOrderedQueryable<NotificationType> GetAll()
        {
            return NotificationTypeDAC.GetAll();
        }

        public NotificationType GetById(int Id)
        {
            return NotificationTypeDAC.GetById(Id);
            
        }

        public NotificationType GetbyNotificationTypeId(int TypeId)
        {
            return NotificationTypeDAC.GetbyNotificationTypeId(TypeId);
        }



        public PagedResult<NotificationType> GetAllWithPagging(PagedResult<NotificationType> model)
        {

            return NotificationTypeDAC.GetAllWithPagging(model);
        }


   
    }
}
