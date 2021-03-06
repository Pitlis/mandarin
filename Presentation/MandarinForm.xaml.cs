﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Mandarin.Controls;
using MaterialDesignThemes.Wpf;
using Mandarin.Code;
using Microsoft.Win32;
using Domain.DataFiles;
using Mandarin.ScheduleEditor;
using System.Collections.Generic;
using System.Windows.Media;
using System;
using Domain.Services;
using System.Threading.Tasks;
using Mandarin.FactorsDataEditors;
using Domain.FactorInterfaces;
using System.Diagnostics;

namespace Mandarin
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
            main.ListBoxDoubleClick += new EventHandler(OpenScheduleFromBase);

            LoadFactorsInfo();
        }

        private void IsSchedulesEmpty(object sender, EventArgs e)
        {
            ListBox schedules = (ListBox)sender;
            if (schedules.Items.Count > 0)
            {
                main.renameScheduleButton.IsEnabled = true;
                main.deleteScheduleButton.IsEnabled = true;
            }
            else
            {
                main.renameScheduleButton.IsEnabled = false;
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
                LoadSchedules();
                LoadFactorsInfo();
                LoadDataBaseInfo();
                LoadFactorsWithUserData();
                miDBSettings.IsEnabled = true;
                miSettings.IsEnabled = true;
                miDataBaseEditor.IsEnabled = true;
                miDBSave.IsEnabled = true;
                miDBSaveAs.IsEnabled = true;
                miCore.IsEnabled = true;
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
                Base openedBase = CurrentBase.LoadBase(openFile.FileName);
                FactorsLoader.UpdateAssemblyPath(openedBase.Factors);
                if (await CheckLostFactors(openedBase.Factors))
                {
                    CheckNewFactors(openedBase.Factors);
                    CurrentBase.OpenBase(openedBase);
                    LoadDataBaseInfo();
                    LoadSchedules();
                    LoadFactorsWithUserData();
                    miDBSettings.IsEnabled = true;
                    miDBSave.IsEnabled = true;
                    miDBSaveAs.IsEnabled = true;
                    miCore.IsEnabled = true;
                    miDataBaseEditor.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "Ошибка открытия файла базы" }
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
                        Message = { Text = "База успешно сохранена" }
                    };
                    await DialogHost.Show(infoWindow, "MandarinHost");
                }
                catch
                {
                    var infoWindow = new InfoWindow
                    {
                        Message = { Text = "Ошибка сохранения базы" }
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
                        Message = { Text = "База успешно сохранена" }
                    };
                    await DialogHost.Show(infoWindow, "MandarinHost");
                    CurrentBase.OpenBase(CurrentBase.LoadBase(saveFileDialog.FileName));
                }
                catch
                {
                    var infoWindow = new InfoWindow
                    {
                        Message = { Text = "Ошибка сохранения базы" }
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

        private void misheduleExportGroups_Click(object sender, RoutedEventArgs e)
        {
            ScheduleSubGroupsExcelForm scheduleFacultyGroups = new ScheduleSubGroupsExcelForm();
            scheduleFacultyGroups.ShowDialog();
        }

        private void OpenScheduleFromBase(object sender, EventArgs e)
        {
            if (CurrentSchedule.ScheduleIsLoaded())
            {
                OpenScheduleEditor();
                misheduleExportFaculty.IsEnabled = true;
            }
        }

        private void miCore_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentBase.BaseIsLoaded())
            {
                OpenCoreRunner();
            }
        }

        #endregion

        #region miSettings

        private void miFacultiesSettings_Click(object sender, RoutedEventArgs e)
        {
            OpenFacultiesSettings();
        }

        private void miSettings_Click(object sender, RoutedEventArgs e)
        {
            MenuItem currentMenuItem = (MenuItem)e.OriginalSource;
            FactorSettingRecord currentFactorRecord = (FactorSettingRecord)currentMenuItem.Header;
            OpenFactorEditor(currentFactorRecord);
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

        #endregion

        #region Code

        private void ReturnToMain()
        {
            contentControl.Content = main;
            miScheduleSave.IsEnabled = false;
            miSheduleSaveAs.IsEnabled = false;
            miSheduleExport.IsEnabled = false;
            miDB.IsEnabled = true;
            miDataBaseEditor.IsEnabled = true;
            if (CurrentBase.BaseIsLoaded())
            {
                if (miSettings.Items.Count > 0)
                {
                    miSettings.IsEnabled = true;
                }
                miCore.IsEnabled = true;
                LoadSchedules();
            }
            if (main.scheduleListBox.Items.Count > 0)
            {
                CurrentSchedule.LoadSchedule((KeyValuePair<string, Schedule>)main.scheduleListBox.SelectedItem);
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

        private async void CheckNewFactors(IEnumerable<FactorSettings> factorsOfBase)
        {
            List<string> newFactors = (List<string>)FactorsLoader.GetNewFactorsList(factorsOfBase);
            if (newFactors.Count > 0)
            {
                string newFactorsMsg = "Обнаружены новые анализаторы!\nОни будут автоматически подключены к текущей базе и станут доступны в настройках.\n\n";
                var infoWindow = new InfoListBoxWindow
                {
                    Message = { Text = newFactorsMsg },
                    infoList = { ItemsSource = newFactors }
                };
                await DialogHost.Show(infoWindow, "MandarinHost");
            }
        }
        private async Task<bool> CheckLostFactors(IEnumerable<FactorSettings> factorsOfBase)
        {
            List<string> lostFactors = (List<string>)FactorsLoader.GetLostFactorsList(factorsOfBase);
            if (lostFactors.Count > 0)
            {
                string lostFactorsMsg = "К сожалению, библиотеки с некоторыми анализаторами не обнаружены.\n" +
                    "При продолжении открытия базы, эти анализаторы будут отключены, а их настройки потеряны.\n" +
                    "Продолжить открытие базы?\n\n" +
                    "Список потерянных анализаторов:\n";
                var dialogWindow = new DialogListBoxWindow
                {
                    Message = { Text = lostFactorsMsg },
                    infoList = { ItemsSource = lostFactors }
                };
                object result = await DialogHost.Show(dialogWindow, "MandarinHost");
                if ((bool)result == false)
                {
                    return false;
                }
            }
            return true;
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
            miDB.IsEnabled = false;
            miSettings.IsEnabled = false;
            miDataBaseEditor.IsEnabled = false;
            miCore.IsEnabled = false;
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

        private IEnumerable<FactorSettingRecord> LoadFactorSettingRecords()
        {
            List<FactorSettingRecord> factorRecords = new List<FactorSettingRecord>();
            foreach (FactorSettings factorSettings in CurrentBase.Factors)
            {
                IFactor factorInstance = factorSettings.CreateInstance();
                FactorSettingRecord factorRecord = new FactorSettingRecord();

                factorRecord.ReferenceToSettings = factorSettings;
                factorRecord.Name = factorInstance.GetName();
                factorRecord.Description = factorInstance.GetDescription();
                factorRecord.Fine = factorSettings.Fine;

                factorRecord.DataType = factorSettings.DataTypeGuid;
                factorRecord.Data = factorSettings.Data;

                if (factorInstance.GetDataTypeGuid().HasValue)
                {
                    if (factorInstance is IFactorFormData && FactorsEditors.GetFactorEditors().ContainsKey(factorInstance.GetDataTypeGuid().Value))
                    {
                        factorRecords.Add(factorRecord);
                    }
                }
            }

            return factorRecords;
        }

        private void LoadFactorsWithUserData()
        {
            List<FactorSettingRecord> factorsRecord = (List<FactorSettingRecord>)LoadFactorSettingRecords();
            if (factorsRecord.Count > 0)
            {
                miSettings.ItemsSource = factorsRecord;
                miSettings.IsEnabled = true;
            }
            else
                miSettings.IsEnabled = false;
        }

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

        private void OpenFactorEditor(FactorSettingRecord factorRecord)
        {
            try
            {
                Type editorType = FactorsEditors.GetFactorEditors()[factorRecord.DataType.Value];
                Window editorForm = (Window)Activator.CreateInstance(editorType);

                FactorsEditors.InitFactorEditor(factorRecord.ReferenceToSettings, (IFactorEditor)editorForm);
                editorForm.ShowDialog();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async void OpenCoreRunner()
        {
            if (CurrentBase.BaseIsLoaded())
            {
                if (CurrentBase.EStorage.Classes.Length > 0)
                {
                    contentControl.Content = new CoreRunnerForm();
                }
                else
                {
                    var infoWindow = new InfoWindow
                    {
                        Message = { Text = "В базе данных отсутствуют пары.\n" +
                                        "Генерация расписания невозможна." }
                    };
                    await DialogHost.Show(infoWindow, "MandarinHost");
                }
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

        private bool canClose;
        private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!canClose)
            {
                if (CurrentBase.BaseIsLoaded())
                {
                    e.Cancel = true;
                    var dialogWindow = new DialogCancelWindow
                    {
                        Message = { Text = "Вы хотите сохранить базу перед закрытием приложения?" }
                    };
                    object result = await DialogHost.Show(dialogWindow, "MandarinHost");
                    try
                    {
                        if (result != null)
                        {
                            if ((bool)result == true)
                            {
                                CurrentBase.SaveBase();
                                var infoWindow = new InfoWindow
                                {
                                    Message = { Text = "База успешно сохранена" }
                                };
                                await DialogHost.Show(infoWindow, "MandarinHost");
                            }
                            canClose = true;
                            this.Close();
                        }
                    }
                    catch
                    {
                        var infoWindow = new InfoWindow
                        {
                            Message = { Text = "Ошибка сохранения базы" }
                        };
                        await DialogHost.Show(infoWindow, "MandarinHost");
                    }

                }
            }


        }

        #region About

        private void emailButtonN_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("mailto://pitlis95@gmail.com");
        }

        private void vkButtonN_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://vk.com/nikita_bozhkov");
        }

        private void gitHubButtonN_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Pitlis");
        }

        private void gitHubButtonY_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Eugenni");
        }

        private void emailButtonY_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("mailto://jenya-masalkov@mail.ru");
        }

        private void vkButtonY_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://vk.com/id69607998");
        }

        private void gitHubButtonD_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/jonny1sniper");
        }

        private void emailButtonD_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("mailto://dimelnikov94@gmail.com");
        }

        private void vkButtonD_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://vk.com/dmelnik0v");
        }

        private void gitHubButtonS_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Ser95");
        }

        private void emailButtonS_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("mailto://puss95@yandex.by");
        }

        private void gitHubButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Pitlis/mandarin");
        }
        #endregion

        private void miDataBaseEditor_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentBase.BaseIsLoaded())
            {
                miMain.Header = "Закрыть";
                contentControl.Content = new StorageEditor.StorageEditorForm();
            }
            else
            {
                var infoWindow = new InfoWindow
                {
                    Message = { Text = "База данных не загружена" }
                };
                DialogHost.Show(infoWindow, "MandarinHost");
            }
        }
    }
}
