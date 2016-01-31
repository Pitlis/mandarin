using Domain.Model;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Code
{
    class CheckDataInFactors
    {
        IEnumerable<FactorSettings> Factors;
        ClassRoom ClassRoom;
        StudentsClass sClass;
        StudentSubGroup Group;
        ClassRoomType Type;
        EntityStorage store;


        public CheckDataInFactors(EntityStorage storage)
        {
            this.store = storage;
        }
        public List<FactorSettings> CheckClassRoom(ClassRoom classRoom)
        {
            this.ClassRoom = classRoom;
            List<FactorSettings> usedFactors = new List<FactorSettings>();
            Factors = FactorsEditors.GetUsersFactors(CurrentBase.Factors);
            foreach (FactorSettings factor in Factors)
            {
                if (factor.Data != null && factor.Data.GetType() == typeof(Dictionary<Teacher, List<ClassRoom>>))
                {
                    Dictionary<Teacher, List<ClassRoom>> data = (Dictionary < Teacher, List< ClassRoom >>)factor.Data;
                    data = (Dictionary<Teacher, List<ClassRoom>>)FactorsEditors.RestoreLinks(data, store);
                    foreach (List<ClassRoom> list in data.Values)
                    {
                        if (list.Contains(ClassRoom))
                        {
                            usedFactors.Add(factor);
                            break;
                        }
                    }
                }
                if (factor.Data != null && factor.Data.GetType() == typeof(Dictionary<StudentsClass, List<ClassRoom>>))
                {
                    Dictionary<StudentsClass, List<ClassRoom>> data = (Dictionary<StudentsClass, List<ClassRoom>>)factor.Data;
                    data = (Dictionary<StudentsClass, List<ClassRoom>>)FactorsEditors.RestoreLinks(data, store);
                    foreach (List<ClassRoom> list in data.Values)
                    {
                        if (list.Contains(ClassRoom))
                        {
                            usedFactors.Add(factor);
                            break;
                        }

                    }
                }
                if (factor.Data != null && factor.Data.GetType() == typeof(List<FixedClasses>))
                {
                    List<FixedClasses> data = (List<FixedClasses>)factor.Data;
                    data = (List<FixedClasses>)FactorsEditors.RestoreLinks(data, store);
                    foreach (FixedClasses list in data)
                    {
                        if (list.Room == ClassRoom )
                        {
                            usedFactors.Add(factor);
                            break;
                        }

                    }
                }

            }
            return usedFactors;

        }

        public List<FactorSettings> CheckGroup(StudentSubGroup group)
        {
            this.Group = group;            
            List<FactorSettings> usedFactors = new List<FactorSettings>();
            Factors = FactorsEditors.GetUsersFactors(CurrentBase.Factors);
            foreach (FactorSettings factor in Factors)
            {
                if (factor.Data != null && factor.Data.GetType() == typeof(Dictionary<StudentsClass, List<ClassRoom>>))
                {
                    Dictionary<StudentsClass, List<ClassRoom>> data = (Dictionary<StudentsClass, List<ClassRoom>>)factor.Data;
                    data = (Dictionary<StudentsClass, List<ClassRoom>>)FactorsEditors.RestoreLinks(data, store);
                    foreach (StudentsClass sClass in data.Keys)
                    {
                        if (sClass.SubGroups.Contains(group))
                        {
                            usedFactors.Add(factor);
                            break;
                        }

                    }
                }
                if (factor.Data != null && factor.Data.GetType() == typeof(List<FixedClasses>))
                {
                    List<FixedClasses> data = (List<FixedClasses>)factor.Data;
                    data =(List<FixedClasses>)FactorsEditors.RestoreLinks(data, store);
                    foreach (FixedClasses fixedClass in data)
                    {
                        if (fixedClass.sClass.SubGroups.Contains(group))
                        {
                            usedFactors.Add(factor);
                            break;
                        }

                    }
                }
            }
            return usedFactors;

        }

        public List<FactorSettings> CheckStudentClass(StudentsClass sclass)
        {
            this.sClass = sclass;
            List<FactorSettings> usedFactors = new List<FactorSettings>();
            Factors = FactorsEditors.GetUsersFactors(CurrentBase.Factors);
            foreach (FactorSettings factor in Factors)
            {
                if (factor.Data != null && factor.Data.GetType() == typeof(Dictionary<StudentsClass, List<ClassRoom>>))
                {
                    Dictionary<StudentsClass, List<ClassRoom>> t = (Dictionary<StudentsClass, List<ClassRoom>>)factor.Data;
                    t = (Dictionary<StudentsClass, List<ClassRoom>>)FactorsEditors.RestoreLinks(t, store);
                    foreach (StudentsClass itemClass in t.Keys)
                    {
                        if (itemClass == sClass)
                        {
                            usedFactors.Add(factor);
                            break;
                        }

                    }
                }
                if (factor.Data != null && factor.Data.GetType() == typeof(List<FixedClasses>))
                {
                    List<FixedClasses> t = (List<FixedClasses>)factor.Data;
                    t = (List<FixedClasses>)FactorsEditors.RestoreLinks(t, store);
                    foreach (FixedClasses fixedClass in t)
                    {
                        if (fixedClass.sClass == sClass)
                        {
                            usedFactors.Add(factor);
                            break;
                        }

                    }
                }
            }
            return usedFactors;

        }

        public List<FactorSettings> CheckType(ClassRoomType type)
        {
            this.Type = type;
            List<FactorSettings> usedFactors = new List<FactorSettings>();
            Factors = FactorsEditors.GetUsersFactors(CurrentBase.Factors);
            foreach (FactorSettings factor in Factors)
            {
                if (factor.Data != null && factor.Data.GetType() == typeof(Dictionary<StudentsClass, List<ClassRoom>>))
                {
                    Dictionary<StudentsClass, List<ClassRoom>> t = (Dictionary<StudentsClass, List<ClassRoom>>)factor.Data;
                    t = (Dictionary<StudentsClass, List<ClassRoom>>)FactorsEditors.RestoreLinks(t, store);
                   bool ex = false;
                   foreach (StudentsClass key in t.Keys)
                    {
                        foreach (ClassRoom itemClassR in t[key])
                        {
                            if (itemClassR.ClassRoomTypes.Contains(Type) || key.RequireForClassRoom.Contains(Type))
                            {
                                usedFactors.Add(factor);
                                ex = true;
                                break;
                            }
                        }
                        if (ex) break;

                    }
                }
                if (factor.Data != null && factor.Data.GetType() == typeof(List<FixedClasses>))
                {
                    List<FixedClasses> t = (List<FixedClasses>)factor.Data;
                    t = (List<FixedClasses>)FactorsEditors.RestoreLinks(t, store);
                    foreach (FixedClasses fixedClass in t)
                    {
                        if (fixedClass.Room.ClassRoomTypes.Contains(Type) || fixedClass.sClass.RequireForClassRoom.Contains(Type))
                        {
                            usedFactors.Add(factor);
                            break;
                        }

                    }
                }
                if (factor.Data != null && factor.Data.GetType() == typeof(Dictionary<Teacher, List<ClassRoom>>))
                {
                    Dictionary<Teacher, List<ClassRoom>> data = (Dictionary<Teacher, List<ClassRoom>>)factor.Data;
                    data = (Dictionary<Teacher, List<ClassRoom>>)FactorsEditors.RestoreLinks(data, store);
                    bool ex = false;
                    foreach (List<ClassRoom> list in data.Values)
                    {
                        foreach (ClassRoom itemClassR in list)
                        {
                            if (itemClassR.ClassRoomTypes.Contains(Type))
                            {
                                usedFactors.Add(factor);
                                ex = true;
                                break;
                            }
                        }
                        if (ex) break;
                    }
                }
            }
            return usedFactors;

        }


    }
}
