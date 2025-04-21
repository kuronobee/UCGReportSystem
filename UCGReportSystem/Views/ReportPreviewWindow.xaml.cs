using System;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using Microsoft.EntityFrameworkCore;
using UCGReportSystem.Data;
using UCGReportSystem.Models;

namespace UCGReportSystem.Views
{
    public partial class ReportPreviewWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly EchoReport _report;

        public ReportPreviewWindow(EchoReport report)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _report = report;

            LoadReportData();
        }

        private void LoadReportData()
        {
            // 患者データのロード
            var patient = _context.Patients.FirstOrDefault(p => p.Id == _report.PatientId);

            if (patient != null)
            {
                PatientIdText.Text = patient.PatientId;
                PatientNameText.Text = patient.Name;
                BirthDateText.Text = patient.DateOfBirth.ToString("yyyy/MM/dd");
                GenderText.Text = patient.Gender;

                // 年齢計算
                int age = CalculateAge(patient.DateOfBirth, _report.ExamDate);
                AgeText.Text = $"{age}歳";
            }

            // レポートデータの表示
            ExamDateText.Text = _report.ExamDate.ToString("yyyy/MM/dd");
            PhysicianText.Text = _report.Physician;
            TechnicianText.Text = _report.Technician;

            // 心臓サイズ測定値の表示
            AoDText.Text = FormatValue(_report.AoD);
            LADText.Text = FormatValue(_report.LAD);
            LVIDdText.Text = FormatValue(_report.LVIDd);
            LVIDsText.Text = FormatValue(_report.LVIDs);
            IVSTdText.Text = FormatValue(_report.IVSTd);
            LVPWTdText.Text = FormatValue(_report.LVPWTd);
            IVDText.Text = FormatValue(_report.IVD);

            // LVH、ASHの表示
            LVHText.Text = _report.LVH ? "+" : "-";
            ASHText.Text = _report.ASH ? "+" : "-";

            // EF関連データの表示
            TeichholtzEFText.Text = FormatValue(_report.TeichholtzEF);
            PomboEFText.Text = FormatValue(_report.PomboEF);
            SimpsonEFText.Text = FormatValue(_report.SimpsonEF);
            FSText.Text = FormatValue(_report.FS);

            // MV flow
            EText.Text = FormatValue(_report.E);
            AText.Text = FormatValue(_report.A);
            E_AText.Text = FormatValue(_report.E_A);
            DecTText.Text = FormatValue(_report.DecT);
            E_EprimeText.Text = FormatValue(_report.E_Eprime);

            // 弁膜評価
            MVPText.Text = _report.MVP ? $"+ {_report.MVPDetails}" : "-";
            MVCalcText.Text = _report.MVCalc ? $"+ {_report.MVCalcDetails}" : "-";
            MSText.Text = _report.MS ? "+" : "-";
            DoomineText.Text = _report.Doomine ? "+" : "-";
            MVAByPHTText.Text = FormatValue(_report.MVAByPHT);
            MVAByPlanimetryText.Text = FormatValue(_report.MVAByPlanimetry);

            AVCalcText.Text = _report.AVCalc ? $"+ {_report.AVCalcDetails}" : "-";
            ASText.Text = _report.AS ? "+" : "-";
            PVEText.Text = FormatValue(_report.PVE);
            AVAByPlanimetryText.Text = FormatValue(_report.AVAByPlanimetry);
            AVAByEquationText.Text = FormatValue(_report.AVAByEquation);
            MeanPGText.Text = FormatValue(_report.MeanPG);
            MaxPGText.Text = FormatValue(_report.MaxPG);

            // 逆流評価の表示
            // 逆流評価の表示
            MRText.Text = _report.MRGrade;
            ARText.Text = _report.ARGrade;
            TRText.Text = _report.TRGrade;
            PRText.Text = _report.PRGrade;
            PHTText.Text = FormatValue(_report.PHT);
            PGText.Text = FormatValue(_report.PG);

            // その他所見の表示
            AsynergyText.Text = _report.Asynergy ? $"+ {_report.AsynergyDetails}" : "-";
            PEText.Text = _report.PE ? $"+ {_report.PEDetails}" : "-";
            ShuntText.Text = _report.Shunt ? $"+ {_report.ShuntDetails}" : "-";

            // 所見・診断の表示
            DiagnosisText.Text = _report.Diagnosis;
            FindingsText.Text = _report.Findings;
        }

        private string GetRegurgitationText(int grade)
        {
            switch (grade)
            {
                case 0: return "-";
                case 1: return "trace";
                case 2: return "mild";
                case 3: return "moderate";
                case 4: return "severe";
                case 5: return "n.d.";
                default: return "-";
            }
        }

        private string FormatValue(double? value)
        {
            return value.HasValue ? value.Value.ToString("0.0") : "";
        }

        private int CalculateAge(DateTime birthDate, DateTime referenceDate)
        {
            int age = referenceDate.Year - birthDate.Year;
            if (referenceDate < birthDate.AddYears(age))
                age--;

            return age;
        }

        public void ShowPrintPreview()
        {
            try
            {
                // Create a print dialog
                PrintDialog printDialog = new PrintDialog();
                printDialog.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);

                // Create an XPS document for preview
                string tempFile = System.IO.Path.GetTempFileName();
                XpsDocument xpsDoc = new XpsDocument(tempFile, FileAccess.ReadWrite);

                // Write the report to the XPS document
                XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);

                // Set up paginator
                var paginator = new ReportPaginator(ReportPanel,
                    new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight),
                    new Thickness(50));

                // Write to XPS
                writer.Write(paginator);

                // Create document viewer
                DocumentViewer viewer = new DocumentViewer();
                viewer.Document = xpsDoc.GetFixedDocumentSequence();

                // Show preview in a new window
                Window previewWindow = new Window
                {
                    Title = "印刷プレビュー",
                    Content = viewer,
                    Width = 800,
                    Height = 800
                };
                previewWindow.ShowDialog();

                // Clean up
                xpsDoc.Close();
                try { System.IO.File.Delete(tempFile); } catch { }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"プレビューエラー: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}