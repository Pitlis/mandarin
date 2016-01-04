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

        string pathToFile = null;
        ISchedule schedule;
        EntityStorage eStorage;


        public ScheduleExcel(ISchedule schedule, EntityStorage eStorage)
        {
            this.schedule = schedule;
            this.eStorage = eStorage;
            System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.InitialDirectory = @"C:\";
            dialog.Filter = "Excel File (*.xlsx)|*.xlsx";
            dialog.FileName = "Расписания групп составленно (" + DateTime.Now.ToString("dd.MM.yyyy(HH.mm)") + ")";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pathToFile = dialog.FileName;
            }
            else
            {
                pathToFile = null;
            }
        }

        public ScheduleExcel(string fileName, ISchedule schedule, EntityStorage eStorage)
        {
            this.schedule = schedule;
            this.eStorage = eStorage;
            pathToFile = fileName;
        }

        public void LoadToExcel()
        {
            if (pathToFile == null)
                return;

            ObjExcel = new Application();
            string path = pathToFile;
            ObjWorkBook = ObjExcel.Workbooks.Add();
            ObjWorkSheet = (Worksheet)ObjWorkBook.Sheets[1];
            CreatTemplate(ObjExcel, ObjWorkBook, ObjWorkSheet);
            int k = 3;
            Range exelcells;
            foreach (StudentSubGroup groop in eStorage.StudentSubGroups)
            {
                PartialSchedule partSchedule;
                partSchedule = schedule.GetPartialSchedule(groop);
                StudentsClass[] sched;
                ClassRoom clas;
                sched = partSchedule.GetClasses();
                int ifor1 = 0, ifor2 = 32;
                ((Range)ObjWorkSheet.Cells[1, k]).Value2 = groop.NameGroup;
                ((Range)ObjWorkSheet.Cells[2, k]).Value2 = groop.NumberSubGroup;
                ((Range)ObjWorkSheet.Cells[1, k]).HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                ((Range)ObjWorkSheet.Cells[1, k]).VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                ((Range)ObjWorkSheet.Cells[2, k]).HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                ((Range)ObjWorkSheet.Cells[2, k]).VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                ((Range)ObjWorkSheet.Cells[1, k]).Font.Bold = true;
                ((Range)ObjWorkSheet.Cells[1, k]).Borders.ColorIndex = 1;
                ((Range)ObjWorkSheet.Cells[2, k]).Borders.ColorIndex = 1;
                for (int i = 0; i < (Domain.Services.Constants.CLASSES_IN_DAY * Domain.Services.Constants.DAYS_IN_WEEK * Domain.Services.Constants.WEEKS_IN_SCHEDULE); i++)
                {
                    ((Range)ObjWorkSheet.Cells[(i + 3), k]).Borders.ColorIndex = 1;
                    ((Range)ObjWorkSheet.Cells[(i + 3), k]).HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                    ((Range)ObjWorkSheet.Cells[(i + 3), k]).VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                    if ((i + 1) % 12 == 0)
                        ((Range)ObjWorkSheet.Cells[(i + 3), k]).Borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlMedium;
                    if (sched[i] != null && i >= 0 && i <= 35)
                    {
                        string str;
                        clas = schedule.GetClassRoom(sched[i]);
                        str = sched[i].Name + " (" + clas.Housing + " а." + clas.Number + " )";
                        for (int n = 0; n < sched[i].Teacher.Length; n++)
                        {
                            str = str + " " + sched[i].Teacher[n].Name;
                        }

                        ((Range)ObjWorkSheet.Cells[(i + 3 + ifor1), k]).ColumnWidth = 30;
                        ((Range)ObjWorkSheet.Cells[(i + 3 + ifor1), k]).Value2 = str;


                    }
                    if (sched[i] != null && i >= 36 && i <= 71)
                    {
                        string str;
                        clas = schedule.GetClassRoom(sched[i]);
                        str = sched[i].Name + " (" + clas.Housing + " а." + clas.Number + " )";
                        for (int n = 0; n < sched[i].Teacher.Length; n++)
                        {
                            str = str + " " + sched[i].Teacher[n].Name;
                        }

                        ((Range)ObjWorkSheet.Cells[(i - ifor2), k]).ColumnWidth = 30;
                        ((Range)ObjWorkSheet.Cells[(i - ifor2), k]).Value2 = str;

                    }
                    if (i >= 0 && i <= 35) ifor1++;
                    if (i >= 36 && i <= 71) ifor2--;
                }
                k++;



            }
            //ANALYSING
            //------------------------------------------------------------------------------------

            string[,] grstr = new string[74, (k - 3)];
            for (int i = 0; i < (k - 3); i++)
            {
                for (int zed = 0; zed < 74; zed++)
                {
                    if (((Range)ObjWorkSheet.Cells[zed + 1, i + 3]).Value2 != null)
                    {
                        grstr[zed, i] = ((Range)ObjWorkSheet.Cells[zed + 1, i + 3]).Value2.ToString();
                    }
                }
            }

            //Horizantal
            for (int zed = 0; zed < 74; zed++)
            {
                if (zed != 1)
                {
                    for (int i = 0; i < (k - 4); i++)
                    {
                        if (grstr[zed, i] != null && grstr[zed, i] == grstr[zed, i + 1])
                        {
                            exelcells = ObjWorkSheet.Range[ObjWorkSheet.Cells[zed + 1, i + 3], ObjWorkSheet.Cells[zed + 1, i + 4]];
                            ((Range)ObjWorkSheet.Cells[zed + 1, i + 4]).Clear();
                            exelcells.Merge(Type.Missing);
                        }
                    }
                }
            }
            //Vertical
            for (int i = 0; i < (k - 3); i++)
            {
                for (int zed = 0; zed < 73; zed += 2)
                {
                    if (grstr[zed, i] == grstr[zed + 1, i])
                    {
                        exelcells = ObjWorkSheet.Range[ObjWorkSheet.Cells[zed + 1, i + 3], ObjWorkSheet.Cells[zed + 2, i + 3]];
                        ((Range)ObjWorkSheet.Cells[zed + 2, i + 3]).Value2 = "";
                        exelcells.Merge(Type.Missing);
                    }
                }
            }
            //------------------------------------------------------------------------------------

            ObjWorkBook.SaveAs(path);
            ObjWorkBook.Close();
            ObjExcel.Quit();
            ObjWorkSheet = null;
            ObjWorkBook = null;
            ObjExcel = null;
        }
        public void LoadPartScheduleExcel(StudentSubGroup[] Groups)
        {
            if (pathToFile == null)
                return;                      
            ObjExcel = new Application();
            string path = pathToFile;
            ObjWorkBook = ObjExcel.Workbooks.Add();
            ObjWorkSheet = (Worksheet)ObjWorkBook.Sheets[1];
            CreatTemplate(ObjExcel, ObjWorkBook, ObjWorkSheet);
            int k = 3;
            Range exelcells;
            foreach (StudentSubGroup groop in Groups)
            {
                PartialSchedule partSchedule;
                partSchedule = schedule.GetPartialSchedule(FacultAndGroop.GetClassGroupStorage(groop, eStorage));
                StudentsClass[] sched;
                ClassRoom clas;
                sched = partSchedule.GetClasses();
                int ifor1 = 0, ifor2 = 32;
                ((Range)ObjWorkSheet.Cells[1, k]).Value2 = groop.NameGroup;
                ((Range)ObjWorkSheet.Cells[1, k]).Borders.Weight = XlBorderWeight.xlMedium;
                ((Range)ObjWorkSheet.Cells[2, k]).Value2 = groop.NumberSubGroup;
                ((Range)ObjWorkSheet.Cells[2, k]).Borders.Weight = XlBorderWeight.xlMedium;
                ((Range)ObjWorkSheet.Cells[1, k]).HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                ((Range)ObjWorkSheet.Cells[1, k]).VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                ((Range)ObjWorkSheet.Cells[2, k]).HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                ((Range)ObjWorkSheet.Cells[2, k]).VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                ((Range)ObjWorkSheet.Cells[1, k]).Font.Bold = true;
                ((Range)ObjWorkSheet.Cells[1, k]).Borders.ColorIndex = 1;
                ((Range)ObjWorkSheet.Cells[2, k]).Borders.ColorIndex = 1;
                for (int i = 0; i < (Domain.Services.Constants.CLASSES_IN_DAY * Domain.Services.Constants.DAYS_IN_WEEK * Domain.Services.Constants.WEEKS_IN_SCHEDULE); i++)
                {
                    ((Range)ObjWorkSheet.Cells[(i + 3), k]).Borders.ColorIndex = 1;
                    ((Range)ObjWorkSheet.Cells[(i + 3), k]).HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                    ((Range)ObjWorkSheet.Cells[(i + 3), k]).VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                    if((i +1)% 12==0)
                        ((Range)ObjWorkSheet.Cells[(i + 3), k]).Borders[XlBordersIndex.xlEdgeBottom].Weight = XlBorderWeight.xlMedium;
                    if (sched[i] != null && i >= 0 && i <= 35)
                    {
                        string str;
                        clas = schedule.GetClassRoom(sched[i]);
                        str = sched[i].Name + " (" + clas.Housing + " а." + clas.Number + " )";
                        for (int n = 0; n < sched[i].Teacher.Length; n++)
                        {
                            str = str + " " + sched[i].Teacher[n].Name;
                        }

                        ((Range)ObjWorkSheet.Cells[(i + 3 + ifor1), k]).ColumnWidth = 30;
                        ((Range)ObjWorkSheet.Cells[(i + 3 + ifor1), k]).Value2 = str;


                    }
                    if (sched[i] != null && i >= 36 && i <= 71)
                    {
                        string str;
                        clas = schedule.GetClassRoom(sched[i]);
                        str = sched[i].Name + " (" + clas.Housing + " а." + clas.Number + " )";
                        for (int n = 0; n < sched[i].Teacher.Length; n++)
                        {
                            str = str + " " + sched[i].Teacher[n].Name;
                        }

                        ((Range)ObjWorkSheet.Cells[(i - ifor2), k]).ColumnWidth = 30;
                        ((Range)ObjWorkSheet.Cells[(i - ifor2), k]).Value2 = str;

                    }
                    if (i >= 0 && i <= 35) ifor1++;
                    if (i >= 36 && i <= 71) ifor2--;
                }
                k++;
              



            }
            //ANALYSING
            //------------------------------------------------------------------------------------

            string[,] grstr = new string[74, (k - 3)];
            for (int i = 0; i < (k - 3); i++)
            {
                for (int zed = 0; zed < 74; zed++)
                {
                    if (((Range)ObjWorkSheet.Cells[zed + 1, i + 3]).Value2 != null)
                    {
                        grstr[zed, i] = ((Range)ObjWorkSheet.Cells[zed + 1, i + 3]).Value2.ToString();
                    }
                }
            }

            //Horizantal
            for (int zed = 0; zed < 74; zed++)
            {
                if (zed != 1)
                {
                    for (int i = 0; i < (k - 4); i++)
                    {
                        if (grstr[zed, i] != null && grstr[zed, i] == grstr[zed, i + 1])
                        {
                            exelcells = ObjWorkSheet.Range[ObjWorkSheet.Cells[zed + 1, i + 3], ObjWorkSheet.Cells[zed + 1, i + 4]];
                            ((Range)ObjWorkSheet.Cells[zed + 1, i + 4]).Clear();
                            exelcells.Merge(Type.Missing);
                        }
                    }
                }
            }
            //Vertical
            for (int i = 0; i < (k - 3); i++)
            {
                for (int zed = 0; zed < 73; zed += 2)
                {
                    if (grstr[zed, i] == grstr[zed + 1, i])
                    {
                        exelcells = ObjWorkSheet.Range[ObjWorkSheet.Cells[zed + 1, i + 3], ObjWorkSheet.Cells[zed + 2, i + 3]];
                        ((Range)ObjWorkSheet.Cells[zed + 2, i + 3]).Value2 = "";
                        exelcells.Merge(Type.Missing);
                    }
                }
            }
            //------------------------------------------------------------------------------------

            ObjWorkBook.SaveAs(path);
            ObjWorkBook.Close();
            ObjExcel.Quit();
            ObjWorkSheet = null;
            ObjWorkBook = null;
            ObjExcel = null;
        }



        public static void CreatTemplate(Application ObjExcel, Workbook ObjWorkBook, Worksheet ObjWorkSheet)
        {
            int k = 3;
            //---------------------------------------------------------
            //Creat template
            Range exelcells;
            exelcells = ObjWorkSheet.Range["A1", "A2"];
            exelcells.Merge(Type.Missing);
            ((Range)ObjWorkSheet.Cells[1, Type.Missing]).ColumnWidth = 4;
            exelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            exelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            ((Range)ObjWorkSheet.Cells[1, 1]).Value2 = "Дни";
            exelcells.Borders.Weight = XlBorderWeight.xlMedium;
            exelcells.Borders.ColorIndex = 1;

            exelcells = ObjWorkSheet.Range["A3", "A14"];
            exelcells.Merge(Type.Missing);
            exelcells.Orientation = 90;
            exelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            exelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            exelcells.Value2 = "Понедельник";
            exelcells.Borders.ColorIndex = 1;
            exelcells.Borders.Weight = XlBorderWeight.xlMedium;

            exelcells = ObjWorkSheet.Range["A15", "A26"];
            exelcells.Merge(Type.Missing);
            exelcells.Orientation = 90;
            exelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            exelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            exelcells.Value2 = "Вторник";
            exelcells.Borders.ColorIndex = 1;
            exelcells.Borders.Weight = XlBorderWeight.xlMedium;

            exelcells = ObjWorkSheet.Range["A27", "A38"];
            exelcells.Merge(Type.Missing);
            exelcells.Orientation = 90;
            exelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            exelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            exelcells.Value2 = "Среда";
            exelcells.Borders.ColorIndex = 1;
            exelcells.Borders.Weight = XlBorderWeight.xlMedium;

            exelcells = ObjWorkSheet.Range["A39", "A50"];
            exelcells.Merge(Type.Missing);
            exelcells.Orientation = 90;
            exelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            exelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            exelcells.Value2 = "Четверг";
            exelcells.Borders.ColorIndex = 1;
            exelcells.Borders.Weight = XlBorderWeight.xlMedium;

            exelcells = ObjWorkSheet.Range["A51", "A62"];
            exelcells.Merge(Type.Missing);
            exelcells.Orientation = 90;
            exelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            exelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            exelcells.Value2 = "Пятница";
            exelcells.Borders.ColorIndex = 1;
            exelcells.Borders.Weight = XlBorderWeight.xlMedium;

            exelcells = ObjWorkSheet.Range["A63", "A74"];
            exelcells.Merge(Type.Missing);
            exelcells.Orientation = 90;
            exelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            exelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            exelcells.Value2 = "Суббота";
            exelcells.Borders.ColorIndex = 1;
            exelcells.Borders.Weight = XlBorderWeight.xlMedium;

            exelcells = ObjWorkSheet.Range["B1", "B2"];
            exelcells.Merge(Type.Missing);
            exelcells.ColumnWidth = 11;
            exelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            exelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
            exelcells.Value2 = "Пары";
            exelcells.Borders.ColorIndex = 1;
            exelcells.Borders.Weight = XlBorderWeight.xlMedium;
            //------------
            for (int ti = 3; ti < 73; ti += 12)
            {
                exelcells = ObjWorkSheet.Range["B" + ti, "B" + (ti + 1)];
                exelcells.Merge(Type.Missing);
                exelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                exelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                exelcells.Value2 = "8.30-10.05";
                exelcells.Borders.ColorIndex = 1;
                exelcells.Borders.Weight = XlBorderWeight.xlMedium;

                exelcells = ObjWorkSheet.Range["B" + (ti + 2), "B" + (ti + 3)];
                exelcells.Merge(Type.Missing);

                exelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                exelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                exelcells.Value2 = "10.25-12.00";
                exelcells.Borders.ColorIndex = 1;
                exelcells.Borders.Weight = XlBorderWeight.xlMedium;

                exelcells = ObjWorkSheet.Range["B" + (ti + 4), "B" + (ti + 5)];
                exelcells.Merge(Type.Missing);

                exelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                exelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                exelcells.Value2 = "12.20-13.55";
                exelcells.Borders.ColorIndex = 1;
                exelcells.Borders.Weight = XlBorderWeight.xlMedium;

                exelcells = ObjWorkSheet.Range["B" + (ti + 6), "B" + (ti + 7)];
                exelcells.Merge(Type.Missing);

                exelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                exelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                exelcells.Value2 = "14.15-15.50";
                exelcells.Borders.ColorIndex = 1;
                exelcells.Borders.Weight = XlBorderWeight.xlMedium;

                exelcells = ObjWorkSheet.Range["B" + (ti + 8), "B" + (ti + 9)];
                exelcells.Merge(Type.Missing);

                exelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                exelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                exelcells.Value2 = "16.00-17.35";
                exelcells.Borders.ColorIndex = 1;
                exelcells.Borders.Weight = XlBorderWeight.xlMedium;

                exelcells = ObjWorkSheet.Range["B" + (ti + 10), "B" + (ti + 11)];
                exelcells.Merge(Type.Missing);
                exelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                exelcells.VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                exelcells.Value2 = "17.45-19.20";
                exelcells.Borders.ColorIndex = 1;
                exelcells.Borders.Weight = XlBorderWeight.xlMedium;
            }
        }
    }
    class ScheduleExcelTeacher
    {
        private Application ObjExcel;
        private Workbook ObjWorkBook;
        private Worksheet ObjWorkSheet;

        string pathToFile = null;
        ISchedule schedule;
        EntityStorage eStorage;

        public ScheduleExcelTeacher(ISchedule schedule, EntityStorage estorage)
        {
            this.schedule = schedule;
            this.eStorage = estorage;

            System.Windows.Forms.SaveFileDialog dialog = new System.Windows.Forms.SaveFileDialog();
            dialog.InitialDirectory = @"C:\";
            dialog.Filter = "Excel File (*.xlsx)|*.xlsx";
            dialog.FileName = "Расписания преподавателей составленно (" + DateTime.Now.ToString("dd.MM.yyyy(HH.mm)") + ")";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pathToFile = dialog.FileName;
            }
            else
            {
                pathToFile = null;
            }
        }

        public ScheduleExcelTeacher(string fileName, ISchedule schedule, EntityStorage eStorage)
        {
            this.schedule = schedule;
            this.eStorage = eStorage;
            pathToFile = fileName;
        }


        public void LoadToExcel()
        {
            if (pathToFile == null)
                return;
            string path = pathToFile;
            ObjExcel = new Application();
            ObjWorkBook = ObjExcel.Workbooks.Add();
            ObjWorkSheet = (Worksheet)ObjWorkBook.Sheets[1];
            ScheduleExcel.CreatTemplate(ObjExcel, ObjWorkBook, ObjWorkSheet);
            int k = 3;      
            foreach (Teacher teach in eStorage.Teachers)
            {
                PartialSchedule partSchedule;
                partSchedule = schedule.GetPartialSchedule(teach);
                StudentsClass[] sched;
                ClassRoom clas;
                sched = partSchedule.GetClasses();
                int ifor1 = 0, ifor2 = 32;
                ((Range)ObjWorkSheet.Cells[2, k]).Clear();
                ((Range)ObjWorkSheet.Cells[2, k]).Value2 = teach.Name;
                ((Range)ObjWorkSheet.Cells[2, k]).Orientation = 75;

                for (int i = 0; i < (Domain.Services.Constants.CLASSES_IN_DAY * Domain.Services.Constants.DAYS_IN_WEEK * Domain.Services.Constants.WEEKS_IN_SCHEDULE); i++)
                {
                    if (sched[i] != null && i >= 0 && i <= 35)
                    {
                        string str;

                        clas = schedule.GetClassRoom(sched[i]);
                        str = sched[i].Name + "(" + clas.Housing + " а." + clas.Number + ")";

                        ((Range)ObjWorkSheet.Cells[(i + 3 + ifor1), k]).ColumnWidth = 30;
                        ((Range)ObjWorkSheet.Cells[(i + 3 + ifor1), k]).HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                        ((Range)ObjWorkSheet.Cells[(i + 3 + ifor1), k]).VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                        ((Range)ObjWorkSheet.Cells[(i + 3 + ifor1), k]).Value2 = str;
                    }

                    if (sched[i] != null && i >= 36 && i < 72)
                    {
                        string str;

                        clas = schedule.GetClassRoom(sched[i]);
                        str = sched[i].Name + "(" + clas.Housing + " а." + clas.Number + ")";

                        ((Range)ObjWorkSheet.Cells[(i - ifor2), k]).ColumnWidth = 30;
                        ((Range)ObjWorkSheet.Cells[(i - ifor2), k]).HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                        ((Range)ObjWorkSheet.Cells[(i - ifor2), k]).VerticalAlignment = Microsoft.Office.Interop.Excel.Constants.xlCenter;
                        ((Range)ObjWorkSheet.Cells[(i - ifor2), k]).Value2 = str;
                    }
                    if (i >= 0 && i <= 35) ifor1++;
                    if (i >= 36 && i <= 71) ifor2--;
                }
                k++;
            }
            ObjWorkBook.SaveAs(path);
            ObjWorkBook.Close();
            ObjExcel.Quit();
            ObjWorkSheet = null;
            ObjWorkBook = null;
            ObjExcel = null;


        }
    }
}

