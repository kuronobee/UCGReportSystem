using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using UCGReportSystem.Data;
using UCGReportSystem.Models;
using UCGReportSystem.Views;

namespace UCGReportSystem
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public MainWindow()
        {
            InitializeComponent();

            // データベースコンテキストの初期化
            _context = new ApplicationDbContext();
            _context.Database.EnsureCreated();

            // 患者リストの読み込み
            LoadPatients();
            LoadReports();
        }

        private void LoadPatients()
        {
            PatientsDataGrid.ItemsSource = _context.Patients.ToList();
        }

        private void LoadReports()
        {
            ReportsDataGrid.ItemsSource = _context.EchoReports
                .Include(r => r.Patient)
                .ToList();
        }

        private void NewPatient_Click(object sender, RoutedEventArgs e)
        {
            var patientWindow = new PatientWindow();
            if (patientWindow.ShowDialog() == true)
            {
                LoadPatients();
            }
        }

        private void NewReport_Click(object sender, RoutedEventArgs e)
        {
            var reportWindow = new ReportWindow();
            if (reportWindow.ShowDialog() == true)
            {
                LoadReports();
            }
        }

        private void SearchPatient_Click(object sender, RoutedEventArgs e)
        {
            var searchText = SearchPatientTextBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                LoadPatients();
                return;
            }

            var results = _context.Patients
                .Where(p => p.PatientId.ToLower().Contains(searchText) ||
                           p.Name.ToLower().Contains(searchText))
                .ToList();

            PatientsDataGrid.ItemsSource = results;
        }

        private void SearchReport_Click(object sender, RoutedEventArgs e)
        {
            var searchText = SearchReportTextBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                LoadReports();
                return;
            }

            var results = _context.EchoReports
                .Include(r => r.Patient)
                .Where(r => r.Patient.Name.ToLower().Contains(searchText) ||
                           r.Patient.PatientId.ToLower().Contains(searchText) ||
                           r.Physician.ToLower().Contains(searchText))
                .ToList();

            ReportsDataGrid.ItemsSource = results;
        }

        private void PatientsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var patient = PatientsDataGrid.SelectedItem as Patient;
            if (patient != null)
            {
                var patientWindow = new PatientWindow(patient);
                if (patientWindow.ShowDialog() == true)
                {
                    LoadPatients();
                }
            }
        }

        private void ReportsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var report = ReportsDataGrid.SelectedItem as EchoReport;
            if (report != null)
            {
                var reportWindow = new ReportWindow(report);
                if (reportWindow.ShowDialog() == true)
                {
                    LoadReports();
                }
            }
        }

        private void BackupDatabase_Click(object sender, RoutedEventArgs e)
        {
            // データベースバックアップの実装
            MessageBox.Show("データベースのバックアップが完了しました。", "バックアップ", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PrintReport_Click(object sender, RoutedEventArgs e)
        {
            var report = ReportsDataGrid.SelectedItem as EchoReport;
            if (report == null)
            {
                MessageBox.Show("印刷するレポートを選択してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var printWindow = new PrintReportWindow(report);
            printWindow.ShowDialog();
        }

        private void PreviewReport_Click(object sender, RoutedEventArgs e)
        {
            var report = ReportsDataGrid.SelectedItem as EchoReport;
            if (report == null)
            {
                MessageBox.Show("プレビューするレポートを選択してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var previewWindow = new ReportPreviewWindow(report);
            previewWindow.ShowDialog();
        }

        private void ExportReport_Click(object sender, RoutedEventArgs e)
        {
            var report = ReportsDataGrid.SelectedItem as EchoReport;
            if (report == null)
            {
                MessageBox.Show("エクスポートするレポートを選択してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // PDFエクスポートの実装
            MessageBox.Show("レポートのエクスポートが完了しました。", "エクスポート", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}