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

namespace Presentation
{
    /// <summary>
    /// Interaction logic for newMandarinForm.xaml
    /// </summary>
    public partial class MandarinForm : Window
    {
        Main main;
        EditSchedule editSchedule;
        FactorSettingsForm fsett;
        public ContentControl ContentCtrl { get; set; }

        public MandarinForm()
        {
            InitializeComponent();
            main = new Main();
            editSchedule = new EditSchedule();
            this.ContentCtrl = contentControl;
            contentControl.Content = main;
            main.contentControl = contentControl;
        }
        #region miDB
        private void miDBCreate_Click(object sender, RoutedEventArgs e)
        {
            //открытие формы создания BaseWizard
            Presentation.BaseWizard.BaseWizardForm baseWizardform = new Presentation.BaseWizard.BaseWizardForm();
            baseWizardform.ShowDialog();
            if (CurrentBase.BaseIsLoaded())
            {
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

        private void Main_Click(object sender, RoutedEventArgs e)
        {
            GetEditScheduleContent();
            contentControl.Content = main;
        }

        private void Schedule_Click(object sender, RoutedEventArgs e)
        {
            //GetEditScheduleContent();
            //if (contentControl.Content == main || contentControl.Content == fsett)
            //{
            //    contentControl.Content = editSchedule;
            //}
        }

        private void FactorSettings_Click(object sender, RoutedEventArgs e)
        {
            GetEditScheduleContent();
            if (fsett == null)
            {
                fsett = new FactorSettingsForm();
                fsett.contentControl = contentControl;
            }
            contentControl.Content = fsett;
        }

        private void GetEditScheduleContent()
        {
            if (contentControl.Content != main && contentControl.Content != editSchedule && contentControl.Content != fsett)
            {
                try
                {
                    editSchedule = (EditSchedule)contentControl.Content;
                }
                catch (System.Exception)
                {
                    
                }
            }
        }
        #region miSchedule
        private async void miScheduleOpen_Click(object sender, RoutedEventArgs e)
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
                miScheduleSave.IsEnabled= true;
                miSheduleSaveAs.IsEnabled = true;
                miSheduleExport.IsEnabled = true;

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
        private async void miScheduleSave_Click(object sender, RoutedEventArgs e)
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
        private async void miSheduleSaveAs_Click(object sender, RoutedEventArgs e)
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
                    CurrentSchedule.LoadSchedule(saveFileDialog.FileName);
                }
            }
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


        #endregion


    }
}
