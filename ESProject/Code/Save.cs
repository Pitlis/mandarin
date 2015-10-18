using Domain.Model;
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
        public static void SaveSchedule(FullSchedule schedule)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream("schedule.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, schedule);
            }
        }
        public static FullSchedule LoadSchedule()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FullSchedule schedule = null;
            using (FileStream fs = new FileStream("schedule.dat", FileMode.OpenOrCreate))
            {
                schedule = (FullSchedule)formatter.Deserialize(fs);
            }
            return schedule;
        }
    }
}
