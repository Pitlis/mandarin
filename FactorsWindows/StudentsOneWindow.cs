using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Services;

namespace FactorsWindows
{
    public class StudentsOneWindow : IFactor
    {
        int fine;
        bool isBlock;
        //Здесь был Серёжа. Привет Никита!!!!!!!
        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            return 0;
        }

        public int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage)
        {
            return 0;
        }



        public string GetDescription()
        {
            return "Одна форточка у студентов";
        }

        public string GetName()
        {
            return "Форточка у студентов";
        }

        public void Initialize(int fine = 0, bool isBlock = false)
        {
            if(fine >= 0 && fine <= 100)
            {
                this.fine = fine;
                this.isBlock = isBlock;
                if (fine == 100)
                    this.isBlock = true;
            }
        }


    }
}
