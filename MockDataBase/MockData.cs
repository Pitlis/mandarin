using Domain.DataBaseTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockDataBase
{
    //Информация в тестовой базе данных
    static class MockData
    {
        public static List<DBTeacher> GetTeachers()
        {
            DBTeacher[] t = new DBTeacher[26];
            t[0] = new DBTeacher(0, "Козлова Л.Г.");
            t[1] = new DBTeacher(1, "Абабурко В.Н");
            t[2] = new DBTeacher(2, "Борисов В.И.");
            t[3] = new DBTeacher(3, "Вайнилович Ю.В.");
            t[4] = new DBTeacher(4, "Довженко Г.В.");
            t[5] = new DBTeacher(5, "Другая Л.В.");
            t[6] = new DBTeacher(6, "Зайченко Е.А.");
            t[7] = new DBTeacher(7, "Кто Т.О.");
            t[8] = new DBTeacher(8, "Мисник А.Е.");
            t[9] = new DBTeacher(9, "МНО Г.О.");
            t[10] = new DBTeacher(10, "Наркевич Л.В.");
            t[11] = new DBTeacher(11, "Незнаю");
            t[12] = new DBTeacher(12, "Крутолевич С.К.");
            t[13] = new DBTeacher(13, "НеПрудников В.М.");
            t[14] = new DBTeacher(14, "Обидина О.В");
            t[15] = new DBTeacher(15, "Парфенович О.Н.");
            t[16] = new DBTeacher(16, "Поздняков В.Ф");
            t[17] = new DBTeacher(17, "Прудников А.М.");
            t[18] = new DBTeacher(18, "Селиванов В.А.");
            t[19] = new DBTeacher(19, "Сергеев С.С.");
            t[20] = new DBTeacher(20, "Ситников В.Н.");
            t[21] = new DBTeacher(21, "Скарыно Б.Б.");
            t[22] = new DBTeacher(22, "Столяров Ю.Д.");
            t[23] = new DBTeacher(23, "Черная Н.Г.");
            t[24] = new DBTeacher(24, "Якимов А.И");
            t[25] = new DBTeacher(25, "Прудников В.М");
            return t.ToList();
        }

        public static List<DBStudentSubGroup> GetGroups()
        {
            DBStudentSubGroup[] t = new DBStudentSubGroup[7];
            t[0] = new DBStudentSubGroup(0, "АСОИ121", 1);
            t[1] = new DBStudentSubGroup(1, "АСОИ121", 2);
            t[2] = new DBStudentSubGroup(2, "АСОИ122", 1);
            t[3] = new DBStudentSubGroup(3, "АЭП121", 1);
            t[4] = new DBStudentSubGroup(4, "АЭП121", 2);
            t[5] = new DBStudentSubGroup(5, "АЭП122", 1);
            t[6] = new DBStudentSubGroup(6, "МПК121", 1);
            return t.ToList();
        }

        public static List<DBClassRoomType> GetRoomTypes()
        {
            DBClassRoomType[] t = new DBClassRoomType[8];
            t[0] = new DBClassRoomType(0, "Лекция");
            t[1] = new DBClassRoomType(1, "Практика");
            t[2] = new DBClassRoomType(2, "Лабораторная-комп");
            t[3] = new DBClassRoomType(3, "Лабораторная-физ");
            t[4] = new DBClassRoomType(4, "Лабораторная-эл");
            t[5] = new DBClassRoomType(5, "СпортЗал");
            t[6] = new DBClassRoomType(6, "Выездная");
            t[7] = new DBClassRoomType(7, "Особая");
            return t.ToList();
        }

        public static List<DBClassRoom> GetClassRooms()
        {
            DBClassRoom[] cl = new DBClassRoom[35];
            cl[0] = new DBClassRoom(0, 410, 2, new List<int> { 0, 7 }, new System.Collections.BitArray(new bool[] { false, true }));
            cl[1] = new DBClassRoom(1, 411, 4, new List<int> { 0 });
            cl[2] = new DBClassRoom(2, 409, 2, new List<int> { 0 });
            cl[3] = new DBClassRoom(3, 412, 2, new List<int> { 0, 1 });
            cl[4] = new DBClassRoom(4, 213, 2, new List<int> { 0 });
            cl[5] = new DBClassRoom(5, 202, 2, new List<int> { 0 });
            cl[6] = new DBClassRoom(6, 502, 2, new List<int> { 0 });
            cl[7] = new DBClassRoom(7, 111, 2, new List<int> { 0, 4 });
            cl[8] = new DBClassRoom(8, 301, 2, new List<int> { 0 });
            cl[9] = new DBClassRoom(9, 416, 2, new List<int> { 2 });
            cl[10] = new DBClassRoom(10, 518, 2, new List<int> { 2 });
            cl[11] = new DBClassRoom(11, 517, 2, new List<int> { 2 });
            cl[12] = new DBClassRoom(12, 439, 1, new List<int> { 2 });
            cl[13] = new DBClassRoom(13, 401, 3, new List<int> { 2 });
            cl[14] = new DBClassRoom(14, 203, 1, new List<int> { 2 });
            cl[15] = new DBClassRoom(15, 231, 1, new List<int> { 2 });
            cl[16] = new DBClassRoom(16, 516, 2, new List<int> { 2 });
            cl[17] = new DBClassRoom(17, 511, 2, new List<int> { 2 });
            cl[18] = new DBClassRoom(18, 514, 2, new List<int> { 3, 1 });
            cl[19] = new DBClassRoom(18, 311, 4, new List<int> { 3 });
            cl[20] = new DBClassRoom(20, 405, 2, new List<int> { 1 });
            cl[21] = new DBClassRoom(21, 404, 2, new List<int> { 3, 4 });
            cl[22] = new DBClassRoom(22, 207, 2, new List<int> { 4 });
            cl[23] = new DBClassRoom(23, 204, 2, new List<int> { 1, 4 });
            cl[24] = new DBClassRoom(24, 402, 2, new List<int> { 4 });
            cl[25] = new DBClassRoom(25, 419, 1, new List<int> { 1 });
            cl[26] = new DBClassRoom(26, 216, 2, new List<int> { 4 });
            cl[27] = new DBClassRoom(27, 0, 1, new List<int> { 5 });
            cl[28] = new DBClassRoom(28, 1, 1, new List<int> { 6 });
            cl[29] = new DBClassRoom(29, 254, 1, new List<int> { 1 });
            cl[30] = new DBClassRoom(30, 401, 4, new List<int> { 1 });
            cl[31] = new DBClassRoom(31, 512, 2, new List<int> { 1 });
            cl[32] = new DBClassRoom(32, 527, 1, new List<int> { 1 });
            cl[33] = new DBClassRoom(33, 506, 2, new List<int> { 1 });
            cl[34] = new DBClassRoom(34, 316, 2, new List<int> { 7 });
            return cl.ToList();
        }

        public static List<DBStudentsClass> GetStudentClasses()
        {
            DBStudentsClass[] SC = new DBStudentsClass[140];
            SC[0] = new DBStudentsClass(0, new List<int>() { 0, 1, 2 },
                                         new List<int>() { 24 }, "ММИПУ",
                                         new List<int>() { 7 });
            SC[1] = new DBStudentsClass(1, new List<int>() { 0 },
                                         new List<int>() { 24 }, "ММИПУ",
                                         new List<int>() { 2 });
            SC[2] = new DBStudentsClass(2, new List<int>() { 1 },
                                         new List<int>() { 24 }, "ММИПУ",
                                         new List<int>() { 2 });
            SC[3] = new DBStudentsClass(3, new List<int>() { 2 },
                                         new List<int>() { 24 }, "ММИПУ",
                                         new List<int>() { 2 });

            SC[4] = new DBStudentsClass(4, new List<int>() { 0, 1, 2 },
                                         new List<int>() { 24 }, "ММИПУ",
                                         new List<int>() { 7 });
            SC[5] = new DBStudentsClass(5, new List<int>() { 0 },
                                         new List<int>() { 24 }, "ММИПУ",
                                         new List<int>() { 2 });
            SC[6] = new DBStudentsClass(6, new List<int>() { 1 },
                                         new List<int>() { 24 }, "ММИПУ",
                                         new List<int>() { 2 });
            SC[7] = new DBStudentsClass(7, new List<int>() { 2 },
                                         new List<int>() { 24 }, "ММИПУ",
                                         new List<int>() { 2 });

            SC[8] = new DBStudentsClass(8, new List<int>() { 0, 1, 2 },
                                         new List<int>() { 6 }, "СПО",
                                         new List<int>() { 0 });
            SC[136] = new DBStudentsClass(136, new List<int>() { 0, 1, 2 },
                             new List<int>() { 6 }, "СПО",
                             new List<int>() { 0 });
            SC[137] = new DBStudentsClass(137, new List<int>() { 0, 1, 2 },
                             new List<int>() { 6 }, "СПО",
                             new List<int>() { 0 });
            SC[9] = new DBStudentsClass(9, new List<int>() { 0 },
                                        new List<int>() { 6 }, "СПО",
                                        new List<int>() { 2 });
            SC[10] = new DBStudentsClass(10, new List<int>() { 1 },
                                         new List<int>() { 6 }, "СПО",
                                         new List<int>() { 2 });
            SC[11] = new DBStudentsClass(11, new List<int>() { 2 },
                                         new List<int>() { 6 }, "СПО",
                                         new List<int>() { 2 });


            SC[12] = new DBStudentsClass(12, new List<int>() { 0, 1, 2 },
                                         new List<int>() { 6 }, "СПО",
                                         new List<int>() { 0 });
            SC[13] = new DBStudentsClass(13, new List<int>() { 0 },
                                         new List<int>() { 6 }, "СПО",
                                         new List<int>() { 2 });
            SC[14] = new DBStudentsClass(14, new List<int>() { 1 },
                                         new List<int>() { 6 }, "СПО",
                                         new List<int>() { 2 });
            SC[15] = new DBStudentsClass(15, new List<int>() { 2 },
                                         new List<int>() { 6 }, "СПО",
                                         new List<int>() { 2 });


            SC[18] = new DBStudentsClass(18, new List<int>() { 0, 1, 2 },
                                        new List<int>() { 22 }, "АЭВМ",
                                        new List<int>() { 0 });
            SC[19] = new DBStudentsClass(19, new List<int>() { 0 },
                                         new List<int>() { 22 }, "АЭВМ",
                                         new List<int>() { 2 });
            SC[20] = new DBStudentsClass(20, new List<int>() { 1 },
                                         new List<int>() { 22 }, "АЭВМ",
                                         new List<int>() { 2 });
            SC[21] = new DBStudentsClass(21, new List<int>() { 2 },
                                         new List<int>() { 22 }, "АЭВМ",
                                         new List<int>() { 2 });

            SC[22] = new DBStudentsClass(22, new List<int>() { 0, 1, 2 },
                                        new List<int>() { 22 }, "АЭВМ",
                                        new List<int>() { 0 });
            SC[23] = new DBStudentsClass(23, new List<int>() { 0 },
                                         new List<int>() { 22 }, "АЭВМ",
                                         new List<int>() { 2 });
            SC[24] = new DBStudentsClass(24, new List<int>() { 1 },
                                         new List<int>() { 22 }, "АЭВМ",
                                         new List<int>() { 2 });
            SC[25] = new DBStudentsClass(25, new List<int>() { 2 },
                                         new List<int>() { 22 }, "АЭВМ",
                                         new List<int>() { 2 });

            SC[26] = new DBStudentsClass(26, new List<int>() { 0, 1, 2 },
                                        new List<int>() { 10 }, "Эконом",
                                        new List<int>() { 0 });
            SC[27] = new DBStudentsClass(27, new List<int>() { 0, 1 },
                                         new List<int>() { 5 }, "Эконом",
                                         new List<int>() { 1 });
            SC[28] = new DBStudentsClass(28, new List<int>() { 2 },
                                         new List<int>() { 5 }, "Эконом",
                                         new List<int>() { 1 });

            SC[29] = new DBStudentsClass(29, new List<int>() { 0, 1, 2 },
                                        new List<int>() { 10 }, "Эконом",
                                        new List<int>() { 0 });
            SC[30] = new DBStudentsClass(30, new List<int>() { 0, 1 },
                                         new List<int>() { 5 }, "Эконом",
                                         new List<int>() { 1 });
            SC[31] = new DBStudentsClass(31, new List<int>() { 2 },
                                         new List<int>() { 5 }, "Эконом",
                                         new List<int>() { 1 });


            SC[32] = new DBStudentsClass(32, new List<int>() { 0, 1, 2 },
                                        new List<int>() { 25 }, "ОИТ",
                                        new List<int>() { 0 });
            SC[33] = new DBStudentsClass(33, new List<int>() { 0 },
                                         new List<int>() { 25 }, "ОИТ",
                                         new List<int>() { 2 });
            SC[34] = new DBStudentsClass(34, new List<int>() { 1 },
                                         new List<int>() { 13 }, "ОИТ",
                                         new List<int>() { 2 });
            SC[35] = new DBStudentsClass(35, new List<int>() { 2 },
                                         new List<int>() { 13 }, "ОИТ",
                                         new List<int>() { 2 });

            SC[36] = new DBStudentsClass(36, new List<int>() { 0, 1, 2 },
                                        new List<int>() { 3 }, "СВЧ",
                                        new List<int>() { 0 });
            SC[37] = new DBStudentsClass(37, new List<int>() { 0 },
                                         new List<int>() { 3 }, "СВЧ",
                                         new List<int>() { 2 });
            SC[38] = new DBStudentsClass(38, new List<int>() { 1 },
                                         new List<int>() { 3 }, "СВЧ",
                                         new List<int>() { 2 });
            SC[39] = new DBStudentsClass(39, new List<int>() { 2 },
                                         new List<int>() { 3 }, "СВЧ",
                                         new List<int>() { 2 });

            SC[40] = new DBStudentsClass(40, new List<int>() { 0, 1, 2 },
                                       new List<int>() { 3 }, "СВЧ",
                                       new List<int>() { 0 });
            SC[41] = new DBStudentsClass(41, new List<int>() { 0 },
                                         new List<int>() { 3 }, "СВЧ",
                                         new List<int>() { 2 });
            SC[42] = new DBStudentsClass(42, new List<int>() { 1 },
                                         new List<int>() { 3 }, "СВЧ",
                                         new List<int>() { 2 });
            SC[43] = new DBStudentsClass(43, new List<int>() { 2 },
                                         new List<int>() { 3 }, "СВЧ",
                                         new List<int>() { 2 });

            SC[44] = new DBStudentsClass(44, new List<int>() { 0, 1, 2 },
                                      new List<int>() { 8 }, "ЭС",
                                      new List<int>() { 0 });
            SC[134] = new DBStudentsClass(134, new List<int>() { 0, 1, 2 },
                                     new List<int>() { 8 }, "ЭС",
                                     new List<int>() { 0 });
            SC[45] = new DBStudentsClass(45, new List<int>() { 0 },
                                        new List<int>() { 8 }, "ЭС",
                                        new List<int>() { 2 });
            SC[135] = new DBStudentsClass(135, new List<int>() { 0 },
                            new List<int>() { 8 }, "ЭС",
                            new List<int>() { 2 });
            SC[46] = new DBStudentsClass(46, new List<int>() { 1 },
                                        new List<int>() { 12 }, "ЭС",
                                        new List<int>() { 2 });
            SC[138] = new DBStudentsClass(138, new List<int>() { 1 },
                            new List<int>() { 12 }, "ЭС",
                            new List<int>() { 2 });
            SC[47] = new DBStudentsClass(47, new List<int>() { 2 },
                                        new List<int>() { 12 }, "ЭС",
                                        new List<int>() { 2 });
            SC[139] = new DBStudentsClass(139, new List<int>() { 2 },
                            new List<int>() { 12 }, "ЭС",
                            new List<int>() { 2 });

            SC[48] = new DBStudentsClass(48, new List<int>() { 0, 1, 2, 3, 4, 5, 6 },
                                        new List<int>() { }, "ФИЗРА",
                                        new List<int>() { 5 });
            //мпк
            SC[49] = new DBStudentsClass(49, new List<int>() { 6 },
                                         new List<int>() { 19 }, "ПИМАК",
                                         new List<int>() { 0 });
            SC[50] = new DBStudentsClass(50, new List<int>() { 6 },
                                        new List<int>() { 19 }, "ПИМАК",
                                        new List<int>() { 0 });

            SC[51] = new DBStudentsClass(51, new List<int>() { 6 },
                                         new List<int>() { 0 }, "Экономика",
                                         new List<int>() { 0 });
            SC[52] = new DBStudentsClass(52, new List<int>() { 6 },
                                         new List<int>() { 0 }, "Экономика",
                                         new List<int>() { 0 });

            SC[53] = new DBStudentsClass(53, new List<int>() { 6 },
                                        new List<int>() { 0 }, "Экономика",
                                        new List<int>() { 1 });

            SC[54] = new DBStudentsClass(54, new List<int>() { 6 },
                                        new List<int>() { 19 }, "ПИМАК",
                                        new List<int>() { 0 });
            SC[55] = new DBStudentsClass(55, new List<int>() { 6 },
                                        new List<int>() { 19 }, "ПИМАК",
                                        new List<int>() { 0 });
            SC[56] = new DBStudentsClass(56, new List<int>() { 6 },
                                       new List<int>() { 19 }, "ПИМАК",
                                       new List<int>() { 1 });

            SC[57] = new DBStudentsClass(57, new List<int>() { 6 },
                                       new List<int>() { 19 }, "ПИМАК",
                                       new List<int>() { 1 });
            SC[58] = new DBStudentsClass(58, new List<int>() { 6 },
                                       new List<int>() { 19 }, "ПИМАК",
                                       new List<int>() { 1 });
            //
            SC[59] = new DBStudentsClass(59, new List<int>() { 6 },
                                      new List<int>() { 16 }, "ПиМВиОК",
                                      new List<int>() { 0 });

            SC[60] = new DBStudentsClass(60, new List<int>() { 0, 1, 2, 3, 4, 5, 6 },
                                                    new List<int>() { }, "ФИЗРА",
                                                    new List<int>() { 5 });

            SC[61] = new DBStudentsClass(61, new List<int>() { 6 },
                                      new List<int>() { 16 }, "ПиМВиОК",
                                      new List<int>() { 0 });

            SC[62] = new DBStudentsClass(62, new List<int>() { 6 },
                                      new List<int>() { 16 }, "ПиМВиОК",
                                      new List<int>() { 0 });
            SC[63] = new DBStudentsClass(63, new List<int>() { 6 },
                                      new List<int>() { 2 }, "ПиМВиРК",
                                      new List<int>() { 0 });
            SC[64] = new DBStudentsClass(64, new List<int>() { 6 },
                                      new List<int>() { 2 }, "ПиМВиРК",
                                      new List<int>() { 0 });

            SC[65] = new DBStudentsClass(65, new List<int>() { 6 },
                                      new List<int>() { 2 }, "ПиМВиРК",
                                      new List<int>() { 0 });
            SC[66] = new DBStudentsClass(66, new List<int>() { 6 },
                            new List<int>() { 2 }, "ПиМВиРК",
                            new List<int>() { 1 });
            //КОПОО
            SC[67] = new DBStudentsClass(67, new List<int>() { 6 },
                                     new List<int>() { 17 }, "КОПОО",
                                     new List<int>() { 0 });
            SC[68] = new DBStudentsClass(68, new List<int>() { 6 },
                            new List<int>() { 17 }, "КОПОО",
                            new List<int>() { 0 });
            SC[69] = new DBStudentsClass(69, new List<int>() { 6 },
                                     new List<int>() { 17 }, "КОПОО",
                                     new List<int>() { 0 });
            SC[70] = new DBStudentsClass(70, new List<int>() { 6 },
                            new List<int>() { 17 }, "КОПОО",
                            new List<int>() { 0 });
            ///
            SC[71] = new DBStudentsClass(71, new List<int>() { 6 },
                                new List<int>() { 17 }, "УИРС",
                                new List<int>() { 1 });

            SC[72] = new DBStudentsClass(72, new List<int>() { 6 },
                                      new List<int>() { 16 }, "ПиМВиОК",
                                      new List<int>() { 1 });

            SC[73] = new DBStudentsClass(73, new List<int>() { 6 },
                                      new List<int>() { 14 }, "Автоматика",
                                      new List<int>() { 0 });
            SC[74] = new DBStudentsClass(74, new List<int>() { 6 },
                                      new List<int>() { 14 }, "Автоматика",
                                      new List<int>() { 0 });
            SC[75] = new DBStudentsClass(75, new List<int>() { 6 },
                                      new List<int>() { 14 }, "Автоматика",
                                      new List<int>() { 0 });
            SC[76] = new DBStudentsClass(76, new List<int>() { 6 },
                                    new List<int>() { 14 }, "Автоматика",
                                    new List<int>() { 1 });
            SC[77] = new DBStudentsClass(77, new List<int>() { 6 },
                                    new List<int>() { 14 }, "Автоматика",
                                    new List<int>() { 1 });
            /////////////////////////////////////
            //аэп
            SC[78] = new DBStudentsClass(78, new List<int>() { 3, 4, 5 },
                                   new List<int>() { 4 }, "ДЕЛОПРОИЗВОДСТВО ",
                                   new List<int>() { 0 });
            SC[79] = new DBStudentsClass(79, new List<int>() { 3, 4, 5 },
                                   new List<int>() { 4 }, "ДЕЛОПРОИЗВОДСТВО ",
                                   new List<int>() { 0 });
            SC[80] = new DBStudentsClass(80, new List<int>() { 3, 4, 5 },
                                        new List<int>() { 15 }, "ЭАП ",
                                        new List<int>() { 0 });
            SC[81] = new DBStudentsClass(81, new List<int>() { 3, 4, 5 },
                                   new List<int>() { 15 }, "ЭАП ",
                                   new List<int>() { 0 });
            SC[82] = new DBStudentsClass(82, new List<int>() { 3, 4 },
                                 new List<int>() { 15 }, "ЭАЭ ",
                                 new List<int>() { 4 });

            SC[83] = new DBStudentsClass(83, new List<int>() { 3, 4 },
                                  new List<int>() { 15 }, "ЭАЭ ",
                                  new List<int>() { 4 });
            SC[84] = new DBStudentsClass(84, new List<int>() { 3, 4 },
                                  new List<int>() { 21 }, "ТЭП ",
                                  new List<int>() { 2 });
            SC[85] = new DBStudentsClass(85, new List<int>() { 5 },
                                  new List<int>() { 21 }, "ТЭП ",
                                  new List<int>() { 2 });
            ///////////
            SC[86] = new DBStudentsClass(86, new List<int>() { 3, 4, 5 },
                                   new List<int>() { 21 }, "ТЭП ",
                                   new List<int>() { 0 });
            SC[87] = new DBStudentsClass(87, new List<int>() { 3, 4, 5 },
                                 new List<int>() { 21 }, "ТЭП ",
                                 new List<int>() { 0 });

            SC[88] = new DBStudentsClass(88, new List<int>() { 3, 4, 5 },
                                  new List<int>() { 0 }, "Экономика ",
                                  new List<int>() { 0 });
            SC[89] = new DBStudentsClass(89, new List<int>() { 3, 4, 5 },
                                 new List<int>() { 0 }, "Экономика ",
                                 new List<int>() { 0 });

            SC[90] = new DBStudentsClass(90, new List<int>() { 3, 4 },
                                  new List<int>() { 0 }, "Экономика ",
                                  new List<int>() { 1 });
            SC[91] = new DBStudentsClass(91, new List<int>() { 3, 4 },
                                 new List<int>() { 0 }, "Экономика ",
                                 new List<int>() { 1 });
            SC[92] = new DBStudentsClass(92, new List<int>() { 5 },
                                 new List<int>() { 21 }, "ТЭП ",
                                 new List<int>() { 2 });
            SC[93] = new DBStudentsClass(93, new List<int>() { 5 },
                                  new List<int>() { 21 }, "ТЭП ",
                                  new List<int>() { 2 });

            SC[94] = new DBStudentsClass(94, new List<int>() { 3 },
                                new List<int>() { 11 }, "МСАЭ ",
                                new List<int>() { 6 });
            SC[95] = new DBStudentsClass(95, new List<int>() { 3 },
                                 new List<int>() { 11 }, "МСАЭ ",
                                 new List<int>() { 6 });
            SC[96] = new DBStudentsClass(96, new List<int>() { 4 },
                               new List<int>() { 11 }, "МСАЭ ",
                               new List<int>() { 6 });
            SC[97] = new DBStudentsClass(97, new List<int>() { 4 },
                                 new List<int>() { 11 }, "МСАЭ ",
                                 new List<int>() { 6 });
            SC[98] = new DBStudentsClass(98, new List<int>() { 5 },
                                new List<int>() { 0 }, "Экономика ",
                                new List<int>() { 1 });

            SC[99] = new DBStudentsClass(99, new List<int>() { 5 },
                               new List<int>() { 0 }, "Экономика ",
                               new List<int>() { 1 });
            /////////////////
            SC[100] = new DBStudentsClass(100, new List<int>() { 3, 4, 5 },
                                  new List<int>() { 21 }, "ТЭП ",
                                  new List<int>() { 0 });
            SC[101] = new DBStudentsClass(101, new List<int>() { 3, 4, 5 },
                                 new List<int>() { 21 }, "ТЭП ",
                                 new List<int>() { 0 });
            SC[102] = new DBStudentsClass(102, new List<int>() { 3 },
                                new List<int>() { 15 }, "ЭАЭ ",
                                new List<int>() { 4 });
            SC[103] = new DBStudentsClass(103, new List<int>() { 3 },
                                  new List<int>() { 21 }, "ТЭП ",
                                  new List<int>() { 4 });

            SC[104] = new DBStudentsClass(104, new List<int>() { 4 },
                                new List<int>() { 15 }, "ЭАЭ ",
                                new List<int>() { 4 });
            SC[105] = new DBStudentsClass(105, new List<int>() { 4 },
                                  new List<int>() { 21 }, "ТЭП ",
                                  new List<int>() { 4 });

            SC[106] = new DBStudentsClass(106, new List<int>() { 5 },
                                 new List<int>() { 18 }, "СУП ",
                                 new List<int>() { 4 });
            SC[107] = new DBStudentsClass(107, new List<int>() { 5 },
                                new List<int>() { 18 }, "СУП ",
                                new List<int>() { 4 });

            SC[108] = new DBStudentsClass(108, new List<int>() { 3 },
                                 new List<int>() { 18 }, "СУП ",
                                 new List<int>() { 4 });
            SC[109] = new DBStudentsClass(109, new List<int>() { 4 },
                                new List<int>() { 18 }, "СУП ",
                                new List<int>() { 4 });
            SC[110] = new DBStudentsClass(110, new List<int>() { 5 },
                              new List<int>() { 15 }, "ЭАЭ ",
                              new List<int>() { 4 });
            SC[111] = new DBStudentsClass(111, new List<int>() { 3 },
                                new List<int>() { 18 }, "СУП ",
                                new List<int>() { 4 });
            SC[112] = new DBStudentsClass(112, new List<int>() { 4 },
                                new List<int>() { 18 }, "СУП ",
                                new List<int>() { 4 });
            SC[113] = new DBStudentsClass(113, new List<int>() { 3 },
                                  new List<int>() { 21 }, "ТЭП ",
                                  new List<int>() { 4 });

            SC[114] = new DBStudentsClass(114, new List<int>() { 4 },
                                  new List<int>() { 21 }, "ТЭП ",
                                  new List<int>() { 4 });

            SC[115] = new DBStudentsClass(115, new List<int>() { 3 },
                               new List<int>() { 11 }, "МСАЭ ",
                               new List<int>() { 6 });
            ///////////////////
            SC[116] = new DBStudentsClass(116, new List<int>() { 3, 4, 5 },
                               new List<int>() { 18 }, "СУП ",
                               new List<int>() { 0 });
            SC[117] = new DBStudentsClass(117, new List<int>() { 3, 4, 5 },
                                new List<int>() { 18 }, "СУП ",
                                new List<int>() { 0 });
            SC[118] = new DBStudentsClass(118, new List<int>() { 3, 4, 5 },
                              new List<int>() { 18 }, "СУП ",
                              new List<int>() { 0 });
            SC[119] = new DBStudentsClass(119, new List<int>() { 3, 4, 5 },
                                new List<int>() { 18 }, "СУП ",
                                new List<int>() { 0 });//
            SC[120] = new DBStudentsClass(120, new List<int>() { 3, 4, 5 },
                               new List<int>() { 1 }, "ВУЭПТ ",
                               new List<int>() { 0 });
            SC[121] = new DBStudentsClass(121, new List<int>() { 3, 4, 5 },
                              new List<int>() { 1 }, "ОНИИД ",
                              new List<int>() { 0 });
            SC[122] = new DBStudentsClass(122, new List<int>() { 4 },
                            new List<int>() { 23 }, "ОНИИД ",
                            new List<int>() { 4 });
            SC[123] = new DBStudentsClass(123, new List<int>() { 5 },
                           new List<int>() { 23 }, "ОНИИД ",
                           new List<int>() { 4 });

            SC[124] = new DBStudentsClass(124, new List<int>() { 5 },
                              new List<int>() { 1 }, "ВУЭПТ ",
                              new List<int>() { 4 });
            SC[125] = new DBStudentsClass(125, new List<int>() { 5 },
                              new List<int>() { 11 }, "МСАЭ ",
                              new List<int>() { 6 });
            SC[126] = new DBStudentsClass(126, new List<int>() { 4 },
                             new List<int>() { 1 }, "ВУЭПТ ",
                             new List<int>() { 4 });
            SC[127] = new DBStudentsClass(127, new List<int>() { 3 },
                             new List<int>() { 1 }, "ВУЭПТ ",
                             new List<int>() { 4 });
            SC[128] = new DBStudentsClass(128, new List<int>() { 3, 4 },
                             new List<int>() { 11 }, "ИНЯЗ ",
                             new List<int>() { 1 });
            SC[129] = new DBStudentsClass(129, new List<int>() { 5 },
                             new List<int>() { 11 }, "ИНЯЗ ",
                             new List<int>() { 1 });
            SC[130] = new DBStudentsClass(130, new List<int>() { 3, 4 },
                             new List<int>() { 11 }, "ИНЯЗ ",
                             new List<int>() { 1 });
            SC[131] = new DBStudentsClass(131, new List<int>() { 5 },
                             new List<int>() { 11 }, "ИНЯЗ ",
                             new List<int>() { 1 });
            SC[16] = new DBStudentsClass(16, new List<int>() { 5 },
                              new List<int>() { 11 }, "МСАЭ ",
                              new List<int>() { 0 });
            SC[17] = new DBStudentsClass(17, new List<int>() { 5 },
                              new List<int>() { 11 }, "МСАЭ ",
                              new List<int>() { 0 });


            SC[132] = new DBStudentsClass(132, new List<int>() { 0, 1, 2, 3, 4, 5, 6 },
                                                   new List<int>() { }, "ФИЗРА",
                                                   new List<int>() { 5 });
            SC[133] = new DBStudentsClass(133, new List<int>() { 0, 1, 2, 3, 4, 5, 6 },
                                                  new List<int>() { }, "ФИЗРА",
                                                  new List<int>() { 5 });

            return SC.ToList();
        }
    }
}
