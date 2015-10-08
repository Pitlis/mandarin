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
            string filename = System.Environment.CurrentDirectory +
                   "\\d2.xlsx";

            ObjWorkBook = ObjExcel.Workbooks.Open(filename);
            ObjWorkSheet = (Worksheet)ObjWorkBook.Sheets[1];
            int k = 3;
            foreach (StudentSubGroup groop in eStorage.StudentSubGroups)
            {
                PartialSchedule partSchedule;
                partSchedule = schedule.GetPartialSchedule(groop);
                StudentsClass[] sched;
                ClassRoom clas;
                sched = partSchedule.GetClasses();

                ((Range)ObjWorkSheet.Cells[1, k]).Clear();
                ((Range)ObjWorkSheet.Cells[2, k]).Clear();

                ((Range)ObjWorkSheet.Cells[1, k]).Value2 = groop.NameGroup;
                ((Range)ObjWorkSheet.Cells[2, k]).Value2 = groop.NumberSubGroup;
                for (int i = 0; i < (Domain.Services.Constants.CLASSES_IN_DAY * Domain.Services.Constants.DAYS_IN_WEEK * Domain.Services.Constants.WEEKS_IN_SCHEDULE); i++)
                {
                    ((Range)ObjWorkSheet.Cells[(i + 3), k]).Clear();
                    if (sched[i] != null)
                    {
                        string str;
                        clas = schedule.GetClassRoom(sched[i]);
                        str = sched[i].Name + "\n" + clas.Housing + " а." + clas.Number;
                        for (int n = 0; n < sched[i].Teacher.Length; n++)
                        {
                            str = str + "\n" + sched[i].Teacher[n].FLSName;
                        }

                        ((Range)ObjWorkSheet.Cells[(i + 3), k]).Value2 = str;
                    }

                }
                k++;

                ObjWorkBook.Save();

            }
            ObjWorkBook.Close();
            ObjExcel = null;

        }


    }
    class ScheduleExcelTeacher
    {
        private Application ObjExcel;
        private Workbook ObjWorkBook;
        private Worksheet ObjWorkSheet;
        public ScheduleExcelTeacher(ISchedule schedule, EntityStorage estorage)
        {
            ObjExcel = new Application();
            string filename = System.Environment.CurrentDirectory +
                   "\\d1.xlsx";
            ObjWorkBook = ObjExcel.Workbooks.Open(filename);
            ObjWorkSheet = (Worksheet)ObjWorkBook.Sheets[1];
            int k = 3;
            foreach (Teacher teach in estorage.Teachers)
            {
                PartialSchedule partSchedule;
                partSchedule = schedule.GetPartialSchedule(teach);
                StudentsClass[] sched;
                ClassRoom clas;
                sched = partSchedule.GetClasses();
                
                ((Range)ObjWorkSheet.Cells[2, k]).Clear();
                ((Range)ObjWorkSheet.Cells[2, k]).Value2 = teach.FLSName;
                ((Range)ObjWorkSheet.Cells[2, k]).Orientation = 75;

                for (int i = 0; i < (Domain.Services.Constants.CLASSES_IN_DAY * Domain.Services.Constants.DAYS_IN_WEEK * Domain.Services.Constants.WEEKS_IN_SCHEDULE); i++)
                {
                    
                    ((Range)ObjWorkSheet.Cells[(i + 3), k]).Clear();
                    if (sched[i] != null)
                    {
                        string str;
                        
                        clas = schedule.GetClassRoom(sched[i]);
                        str = sched[i].Name + "\n" + clas.Housing + " а." + clas.Number;

                        ((Range)ObjWorkSheet.Cells[(i + 3), k]).Value2 = str;
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

