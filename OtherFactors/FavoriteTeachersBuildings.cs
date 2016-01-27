using Domain;
using Domain.FactorInterfaces;
using Domain.Model;
using Domain.Services;
using System;
using System.Collections.Generic;

namespace OtherFactors
{
    class FavoriteTeachersBuildings : IFactor, IFactorFormData
    {
        int fine;
        bool isBlock;
        Dictionary<Teacher, List<int>> favBuildings;

        #region IFactor

        public int GetFineOfAddedClass(ISchedule schedule, EntityStorage eStorage)
        {
            int fineResult = 0;
            if (favBuildings == null)
            { return fineResult; }
            foreach (Teacher teacher in schedule.GetTempClass().Teacher)
            {
                if (favBuildings.ContainsKey(teacher))
                {
                    if (favBuildings[teacher].IndexOf(schedule.GetTempClassRooom().Housing) == -1)
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
            if (favBuildings == null)
            { return fineResult; }
            foreach (StudentsClass sClass in eStorage.Classes)
            {
                foreach (Teacher teacher in sClass.Teacher)
                {
                    if (favBuildings.ContainsKey(teacher))
                    {
                        if (favBuildings[teacher].IndexOf(schedule.GetClassRoom(sClass).Housing) == -1)
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
            return "Предпочтительные корпуса преподавателей";
        }

        public string GetDescription()
        {
            return "Если у преподавателя есть предпочтительные корпуса, то его пары лучше ставить в них";
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
                    favBuildings = (Dictionary<Teacher, List<int>>)data;
                }
                catch (Exception ex)
                {
                    new Exception("Неверный формат данных. Требуется список объектов типа IDictionary < Teacher, IEnumerable < int > >. " + ex.Message);
                }
            }
            else { favBuildings = null; }
        }
        public Guid? GetDataTypeGuid()
        {
            return new Guid("6CF3F58B-5FA2-464B-8FF5-5B8E1724E0C9");
        }

        #endregion


        #region IFactorFormData

        public string GetUserInstructions()
        {
            return "Выберите преподавателя и задайте ему корпуса, в которые предпочтительнее ставить его пары";
        }

        public EntityStorage FilterStorage(EntityStorage eStorage)
        {
            return eStorage;
        }

        #endregion
    }
}
