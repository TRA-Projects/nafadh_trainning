using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_Models.DatabaseModel
{
    public class TrainingMaster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string ClientName { get; set; }

        public bool WithEmployment { get; set; } = false;

        public bool WithCertificate { get; set; } = false;

        public List<TRATraining> TRATrainingList { get; set; }
    }
}
