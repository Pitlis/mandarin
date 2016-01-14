using Domain.FactorInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    [Serializable]
    public class FactorSettings
    {
        public int Fine { get; private set; }
        public Guid? DataTypeGuid { get; private set; }
        public object Data { get; set; }
        public Type Factor { get; private set; }

        public FactorSettings(int fine, Type factor, Guid? dataTypeGuid = null,object data = null)
        {
            Factor = factor;
            Fine = fine;
            DataTypeGuid = dataTypeGuid;
            Data = data;
        }

    }
}
