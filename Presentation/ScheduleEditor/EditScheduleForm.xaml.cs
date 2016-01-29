using Domain.Model;
using Presentation.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using Domain.Services;
using Presentation.FacultyEditor;
using Domain.DataFiles;
using Presentation.Controls;
using System.Windows.Controls.Primitives;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System.Threading;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for EditSchedule.xaml
    /// </summary>
    public partial class EditSchedule : UserControl
    {
        ScheduleForEdit schedule;
        String scheduleName;
        FacultiesAndGroups facultiesAndGroups;
        bool[] finePosition;
        Label[] timeLabels;
        string filepath;
        Thread thread;
        private int ColForRemove = 0;
        private int RowForRemove = 0;
        private int TimeRows = -1;

        public EditSchedule()
        {
            schedule = new ScheduleForEdit(CurrentSchedule.Schedule.Value);
            scheduleName = CurrentSchedule.Schedule.Key;
            InitializeComponent();
        }

        #region Events        
        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            CreateTimeTable();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillFacultyAndCoursCombobox();
        }     
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            CreateExcelDocument();
        }
        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            RemoveClasses();
        }
        private void RemovelistBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectRemoveClass();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CheckRemoveClasses(e);           
        }
        private void InfoTeachers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InfoTeachersListbox.SelectedIndex = -1;
        }
        private void InfoGroop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InfoGrouplistView.SelectedIndex = -1;
        }
        private void listViewClassRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClassRoomlistView.SelectedIndex = -1;
        }
        #endregion

        #region Method
        private async void LoadFacultyAndGroups()
        {
            if (facultiesAndGroups.GroupsWithoutFacultyExists())
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Внимание есть группы не относящееся ни к 1 факультету.\n" +
                                        "Зайдите в настройки чтобы отнести группы к факультету!" }
                };
                await DialogHost.Show(infoWindow, "MandarinHost");
            }
        }
        private void CreateTimeTable()
        {
            gdData.Children.Clear();
            btnExcel.IsEnabled = true;
            gdData.ColumnDefinitions.Clear();
            gdData.RowDefinitions.Clear();

            gdData.ColumnDefinitions.Add(CreateDayHeaderColumn());
            gdData.ColumnDefinitions.Add(CreateTimeHeaderColumn());

            //+1 - заголовок с названиями групп
            for (int rowIndex = 0; rowIndex < schedule.partSchedule.GetLength(0) + 1; rowIndex++)
            {
                gdData.RowDefinitions.Add(CreateRow());
            }
            for (int colIndex = 0; colIndex < schedule.partSchedule.GetLength(1); colIndex++)
            {
                gdData.ColumnDefinitions.Add(CreateColumn());
            }
            CreateGroupHeader(schedule.Groups);
            CreateDayHeader();
            CreateTimeHeader(TimeSelect, DoubleClick);
            for (int rowIndex = ROW_HEADER; rowIndex < schedule.partSchedule.GetLength(0) + ROW_HEADER; rowIndex++)
            {
                Label prevLabel = null;
                for (int colIndex = COLUMN_HEADER; colIndex < schedule.partSchedule.GetLength(1) + COLUMN_HEADER; colIndex++)
                {
                    if (colIndex > COLUMN_HEADER && schedule.partSchedule[rowIndex - ROW_HEADER, colIndex - COLUMN_HEADER] != null && schedule.partSchedule[rowIndex - ROW_HEADER, colIndex - COLUMN_HEADER] == schedule.partSchedule[rowIndex - ROW_HEADER, colIndex - COLUMN_HEADER - 1])
                    {
                        MergeCells(prevLabel, colIndex);
                    }
                    else
                    {
                        prevLabel = CreateLabel(rowIndex, colIndex, schedule.partSchedule[rowIndex - ROW_HEADER, colIndex - COLUMN_HEADER], schedule.GetClassRoom(schedule.partSchedule[rowIndex - ROW_HEADER, colIndex - COLUMN_HEADER]), SelectCell, ShowCellValue);
                        gdData.Children.Add(prevLabel);
                    }
                }
            }
        }
        private void FillFacultyAndCoursCombobox()
        {
            if (CurrentBase.BaseIsLoaded() && !CurrentSchedule.ScheduleIsFromFile())
            {
                facultiesAndGroups = new FacultiesAndGroups();
                foreach (Faculty faculty in facultiesAndGroups.Faculties)
                {
                    facultComboBox.Items.Add(faculty.Name);
                }
                facultComboBox.SelectedIndex = 0;
                for (int i = 1; i <= 5; i++)
                {
                    coursComboBox.Items.Add(i);
                }
                coursComboBox.SelectedIndex = 0;
                LoadFacultyAndGroups();
            }
        }
        private async void CreateExcelDocument()
        {
            if (schedule.RemoveClases.Count == 0)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel (*.xlsx|*.xlsx";
                saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;
                if (saveFileDialog.ShowDialog() == true)//сделать формирование в отдельном потоке
                {
                    filepath = saveFileDialog.FileName;
                    thread = new Thread(new ThreadStart(SaveExcel));
                    thread.Start();
                    var infoWindow = new InfoWindow
                    {
                        Message = { Text = "Расписание будет сформировано:\n" + filepath + "\nПожалуйста подождите" }
                    };
                    await DialogHost.Show(infoWindow, "MandarinHost");
                    btnExcel.IsEnabled = false;


                }
            }
            else
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Nope" }
                };

                await DialogHost.Show(infoWindow, "MandarinHost");
            }
        }

        private void SaveExcel()
        {
            Code.ScheduleExcel excel = new Code.ScheduleExcel(filepath, schedule, schedule.EStorage);
            excel.LoadPartScheduleExcel(schedule.Groups);
            this.Dispatcher.Invoke(new Action(async delegate ()
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Расписание сформировано:\n" + filepath }
                };
                await DialogHost.Show(infoWindow, "MandarinHost");
                btnExcel.IsEnabled = true;

            }));
        }

        private void RemoveClasses()
        {
            StudentsClass sClass;
            schedule.RemoveClases.Add(schedule.partSchedule[RowForRemove - ROW_HEADER, ColForRemove - COLUMN_HEADER]);
            RemoveClasseslistBox.ItemsSource = null;
            RemoveClasseslistBox.ItemsSource = schedule.RemoveClases;
            sClass = schedule.partSchedule[RowForRemove - ROW_HEADER, ColForRemove - COLUMN_HEADER];
            schedule.RemoveFromClasses(sClass, RowForRemove - ROW_HEADER);
            for (int colIndex = 0; colIndex < schedule.partSchedule.GetLength(1); colIndex++)
            {
                if (schedule.partSchedule[RowForRemove - ROW_HEADER, colIndex] == sClass)
                {
                    schedule.partSchedule[RowForRemove - ROW_HEADER, colIndex] = null;
                }
            }
            btnShow_Click(Type.Missing, null);
            btnRemove.IsEnabled = false;
            infoClassTextbox.Text = "";
            InfoTeachersListbox.Items.Clear();
            InfoGrouplistView.Items.Clear();
        }
        private async void CheckRemoveClasses(System.ComponentModel.CancelEventArgs e)
        {
            if (schedule.RemoveClases.Count != 0)
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Остались непоставленные пары" }
                };

                await DialogHost.Show(infoWindow, "MandarinHost");
                e.Cancel = true;
            }
        }
        public async void SetClasses()
        {
            if (schedule.GetCrossClasses((ClassRoom)ClassRoomlistView.Items.GetItemAt(0), (StudentsClass)RemoveClasseslistBox.SelectedItem, TimeRows).Count != 0)
            {
                var dialogWindow = new DialogWindow
                {
                    Message = { Text = "Есть накладки в расписании. Хотите снять пересекающееся пары?" }
                };

                object result = await DialogHost.Show(dialogWindow, "MandarinHost");
                if ((bool)result == true)
                {
                    schedule.RemoveCrossClasses(schedule.GetCrossClasses((ClassRoom)ClassRoomlistView.Items.GetItemAt(0), (StudentsClass)RemoveClasseslistBox.SelectedItem, TimeRows), TimeRows);
                    schedule.SetClass((ClassRoom)ClassRoomlistView.Items.GetItemAt(0), (StudentsClass)RemoveClasseslistBox.SelectedItem, TimeRows);
                }
            }
            else
            {
                schedule.SetClass((ClassRoom)ClassRoomlistView.Items.GetItemAt(0), (StudentsClass)RemoveClasseslistBox.SelectedItem, TimeRows);
            }
            RemoveClasseslistBox.ItemsSource = null;
            RemoveClasseslistBox.ItemsSource = schedule.RemoveClases;
            btnShow_Click(Type.Missing, null);
        }
        private void SelectRemoveClass()
        {
            int classesInSchedule = Constants.WEEKS_IN_SCHEDULE * Constants.DAYS_IN_WEEK * Constants.CLASSES_IN_DAY;
            if (RemoveClasseslistBox.SelectedIndex != -1)
            {
                StudentsClass sClass = (StudentsClass)RemoveClasseslistBox.SelectedItem;
                FinePositionForClass(classesInSchedule, sClass);
                FillInformationAboutClass(sClass);
            }
            AvailableButtonAndColor(classesInSchedule);

        }
        private void AvailableButtonAndColor(int classesInSchedule)
        {
            if (RemoveClasseslistBox.SelectedIndex != -1)
            {
                btnRemove.IsEnabled = false;

                for (int i = 0; i < classesInSchedule; i++)
                {
                    if (finePosition[i]) { timeLabels[i].Background = new SolidColorBrush(Colors.Green); }
                    else { timeLabels[i].Background = new SolidColorBrush(Colors.White); }
                }


            }
            if (SelectedCell != null && RemoveClasseslistBox.SelectedIndex != -1)
            {
                SelectedCell.BorderBrush = new SolidColorBrush(Colors.Red);
                SelectedCell.BorderThickness = new Thickness(1);
                SelectedCell = null;
            }

            ClassRoomlistView.Items.Clear();
            if (RemoveClasseslistBox.SelectedIndex == -1)
            {
                for (int i = 0; i < classesInSchedule; i++)
                {
                    if (finePosition[i]) { timeLabels[i].Background = new SolidColorBrush(Colors.White); }
                }
            }
        }
        private void FillInformationAboutClass(StudentsClass sClass)
        {
            infoClassTextbox.Text = sClass.Name;
            InfoTeachersListbox.Items.Clear();
            InfoGrouplistView.Items.Clear();
            foreach (Teacher tecah in sClass.Teacher)
            {
                InfoTeachersListbox.Items.Add(tecah);
            }
            foreach (StudentSubGroup groop in sClass.SubGroups)
            {
                InfoGrouplistView.Items.Add(groop);
            }
        }
        private void FinePositionForClass(int classesInSchedule, StudentsClass sClass)
        {
            finePosition = new bool[classesInSchedule];
            finePosition = schedule.GetFinePosition(sClass);
        }
        #endregion

        #region Create Schedule

        const int CELL_WIDTH = 100;
        const int CELL_HEIGHT = 30;

        const int ROW_HEADER = 1;
        const int COLUMN_HEADER = 2;


        RowDefinition CreateRow()
        {
            RowDefinition r = new RowDefinition();
            r.Height = new GridLength(CELL_HEIGHT);
            r.MaxHeight = CELL_HEIGHT;
            r.MinHeight = CELL_HEIGHT;
            return r;
        }

        ColumnDefinition CreateColumn()
        {
            ColumnDefinition c = new ColumnDefinition();
            c.Width = new GridLength(CELL_WIDTH);
            c.MaxWidth = CELL_WIDTH;
            c.MinWidth = CELL_WIDTH;
            return c;
        }

        Label CreateLabel(int row, int column, StudentsClass sClass, ClassRoom room, MouseButtonEventHandler mLeft, MouseButtonEventHandler mRight)
        {
            Label l = new Label();
            if (sClass != null)
            {
                l.Background = Brushes.LightGray;
            }
            else
            {
                l.Background = Brushes.White;
            }
            l.Height = CELL_HEIGHT;
            l.Width = CELL_WIDTH;
            l.BorderBrush = new SolidColorBrush(Colors.Red);
            l.BorderThickness = new Thickness(1);
            l.SetValue(Grid.RowProperty, row);
            l.SetValue(Grid.ColumnProperty, column);
            if (sClass != null)
                l.Content = sClass.Name + "(" + room.Housing + "/" + room.Number + ")";
            l.HorizontalContentAlignment = HorizontalAlignment.Center;
            l.MouseLeftButtonDown += mLeft;
            l.MouseRightButtonDown += mRight;
            l.FontSize = 8;
            return l;
        }

        void MergeCells(Label prevLabel, int colIndex)
        {
            int colIndexPrevLabel = (int)prevLabel.GetValue(Grid.ColumnProperty);
            int mergeCount = colIndex - colIndexPrevLabel + 1;

            prevLabel.SetValue(Grid.ColumnSpanProperty, mergeCount);
            prevLabel.Width = CELL_WIDTH * mergeCount;
        }

        private async void ShowCellValue(object sender, MouseButtonEventArgs e)
        {
            Label cell = (Label)sender;
            int col = (int)cell.GetValue(Grid.ColumnProperty);
            int row = (int)cell.GetValue(Grid.RowProperty);
            if (schedule.partSchedule[row - ROW_HEADER, col - COLUMN_HEADER] != null)
            {
                ClassRoom room = schedule.GetClassRoom(schedule.partSchedule[row - ROW_HEADER, col - COLUMN_HEADER]);
                string teacher = schedule.partSchedule[row - ROW_HEADER, col - COLUMN_HEADER].Teacher.Length > 0 ?
                    schedule.partSchedule[row - ROW_HEADER, col - COLUMN_HEADER].Teacher[0].Name :
                    "";
                var infoWindow = new InfoWindow
                {
                    Message = { Text = String.Format("Пара: {0}\nАудитория: {1} (корпус {2})\nПреподаватель: {3}",
                                        schedule.partSchedule[row - ROW_HEADER, col - COLUMN_HEADER].Name,
                                        room.Number, room.Housing,
                                        teacher) }
                };

                await DialogHost.Show(infoWindow, "MandarinHost");
            }
            else
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Пусто" }
                };

                await DialogHost.Show(infoWindow, "MandarinHost");
            }
        }

        Label SelectedCell;
        private void SelectCell(object sender, MouseButtonEventArgs e)
        {
            Label cell = (Label)sender;
            int col = (int)cell.GetValue(Grid.ColumnProperty);
            int row = (int)cell.GetValue(Grid.RowProperty);
            if (SelectedCell != null)
            {
                SelectedCell.BorderBrush = new SolidColorBrush(Colors.Red);
                SelectedCell.BorderThickness = new Thickness(1);
                infoClassTextbox.Text = "";
                InfoTeachersListbox.Items.Clear();
                InfoGrouplistView.Items.Clear();
                RemoveClasseslistBox.SelectedIndex = -1;
                //btnClass.IsEnabled = false;
            }
            if (SelectedCell == cell)
            {
                SelectedCell = null;
                infoClassTextbox.Text = "";
                InfoTeachersListbox.Items.Clear();
                InfoGrouplistView.Items.Clear();
                btnRemove.IsEnabled = false;
                RemoveClasseslistBox.SelectedIndex = -1;
                //btnClass.IsEnabled = false;
            }
            else
            {
                SelectedCell = cell;
                SelectedCell.BorderBrush = new SolidColorBrush(Colors.Blue);
                SelectedCell.BorderThickness = new Thickness(2);
                if (schedule.partSchedule[row - ROW_HEADER, col - COLUMN_HEADER] != null)
                {
                    btnRemove.IsEnabled = true; RowForRemove = row; ColForRemove = col;
                    RemoveClasseslistBox.SelectedIndex = -1;
                    StudentsClass clas = schedule.partSchedule[row - ROW_HEADER, col - COLUMN_HEADER];
                    infoClassTextbox.Text = clas.Name;
                    InfoTeachersListbox.Items.Clear();
                    InfoGrouplistView.Items.Clear();
                    foreach (Teacher tecah in clas.Teacher)
                    {
                        InfoTeachersListbox.Items.Add(tecah);
                    }
                    foreach (StudentSubGroup groop in clas.SubGroups)
                    {
                        InfoGrouplistView.Items.Add(groop);
                    }
                    //btnClass.IsEnabled = false;

                }
                else
                {
                    btnRemove.IsEnabled = false;
                    //btnClass.IsEnabled = false;
                    infoClassTextbox.Text = "";
                    InfoTeachersListbox.Items.Clear();
                    InfoGrouplistView.Items.Clear();
                    SelectedCell = cell;
                    SelectedCell.BorderBrush = new SolidColorBrush(Colors.Blue);
                    SelectedCell.BorderThickness = new Thickness(2);
                    RemoveClasseslistBox.SelectedIndex = -1;
                }
            }
        }

        ColumnDefinition CreateDayHeaderColumn()
        {
            ColumnDefinition c = new ColumnDefinition();
            c.Width = new GridLength(30);
            c.MaxWidth = 30;
            c.MinWidth = 30;
            return c;
        }
        ColumnDefinition CreateTimeHeaderColumn()
        {
            ColumnDefinition c = new ColumnDefinition();
            c.Width = new GridLength(100);
            c.MaxWidth = 100;
            c.MinWidth = 100;
            return c;
        }

        void CreateGroupHeader(StudentSubGroup[] groups)
        {
            int columnIndex = COLUMN_HEADER;
            foreach (StudentSubGroup group in groups)
            {
                Label groupLabel = new Label();

                groupLabel.Content = group.NameGroup + "/" + group.NumberSubGroup;
                groupLabel.Height = CELL_HEIGHT;
                groupLabel.Width = CELL_WIDTH;
                groupLabel.BorderBrush = new SolidColorBrush(Colors.Red);
                groupLabel.BorderThickness = new Thickness(1);
                groupLabel.SetValue(Grid.RowProperty, ROW_HEADER - 1);
                groupLabel.SetValue(Grid.ColumnProperty, columnIndex);
                groupLabel.FontSize = 12;
                groupLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                columnIndex++;
                gdData.Children.Add(groupLabel);
            }
        }
        void CreateDayHeader()
        {
            int rowIndex = ROW_HEADER;
            string[] days = new string[] { "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
            foreach (string day in days)
            {
                Label groupLabel = new Label();
                groupLabel.LayoutTransform = new RotateTransform(270);
                groupLabel.Content = day;
                groupLabel.Height = 30;
                groupLabel.Width = CELL_HEIGHT * 6;
                groupLabel.BorderBrush = new SolidColorBrush(Colors.Green);
                groupLabel.BorderThickness = new Thickness(1);
                groupLabel.SetValue(Grid.RowProperty, rowIndex);
                groupLabel.SetValue(Grid.ColumnProperty, 0);
                groupLabel.SetValue(Grid.RowSpanProperty, 6);
                groupLabel.FontSize = 14;
                groupLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                gdData.Children.Add(groupLabel);
                rowIndex += 6;
            }
        }
        void CreateTimeHeader(MouseButtonEventHandler mLeft, MouseButtonEventHandler MouseDoubleClick)
        {
            string[] times = new string[] { "8.30-10.05", "10.25-12.00", "12.20-13.55", "14.15-15.50", "16.00-17.35", "17.45-19.20" };

            int timeStringIndex = 0;
            timeLabels = new Label[72];
            for (int timeIndex = 0; timeIndex < 72; timeIndex++)
            {

                Label groupLabel = new Label();
                groupLabel.Content = times[timeStringIndex];
                groupLabel.Height = CELL_HEIGHT;
                groupLabel.Width = 100;
                groupLabel.BorderBrush = new SolidColorBrush(Colors.Green);
                groupLabel.BorderThickness = new Thickness(1);
                groupLabel.SetValue(Grid.RowProperty, timeIndex + ROW_HEADER);
                groupLabel.SetValue(Grid.ColumnProperty, 1);
                groupLabel.FontSize = 10;
                groupLabel.HorizontalContentAlignment = HorizontalAlignment.Center;
                gdData.Children.Add(groupLabel);
                timeLabels[timeIndex] = groupLabel;
                timeStringIndex++;
                groupLabel.MouseLeftButtonDown += mLeft;
                groupLabel.MouseDoubleClick += MouseDoubleClick;
                if (timeStringIndex == 6)
                {
                    timeStringIndex = 0;
                }
            }
        }
        Label SelectedTimeCell;
        private void TimeSelect(object sender, MouseButtonEventArgs e)
        {
            Label cell = (Label)sender;
            int col = (int)cell.GetValue(Grid.ColumnProperty);
            int row = (int)cell.GetValue(Grid.RowProperty);
            if (SelectedTimeCell != null)
            {
                SelectedTimeCell.BorderBrush = new SolidColorBrush(Colors.Green);
                SelectedTimeCell.BorderThickness = new Thickness(1);
                if (SelectedTimeCell.Content != null)
                {
                    TimeTextBox.Text = SelectedTimeCell.Content.ToString();
                    TimeRows = row - 1;
                }

            }
            if (SelectedTimeCell == cell)
            {
                SelectedTimeCell = null;
                TimeTextBox.Text = "";
                TimeRows = -1;
            }
            else
            {
                SelectedTimeCell = cell;
                SelectedTimeCell.BorderBrush = new SolidColorBrush(Colors.Yellow);
                SelectedTimeCell.BorderThickness = new Thickness(2);
                if (SelectedTimeCell.Content != null)
                {
                    TimeTextBox.Text = SelectedTimeCell.Content.ToString();
                    TimeRows = row - 1;
                }
            }
        }

        private void DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Label cell = (Label)sender;
            TimeRows = (int)cell.GetValue(Grid.RowProperty) - 1;
            if (RemoveClasseslistBox.SelectedIndex != -1)
            {
                StudentsClass clas;
                clas = (StudentsClass)RemoveClasseslistBox.SelectedItem;
                ChooseClassRoom form = new ChooseClassRoom(TimeRows, schedule, clas);
                //form.Owner = this;
                form.ShowDialog();
                if (form.DialogResult == true)
                {
                    ClassRoomlistView.Items.Clear();
                    ClassRoomlistView.Items.Add(form.classRoom);
                    SetClasses();
                    CurrentSchedule.LoadSchedule(new KeyValuePair<string, Schedule>(scheduleName, schedule.GetCurrentSchedule()));
                }
            }
        }
        #endregion
    }
}
