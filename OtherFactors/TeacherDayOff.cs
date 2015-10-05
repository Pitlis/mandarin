using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Services;

namespace OtherFactors
{
    public class TeacherDayOff : IFactor
    {
        public string GetDescription()
        {
            throw new NotImplementedException();
        }

        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            throw new NotImplementedException();
        }

        public int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage)
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }

        public void Initialize(int fine = 0, bool isBlock = false)
        {
            throw new NotImplementedException();
        }
    }
}
