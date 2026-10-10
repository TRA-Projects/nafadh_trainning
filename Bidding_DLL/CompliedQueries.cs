using Bidding_Models;
using Bidding_Models.DatabaseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;

namespace Bidding_DLL
{


    public class CompliedQueries
    {

public static Func<CHDBContext, RequestObject<Request>, Request> RequestDetailForVendor =
 EF.CompileQuery<CHDBContext, RequestObject<Request>, Request>(
        (context, model) =>

        
            context.Set<Request>()
                    .Include(r => r.TendorCategory)
                    .Include(b => b.Bidding)
                    .Include(r => r.User).ThenInclude(u => u.Profile).ThenInclude(pa => pa.ParentProfile)
                    .Include(r => r.RequestDocumentList)
                    .Include(e => e.RequestEvaluationCriteriaList)
                    .Include(e => e.BidList)

                    .Include(e => e.BidList).ThenInclude(b => b.Vendor).ThenInclude(b => b.Profile)
                    .Include(e => e.RequestPrerequisteList)
                    .Include(e => e.RequestRequiredDocumentList)
                     .Include(e => e.RequestNDADocumentList)
                    .Include(e => e.RequestClarificationQuestionList)

                          .Include(e => e.RequestParticipentsList).ThenInclude(b => b.User).ThenInclude(b => b.Profile)
                                        //.Include(e => e.VendorExtensionRequestList)
                                        .Include(e => e.NDAList)
                    .AsNoTracking()
                    .Where(r => r.Id == model.Object.Id && r.ForProfileTypes.Contains(model.ProfileType.ToString()))
                    .Select(re => new Request()
                    {
                        Id = re.Id,
                        EncryptedId = re.EncryptedId,
                        Name = re.Name,
                        RequestType = re.RequestType,
                        OpportunityType = re.OpportunityType,
                        Code = re.Code,
                        CodePrefix = re.CodePrefix,
                        IsCodeManual = re.IsCodeManual,
                        Budget = re.Budget,
                        City = re.City,
                        Description = re.Description,
                        ExpiryDate = re.ExpiryDate,
                        Governorate = re.Governorate,
                        IsNDARequired = re.IsNDARequired,
                        IsProposalRequired = re.IsProposalRequired,
                        IsPublished = re.IsPublished,
                        IsDeleted = re.IsDeleted,
                        ForProfileTypes = re.ForProfileTypes,
                        IsExpiryExtendedByClient = re.IsExpiryExtendedByClient,
                            //   GradeofVendor = re.GradeofVendor,
                        RequestDocumentList = re.RequestDocumentList.ToList(),
                        BidList = re.BidList.Select(bid => new Bid()
                        {
                            Id = bid.Id,
                            EncryptedId = bid.EncryptedId,
                            BidBudget = bid.BidBudget,
                            Days = bid.Days,
                            Description = bid.Description,
                            Request = bid.Request,
                            RequestId = bid.RequestId,
                            Vendor = bid.Vendor,
                            Status = bid.Status,
                            VendorId = bid.VendorId,
                            InsertedDate = bid.InsertedDate,
                                //    BidDocumentList = bid.BidDocumentList,
                            BidDocumentCount = bid.BidDocumentList.Count
                        }).Where(b => b.VendorId == model.UserId).ToList(),
                        NDAList = re.NDAList.Where(nda => nda.VendorId == model.UserId).Select(n => new NDA() { EncryptedId = n.EncryptedId, VendorId = n.VendorId, Status = n.Status }).ToList(),
                        RequestEvaluationCriteriaList = re.RequestEvaluationCriteriaList.Select(c => new RequestEvaluationCriteria()
                        {
                            Id = c.Id,
                            Type = c.Type,
                            CriteriaName = c.CriteriaName,
                            CriteriaScore = c.CriteriaScore,
                            CategoryName = c.CategoryName,
                        }).ToList(),
                        RequestClarificationQuestionList = re.RequestClarificationQuestionList.Where(c => c.ISPublic || c.InsertedById == model.UserId).ToList(),
                        RequestPrerequisteList = re.RequestPrerequisteList.Select(c => new RequestPrerequiste
                        {
                            Name = c.Name,
                        }).ToList(),
                        RequestRequiredDocumentList = re.RequestRequiredDocumentList.Select(c => new RequestRequiredDocument
                        {
                            DocumentName = c.DocumentName,
                            DocumentType = c.DocumentType
                        }).ToList(),
                        RequestNDADocumentList = re.RequestNDADocumentList.Select(c => new RequestNDADocument
                        {
                            DocumentName = c.DocumentName,
                            DocumentType = c.DocumentType
                        }).ToList(),
                        Bidding = new Bidding()
                        {
                            Id = re.Bidding.Id,
                            BiddingStart = re.Bidding.BiddingStart,
                            BiddingEnd = re.Bidding.BiddingEnd,
                            IsApproved = re.Bidding.IsApproved,
                        },
                            //RequestParticipentsList = re.RequestParticipentsList,

                        RequestParticipentsList = re.RequestParticipentsList.Select(p => new RequestParticipents()
                        {
                            Comments = p.Comments,
                            FinicialAmount = p.FinicialAmount,
                            UserId = p.UserId,
                            IsAwarded = p.IsAwarded,
                            User = new AppUser() { Email = p.User.Email, PhoneNumber = p.User.PhoneNumber, Profile = new Profile() { Name = p.User.Profile.Name, ComanyEmail = p.User.Profile.ComanyEmail, ComanyPhone = p.User.Profile.ComanyPhone, CompanyNameEnglish = p.User.Profile.CompanyNameEnglish } }

                        }).Where(b => b.UserId == model.UserId).ToList(),
                            //VendorExtensionRequestList = re.VendorExtensionRequestList.Where(ce => ce.VendorId == model.UserId).Select(item => new VendorExtensionRequest()
                            //{
                            //    Vendor = new AppUser()
                            //    {
                            //        Profile = new Profile() { Name = item.Vendor.Profile.Name, NameAr = item.Vendor.Profile.NameAr, EmailAddress = item.Vendor.Profile.EmailAddress },
                            //    },
                            //    LastClarificationDate = item.LastClarificationDate,
                            //    Lastsubmissiondate = item.Lastsubmissiondate,
                            //    ExpiryExtendedReason = item.ExpiryExtendedReason

                            //}).ToList(),
                        ServiceCategory = re.ServiceCategory,
                        StartDate = re.StartDate,
                        LastClarificationDate = re.LastClarificationDate,
                        Lastsubmissiondate = re.Lastsubmissiondate,
                        Status = re.Status,
                        TendorCategory = re.TendorCategory,
                        TendorCategoryId = re.TendorCategoryId,
                        UserId = re.UserId,
                        User = new AppUser()
                        {
                            Id = re.User.Id,
                            Profile = new Profile()
                            {
                                Id = re.User.Profile.Id,
                                Name = re.User.Profile.Name,
                                NameAr = re.User.Profile.NameAr,
                                CompanyNameEnglish = re.User.Profile.CompanyNameEnglish,
                                CompanyNameArabic = re.User.Profile.CompanyNameArabic,
                                ParentProfileId = re.User.Profile.ParentProfileId,
                                Settings = re.User.Profile.Settings,
                                ParentProfile = new Profile()
                                {
                                    Id = Convert.ToInt32(re.User.Profile.ParentProfile.Id),
                                    Name = Convert.ToString(re.User.Profile.ParentProfile.Name),
                                    NameAr = Convert.ToString(re.User.Profile.ParentProfile.NameAr),
                                    CompanyNameEnglish = Convert.ToString(re.User.Profile.ParentProfile.CompanyNameEnglish),
                                    CompanyNameArabic = Convert.ToString(re.User.Profile.ParentProfile.CompanyNameArabic),

                                }
                            },

                            Email = re.User.Email,
                            PhoneNumber = re.User.PhoneNumber,
                            AppUserRoles = re.User.AppUserRoles.Select(roles => new AppUserRoles() { Role = roles.Role, RoleId = roles.RoleId, User = roles.User, UserId = roles.UserId }).ToList()
                        },
                            // Vendor = re.Vendor,
                        Wilayat = re.Wilayat,
                            //VendorId = re.VendorId,
                        CloseVendors = re.CloseVendors,
                        JustificationReason = re.JustificationReason,

                        IsPublicAcceptanceCriteria = re.IsPublicAcceptanceCriteria

                    })
                    .FirstOrDefault());
    }
}
