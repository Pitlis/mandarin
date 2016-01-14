using Domain.FactorInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public string FactorName { get; private set; }
        public string PathToDll { get; private set; }

        public FactorSettings(int fine, Type factor, string pathToDll, Guid? dataTypeGuid = null,object data = null)
        {
            FactorName = factor.Name;
            PathToDll = pathToDll;
            Fine = fine;
            DataTypeGuid = dataTypeGuid;
            Data = data;
        }

        public IFactor CreateInstance(bool isDebugMode = false)
        {
            Assembly asm;
            if (isDebugMode)
            {
                asm = Assembly.Load(PathToDll);
            }
            else
            {
                asm = Assembly.LoadFile(PathToDll);
            }
            Type factor = asm.GetTypes().ToList().Find(f => f.Name == FactorName);
            return (IFactor)Activator.CreateInstance(factor);
        }

    }
}
