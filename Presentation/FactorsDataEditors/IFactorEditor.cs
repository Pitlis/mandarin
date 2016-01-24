using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.FactorsDataEditors
{
    interface IFactorEditor
    {
        void Init(string factorName, string factorDescription, string userInstruction, EntityStorage storage, FactorSettings factorSettings);
    }
}
