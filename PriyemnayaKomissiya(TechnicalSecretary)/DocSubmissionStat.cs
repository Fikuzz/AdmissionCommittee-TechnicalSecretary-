using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriyemnayaKomissiya_TechnicalSecretary_
{
    class DocSubmissionStat
    {
        public int IDAdmissionPlan { get; set; }
        public int TotalToAdmissionPlan { get; set; }
        public int AdmissionPlanDogovor { get; set; }
        public int AdmissionPlanPayers { get; set; }
        public int TotalToEntrant { get; set; }
        public int EntrantDogovor { get; set; }
        public int EntrantOutOfCompetition { get; set; }
        public int[] Marks { get; set; }

        public DocSubmissionStat(int markCount)
        {
            this.Marks = new int[markCount];
        }
    }
}
