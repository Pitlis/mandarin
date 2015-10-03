using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using Data;
using Domain;
using ESCore;

namespace Presentation.Code
{
    class Logic
    {
        public IRepository Repo { get; private set; }
        public List<IFactor> Factors { get; private set; }

        //TODO Заглушка для Dependency Inversion
        public void DI()
        {
            Repo = new Repository();

            Assembly asm = Assembly.Load("FactorsWindows");
            Factors = new List<IFactor>();
            foreach (var factor in asm.GetTypes())
            {
                Factors.Add((IFactor)Activator.CreateInstance(factor));
            }
        }


        public void ConfigFactors(Dictionary<IFactor, int> configs)
        {
            foreach(var config in configs)
            {
                IFactor factor = Factors.Find((f) => f == config.Key);
                factor.Initialize(config.Value, config.Value == 1);
            }
        }

        public void Start()
        {
            ESProjectCore core = new ESProjectCore(Repo.GetCouples().ToList(), Repo.GetClassRooms().ToList(), Factors);
            core.Run();
        }

    }
}
