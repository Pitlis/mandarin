﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Presentation.Controls;
using MaterialDesignThemes.Wpf;
using Presentation.Code;
using Microsoft.Win32;
using Domain.DataFiles;
using Presentation.ScheduleEditor;
using System.Collections.Generic;
using System.Windows.Media;
using System;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for newMandarinForm.xaml
    /// </summary>
    public partial class MandarinForm : Window
    {
        Main main;

        public MandarinForm()
        {
            InitializeComponent();
            main = new Main();
            contentControl.Content = main;
            main.IsListBoxEmpty += new EventHandler(IsSchedulesEmpty);
        }

        private void IsSchedulesEmpty(object sender, EventArgs e)
        {
            ListBox schedules = (ListBox)sender;
            if (schedules.Items.Count > 0)
            {
                miScheduleEdit.IsEnabled = true;
                main.deleteScheduleButton.IsEnabled = true;
            }
            else
            {
                miScheduleEdit.IsEnabled = false;
                main.deleteScheduleButton.IsEnabled = false;
            }
        }

        private async void Main_Click(object sender, RoutedEventArgs e)
        {
            if (miMain.Header.Equals("Закрыть"))
            {
                var dialogWindow = new DialogWindow
                {
                    Message = { Text = "Вы уверены, что хотите завершить редактирование?\n" +
                                    "Все несохраненные изменения будут потеряны!" }
                };
                object result = await DialogHost.Show(dialogWindow, "MandarinHost");
                if ((bool)result == true)
                {
                    ReturnToMain();
                }
            }
            else
                ReturnToMain();
        }

        #region miDB
        private void miDBCreate_Click(object sender, RoutedEventArgs e)
        {
            //открытие формы создания BaseWizard
            BaseWizard.BaseWizardForm baseWizardform = new BaseWizard.BaseWizardForm();
            baseWizardform.ShowDialog();
            if (CurrentBase.BaseIsLoaded())
            {
                LoadDataBaseInfo();
                LoadSchedules();
                LoadFactorsInfo();
                miFactorSettings.IsEnabled = true;
                miSettings.IsEnabled = true;
                miDBSave.IsEnabled = true;
                miDBSaveAs.IsEnabled = true;
            }
        }

        private async void miDBOpen_Click(object sender, RoutedEventArgs e)
        {
            //здесь сделать окно для открытия
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "DB files (*.mandarin)|*.mandarin";
            if (openFile.ShowDialog() == false)
            {
                return;
            }
            try
            {
                CurrentBase.LoadBase(openFile.FileName);
                CheckFactors();
                LoadDataBaseInfo();
                LoadSchedules();
                LoadFactorsInfo();
                miFactorSettings.IsEnabled = true;
                miSettings.IsEnabled = true;
                miDBSave.IsEnabled = true;
                miDBSaveAs.IsEnabled = true;
            }
            catch
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Не удалось открыть" }
                };
                await DialogHost.Show(infoWindow, "MandarinHost");
                return;
            }

        }

        private async void miDBSave_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentBase.BaseIsLoaded())
            {
                try
                {
                    CurrentBase.SaveBase();
                    var infoWindow = new InfoWindow
                    {
                        Message = { Text = "Сохранение прошло успешно" }
                    };
                    await DialogHost.Show(infoWindow, "MandarinHost");
                }
                catch
                {
                    var infoWindow = new InfoWindow
                    {
                        Message = { Text = "Не сохранено, попробуйте еще раз" }
                    };
                    await DialogHost.Show(infoWindow, "MandarinHost");
                }

            }
        }

        private async void miDBSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "DB files (*.mandarin|*.mandarin";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    CurrentBase.SaveBase(saveFileDialog.FileName);
                    var infoWindow = new InfoWindow
                    {
                        Message = { Text = "Сохранение прошло успешно" }
                    };
                    await DialogHost.Show(infoWindow, "MandarinHost");
                    CurrentBase.LoadBase(saveFileDialog.FileName);
                }
                catch
                {
                    var infoWindow = new InfoWindow
                    {
                        Message = { Text = "Не удалось сохранить" }
                    };
                    await DialogHost.Show(infoWindow, "MandarinHost");
                    return;
                }

            }

        }
        #endregion

        #region miSchedule

        private void miScheduleOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenSchedule();
        }

        private void miScheduleSave_Click(object sender, RoutedEventArgs e)
        {
            SaveSchedule();
        }

        private void miSheduleSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveScheduleAs();
        }

        private void misheduleExportTeacher_Click(object sender, RoutedEventArgs e)
        {
            ScheduleTeacherExcel scheduleTeacherExcel = new ScheduleTeacherExcel();
            scheduleTeacherExcel.ShowDialog();
        }

        private void misheduleExportFaculty_Click(object sender, RoutedEventArgs e)
        {
            ScheduleFacultyExcelForm scheduleFacultyExcel = new ScheduleFacultyExcelForm();
            scheduleFacultyExcel.ShowDialog();
        }

        private void miScheduleEdit_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSchedule.ScheduleIsLoaded())
            {
                OpenScheduleEditor();
                misheduleExportFaculty.IsEnabled = true;
            }
        }

        #endregion

        #region miSettings

        private void miFacultiesSettings_Click(object sender, RoutedEventArgs e)
        {
            OpenFacultiesSettings();
        }

        private async void miFactorSettings_Click(object sender, RoutedEventArgs e)
        {
            if (miMain.Header.Equals("Закрыть"))
            {
                var dialogWindow = new DialogWindow
                {
                    Message = { Text = "Вы уверены, что хотите завершить редактирование?\n" +
                                    "Все несохраненные изменения будут потеряны!" }
                };
                object result = await DialogHost.Show(dialogWindow, "MandarinHost");
                if ((bool)result == true)
                {
                    OpenFactorSettings();
                }
            }
            else
            {
                OpenFactorSettings();
            }
        }

        private void miVIPSettings_Click(object sender, RoutedEventArgs e)
        {
            OpenVIPSettings();
        }

        #endregion

        #region Code

        private void ReturnToMain()
        {
            contentControl.Content = main;
            miScheduleSave.IsEnabled = false;
            miSheduleSaveAs.IsEnabled = false;
            miSheduleExport.IsEnabled = false;
            miDB.IsEnabled = true;
            if (CurrentBase.BaseIsLoaded())
            {
                miSettings.IsEnabled = true;
            }
            if (main.scheduleListBox.Items.Count > 0)
            {
                miScheduleEdit.IsEnabled = true;
                if (CurrentSchedule.ScheduleIsFromFile())
                {
                    CurrentSchedule.LoadSchedule((KeyValuePair<string, Schedule>)main.scheduleListBox.SelectedItem);
                }
            }
            miMain.Header = "Главная";
        }

        private void LoadDataBaseInfo()
        {
            main.classesTextBlock.Text = CurrentBase.EStorage.Classes.Length.ToString();
            main.classRoomTextBlock.Text = CurrentBase.EStorage.ClassRooms.Length.ToString();
            main.classRoomTypesTextBlock.Text = CurrentBase.EStorage.ClassRoomsTypes.Length.ToString();
            main.teachersTextBlock.Text = CurrentBase.EStorage.Teachers.Length.ToString();
            main.subGroupsTextBlock.Text = CurrentBase.EStorage.StudentSubGroups.Length.ToString();
        }

        private void LoadFactorsInfo()
        {
           main.factorsListBox.ItemsSource = FactorsLoader.GetActualFactorsList();
        }

        private async void CheckFactors()
        {
            List<string> lostFactors = (List<string>)FactorsLoader.GetLostFactorsList();
            List<string> newFactors = (List<string>)FactorsLoader.GetNewFactorsList();
            if (lostFactors.Count > 0)
            {
                string lostFactorsMsg = "К сожалению, следующие анализаторы отсутствуют и будут отключены.\n" +
                                            "Данные анализаторов будут потеряны.\n";
                foreach (string lostFactor in lostFactors)
                {
                    lostFactorsMsg += lostFactor + '\n';
                }
                var infoWindow = new InfoWindow
                {
                    Message = { Text = lostFactorsMsg }
                };
                await DialogHost.Show(infoWindow, "MandarinHost");

            }
            if (newFactors.Count > 0)
            {
                string newFactorsMsg = "Появились новые анализаторы.\n";
                foreach (string newFactor in newFactors)
                {
                    newFactorsMsg += newFactor + '\n';
                }
                var infoWindow = new InfoWindow
                {
                    Message = { Text = newFactorsMsg }
                };
                await DialogHost.Show(infoWindow, "MandarinHost");
            }
        }

        #region Schedule

        private void LoadSchedules()
        {
            main.scheduleListBox.ItemsSource = null;
            main.scheduleListBox.ItemsSource = CurrentBase.Schedules;
            if (main.scheduleListBox.Items.Count > 0)
            {
                main.scheduleListBox.SelectedIndex = 0;
            }
        }

        private void OpenScheduleEditor()
        {
            contentControl.Content = new EditSchedule();
            miScheduleSave.IsEnabled = true;
            miSheduleSaveAs.IsEnabled = true;
            miSheduleExport.IsEnabled = true;
            miScheduleEdit.IsEnabled = false;
            miDB.IsEnabled = false;
            miSettings.IsEnabled = false;
            miMain.Header = "Закрыть";
        }

        private async void OpenScheduleFromFile()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Mandarin Schedule File(*.msf) | *.msf";
            if (openFile.ShowDialog() == false)
            {
                return;
            }
            try
            {
                CurrentSchedule.LoadSchedule(openFile.FileName);
                OpenScheduleEditor();
                misheduleExportFaculty.IsEnabled = false;
            }
            catch
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Не удалось открыть" }
                };
                await DialogHost.Show(infoWindow, "MandarinHost");
                return;
            }
        }

        private async void SaveScheduleAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Mandarin Schedule File(*.msf) | *.msf";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == true)
            {
                if (CurrentSchedule.ScheduleIsLoaded())
                {
                    try
                    {
                        CurrentSchedule.SaveSchedule(saveFileDialog.FileName);
                        var infoWindow = new InfoWindow
                        {
                            Message = { Text = "Сохранение прошло успешно" }
                        };
                        await DialogHost.Show(infoWindow, "MandarinHost");

                    }
                    catch
                    {
                        var infoWindow = new InfoWindow
                        {
                            Message = { Text = "Не удалось сохранить" }
                        };
                        await DialogHost.Show(infoWindow, "MandarinHost");
                        return;
                    }
                }
            }
        }

        private async void SaveSchedule()
        {
            try
            {
                CurrentSchedule.SaveSchedule();
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Сохранено" }
                };
                await DialogHost.Show(infoWindow, "MandarinHost");
                return;

            }
            catch
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Не удалось сохранить" }
                };
                await DialogHost.Show(infoWindow, "MandarinHost");
                return;
            }
        }

        private async void OpenSchedule()
        {
            if (miMain.Header.Equals("Закрыть"))
            {
                var dialogWindow = new DialogWindow
                {
                    Message = { Text = "Вы уверены, что хотите завершить редактирование?\n" +
                                    "Все несохраненные изменения будут потеряны!" }
                };
                object result = await DialogHost.Show(dialogWindow, "MandarinHost");
                if ((bool)result == true)
                {
                    OpenScheduleFromFile();
                }
            }
            else
                OpenScheduleFromFile();
        }

        #endregion

        #region Settings

        private async void OpenFactorSettings()
        {
            if (CurrentBase.BaseIsLoaded())
            {
                miMain.Header = "Закрыть";
                contentControl.Content = new FactorSettingsForm();
            }
            else
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "База данных не загружена" }
                };
                await DialogHost.Show(infoWindow, "MandarinHost");
            }
        }

        private async void OpenVIPSettings()
        {
            if (CurrentBase.BaseIsLoaded())
            {
                VIPForm form = new VIPForm();
                form.ShowDialog();
            }
            else
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "База данных не загружена" }
                };
                await DialogHost.Show(infoWindow, "MandarinHost");
            }
        }

        private async void OpenFacultiesSettings()
        {
            if (CurrentBase.BaseIsLoaded())
            {
                FacultyAndGroupsForm facult = new FacultyAndGroupsForm();
                facult.ShowDialog();
            }
            else
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "База данных не загружена" }
                };
                await DialogHost.Show(infoWindow, "MandarinHost");
            }
        }

        #endregion

        #endregion 
    }
}
