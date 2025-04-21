using System;
using System.Windows;
using UCGReportSystem.Data;
using UCGReportSystem.Models;

namespace UCGReportSystem.Views
{
    public partial class PatientWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private Patient _patient;
        private bool _isEditMode;

        public PatientWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _patient = new Patient();
            _isEditMode = false;

            BirthDatePicker.SelectedDate = DateTime.Today;
        }

        public PatientWindow(Patient patient)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _patient = patient;
            _isEditMode = true;

            // 患者データの表示
            PatientIdTextBox.Text = patient.PatientId;
            NameTextBox.Text = patient.Name;
            BirthDatePicker.SelectedDate = patient.DateOfBirth;
            MaleRadioButton.IsChecked = patient.Gender == "男性";
            FemaleRadioButton.IsChecked = patient.Gender == "女性";
            PhoneTextBox.Text = patient.PhoneNumber;

            // 編集モードでは患者IDを変更できないようにする
            PatientIdTextBox.IsEnabled = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PatientIdTextBox.Text) || string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("患者IDと氏名は必須項目です。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // 患者データの更新
                _patient.PatientId = PatientIdTextBox.Text;
                _patient.Name = NameTextBox.Text;
                _patient.DateOfBirth = BirthDatePicker.SelectedDate ?? DateTime.Today;
                _patient.Gender = MaleRadioButton.IsChecked == true ? "男性" : "女性";
                _patient.PhoneNumber = PhoneTextBox.Text;

                if (!_isEditMode)
                {
                    _context.Patients.Add(_patient);
                }

                _context.SaveChanges();
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エラーが発生しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}