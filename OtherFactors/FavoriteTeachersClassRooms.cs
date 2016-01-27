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
    class FavoriteTeachersClassRooms : IFactor, IFactorFormData
    {
        int fine;
        bool isBlock;
        Dictionary<Teacher, List<ClassRoom>> favClassRooms;

        #region IFactor

        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            int fineResult = 0;
            if(favClassRooms == null)
            { return fineResult; }
            foreach (Teacher teacher in schedule.GetTempClass().Teacher)
            {
                if (favClassRooms.ContainsKey(teacher))
                {
                    if (favClassRooms[teacher].Find((c) => c == schedule.GetTempClassRooom()) == null)
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

        public int GetFineOfFullSchedule(ISchedule schedule, EntityStorage eStorage)
        {
            int fineResult = 0;
            if (favClassRooms == null)
            { return fineResult; }
            foreach (StudentsClass sClass in eStorage.Classes)
            {
                foreach (Teacher teacher in sClass.Teacher)
                {
                    if (favClassRooms.ContainsKey(teacher))
                    {
                        if (favClassRooms[teacher].Find((c) => c == schedule.GetClassRoom(sClass)) == null)
                        {
                            if (isBlock)
                                return Constants.BLOCK_FINE;
                            else
                                fineResult += fine;
                        }
                    }
                }
            }
            return fineResult;
        }

        public string GetName()
        {
            return "Предпочтительные аудитории преподавателей";
        }

        public string GetDescription()
        {
            return "Если у преподавателя есть предпочтительные аудитории, то его пары лучше ставить в них";
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
                    favClassRooms = (Dictionary<Teacher, List<ClassRoom>>)data;
                }
                catch (Exception ex)
                {
                    new Exception("Неверный формат данных. Требуется список объектов типа IDictionary < Teacher, IEnumerable < ClassRoom > >. " + ex.Message);
                }
            }
            else { favClassRooms = null; }
        }
        public Guid? GetDataTypeGuid()
        {
            return new Guid("FA8861A3-02A1-4638-AF20-DF29A61A50F5");
        }

        #endregion


        #region IFactorFormData

        public string GetUserInstructions()
        {
            return "Выберите преподавателя и задайте ему аудитории, в которые предпочтительнее ставить его пары";
        }

        public EntityStorage FilterStorage(EntityStorage eStorage)
        {
            return eStorage;
        }

        #endregion
    }
}
