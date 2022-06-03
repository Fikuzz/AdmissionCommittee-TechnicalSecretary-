using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriyemnayaKomissiya_TechnicalSecretary_
{
    /// <summary>
    /// Класс для заполнения таблицы абитуриентов
    /// </summary>
    class AbiturientDGItem
    {
        public int ID { get; }
        public int Num { get; set; }
        public string ExamNum { get; }
        public string FIO { get; }
        public string Shool { get; }
        public int Year { get; }
        public string Lgoti { get; set; }
        public string Stati { get; set; }
        public int[] Marks { get; set; }
        public double MarkAvg { get; set; }
        public double MarkDecAvg { get; set; }
        public string Status { get; set; }
        public bool DocumentiVidani { get; set; }
        public bool Hide { get; set; }
        public bool DifferentAttestat { get; set; }
        public int ScaleSize { get; set; }

        /// <summary>
        /// Конструктор для элемента таблицы абитуриента
        /// </summary>
        /// <param name="id">ИД</param>
        /// <param name="fio">фИО</param>
        /// <param name="shool">Школа</param>
        /// <param name="year">Год окончания школы</param>
        /// <param name="marks">Оценки в аттестате</param>
        /// <param name="markAvg">Средний балл</param>
        /// <param name="examNum">Экзаменационный номер</param>
        /// <param name="doc">Орган выдавший документ(Паспорт)</param>
        /// <param name="marcDecAvg">Десятибальное значение для среднего балла</param>
        /// <param name="lgoti">Льготы</param>
        /// <param name="status">Статус</param>
        public AbiturientDGItem(int id, string fio, string shool, int year, int[] marks, double markAvg, string examNum, bool doc, double marcDecAvg, string lgoti, string status)
        {
            Marks = new int[15];
            Num = 0;
            ID = id;
            FIO = fio;
            Shool = shool;
            Year = year;
            Marks = marks;
            MarkAvg = markAvg;
            ExamNum = examNum;
            DocumentiVidani = doc;
            MarkDecAvg = marcDecAvg;
            Lgoti = lgoti;
            Status = status;
            Stati = "";
            Hide = false;
            DifferentAttestat = false;
        }
    }
}
