using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public interface IFactor
    {
        string GetName();
        string GetDescription();
        void Initialize(int fine = 0, bool isBlock = false);

        int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage);
        int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage);
    }
}
