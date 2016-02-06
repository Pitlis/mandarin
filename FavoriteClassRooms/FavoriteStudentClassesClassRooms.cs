using Domain;
using Domain.FactorInterfaces;
using Domain.Model;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherFactors
{
    class FavoriteStudentClassesClassRooms : IFactor, IFactorFormData
    {
        int fine;
        bool isBlock;
        Dictionary<StudentsClass, List<ClassRoom>> favClassRooms;

        #region IFactor

        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            int fineResult = 0;
            if (favClassRooms == null)
            { return fineResult; }
            StudentsClass tempClass = schedule.GetTempClass();
            if (favClassRooms.ContainsKey(tempClass))
            {
                if (favClassRooms[tempClass].Find((c) => c == schedule.GetTempClassRooom()) == null)
                {
                    if (isBlock)
                        return Constants.BLOCK_FINE;
                    else
                        fineResult += fine;
                }
            }
            return fineResult;
        }

        public int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage)
        {
            int fineResult = 0;
            if (favClassRooms == null)
            { return fineResult; }
            foreach (StudentsClass sClass in eStorage.Classes)
            {
                if (favClassRooms.ContainsKey(sClass))
                {
                    if (favClassRooms[sClass].Find((c) => c == schedule.GetClassRoom(sClass)) == null)
                    {
                        if (isBlock)
                            return Constants.BLOCK_FINE;
                        else
                            fineResult += fine;
                    }
                }
            }
            return fineResult;
        }

        public string GetName()
        {
            return "Предпочтительные аудитории пар";
        }

        public string GetDescription()
        {
            return "Если у пары есть предпочтительные аудитории, то эти пары лучше ставить в них";
        }

        public void Initialize(int fine = 0, bool isBlock = false, object data = null)
        {
            if (fine >= 0 && fine <= 100)
            {
                this.fine = fine;
                this.isBlock = isBlock;
                if (fine == 100)
                    this.isBlock = true;
            }
            if (data != null)
            {
                try
                {
                    favClassRooms = (Dictionary<StudentsClass, List<ClassRoom>>)data;
                }
                catch (Exception ex)
                {
                    new Exception("Неверный формат данных. Требуется список объектов типа IDictionary < StudentClass, IEnumerable < ClassRoom > >. " + ex.Message);
                }
            }
            else { favClassRooms = null; }
        }
        public Guid? GetDataTypeGuid()
        {
            return new Guid("A8151C4F-87A1-4FEA-AA2E-FF6E535EDAE1");
        }

        #endregion


        #region IFactorFormData

        public string GetUserInstructions()
        {
            return "Выберите пару и задайте ей аудитории, в которые предпочтительнее её ставить";
        }

        public EntityStorage FilterStorage(EntityStorage eStorage)
        {
            return eStorage;
        }

        #endregion
    }
}
