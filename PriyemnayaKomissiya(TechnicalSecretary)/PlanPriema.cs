using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriyemnayaKomissiya_TechnicalSecretary_
{
    class PlanPriema
    {
        public int Id { get; set; }
        public int IdSpec { get; set; }
        public int IdForm { get; set; }
        public int IdFinance { get; set; }
        public int Count { get; set; }
        public int CountCelevihMest { get; set; }
        public string Year { get; set; }
        public string CodeSpec { get; set; }
        public string NameSpec { get; set; }
        public string NameForm { get; set; }
        public string NameFinance { get; set; }
        public string NameObrazovaie { get; set; }
        public bool Ct { get; set; }
    }
}
