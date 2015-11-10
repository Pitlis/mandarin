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

namespace Presentation.Code
{
    [Serializable]
    class Settings
    {
        public List<Facult> LFacult { get; set; }
        public List<string> NameFacult { get; set; }
        public List<StudentSubGroup> UGroops = new List<StudentSubGroup>();
        public Settings()
        {
                LFacult = new List<Facult>();
                NameFacult = new List<string>() { "ЭЛЕКТРОТЕХНИЧЕСКИЙ", "АВТОМЕХАНИЧЕСКИЙ", "СТРОИТЕЛЬНЫЙ", "МАШИНОСТРОИТЕЛЬНЫЙ", "ЭКОНОМИЧЕСКИЙ", "ИНЖЕНЕРНОЭКОНОМИЧЕСКИЙ" };
        }
        /// <summary>
        /// Возвращает список групп по имени факультета
        /// </summary>
        public List<StudentSubGroup> GetGroops(string Facul)
        {
            foreach (Facult item in LFacult)
            {
                if (item.Name.ToString() == Facul)
                { return item.LGroop; }
            }
            return null;
        }
        /// <summary>
        /// Возвращает список групп по имени факультета и курсу
        /// </summary>
        public List<StudentSubGroup> GetGroops(string Facul, int Numb)
        {
            List<StudentSubGroup> LGro = new List<StudentSubGroup>();
            foreach (Facult item in LFacult)
            {                
                if (item.Name.ToString() == Facul)
                {
                    for (int i = 0; i < item.LGroop.Count; i++)
                    {
                        if (GetСourse(item.LGroop[i]) == Numb) { LGro.Add(item.LGroop[i]); }
                    }
                }
            }
            return LGro;
        }
        /// <summary>
        /// Возвращает номер курса по группе
        /// </summary>
        public static int GetСourse(StudentSubGroup groop)
        {
            string s = groop.NameGroup.Substring(groop.NameGroup.Length - 3, 2);
            int YerEnter = 2000 + Convert.ToInt32(s);
            if (DateTime.Now.Month >= 9 && DateTime.Now.Month <= 12)
            {
                return DateTime.Now.Year - YerEnter + 1;
            }
            else { return DateTime.Now.Year - YerEnter; }
        }
        /// <summary>
        /// Возвращает факультет по группе
        /// </summary>
        public string GetFaculty(StudentSubGroup groop)
        {
            foreach (Facult item in LFacult)
            {
                if (item.LGroop.Contains(groop)) { return item.Name.ToString(); }
            }
            return null;
        }
        public bool ContFacult(string Facul)
        {
            foreach (Facult item in LFacult)
            {
                if (item.Name.ToString() == Facul)
                { return true; }
            }
            return false;
        }
        public void CreateListGroops(string Facul)
        {
            foreach (Facult item in LFacult)
            {
                if (item.Name.ToString() == Facul)
                { item.LGroop = new List<StudentSubGroup>(); }
            }
        }
        public void AddGroop(string Facul, StudentSubGroup groop)
        {
            foreach (var item in LFacult)
            {
                if (item.Name == Facul)
                {
                    item.LGroop.Add(groop);

                }
            }
        }
        public void RemoveGroop(string Facul, StudentSubGroup groop)
        {
            foreach (var item in LFacult)
            {
                if (item.Name == Facul)
                {
                    item.LGroop.Remove(groop);

                }
            }
        }
        public static StudentSubGroup GetClassGroupStorage(StudentSubGroup group, EntityStorage storage)
        {
            foreach(StudentSubGroup item in storage.StudentSubGroups)
            {
                if (item.NameGroup == group.NameGroup && item.NumberSubGroup == group.NumberSubGroup)
                    return item;
            }
            return null;
        }
    }

   
    [Serializable]
    public class Facult
    {
        public string Name;
        public List<StudentSubGroup> LGroop;
        public Facult() { }
        public Facult(string name)
        {
            Name = name;
            LGroop = new List<StudentSubGroup>();
        }


    }


}
