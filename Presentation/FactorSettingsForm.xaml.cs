using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows;
using Domain;
using Domain.Services;
using Domain.Model;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;
using MandarinCore;
using Domain.FactorInterfaces;
using Presentation.Code;
using Presentation.FactorsDataEditors;
using MaterialDesignThemes.Wpf;
using Presentation.Controls;
using System.Windows.Controls;
using System.Windows.Media;

namespace Presentation
{
    /// <summary>
    /// Логика взаимодействия для FactorSettingsForm.xaml
    /// </summary>
    public partial class FactorSettingsForm : UserControl
    {
        List<FactorSettingRecord> FactorRecords;
        public ContentControl contentControl { get; set; }

        public FactorSettingsForm()
        {
            InitializeComponent();
            FactorRecords = LoadFactorSettingRecords().ToList();
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGrid.ItemsSource = FactorRecords;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFactorSettings();
        }
        private async void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var infoWindow = new DialogWindow
            {
                Message = { Text = "Вы уверены, что хотите завершить настройку?\n" +
                                    "Все несохраненные изменения будут потеряны!" }
            };

            object result = await DialogHost.Show(infoWindow, "FactorSettingsHost");

            if ((bool)result == true)
            {
                if (this.contentControl != null)
                {
                    Main main = new Main();
                    contentControl.Content = main;
                    FactorRecords = LoadFactorSettingRecords().ToList();
                    this.DataContext = this;
                }
            }
        }

        private async void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            var infoWindow = new DialogWindow
            {
                Message = { Text = "Вы уверены, что хотите сбросить все настройки анализаторов?\n" +
                "Все введенные данные (например предпочтения преподавателей по корпусам) будут потеряны!" }
            };

            object result = await DialogHost.Show(infoWindow, "FactorSettingsHost");

            if ((bool)result == true)
            {
                FactorsLoader.SetDefaultSettings();
                FactorRecords = LoadFactorSettingRecords().ToList();
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = FactorRecords;
            }
        }

        private void btnEditData_Click(object sender, RoutedEventArgs e)
        {
            FactorSettingRecord row = ((FrameworkElement)sender).DataContext as FactorSettingRecord;
            OpenFactorEditor(row);
        }

        #region Code

        IEnumerable<FactorSettingRecord> LoadFactorSettingRecords()
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

                if (factorRecord.DataType.HasValue)
                {
                    factorRecord.UseProgramData = factorInstance is IFactorProgramData;
                    factorRecord.UseUsersData = factorInstance is IFactorFormData && FactorsEditors.GetFactorEditors().ContainsKey(factorRecord.DataType.Value);
                }

                factorRecords.Add(factorRecord);
            }

            return factorRecords;
        }
        void SaveFactorSettings()
        {
            foreach (FactorSettingRecord record in FactorRecords)
            {
                FactorSettings factor = record.ReferenceToSettings;

                factor.Fine = record.Fine;
                factor.Data = record.Data;
            }
        }
        void OpenFactorEditor(FactorSettingRecord factorRecord)
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

        #endregion
    }

    class FactorSettingRecord
    {
        public FactorSettings ReferenceToSettings { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        int fine;
        public int Fine
        {
            get { return fine; }
            set
            {
                if (value > 100 || value < 0)
                    MessageBox.Show("Значение должно находиться в диапазоне от 0 до 100");
                else { fine = value; }
            }
        }

        public bool UseProgramData { get; set; }
        public bool UseUsersData { get; set; }

        public Guid? DataType { get; set; }
        public object Data { get; set; }
    }
}
