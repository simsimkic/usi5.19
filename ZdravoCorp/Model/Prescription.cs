using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZdravoCorp.Model
{
    public enum Instructions
    {
        WhileEating,
        AfterEating,
        BeforEating,
        DoesNotMatter
    }
    public class Prescription
    {

        public int DailyUsage { get; set; }
        public  Instructions Instructions { get; set;}
        public List<string> MedicineIds { get; set; }

        public Prescription() {}

        public Prescription(int dailyUsage, Instructions instructions, List<string> medicineIds)
        {
            this.DailyUsage = dailyUsage;
            this.Instructions = instructions;
            this.MedicineIds = medicineIds;
        }

    }
}
