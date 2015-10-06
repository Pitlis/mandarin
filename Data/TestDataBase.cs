using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Model;
using Domain.Services;

namespace Data
{
   public  class TestDataBase
    {
     
       public TestDataBase(ref Teacher[] t)
       {
           t = new Teacher[25];
           t[0] = new Teacher((int)0, "Козлова Л.Г.");
           t[1] = new Teacher(1, "Абабурко В.Н");
           t[2] = new Teacher(2, "Борисов В.И.");
           t[3] = new Teacher(3, "Вайнилович Ю.В.");
           t[4] = new Teacher(4, "Довженко Г.В.");
           t[5] = new Teacher(5, "Другая Л.В.");
           t[6] = new Teacher(6, "Зайченко Е.А.");
           t[7] = new Teacher(7, "Кто Т.О.");
           t[8] = new Teacher(8, "Мисник А.Е.");
           t[9] = new Teacher(9, "МНО Г.О.");
           t[10] = new Teacher(10, "Наркевич Л.В.");
           t[11] = new Teacher(11, "Незнаю");
           t[12] = new Teacher(12, "НеМисник А.Е.");
           t[13] = new Teacher(13, "НеПрудников В.М.");
           t[14] = new Teacher(14, "Обидина О.В");
           t[15] = new Teacher(15, "Парфенович О.Н.");
           t[16] = new Teacher(16, "Поздняков В.Ф");
           t[17] = new Teacher(17, "Прудников А.М.");
           t[18] = new Teacher(18, "Селиванов В.А.");
           t[19] = new Teacher(19, "Сергеев С.С.");
           t[20] = new Teacher(20, "Ситников В.Н.");
           t[21] = new Teacher(21, "Скарыно Б.Б.");
           t[22] = new Teacher(22, "Столяров Ю.Д.");
           t[23] = new Teacher(23, "Черная Н.Г.");
           t[24] = new Teacher(24, "Якимов А.И");
       }
       public TestDataBase(ref StudentSubGroup[] t)
       {
           t = new StudentSubGroup[7];
           t[0] = new StudentSubGroup("АСОИ121", 1);
           t[1] = new StudentSubGroup("АСОИ121", 2);
           t[2] = new StudentSubGroup("АСОИ122", 1);
           t[3] = new StudentSubGroup("АЭП121", 1);
           t[4] = new StudentSubGroup("АЭП121", 2);
           t[5] = new StudentSubGroup("АЭП122", 1);
           t[6] = new StudentSubGroup("МПК121", 1);
       }
       public TestDataBase(ref ClassRoomType[] t)
       {
           t = new ClassRoomType[7];
           t[0] = new ClassRoomType("Лекция");
           t[1] = new ClassRoomType("Практика");
           t[2] = new ClassRoomType("Лабораторная-комп");
           t[3] = new ClassRoomType("Лабораторная-физ");
           t[4] = new ClassRoomType("Лабораторная-эл");
           t[5] = new ClassRoomType("СпортЗал");
           t[6] = new ClassRoomType("Выездная");

       }
       public TestDataBase(ref ClassRoom[] cl,ClassRoomType[] t)
       {
           cl = new ClassRoom[35];
           cl[0] = new ClassRoom(410, 2, new ClassRoomType[] { t[0]});
           cl[1] = new ClassRoom(411, 4, new ClassRoomType[] { t[0]});
           cl[2] = new ClassRoom(409, 2, new ClassRoomType[] { t[0] });
           cl[3] = new ClassRoom(412, 2, new ClassRoomType[] { t[0], t[1] });
           cl[4] = new ClassRoom(213, 2, new ClassRoomType[] { t[0]});
           cl[5] = new ClassRoom(202, 2, new ClassRoomType[] { t[0] });
           cl[6] = new ClassRoom(502, 2, new ClassRoomType[] { t[0] });
           cl[7] = new ClassRoom(111, 2, new ClassRoomType[] { t[0],t[4] });
           cl[8] = new ClassRoom(301, 2, new ClassRoomType[] { t[0]});
           cl[9] = new ClassRoom(416, 2, new ClassRoomType[] { t[2] });
           cl[10] = new ClassRoom(518, 2, new ClassRoomType[] { t[2] });
           cl[11] = new ClassRoom(517, 2, new ClassRoomType[] { t[2] });
           cl[12] = new ClassRoom(439, 1, new ClassRoomType[] { t[2] });
           cl[13] = new ClassRoom(401, 3, new ClassRoomType[] { t[2] });//
           cl[14] = new ClassRoom(203, 1, new ClassRoomType[] { t[2] });
           cl[15] = new ClassRoom(231, 1, new ClassRoomType[] { t[2] });
           cl[16] = new ClassRoom(516, 2, new ClassRoomType[] { t[2] });
           cl[17] = new ClassRoom(511, 2, new ClassRoomType[] { t[3] });
           cl[18] = new ClassRoom(514, 2, new ClassRoomType[] { t[3],t[1] });
           cl[19] = new ClassRoom(311, 4, new ClassRoomType[] { t[3]});
           cl[20] = new ClassRoom(405, 2, new ClassRoomType[] { t[1] });
           cl[21] = new ClassRoom(404, 2, new ClassRoomType[] { t[3], t[4] });
           cl[22] = new ClassRoom(207, 2, new ClassRoomType[] { t[4] });
           cl[23] = new ClassRoom(204, 2, new ClassRoomType[] {t[1] , t[4] });
           cl[24] = new ClassRoom(402, 2, new ClassRoomType[] { t[4] });
           cl[25] = new ClassRoom(419, 1, new ClassRoomType[] { t[1] });
           cl[26] = new ClassRoom(216, 2, new ClassRoomType[] { t[4] });
           cl[27] = new ClassRoom(0, 1, new ClassRoomType[] { t[5] });
           cl[28] = new ClassRoom(1, 1, new ClassRoomType[] { t[6] });
           cl[29] = new ClassRoom(254, 1, new ClassRoomType[] { t[1] });
           cl[30] = new ClassRoom(401,4, new ClassRoomType[] { t[1] });//
           cl[31] = new ClassRoom(512, 2, new ClassRoomType[] { t[1] });
           cl[32] = new ClassRoom(527, 1, new ClassRoomType[] { t[1] });
           cl[33] = new ClassRoom(506, 2, new ClassRoomType[] { t[1] });
           cl[34] = new ClassRoom(316, 2, new ClassRoomType[] { t[1] });
          
         
          
          
       }
       public TestDataBase(ref StudentsClass[] SC, EntityStorage ES)
       {
           SC = new StudentsClass[49];
           SC[0] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0], ES.StudentSubGroups[1], ES.StudentSubGroups[2] },
                                        new Teacher[] { ES.Teachers[24] }, "ММИПУ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[0] });
           SC[1] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0] },
                                        new Teacher[] { ES.Teachers[24] }, "ММИПУ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[2] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[1]},
                                        new Teacher[] { ES.Teachers[24] }, "ММИПУ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[3] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[2]},
                                        new Teacher[] { ES.Teachers[24] }, "ММИПУ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });

           SC[4] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0], ES.StudentSubGroups[1], ES.StudentSubGroups[2] },
                                        new Teacher[] { ES.Teachers[24] }, "ММИПУ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[0] });
           SC[5] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0] },
                                        new Teacher[] { ES.Teachers[24] }, "ММИПУ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[6] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[1] },
                                        new Teacher[] { ES.Teachers[24] }, "ММИПУ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[7] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[2] },
                                        new Teacher[] { ES.Teachers[24] }, "ММИПУ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });

           SC[8] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0], ES.StudentSubGroups[1], ES.StudentSubGroups[2] },
                                        new Teacher[] { ES.Teachers[6] }, "СПО",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[0] });
           SC[9] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0] },
                                        new Teacher[] { ES.Teachers[6] }, "СПО",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[10] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[1] },
                                        new Teacher[] { ES.Teachers[6] }, "СПО",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[11] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[2] },
                                        new Teacher[] { ES.Teachers[6] }, "СПО",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });


           SC[12] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0], ES.StudentSubGroups[1], ES.StudentSubGroups[2] },
                                        new Teacher[] { ES.Teachers[6] }, "СПО",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[0] });
           SC[13] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0] },
                                        new Teacher[] { ES.Teachers[6] }, "СПО",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[14] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[1] },
                                        new Teacher[] { ES.Teachers[6] }, "СПО",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[15] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[2] },
                                        new Teacher[] { ES.Teachers[6] }, "СПО",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });


           SC[16] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0], ES.StudentSubGroups[1], ES.StudentSubGroups[2] },
                                       new Teacher[] { ES.Teachers[6] }, "СПО",
                                       new ClassRoomType[] { ES.ClassRoomsTypes[0] });
           SC[17] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0], ES.StudentSubGroups[1], ES.StudentSubGroups[2] },
                                       new Teacher[] { ES.Teachers[6] }, "СПО",
                                       new ClassRoomType[] { ES.ClassRoomsTypes[0] });



           SC[18] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0], ES.StudentSubGroups[1], ES.StudentSubGroups[2] },
                                       new Teacher[] { ES.Teachers[22] }, "АЭВМ",
                                       new ClassRoomType[] { ES.ClassRoomsTypes[0] });
           SC[19] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0] },
                                        new Teacher[] { ES.Teachers[22] }, "АЭВМ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[20] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[1] },
                                        new Teacher[] { ES.Teachers[22] }, "АЭВМ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[21] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[2] },
                                        new Teacher[] { ES.Teachers[22] }, "АЭВМ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });

           SC[22] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0], ES.StudentSubGroups[1], ES.StudentSubGroups[2] },
                                       new Teacher[] { ES.Teachers[22] }, "АЭВМ",
                                       new ClassRoomType[] { ES.ClassRoomsTypes[0] });
           SC[23] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0] },
                                        new Teacher[] { ES.Teachers[22] }, "АЭВМ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[24] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[1] },
                                        new Teacher[] { ES.Teachers[22] }, "АЭВМ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[25] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[2] },
                                        new Teacher[] { ES.Teachers[22] }, "АЭВМ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });



           SC[26] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0], ES.StudentSubGroups[1], ES.StudentSubGroups[2] },
                                       new Teacher[] { ES.Teachers[10] }, "Эконом",
                                       new ClassRoomType[] { ES.ClassRoomsTypes[0] });
           SC[27] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0], ES.StudentSubGroups[1] },
                                        new Teacher[] { ES.Teachers[5] }, "Эконом",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[1] });
           SC[28] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[2] },
                                        new Teacher[] { ES.Teachers[5] }, "Эконом",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[1] });

           SC[29] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0], ES.StudentSubGroups[1], ES.StudentSubGroups[2] },
                                       new Teacher[] { ES.Teachers[10] }, "Эконом",
                                       new ClassRoomType[] { ES.ClassRoomsTypes[0] });
           SC[30] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0], ES.StudentSubGroups[1] },
                                        new Teacher[] { ES.Teachers[5] }, "Эконом",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[1] });
           SC[31] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[2] },
                                        new Teacher[] { ES.Teachers[5] }, "Эконом",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[1] });


           SC[32] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0], ES.StudentSubGroups[1], ES.StudentSubGroups[2] },
                                       new Teacher[] { ES.Teachers[17] }, "ОИТ",
                                       new ClassRoomType[] { ES.ClassRoomsTypes[0] });
           SC[33] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0] },
                                        new Teacher[] { ES.Teachers[17] }, "ОИТ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[34] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[1] },
                                        new Teacher[] { ES.Teachers[13] }, "ОИТ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[35] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[2] },
                                        new Teacher[] { ES.Teachers[13] }, "ОИТ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });

           SC[36] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0], ES.StudentSubGroups[1], ES.StudentSubGroups[2] },
                                       new Teacher[] { ES.Teachers[3] }, "СВЧ",
                                       new ClassRoomType[] { ES.ClassRoomsTypes[0] });
           SC[37] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0] },
                                        new Teacher[] { ES.Teachers[3] }, "СВЧ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[38] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[1] },
                                        new Teacher[] { ES.Teachers[3] }, "СВЧ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[39] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[2] },
                                        new Teacher[] { ES.Teachers[3] }, "СВЧ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });

           SC[40] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0], ES.StudentSubGroups[1], ES.StudentSubGroups[2] },
                                      new Teacher[] { ES.Teachers[3] }, "СВЧ",
                                      new ClassRoomType[] { ES.ClassRoomsTypes[0] });
           SC[41] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0] },
                                        new Teacher[] { ES.Teachers[3] }, "СВЧ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[42] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[1] },
                                        new Teacher[] { ES.Teachers[3] }, "СВЧ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[43] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[2] },
                                        new Teacher[] { ES.Teachers[3] }, "СВЧ",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });

           SC[44] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0], ES.StudentSubGroups[1], ES.StudentSubGroups[2] },
                                     new Teacher[] { ES.Teachers[8] }, "ЭС",
                                     new ClassRoomType[] { ES.ClassRoomsTypes[0] });
           SC[45] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0] },
                                        new Teacher[] { ES.Teachers[8] }, "ЭС",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[46] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[1] },
                                        new Teacher[] { ES.Teachers[12] }, "ЭС",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });
           SC[47] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[2] },
                                        new Teacher[] { ES.Teachers[12] }, "ЭС",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[2] });

           SC[48] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[0], ES.StudentSubGroups[1], ES.StudentSubGroups[2], ES.StudentSubGroups[3], ES.StudentSubGroups[4], ES.StudentSubGroups[5], ES.StudentSubGroups[6] },
                                        new Teacher[] { ES.Teachers[12] }, "ФИЗРА",
                                        new ClassRoomType[] { ES.ClassRoomsTypes[5] });
           //мпк
           //SC[49] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[6] },
           //                             new Teacher[] { ES.Teachers[19] }, "ПИМАК",
           //                             new ClassRoomType[] { ES.ClassRoomsTypes[0] });

           //SC[49] = new StudentsClass(new StudentSubGroup[] { ES.StudentSubGroups[6] },
           //                             new Teacher[] { ES.Teachers[0] }, "Экономика",
           //                             new ClassRoomType[] { ES.ClassRoomsTypes[0] });

      
       }

    }
}
