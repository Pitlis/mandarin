using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Code
{
    static class FactorsLoader
    {
        public static IEnumerable<FactorSettings> GetFactors()
        {
            List<FactorSettings> Factors = new List<FactorSettings>();
            Assembly asm = Assembly.Load("FactorsWindows");
            EntityStorage storage = CurrentBase.EStorage;
            foreach (var factor in asm.GetTypes())
            {
                if (factor.GetInterface("IFactor") != null)
                {
                    int fine = 0;
                    switch (factor.Name)
                    {
                        case "StudentFourWindows":
                            fine = 12;
                            break;
                        case "StudentsOneWindow":
                            fine = 2;
                            break;
                        case "StudentThreeWindows":
                            fine = 8;
                            break;
                        case "StudentTwoWindows":
                            fine = 4;
                            break;
                        case "TeachersFourWindows":
                            fine = 12;
                            break;
                        case "TeacherssOneWindow":
                            fine = 1;
                            break;
                        case "TeachersThreeWindows":
                            fine = 6;
                            break;
                        case "TeachersTwoWindows":
                            fine = 3;
                            break;
                        default:
                            break;
                    }
                    Factors.Add(new FactorSettings(fine, factor, "FactorsWindows"));
                }
            }
            
            asm = Assembly.Load("OtherFactors");
            foreach (var factor in asm.GetTypes())
            {
                object obj = null;
                if (factor.GetInterface("IFactor") != null)
                {
                    int fine = 0;
                    switch (factor.Name)
                    {
                        case "SixStudentsClasses":
                            fine = 10;
                            break;
                        case "TeacherDayOff":
                            fine = 100;
                            break;
                        case "FiveStudentsClassesInRow":
                            fine = 10;
                            break;
                        case "FiveStudentsClassesInDay":
                            fine = 8;
                            break;
                        case "SixthClass":
                            fine = 8;
                            break;
                        case "SaturdayTwoClasses":
                            fine = 8;
                            break;
                        case "TwoClassesInWeek":
                            fine = 10;
                            obj = OtherFactors.GroupClasses.GetGroupFourSameClasses(storage.Classes);
                            break;
                        case "OnlyOneClassInDay":
                            fine = 100;
                            obj = OtherFactors.GroupClasses.GetGroupSameClasses(storage.Classes);
                            break;
                        case "SameClassesInSameTime":
                            fine = 99;
                            obj = OtherFactors.GroupClasses.GetGroupSameClassesMoreTwoInTwoWeeks(storage.Classes);
                            break;
                        case "SameClassesInSameRoom":
                            fine = 20;
                            obj = OtherFactors.GroupClasses.GetGroupSameClassesMoreTwoInTwoWeeks(storage.Classes);
                            break;
                        case "OneClassInWeek":
                            fine = 99;
                            obj = OtherFactors.GroupClasses.GetGroupTwoSameClasses(storage.Classes);
                            break;
                        case "LectureClassesInDay":
                            fine = 6;
                            obj = OtherFactors.GroupClasses.GetLectureClasses(storage.Classes);
                            break;
                        case "MoreThreeClassesInDay":
                            fine = 4;
                            break;
                        case "SaturdayClass":
                            fine = 4;
                            break;
                        case "TeacherBalanceClasses":
                            fine = 100;
                            break;
                        case "SameLecturesInSameTime":
                            fine = 100;
                            obj = OtherFactors.GroupClasses.GetLecturePairs(storage.Classes);
                            break;
                        case "FifthClass":
                            fine = 8;
                            break;
                        case "ClassInSameTimeOnOtherWeek":
                            fine = 100;
                            obj = OtherFactors.GroupClasses.GetGroupSameClassesMoreTwoInTwoWeeks(storage.Classes);
                            break;
                        case "SameRoomIfClassesInSameTime":
                            fine = 100;
                            obj = OtherFactors.GroupClasses.GetGroupSameClassesMoreTwoInTwoWeeks(storage.Classes);
                            break;
                        case "PairClassesInSameRoom":
                            fine = 100;
                            obj = OtherFactors.GroupClasses.GetGroupSameClassesMoreTwoInTwoWeeks(storage.Classes);
                            break;
                        case "VIPClasses":
                            fine = 0;
                            obj = new List<FixedClasses>();
                            break;
                        case "SaturdayClassOneAtWeek":
                            fine = 5;
                            break;
                        case "FavoriteTeachersClassRooms":
                            fine = 15;
                            obj = new List<FavoriteTeacherClassRooms>();
                            break;
                        default:
                            break;
                    }
                    Factors.Add(new FactorSettings(fine, factor, "OtherFactors", null, obj));
                }
            }
            return Factors;
        }
    }
}