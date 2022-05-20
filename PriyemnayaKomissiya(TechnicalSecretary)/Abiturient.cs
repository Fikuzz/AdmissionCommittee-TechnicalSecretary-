﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriyemnayaKomissiya_TechnicalSecretary_
{
    class Abiturient
    {
        public int ID { get; }
        public int Num { get; }
        public string FIO { get; set; }
        public string Lgoti { get; set; }
        public string Stati { get; set; }
        public string Vladelec { get; set; }
        public string Date { get; set; }
        public string Status { get; set; }
        public bool Hide { get; set; }

        public string Shool { get; set; }
        public string YearOfGraduation { get; set; }
        public string BirthDate { get; set; }
        public string PassportDateIssued { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNum { get; set; }
        public string PassportIssuedBy { get; set; }
        public string PassportIdentnum { get; set; }
        public string Сitizenship { get; set; }
        public string WorkPlase { get; set; }
        public string Position { get; set; }
        public string Editor { get; set; }
        public string EditDate { get; set; }

        public Abiturient(int num, int id, string fio, string vladelec, string date)
        {
            Num = num;
            ID = id;
            FIO = fio;
            Vladelec = vladelec;
            Date = date;
            Hide = false;
        }
        public Abiturient() { }
    }
}
