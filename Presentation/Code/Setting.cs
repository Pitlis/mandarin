using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Domain;
using Domain.Model;
using Domain.Services;
using MandarinCore;

namespace Presentation.Code
{
    class Setting
    {
        public IRepository Repo { get; private set; }
        public List<VIPClasesBin> LVIPB { get; set; }
        public List<VIPClases> LVIP { get; set; }
        public EntityStorage storage { get; set; }
        public StudentsClass[] Clases { get; set; }


        public Setting()
        {
            LVIP = new List<VIPClases>();
            //Repo = new MockDataBase.MockRepository();
            //Repo.Init(null);
            Repo = new Data.DataRepository();
            Repo.Init(new string[] { @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=C:\USERS\СЕРГЕЙ\DOCUMENTS\ESPROJECT\ESPROJECT\BIN\DEBUG\BD4.MDF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False" });

            DataConvertor.DomainData data = DataConvertor.ConvertData(Repo.GetTeachers(), Repo.GetStudentsGroups(), Repo.GetClassRoomsTypes(), Repo.GetClassRooms(), Repo.GetStudentsClasses());

            LVIPB = new List<VIPClasesBin>();
            storage = data.eStorage;
            Clases = data.sClasses;
            BinaryFormatter formatter = new BinaryFormatter();


            if (File.Exists("Setting.dat"))
            {

                using (FileStream fs = new FileStream("Setting.dat", FileMode.OpenOrCreate))
                {
                    LVIPB = (List<VIPClasesBin>)formatter.Deserialize(fs);
                }

                foreach (var item in LVIPB)
                {
                    VIPClases n = new VIPClases(Clases[item.Cla], item.Time, storage.ClassRooms[item.Aud]);
                    LVIP.Add(n);
                }

            }



        }
        public Setting(EntityStorage storage, StudentsClass[] Clases)
        {
            LVIP = new List<VIPClases>();
            LVIPB = new List<VIPClasesBin>();
            this.storage = storage;
            this.Clases = Clases;
            BinaryFormatter formatter = new BinaryFormatter();


            if (File.Exists("Setting.dat"))
            {

                using (FileStream fs = new FileStream("Setting.dat", FileMode.OpenOrCreate))
                {
                    LVIPB = (List<VIPClasesBin>)formatter.Deserialize(fs);
                }

                foreach (var item in LVIPB)
                {
                    VIPClases n = new VIPClases(Clases[item.Cla], item.Time, storage.ClassRooms[item.Aud]);
                    LVIP.Add(n);
                }

            }



        }
        public int GetPosClas(StudentsClass sc)
        {
            for (int i = 0; i < Clases.Length; i++)
            {
                if (Clases[i] == sc)
                    return i;
            }
            return -1;
        }

        public int GetAudPos(ClassRoom cl)
        {
            for (int i = 0; i < storage.ClassRooms.Length; i++)
            {
                if (storage.ClassRooms[i] == cl)
                    return i;
            }
            return -1;
        }

        public List<StudentsClass> GetListClases(Teacher teach)
        {
            List<StudentsClass> List = new List<StudentsClass>();
            foreach (StudentsClass item in Clases)
            {
                if (item.Teacher.Contains(teach)) List.Add(item);
            }
            return List;
        }

        public StudentsClass[] GetVipClasses()
        {
            if (LVIP != null)
            {
                StudentsClass[] vipClasses = new StudentsClass[LVIP.Count];
                for (int classIndex = 0; classIndex < LVIP.Count; classIndex++)
                {
                    vipClasses[classIndex] = LVIP[classIndex].sClass;
                }
                return vipClasses;
            }
            else
            {
                return new StudentsClass[0];
            }
        }
    }
    [Serializable]
    class VIPClasesBin
    {
        public int Cla { get; set; }
        public int Time { get; set; }
        public int Aud { get; set; }

        public VIPClasesBin(int cla, int time, int aud)
        {
            this.Cla = cla;
            this.Time = time;
            this.Aud = aud;
        }
    }
}
