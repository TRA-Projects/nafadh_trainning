using Bidding_Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bidding_Models
{
    public class GeneralReport : Request
    {
        public string? FromDate { get; set; }

        public string? ToDate { get; set; }

        public List<string> ProfileType { get; set; }

        public int SaveAs { get; set; } = 1;

        public int? ProposalDeatil { get; set; }


        public int? OperatorProfileId { get; set; }

        public bool IsDownload { get; set; }

        public string AwardTypeStri { get {
          
                if (Status == RequestStatus.Awarded)
                {
                    if (OpportunityType == 3)
                    {
                        return "Direct Award";
                    }
                    else
                    {
                      return  "System Award";
                    }
                }
                return String.Empty;

            } }

        public decimal AwardedValue
        {
            get
            {
                decimal res = 0;
                if (BidList?.Count > 0)
                {
                    List<Bid> awardedbid = BidList.Where(x => x.Status == BidStatus.UserAwarded).ToList();
                    if (awardedbid != null && awardedbid.Count > 0)
                    {

                        for (var i = 0; i < awardedbid.Count; i++)
                        {
                            res = Convert.ToDecimal(res) + Convert.ToDecimal(awardedbid[i]?.AwardedBudget);
                        }
                    }

                }
                else if (RequestParticipentsList?.Count > 0)
                {
                    List<RequestParticipents> awardedParticipents = RequestParticipentsList.Where(x => x.IsAwarded).ToList();

                    if (awardedParticipents != null && awardedParticipents.Count > 0)
                    {

                        for (var i = 0; i < awardedParticipents.Count; i++)
                        {
                            res = Convert.ToDecimal(res) + Convert.ToDecimal(awardedParticipents[i]?.FinicialAmount);
                        }
                    }
                }

                return res;

            }
        }

        public string AwardedValueStr { get { return AwardedValue.ToString("N") + " OMR"; } }

        public string BidderTypeStr { get {
                string str = String.Empty;

                List<Bid> bids = BidList?.Where(b => b.Status == BidStatus.UserAwarded).ToList();
                if (bids?.Count > 0)
                {
                    foreach (var item in bids)
                    {
                        str +=  item?.Vendor?.Profile?.Name + " - " + GetUserProfileTypeName(item.Vendor.Profile.ProfileType) + " - " + item.AwardedBudgetStr.ToString() + " " +(item.Currency ?? "OMR") + System.Environment.NewLine;
                    }
                }
                else if (BidList?.Count == 0 && RequestParticipentsList?.Count > 0)
                {
                    List<RequestParticipents> part = RequestParticipentsList.Where(p=>p.IsAwarded).ToList();
                    foreach (var item in part)
                    {
                        str += item?.User?.Profile?.Name + " - " + GetUserProfileTypeName(item.User.Profile.ProfileType) + " - " + item.FinicialAmountStr.ToString() + " "+ (item.Currency ?? "OMR") + System.Environment.NewLine;

                    }
                }

                return str.Trim();
               
            
            } }


        public bool IsForAwardingReport { get; set; } = false;
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
    }
    public class ReportResult<T>
        {
            public List<T> Result { get; set; }

            public string ReportBytes { get; set; }
        }
    }
