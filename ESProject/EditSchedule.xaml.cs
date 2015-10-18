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
            CreateTimeHeader();
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
            if(schedule.partSchedule[row - ROW_HEADER, col - COLUMN_HEADER] != null)
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
            if (SelectedCell != null)
            {
                SelectedCell.BorderBrush = new SolidColorBrush(Colors.Red);
                SelectedCell.BorderThickness = new Thickness(1);
            }

            Label cell = (Label)sender;
            if (SelectedCell == cell)
            {
                SelectedCell = null;
            }
            else
            {
                SelectedCell = cell;
                SelectedCell.BorderBrush = new SolidColorBrush(Colors.Blue);
                SelectedCell.BorderThickness = new Thickness(2);
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
                groupLabel.SetValue(Grid.RowProperty, ROW_HEADER-1);
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
            string[] days = new string[] {"Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота", "Понедельник", "Вторник", "Среда", "Четверг", "Пятница", "Суббота" };
            foreach (string day in days)
            {
                Label groupLabel = new Label();
                groupLabel.LayoutTransform = new RotateTransform(270);
                groupLabel.Content = day;
                groupLabel.Height = 30;
                groupLabel.Width = CELL_HEIGHT*6;
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
        void CreateTimeHeader()
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
                if (timeStringIndex == 6)
                {
                    timeStringIndex = 0;
                }
            }
        }
        #endregion


    }
}
