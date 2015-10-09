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
        object GetDataType(); // null - если дополнительные данные не поддерживаются.
        //Метод нужен, чтобы в Presentation определить, какой тип дополнительных данных нужно передавать в фактор
        //и вывести в соответствии с ним нужную форму для ввода

        void Initialize(int fine = 0, bool isBlock = false, object data = null);

        int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage);
        int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage);
    }
    public struct DataFactor
    {
        public DataFactor(int fine, object data = null) : this()
        {
            Fine = fine;
            Data = data;
        }

        public int Fine { get; private set; }
        public object Data { get; private set; }
    }
}
