using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DataFiles
{
    [Serializable]
    public class Base
    {
        public EntityStorage EStorage { get; set; }
        public List<FactorSettings> Factors { get; set; }
        public Dictionary<string, Object> Settings { get; set; }
    }
}
