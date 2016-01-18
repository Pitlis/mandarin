using Domain;
using Domain.DataBaseTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class DataRepository : IRepository
    {
        SqlConnection connection;
       
        public List<string> GetParametersNames()
        {
            return new List<string>() { "Строка подключения" };
        }

        public bool Init(string[] connectionStrings)
        {
            try
            {
                connection = new SqlConnection(connectionStrings[0]);
                connection.Open();
                return true; // инициализация прошла успешно
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<DBClassRoom> GetClassRooms()
        {
            string commandText = "select IDrooms,Housing,Number from ClassRooms order by IDrooms asc";
            SqlCommand command = new SqlCommand(commandText, connection);
            DataTable dtClassRooms = FilingDT(command);
            DBClassRoom[] classRooms = new DBClassRoom[dtClassRooms.Rows.Count];
            for (int rowIndexRooms = 0; rowIndexRooms < dtClassRooms.Rows.Count; rowIndexRooms++)
            {
                //список всех типов  для данной аудитории получаем
                commandText = "select IDTypes,flag from ClassRoomsTypes where IDrooms=@IDrooms order by IDTypes asc";
                command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@IDrooms", Convert.ToInt32(dtClassRooms.Rows[rowIndexRooms][0].ToString()));
                DataTable dttypes = FilingDT(command);
                int[] ClassRoomsTypesRoom = new int[dttypes.Rows.Count];
                BitArray secondTypesMask = new BitArray(dttypes.Rows.Count, false);
                for (int rowIndexTypes = 0; rowIndexTypes < dttypes.Rows.Count; rowIndexTypes++)
                {
                    ClassRoomsTypesRoom[rowIndexTypes] = Convert.ToInt32(dttypes.Rows[rowIndexTypes][0].ToString());
                    if (Convert.ToInt32(dttypes.Rows[rowIndexTypes][1].ToString()) == 1)
                    {
                        secondTypesMask[rowIndexTypes] = true;
                    }
                }
                classRooms[rowIndexRooms] = new DBClassRoom(Convert.ToInt32(dtClassRooms.Rows[rowIndexRooms][0].ToString()),
                                                Convert.ToInt32(dtClassRooms.Rows[rowIndexRooms][2].ToString()),
                                                Convert.ToInt32(dtClassRooms.Rows[rowIndexRooms][1].ToString()),
                                                ClassRoomsTypesRoom.ToList(),
                                                secondTypesMask);
            }
            return classRooms.ToList();
        }

        public IEnumerable<DBClassRoomType> GetClassRoomsTypes()
        {
            string commandText = "select IDTypes, Description from Types order by IDTypes asc";
            SqlCommand command = new SqlCommand(commandText, connection);
            DataTable dtRoomTypes = FilingDT(command);
            DBClassRoomType[] сlassRoomsTypes = new DBClassRoomType[dtRoomTypes.Rows.Count];
            for (int rowIndex = 0; rowIndex < dtRoomTypes.Rows.Count; rowIndex++)
            {
                сlassRoomsTypes[rowIndex] = new DBClassRoomType(Convert.ToInt32(dtRoomTypes.Rows[rowIndex][0].ToString()),
                                                                 dtRoomTypes.Rows[rowIndex][1].ToString());
            }
            return сlassRoomsTypes.ToList();
        }

        public IEnumerable<DBStudentsClass> GetStudentsClasses()
        {
            string commandText = "select ID,Name,Count from Classes order by ID asc";
            SqlCommand command = new SqlCommand(commandText, connection);
            DataTable dtStudentClasses = FilingDT(command);
            commandText = "select Sum(Count) from Classes ";
            command = new SqlCommand(commandText, connection);
            DataTable dtcount = FilingDT(command);
            int countClasses = Convert.ToInt32(dtcount.Rows[0][0].ToString());
            DBStudentsClass[] studentsClass = new DBStudentsClass[countClasses];
            int posIndexClasses = 0;
            for (int rowIndexClasses = 0; rowIndexClasses < dtStudentClasses.Rows.Count; rowIndexClasses++)
            {
                //список типов всех получаем
                commandText = "select IDTypes from ClassesTypes where ID=@ID order by IDTypes asc";
                command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@ID", Convert.ToInt32(dtStudentClasses.Rows[rowIndexClasses][0].ToString()));
                DataTable dtClassTypes = FilingDT(command);
                int[] classRoomsTypesClass = new int[dtClassTypes.Rows.Count];
                for (int rowIndexTypes = 0; rowIndexTypes < dtClassTypes.Rows.Count; rowIndexTypes++)
                {
                    int id = Convert.ToInt32(dtClassTypes.Rows[rowIndexTypes][0].ToString());
                    classRoomsTypesClass[rowIndexTypes] = id;
                }
                //////////////
                commandText = "select IDTeachers from ClassesTeachers where ID=@ID order by IDTeachers asc";
                command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@ID", Convert.ToInt32(dtStudentClasses.Rows[rowIndexClasses][0].ToString()));
                DataTable dtclassTeachers = FilingDT(command);
                int[] teachersClass = new int[dtclassTeachers.Rows.Count];
                for (int rowIndexTeachers = 0; rowIndexTeachers < dtclassTeachers.Rows.Count; rowIndexTeachers++)
                {
                    int id = Convert.ToInt32(dtclassTeachers.Rows[rowIndexTeachers][0].ToString());
                    teachersClass[rowIndexTeachers] = id;
                }
                /////////////////
                commandText = "select IDGroups from ClassesSubGroups where ID=@ID order by IDGroups asc";
                command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@ID", Convert.ToInt32(dtStudentClasses.Rows[rowIndexClasses][0].ToString()));
                DataTable dtclassSubGroup = FilingDT(command);
                int[] studentSubGroup = new int[dtclassSubGroup.Rows.Count];
                for (int rowIndexSubGroup = 0; rowIndexSubGroup < dtclassSubGroup.Rows.Count; rowIndexSubGroup++)
                {
                    int id = Convert.ToInt32(dtclassSubGroup.Rows[rowIndexSubGroup][0].ToString());
                    studentSubGroup[rowIndexSubGroup] = id;
                }
                if (teachersClass.Count() == 0) teachersClass = new int[] { };//????????
                ////////////
                for (int countClass = 0; countClass < Convert.ToInt32(dtStudentClasses.Rows[rowIndexClasses][2].ToString()); countClass++)
                {
                    studentsClass[posIndexClasses] = new DBStudentsClass(posIndexClasses,
                                                            studentSubGroup,
                                                            teachersClass,
                                                            dtStudentClasses.Rows[rowIndexClasses][1].ToString().ToUpper(),
                                                            classRoomsTypesClass);
                    posIndexClasses++;
                }


            }
            return studentsClass.ToList();
        }

        public IEnumerable<DBStudentSubGroup> GetStudentsGroups()
        {
            string commandText = "select IDGroups,NameGroup,NumberSubGroup from SubGroups order by  IDGroups Asc ";
            SqlCommand command = new SqlCommand(commandText, connection);
            DataTable dtSubGroups = FilingDT(command);
            DBStudentSubGroup[] studentSubGroups = new DBStudentSubGroup[dtSubGroups.Rows.Count];
            for (int rowIndex = 0; rowIndex < dtSubGroups.Rows.Count; rowIndex++)
            {
                studentSubGroups[rowIndex] = new DBStudentSubGroup(Convert.ToInt32(dtSubGroups.Rows[rowIndex][0].ToString()),
                                                          dtSubGroups.Rows[rowIndex][1].ToString().ToUpper(),
                                                          Convert.ToByte(dtSubGroups.Rows[rowIndex][2].ToString()));
            }
            return studentSubGroups.ToList();
        }

        public IEnumerable<DBTeacher> GetTeachers()
        {
            string commandText = "select IDTeachers,LName,FName,SName from Teachers order by  IDTeachers Asc";
            SqlCommand command = new SqlCommand(commandText, connection);
            DataTable dtTeachers = FilingDT(command);
            DBTeacher[] teachers = new DBTeacher[dtTeachers.Rows.Count];
            for (int rowIndex = 0; rowIndex < dtTeachers.Rows.Count; rowIndex++)
            {
                string fullnameteacher = dtTeachers.Rows[rowIndex][1].ToString().ToUpper() + " "
                                        + dtTeachers.Rows[rowIndex][2].ToString().ToUpper()[0] + "."
                                        + dtTeachers.Rows[rowIndex][3].ToString().ToUpper()[0] + ".";
                teachers[rowIndex] = new DBTeacher(Convert.ToInt32(dtTeachers.Rows[rowIndex][0].ToString()),
                                                   fullnameteacher);
            }
            return teachers.ToList();
        }

        static DataTable FilingDT(SqlCommand command)
        {
            DataTable DT = new DataTable();
            SqlDataAdapter sqladapter = new SqlDataAdapter(command);
            SqlCommandBuilder sqlcmd = new SqlCommandBuilder(sqladapter);
            sqladapter.Fill(DT);
            return DT;
        }
    }
}
