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
        public int Fine { get; set; }
        public object Data { get; set; }
        public Guid? DataTypeGuid { get; private set; }
        public string FactorName { get; private set; }
        public string PathToDll { get; set; }
        public string UsersFactorName { get; private set; }
        public string TypeFullName { get; private set; }

        public FactorSettings(int fine, Type factor, string pathToDll, string usersFactorName, Guid? dataTypeGuid = null, object data = null)
        {
            FactorName = factor.Name;
            PathToDll = pathToDll;
            Fine = fine;
            DataTypeGuid = dataTypeGuid;
            Data = data;
            UsersFactorName = usersFactorName;
            TypeFullName = factor.FullName;
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
