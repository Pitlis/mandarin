using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.FactorInterfaces
{
    public interface IFactor
    {
        string GetName();
        string GetDescription();
        Guid? GetDataTypeGuid(); // null - если анализатору не требуются дополнительные данные.
        //Метод нужен, чтобы в Presentation определить, какой тип дополнительных данных нужно передавать в анализатор
        //и вывести в соответствии с ним нужную форму для ввода

        void Initialize(int fine = 0, bool isBlock = false, object data = null);

        int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage);
        int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage);
    }
}
