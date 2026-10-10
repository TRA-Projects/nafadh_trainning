

using Bidding_Models;
using Bidding_Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bidding_Core.Interfaces
{
    public interface INotificationType
    {
        NotificationType Save(NotificationType model);

  
        NotificationType GetById(int Id);
        IOrderedQueryable<NotificationType> GetAll();


   
   

        PagedResult<NotificationType> GetAllWithPagging(PagedResult<NotificationType> model);

        NotificationType GetbyNotificationTypeId(int NotificationTypeId);


    }
}
