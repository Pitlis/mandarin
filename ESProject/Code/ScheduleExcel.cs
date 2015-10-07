using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using Domain;
using Domain.Services;
using Domain.Model;

namespace Presentation.Code
{
    class ScheduleExcel
    {

        private Application ObjExcel;
        private Workbook ObjWorkBook;
        private Worksheet ObjWorkSheet;

        public ScheduleExcel(ISchedule schedule, EntityStorage eStorage)
        {
            ObjExcel = new Application();
            ObjWorkBook = ObjExcel.Workbooks.Open(@"D:\ЭС проект\PR\ESProject\d2.xlsx");
            ObjWorkSheet = (Worksheet)ObjWorkBook.Sheets[1];
            int k = 3;
            foreach (StudentSubGroup groop in eStorage.StudentSubGroups)
            {
                PartialSchedule partSchedule;
                partSchedule = schedule.GetPartialSchedule(groop);
                StudentsClass[] sched;
                sched = partSchedule.GetClasses();

                
                
                ((Range)ObjWorkSheet.Cells[1, k]).Value2 = groop.NameGroup;
                ((Range)ObjWorkSheet.Cells[2, k]).Value2 = groop.NumberSubGroup;
                for (int i = 0; i < (Domain.Services.Constants.CLASSES_IN_DAY * Domain.Services.Constants.DAYS_IN_WEEK * Domain.Services.Constants.WEEKS_IN_SCHEDULE); i++)
                {
                    if (sched[i] != null)
                    {
                        ((Range)ObjWorkSheet.Cells[(i + 3), k]).Value2 = sched[i].Name;
                    }

                }
                k++;

                ObjWorkBook.Save();
                
            }
            ObjWorkBook.Close();
            ObjExcel = null;

        }


    }
}
