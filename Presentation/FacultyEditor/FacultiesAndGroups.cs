using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Domain.Services;
using System.Collections;
using Presentation.Code;

namespace Presentation.FacultyEditor
{
    class FacultiesAndGroups
    {
        public List<Faculty> Faculties { get; private set; }

        //по умолчанию объект работает с базой
        public FacultiesAndGroups()
        {
            Faculties = CurrentBase.Faculties.ToList();
        }
        //но можно работать и с локальной копией списка факультетов - чтобы контролировать изменения
        public FacultiesAndGroups(IEnumerable<Faculty> faculties)
        {
            Faculties = faculties.ToList();
        }

        // Возвращает список групп по имени факультета
        public List<StudentSubGroup> GetGroups(string facultyName)
        {
            foreach (Faculty item in CurrentBase.Faculties)
            {
                if (item.Name == facultyName)
                {
                    return item.Groups;
                }
            }
            return null;
        }

        // Возвращает список групп по имени факультета и курсу
        public List<StudentSubGroup> GetGroups(string facultyName, int course)
        {
            List<StudentSubGroup> groups = new List<StudentSubGroup>();
            foreach (Faculty item in CurrentBase.Faculties)
            {
                if (item.Name == facultyName)
                {
                    for (int groupIndex = 0; groupIndex < item.Groups.Count; groupIndex++)
                    {
                        if (GetСourseByGroup(item.Groups[groupIndex]) == course)
                        {
                            groups.Add(item.Groups[groupIndex]);
                        }
                    }
                }
            }
            return groups;
        }

        // Возвращает номер курса по группе
        public int GetСourseByGroup(StudentSubGroup group)
        {
            string groupCode = group.NameGroup.Substring(group.NameGroup.Length - 3, 2);
            int YerEnter = 2000 + Convert.ToInt32(groupCode);
            if (DateTime.Now.Month >= 9 && DateTime.Now.Month <= 12)
            {
                return DateTime.Now.Year - YerEnter + 1;
            }
            else
            {
                return DateTime.Now.Year - YerEnter;
            }
        }

        // Возвращает название факультета
        public string GetFacultyNameByGroup(StudentSubGroup group)
        {
            foreach (Faculty item in CurrentBase.Faculties)
            {
                if (item.Groups.Contains(group))
                {
                    return item.Name;
                }
            }
            return null;
        }
        public bool FacultyExists(string facultyName)
        {
            foreach (Faculty item in CurrentBase.Faculties)
            {
                if (item.Name == facultyName)
                {
                    return true;
                }
            }
            return false;
        }

        public void AddGroup(string facultyName, StudentSubGroup group)
        {
            foreach (var item in CurrentBase.Faculties)
            {
                if (item.Name == facultyName)
                {
                    item.Groups.Add(group);
                }
            }
        }
        public void RemoveGroup(string facultyName, StudentSubGroup groop)
        {
            foreach (var item in CurrentBase.Faculties)
            {
                if (item.Name == facultyName)
                {
                    item.Groups.Remove(groop);
                }
            }
        }
        public bool GroupsWithoutFacultyExists()
        {
            foreach (var group in CurrentBase.EStorage.StudentSubGroups)
            {
                if(GetFacultyNameByGroup(group) == null)
                {
                    return true;
                }
            }
            return false;
        }
    }

    [Serializable]
    public class Faculty
    {
        public string Name { get; set; }
        public List<StudentSubGroup> Groups { get; set; }
        public Faculty(string name)
        {
            Name = name;
            Groups = new List<StudentSubGroup>();
        }
    }
}

