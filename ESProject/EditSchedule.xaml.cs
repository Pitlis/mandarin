using Domain.Model;
using Presentation.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for EditSchedule.xaml
    /// </summary>
    public partial class EditSchedule : Window
    {
        ScheduleForEdit schedule;

        public EditSchedule(ScheduleForEdit s)
        {
            schedule = s;
            InitializeComponent();
        }

        private void btnShow_Click(object sender, RoutedEventArgs e)
        {

            
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
            CreateTimeHeader(TimeSelect);
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

        private void ShowCellValue(object sender, MouseButtonEventArgs e)
        {
            Label cell = (Label)sender;
            int col = (int)cell.GetValue(Grid.ColumnProperty);
            int row = (int)cell.GetValue(Grid.RowProperty);
            if (schedule.partSchedule[row - ROW_HEADER, col - COLUMN_HEADER] != null)
            {
                ClassRoom room = schedule.GetClassRoom(schedule.partSchedule[row - ROW_HEADER, col - COLUMN_HEADER]);
                string teacher = schedule.partSchedule[row - ROW_HEADER, col - COLUMN_HEADER].Teacher.Length > 0 ?
                    schedule.partSchedule[row - ROW_HEADER, col - COLUMN_HEADER].Teacher[0].FLSName :
                    "";
                MessageBox.Show(String.Format("Пара: {0}\nАудитория: {1} (корпус {2})\nПреподаватель: {3}",
                    schedule.partSchedule[row - ROW_HEADER, col - COLUMN_HEADER].Name,
                    room.Number, room.Housing,
                    teacher));
            }
            else
            {
                MessageBox.Show("Пусто");
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
                InfoClass.Text = "";
                InfoTeachers.Items.Clear();
                InfoGroop.Items.Clear();
                RemovelistBox.SelectedIndex = -1;
                btnSet.IsEnabled = false;
            }
            if (SelectedCell == cell)
            {
                SelectedCell = null;
                InfoClass.Text = "";
                InfoTeachers.Items.Clear();
                InfoGroop.Items.Clear();
                btnRemove.IsEnabled = false;
                RemovelistBox.SelectedIndex = -1;
                btnSet.IsEnabled = false;
            }
            else
            {
                SelectedCell = cell;
                SelectedCell.BorderBrush = new SolidColorBrush(Colors.Blue);
                SelectedCell.BorderThickness = new Thickness(2);
                if (schedule.partSchedule[row - ROW_HEADER, col - COLUMN_HEADER] != null)
                {
                    btnRemove.IsEnabled = true; RowForRemove = row; ColForRemove = col;
                    RemovelistBox.SelectedIndex = -1;
                    StudentsClass clas = schedule.partSchedule[row - ROW_HEADER, col - COLUMN_HEADER];
                    InfoClass.Text = clas.Name;
                    InfoTeachers.Items.Clear();
                    InfoGroop.Items.Clear();
                    btnSet.IsEnabled = false;
                    foreach (Teacher tecah in clas.Teacher)
                    {
                        InfoTeachers.Items.Add(tecah);
                    }
                    foreach (StudentSubGroup groop in clas.SubGroups)
                    {
                        InfoGroop.Items.Add(groop);
                    }
                    btnClass.IsEnabled = false;

                }
                else
                {
                    btnRemove.IsEnabled = false;
                    btnSet.IsEnabled = false;
                    InfoClass.Text = "";
                    InfoTeachers.Items.Clear();
                    InfoGroop.Items.Clear();
                    SelectedCell = cell;
                    SelectedCell.BorderBrush = new SolidColorBrush(Colors.Blue);
                    SelectedCell.BorderThickness = new Thickness(2);
                    RemovelistBox.SelectedIndex = -1;
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
        void CreateTimeHeader(MouseButtonEventHandler mLeft)
        {
            string[] times = new string[] { "8.30-10.05", "10.25-12.00", "12.20-13.55", "14.15-15.50", "16.00-17.35", "17.45-19.20" };

            int timeStringIndex = 0;
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
                timeStringIndex++;
                groupLabel.MouseLeftButtonDown += mLeft;
                if (timeStringIndex == 6)
                {
                    timeStringIndex = 0;
                }
            }
        }
        #endregion

        #region Edit Schedule

        private int ColForRemove = 0;
        private int RowForRemove = 0;
        private int TimeRows = -1;
        

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            StudentsClass clas;
            schedule.RemoveClases.Add(schedule.partSchedule[RowForRemove - ROW_HEADER, ColForRemove - COLUMN_HEADER]);
            RemovelistBox.ItemsSource = null;
            RemovelistBox.ItemsSource = schedule.RemoveClases;
            clas = schedule.partSchedule[RowForRemove - ROW_HEADER, ColForRemove - COLUMN_HEADER];
            schedule.RemoveFromClasses(clas, RowForRemove - ROW_HEADER);
            for (int colIndex = 0; colIndex < schedule.partSchedule.GetLength(1); colIndex++)
            {
                if (schedule.partSchedule[RowForRemove - ROW_HEADER, colIndex] == clas)
                {
                    schedule.partSchedule[RowForRemove - ROW_HEADER, colIndex] = null;
                }
            }
            btnShow_Click(Type.Missing, e);
            btnRemove.IsEnabled = false;
            InfoClass.Text = "";
            InfoTeachers.Items.Clear();
            InfoGroop.Items.Clear();


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
                    TimeBox.Text = SelectedTimeCell.Content.ToString();
                    TimeRows = row-1;
                    if (RemovelistBox.SelectedItem != null && TimeRows != -1 && listViewClassRoom.Items.Count != 0)
                    { btnSet.IsEnabled = true; }
                }

            }
            if (SelectedTimeCell == cell)
            {
                SelectedTimeCell = null;
                TimeBox.Text = "";
                TimeRows = -1;
                btnSet.IsEnabled = false;
            }
            else
            {
                SelectedTimeCell = cell;
                SelectedTimeCell.BorderBrush = new SolidColorBrush(Colors.Yellow);
                SelectedTimeCell.BorderThickness = new Thickness(2);
                if (SelectedTimeCell.Content != null)
                {
                    TimeBox.Text = SelectedTimeCell.Content.ToString();
                    TimeRows = row-1;
                    if (RemovelistBox.SelectedItem != null && TimeRows != -1 && listViewClassRoom.Items.Count != 0)
                    { btnSet.IsEnabled = true; }
                }
            }
        }

        private void RemovelistBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RemovelistBox.SelectedIndex != -1)
            {
                btnSet.IsEnabled = false;
                StudentsClass clas = (StudentsClass)RemovelistBox.SelectedItem;
                InfoClass.Text = clas.Name;
                InfoTeachers.Items.Clear();
                InfoGroop.Items.Clear();
                foreach (Teacher tecah in clas.Teacher)
                {
                    InfoTeachers.Items.Add(tecah);
                }
                foreach (StudentSubGroup groop in clas.SubGroups)
                {
                    InfoGroop.Items.Add(groop);
                }
                btnClass.IsEnabled = true;
                btnRemove.IsEnabled = false;

            }
            if (SelectedCell != null && RemovelistBox.SelectedIndex != -1)
            {
                SelectedCell.BorderBrush = new SolidColorBrush(Colors.Red);
                SelectedCell.BorderThickness = new Thickness(1);
                SelectedCell = null;
            }

            listViewClassRoom.Items.Clear();
            if (RemovelistBox.SelectedItem != null && TimeRows !=-1 && listViewClassRoom.Items.Count != 0)
            { btnSet.IsEnabled = true; }
            



        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (schedule.RemoveClases.Count !=0)
            {
                MessageBox.Show("Остались непоставленные пары");
                e.Cancel = true;
            }
        }


        private void btnClass_Click_1(object sender, RoutedEventArgs e)
        {
            StudentsClass clas;
            clas = (StudentsClass)RemovelistBox.SelectedItem;
            ChooseClassRoom form = new ChooseClassRoom(TimeRows, schedule, clas);
            form.Owner = this;
            form.ShowDialog();

        }
        private void InfoTeachers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InfoTeachers.SelectedIndex = -1;
        }

        private void InfoGroop_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InfoGroop.SelectedIndex = -1;
        }

        private void listViewClassRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listViewClassRoom.SelectedIndex = -1;
        }


        private void btnSet_Click(object sender, RoutedEventArgs e)
        {          
            schedule.SetClass((ClassRoom)listViewClassRoom.Items.GetItemAt(0), (StudentsClass)RemovelistBox.SelectedItem, TimeRows);
            RemovelistBox.ItemsSource = null;
            RemovelistBox.ItemsSource = schedule.RemoveClases;
            btnShow_Click(Type.Missing, e);
        }


        #endregion


    }
}
