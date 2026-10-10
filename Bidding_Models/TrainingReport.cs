using Bidding_Models.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models
{
    public class TrainingReport : TRATraining
    {
        public string? FromDate { get; set; }

        public string? ToDate { get; set; }

        public int SaveAs { get; set; } = 1;

        public string TrainingName { get; set; }

        public int? TrainingId { get; set; }

        public string TrainingProvider { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Cerifications { get; set; }
        public new string EmailAddress { get; set; }

        public new string PhoneNumber { get; set; }

        public new string Confirmation { get; set; }

        public new string CivilNumber { get; set; }

        public bool IsDownload { get; set; }
        public string ClientName { get; set; }

        public bool WithEmployment { get; set; }
        public bool WithCertificate { get; set; }

        public string WithEmploymentString { get; set; }
        public string WithCertificateString { get; set; }
        public string Categories { get; set; }

        public AppUser user { get; set; }

    }
}
