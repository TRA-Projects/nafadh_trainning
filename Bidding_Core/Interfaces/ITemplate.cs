

using Bidding_Models;
using Bidding_Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bidding_Core.Interfaces
{
    public interface ITemplate
    {
        Template Save(Template model);

  
        Template GetById(int Id);
        IOrderedQueryable<Template> GetAll();


   
   

        PagedResult<Template> GetAllWithPagging(PagedResult<Template> model);

        bool IsTemplateExist(int TypeId, int CarrierId, bool ForAdmin, bool ForUser, bool ForVendor);
        Template GetbyTypeandCarrieranduserType(int TypeId, int CarrierId, bool ForAdmin, bool ForUser, bool ForVendor);
         void CheckNotificationSettings(out bool EmailTempalteExist, out bool SMSTempalteEist, NotificationTypesEnum NotificationType, bool ForAdmin, bool ForClient, bool ForVendor);

    }
}
