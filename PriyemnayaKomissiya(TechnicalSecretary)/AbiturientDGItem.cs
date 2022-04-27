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
        public int Mark1 { get; set; }
        public int Mark2 { get; set; }
        public int Mark3 { get; set; }
        public int Mark4 { get; set; }
        public int Mark5 { get; set; }
        public int Mark6 { get; set; }
        public int Mark7 { get; set; }
        public int Mark8 { get; set; }
        public int Mark9 { get; set; }
        public int Mark10 { get; set; }
        public double MarkAvg { get; set; }
        public double MarkDecAvg { get; set; }
        public string Status { get; set; }
        public bool DocumentiVidani { get; set; }
        public bool Hide { get; set; }
        public bool DifferentAttestat { get; set; }

        public AbiturientDGItem(int id, string fio, string shool, int year, int mark1, int mark2, int mark3, int mark4, int mark5, int mark6, int mark7, int mark8, int mark9, int mark10, double markAvg, string examNum, bool doc, double marcDecAvg, string lgoti, string status)
        {
            Num = 0;
            ID = id;
            FIO = fio;
            Shool = shool;
            Year = year;
            Mark1 = mark1;
            Mark2 = mark2;
            Mark3 = mark3;
            Mark4 = mark4;
            Mark5 = mark5;
            Mark6 = mark6;
            Mark7 = mark7;
            Mark8 = mark8;
            Mark9 = mark9;
            Mark10 = mark10;
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
