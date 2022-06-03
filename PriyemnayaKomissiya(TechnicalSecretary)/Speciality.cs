using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriyemnayaKomissiya_TechnicalSecretary_
{
    /// <summary>
    /// Класс для хранения информации об спнциальности
    /// </summary>
    public class Speciality
    {
        /// <summary>
        /// ИД специальности
        /// </summary>
        public int Num { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Код специальности
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Краткое наименование
        /// </summary>
        public string ShortTitle { get; set; }
        /// <summary>
        /// Буква
        /// </summary>
        public string Letter { get; set; }
    }
}
