using Bidding_Core.Interfaces;
using Bidding_DLL.Data_Access_Layer;
using Bidding_Models;
using Bidding_Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bidding_Core.Repository
{
    public class TemplateRepository : ITemplate
    {
        private readonly TemplateDAC TemplateDAC;
        private readonly INotificationType notificationTypeRepository;

        public TemplateRepository(TemplateDAC TemplateDAC,INotificationType NotificationTypeRepository)
        {
            this.TemplateDAC = TemplateDAC;
            notificationTypeRepository = NotificationTypeRepository;
        }
        public Template Save(Template model)
        {
            if (model.Id > 0)
            {
              return  TemplateDAC.Update(model);
            }
            else {

                return TemplateDAC.Add(model);

            }
            
        }


        public IOrderedQueryable<Template> GetAll()
        {
            return TemplateDAC.GetAll();
        }

        public Template GetById(int Id)
        {
            return TemplateDAC.GetById(Id);
            
        }

        public  Template GetbyTypeandCarrieranduserType(int TypeId, int CarrierId, bool ForAdmin, bool ForUser, bool ForVendor)
        {
            Template temp = TemplateDAC.GetbyTypeandCarrieranduserType(TypeId, CarrierId,  ForAdmin,  ForUser,  ForVendor);
            return temp;
        }

        public bool IsTemplateExist(int TypeId, int CarrierId, bool ForAdmin, bool ForUser, bool ForVendor)
        {
           return  TemplateDAC.IsTemplateExist(TypeId, CarrierId, ForAdmin, ForUser, ForVendor);
           
        }

        public PagedResult<Template> GetAllWithPagging(PagedResult<Template> model)
        {

            return TemplateDAC.GetAllWithPagging(model);
        }


        public void CheckNotificationSettings(out bool EmailTempalteExist, out bool SMSTempalteEist, NotificationTypesEnum NotificationType, bool ForAdmin, bool ForClient, bool ForVendor)
        {
            bool IsEmailTempalteExist = false;
            bool IsSMSTempalteEist = false;

            NotificationType type = notificationTypeRepository.GetById(Convert.ToInt32(NotificationType));
            if (type != null && type.NotificationTypeSettings != null)
            {
                if (type.NotificationTypeSettings.IsSendEmail)
                {
                    IsEmailTempalteExist = TemplateDAC.IsTemplateExist(Convert.ToInt32(NotificationType), Convert.ToInt32(NotiifcationChannelEnum.Email), ForAdmin, ForClient, ForVendor);
                }
                if (type.NotificationTypeSettings.IsSendSMS)
                {
                    IsSMSTempalteEist = TemplateDAC.IsTemplateExist(Convert.ToInt32(NotificationType), Convert.ToInt32(NotiifcationChannelEnum.SMS), ForAdmin, ForClient, ForVendor);
                }
            }
            EmailTempalteExist = IsEmailTempalteExist == true;
            SMSTempalteEist = IsSMSTempalteEist == true;
        }




    }
}
