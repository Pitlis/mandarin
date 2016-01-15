using Domain.DataFiles;
using Domain.Model;
using Domain.Services;
using Presentation.FacultyEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Code
{
    static class Save
    {
        public static void SaveSettings(FacultiesAndGroups sett)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream("Settings.dat", FileMode.Create))
            {
                formatter.Serialize(fs, sett);
            }
        }
        public static FacultiesAndGroups LoadSettings()
        {
            //BinaryFormatter formatter = new BinaryFormatter();
            //FacultiesAndGroups sett = null;
            //using (FileStream fs = new FileStream("Settings.dat", FileMode.OpenOrCreate))
            //{
            //    sett = (FacultiesAndGroups)formatter.Deserialize(fs);
            //}
            return new FacultiesAndGroups();
        }
    }
}
