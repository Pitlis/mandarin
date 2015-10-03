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

        int GetFine(ISchedule schedule);
    }
}
