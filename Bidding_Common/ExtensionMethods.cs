using Bidding_Models;
using Bidding_Models.DatabaseModel;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Bidding_Common
{
    public static class ExtensionMethods
    {
        public static string GetComapnyName(this Profile profile)
        {
            string culture = System.Globalization.CultureInfo.CurrentCulture.Name;
            string compay = String.Empty; ;
            string comapnyName = String.Empty;
            string companyNameArabic = String.Empty;
            if (profile?.ParentProfileId != null && profile?.ParentProfileId > 0)
            {
                comapnyName = profile?.ParentProfile?.CompanyNameEnglish?.ToString();
                companyNameArabic = profile?.ParentProfile?.CompanyNameArabic?.ToString();
            }
            else
            {
                comapnyName = profile?.CompanyNameEnglish?.ToString();
                companyNameArabic = profile?.CompanyNameArabic?.ToString();
            }

            if (culture == "ar-OM")
            {
                compay = companyNameArabic;
                compay = compay ?? comapnyName;
            }
            else
            {
                compay = comapnyName;
                compay = compay ?? companyNameArabic;
            }

            return compay;
        }
        public static string GetOpertunityStatus(this Request model)
        {
            string classname = String.Empty;
            string title = String.Empty;
            if (model.Status == RequestStatus.Draft)
            {
                classname = "Created";
                title = "Draft";
            }
            if (model.Status == RequestStatus.Created)
            {
                classname = "Created";
                title = "Created";
            }

            else if (model.Status == RequestStatus.Expired)
            {
                classname = "Expired";
                title = "Expired Request";
            }
            else if (model.Status == RequestStatus.Closed) // || model.ExpiryDate < DateTime.Today
            {
                classname = "Closed";
                title = "Closed";
            }
            else if (model.Status == RequestStatus.Canceled)
            {
                classname = "Canceled";
                title = "Canceled Request";
            }
            else if (model.Status == RequestStatus.Awarded)
            {
                classname = "Awarded";
                title = "Awarded";
            }

            else if (model.Status == RequestStatus.AdminRejected)
            {
                classname = "Rejected";
                title = "Rejected";
            }

            else if (model.Status == RequestStatus.AdminApproved && (model.Lastsubmissiondate <= DateTime.Now && model.ExpiryDate >= DateTime.Now))
            {
                classname = "Evaluting"; //Awarded
                title = "Evaluting";
            }

            else if (model.Status == RequestStatus.AdminApproved && model.Lastsubmissiondate > DateTime.Now) // && (model.Lastsubmissiondate > currentTime)
            {
                classname = "Active";  //Approved
                title = "Active Request";
            }
            else if (model.Status == RequestStatus.Deleted) // && (model.Lastsubmissiondate > currentTime)
            {
                classname = "Delete";  //Approved
                title = "Deleted Request";
            }
            return title;
        }
        public static string GetProposalStatus(int Status)
        {
            var propsalstatus = "";
            if (Status == 0)
            {
                propsalstatus = "Send";
            }
            else if (Status == 4)
            {
                propsalstatus = "Awarded";
            }
            else if (Status == 7)
            {
                propsalstatus = "Awarded";
            }
            else if (Status == 14)
            {
                propsalstatus = "Rejected";
            }
            else
            {
                propsalstatus = "";
            }
            return propsalstatus;

        }

        public static string GetProposalStatusForReport(int Status)
        {
            var propsalstatus = "";
            if (Status == 0)
            {
                propsalstatus = "Proposal is Submitted";
            }
            else if (Status == 4)
            {
                propsalstatus = "Awarded";
            }
            else if (Status == 7)
            {
                propsalstatus = "Awarded";
            }
            else if (Status == 14)
            {
                propsalstatus = "Rejected";
            }
            else if (Status == 15)
            {
                propsalstatus = "Not Awarded";
            }
            else
            {
                propsalstatus = "";
            }
            return propsalstatus;

        }

        public static string GetUserProfileTypeName(int ProfileType)
        {
            string ProfileTypeName = String.Empty;
            if (ProfileType == Convert.ToInt32(Enums.ProfileTypes.SME))
            {
                ProfileTypeName = "SME";
            }
            else if (ProfileType == Convert.ToInt32(Enums.ProfileTypes.FreeLancer))
            {
                ProfileTypeName = "Freelancer";
            }
            else if (ProfileType == Convert.ToInt32(Enums.ProfileTypes.Contractor))
            {
                ProfileTypeName = "Contractor";
            }
            else if (ProfileType == Convert.ToInt32(Enums.ProfileTypes.Operator))
            {
                ProfileTypeName = "Operator";
            }
            else if (ProfileType == Convert.ToInt32(Enums.ProfileTypes.OUser))
            {
                ProfileTypeName = "Operator User";
            }
            else if (ProfileType == Convert.ToInt32(Enums.ProfileTypes.Admin))
            {
                ProfileTypeName = "Admin";
            }
            else if (ProfileType == Convert.ToInt32(Enums.ProfileTypes.Non_SME))
            {
                ProfileTypeName = "Non SME";

            }

            return ProfileTypeName;

        }


        public static class EnumHelper<T>
        {
            public static T GetValueFromName(string name)
            {
                var type = typeof(T);
                if (!type.IsEnum) throw new InvalidOperationException();

                foreach (var field in type.GetFields())
                {
                    var attribute = Attribute.GetCustomAttribute(field,
                        typeof(DisplayAttribute)) as DisplayAttribute;
                    if (attribute != null)
                    {
                        if (attribute.Name == name)
                        {
                            return (T)field.GetValue(null);
                        }
                    }
                    else
                    {
                        if (field.Name == name)
                            return (T)field.GetValue(null);
                    }
                }

                throw new ArgumentOutOfRangeException("name");
            }
        }


    }
}
