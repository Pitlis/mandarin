using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.FactorInterfaces
{
    //Поддержка анализатором самостоятельного формирования дополнительных данных из хранилища
    public interface IFactorProgramData
    {
        object CreateAndReturnData(EntityStorage eStorage);
    }
}
