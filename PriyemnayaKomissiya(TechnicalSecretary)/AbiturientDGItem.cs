using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriyemnayaKomissiya_TechnicalSecretary_
{
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
