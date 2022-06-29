using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriyemnayaKomissiya_TechnicalSecretary_
{
    /// <summary>
    /// Статистика подачи документов по плану приема
    /// </summary>
    class DocSubmissionStat
    {
        /// <summary>
        /// ИД плана приема
        /// </summary>
        public int IDAdmissionPlan { get; set; }
        /// <summary>
        /// Всего мест
        /// </summary>
        public int TotalToAdmissionPlan { get; set; }
        /// <summary>
        /// Мест по целевому договору
        /// </summary>
        public int AdmissionPlanDogovor { get; set; }
        /// <summary>
        /// На условиях оплаты
        /// </summary>
        public int AdmissionPlanPayers { get; set; }
        /// <summary>
        /// Всего подали документы
        /// </summary>
        public int TotalToEntrant { get; set; }
        /// <summary>
        /// Подали документы по целевому договору
        /// </summary>
        public int EntrantDogovor { get; set; }
        /// <summary>
        /// Вне конкурса
        /// </summary>
        public int EntrantOutOfCompetition { get; set; }
        /// <summary>
        /// Кличество абитуриентов по промежуткам отметок
        /// </summary>
        public int[] Marks { get; set; }

        /// <param name="markCount">количество промежутков отметок</param>
        public DocSubmissionStat(int markCount)
        {
            this.Marks = new int[markCount];
        }
        /// <summary>
        /// Реквизиты плана приема
        /// </summary>
        public string AdmissionBased { get; set; }
    }
}
