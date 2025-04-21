using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using UCGReportSystem.Data;
using UCGReportSystem.Models;

namespace UCGReportSystem.Views
{
    public partial class ReportWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private EchoReport _report;
        private Patient? _selectedPatient;
        private bool _isEditMode;

        public ReportWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _report = new EchoReport();
            _isEditMode = false;

            ExamDatePicker.SelectedDate = DateTime.Today;
        }

        public ReportWindow(EchoReport report)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _report = report;
            _isEditMode = true;

            // 関連する患者情報の取得
            _selectedPatient = _context.Patients.FirstOrDefault(p => p.Id == report.PatientId);

            if (_selectedPatient != null)
            {
                PatientIdTextBox.Text = _selectedPatient.PatientId;
                PatientNameTextBox.Text = _selectedPatient.Name;
            }

            // レポートデータの表示
            ExamDatePicker.SelectedDate = report.ExamDate;
            PhysicianTextBox.Text = report.Physician;
            TechnicianTextBox.Text = report.Technician;

            // 心臓サイズ測定値の表示
            AoDTextBox.Text = report.AoD?.ToString();
            LADTextBox.Text = report.LAD?.ToString();
            LVIDdTextBox.Text = report.LVIDd?.ToString();
            LVIDsTextBox.Text = report.LVIDs?.ToString();
            IVSTdTextBox.Text = report.IVSTd?.ToString();
            LVPWTdTextBox.Text = report.LVPWTd?.ToString();
            IVDTextBox.Text = report.IVD?.ToString();

            // LVH、ASHの表示
            LVHPositiveRadioButton.IsChecked = report.LVH;
            LVHNegativeRadioButton.IsChecked = !report.LVH;
            ASHPositiveRadioButton.IsChecked = report.ASH;
            ASHNegativeRadioButton.IsChecked = !report.ASH;

            // EF関連データの表示
            TeichholtzEFTextBox.Text = report.TeichholtzEF?.ToString();
            PomboEFTextBox.Text = report.PomboEF?.ToString();
            SimpsonEFTextBox.Text = report.SimpsonEF?.ToString();
            FSTextBox.Text = report.FS?.ToString();

            // MV flow
            ETextBox.Text = report.E?.ToString();
            ATextBox.Text = report.A?.ToString();
            E_ATextBox.Text = report.E_A?.ToString();
            DecTTextBox.Text = report.DecT?.ToString();
            E_EprimeTextBox.Text = report.E_Eprime?.ToString();

            // 弁膜評価
            MVPPositiveRadioButton.IsChecked = report.MVP;
            MVPNegativeRadioButton.IsChecked = !report.MVP;
            MVPDetailsTextBox.Text = report.MVPDetails;

            MVCalcPositiveRadioButton.IsChecked = report.MVCalc;
            MVCalcNegativeRadioButton.IsChecked = !report.MVCalc;
            MVCalcDetailsTextBox.Text = report.MVCalcDetails;

            MSPositiveRadioButton.IsChecked = report.MS;
            MSNegativeRadioButton.IsChecked = !report.MS;

            DoominePositiveRadioButton.IsChecked = report.Doomine;
            DoomineNegativeRadioButton.IsChecked = !report.Doomine;

            MVAByPHTTextBox.Text = report.MVAByPHT?.ToString();
            MVAByPlanimetryTextBox.Text = report.MVAByPlanimetry?.ToString();

            AVCalcPositiveRadioButton.IsChecked = report.AVCalc;
            AVCalcNegativeRadioButton.IsChecked = !report.AVCalc;
            AVCalcDetailsTextBox.Text = report.AVCalcDetails;

            ASPositiveRadioButton.IsChecked = report.AS;
            ASNegativeRadioButton.IsChecked = !report.AS;

            PVETextBox.Text = report.PVE?.ToString();
            AVAByPlanimetryTextBox.Text = report.AVAByPlanimetry?.ToString();
            AVAByEquationTextBox.Text = report.AVAByEquation?.ToString();
            MeanPGTextBox.Text = report.MeanPG?.ToString();
            MaxPGTextBox.Text = report.MaxPG?.ToString();

            // 逆流評価の表示
            SetRegurgitationCheckBoxes(_report.MRGrade, MRNoneCheckBox, MRTraceCheckBox, MRMildCheckBox,
                                      MRModerateCheckBox, MRSevereCheckBox, MRNdCheckBox);
            SetRegurgitationCheckBoxes(_report.ARGrade, ARNoneCheckBox, ARTraceCheckBox, ARMildCheckBox,
                                      ARModerateCheckBox, ARSevereCheckBox, ARNdCheckBox);
            SetRegurgitationCheckBoxes(_report.TRGrade, TRNoneCheckBox, TRTraceCheckBox, TRMildCheckBox,
                                      TRModerateCheckBox, TRSevereCheckBox, TRNdCheckBox);
            SetRegurgitationCheckBoxes(_report.PRGrade, PRNoneCheckBox, PRTraceCheckBox, PRMildCheckBox,
                                      PRModerateCheckBox, PRSevereCheckBox, PRNdCheckBox);

            PHTTextBox.Text = report.PHT?.ToString();
            PGTextBox.Text = report.PG?.ToString();

            // その他所見の表示
            AsynergyPositiveRadioButton.IsChecked = report.Asynergy;
            AsynergyNegativeRadioButton.IsChecked = !report.Asynergy;
            AsynergyDetailsTextBox.Text = report.AsynergyDetails;

            PEPositiveRadioButton.IsChecked = report.PE;
            PENegativeRadioButton.IsChecked = !report.PE;
            PEDetailsTextBox.Text = report.PEDetails;

            ShuntPositiveRadioButton.IsChecked = report.Shunt;
            ShuntNegativeRadioButton.IsChecked = !report.Shunt;
            ShuntDetailsTextBox.Text = report.ShuntDetails;

            // 所見・診断の表示
            FindingsTextBox.Text = report.Findings;
            DiagnosisTextBox.Text = report.Diagnosis;
            RecommendationsTextBox.Text = report.Recommendations;
        }

        private void SetRegurgitationCheckBoxes(string gradeString, CheckBox none, CheckBox trace,
                                       CheckBox mild, CheckBox moderate, CheckBox severe, CheckBox nd)
        {
            // すべてのチェックボックスをクリア
            none.IsChecked = false;
            trace.IsChecked = false;
            mild.IsChecked = false;
            moderate.IsChecked = false;
            severe.IsChecked = false;
            nd.IsChecked = false;

            // 保存されている文字列に基づいてチェックを設定
            if (string.IsNullOrEmpty(gradeString))
            {
                none.IsChecked = true;
                return;
            }

            string[] grades = gradeString.Split('〜');

            foreach (var grade in grades)
            {
                switch (grade.Trim())
                {
                    case "-": none.IsChecked = true; break;
                    case "trace": trace.IsChecked = true; break;
                    case "mild": mild.IsChecked = true; break;
                    case "moderate": moderate.IsChecked = true; break;
                    case "severe": severe.IsChecked = true; break;
                    case "n.d.": nd.IsChecked = true; break;
                }
            }
        }
        private int GetRegurgitationGrade(System.Windows.Controls.RadioButton none, System.Windows.Controls.RadioButton trace, System.Windows.Controls.RadioButton mild, System.Windows.Controls.RadioButton moderate, System.Windows.Controls.RadioButton severe, System.Windows.Controls.RadioButton nd)
        {
            if (none.IsChecked == true) return 0;
            if (trace.IsChecked == true) return 1;
            if (mild.IsChecked == true) return 2;
            if (moderate.IsChecked == true) return 3;
            if (severe.IsChecked == true) return 4;
            if (nd.IsChecked == true) return 5;
            return 0;
        }

        private void SearchPatient_Click(object sender, RoutedEventArgs e)
        {
            var patientId = PatientIdTextBox.Text;
            if (string.IsNullOrWhiteSpace(patientId))
            {
                MessageBox.Show("患者IDを入力してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var patient = _context.Patients.FirstOrDefault(p => p.PatientId == patientId);
            if (patient == null)
            {
                MessageBox.Show("入力された患者IDに該当する患者が見つかりません。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _selectedPatient = patient;
            PatientNameTextBox.Text = _selectedPatient.Name;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPatient == null)
            {
                MessageBox.Show("患者を選択してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ExamDatePicker.SelectedDate == null)
            {
                MessageBox.Show("検査日を選択してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                // レポートデータの更新
                _report.PatientId = _selectedPatient.Id;
                _report.ExamDate = ExamDatePicker.SelectedDate.Value;
                _report.Physician = PhysicianTextBox.Text;
                _report.Technician = TechnicianTextBox.Text;

                // 心臓サイズ測定値の更新
                _report.AoD = ParseDoubleOrNull(AoDTextBox.Text);
                _report.LAD = ParseDoubleOrNull(LADTextBox.Text);
                _report.LVIDd = ParseDoubleOrNull(LVIDdTextBox.Text);
                _report.LVIDs = ParseDoubleOrNull(LVIDsTextBox.Text);
                _report.IVSTd = ParseDoubleOrNull(IVSTdTextBox.Text);
                _report.LVPWTd = ParseDoubleOrNull(LVPWTdTextBox.Text);
                _report.IVD = ParseDoubleOrNull(IVDTextBox.Text);

                // LVH、ASHの更新
                _report.LVH = LVHPositiveRadioButton.IsChecked == true;
                _report.ASH = ASHPositiveRadioButton.IsChecked == true;

                // EF関連データの更新
                _report.TeichholtzEF = ParseDoubleOrNull(TeichholtzEFTextBox.Text);
                _report.PomboEF = ParseDoubleOrNull(PomboEFTextBox.Text);
                _report.SimpsonEF = ParseDoubleOrNull(SimpsonEFTextBox.Text);
                _report.FS = ParseDoubleOrNull(FSTextBox.Text);

                // MV flowの更新
                _report.E = ParseDoubleOrNull(ETextBox.Text);
                _report.A = ParseDoubleOrNull(ATextBox.Text);
                _report.E_A = ParseDoubleOrNull(E_ATextBox.Text);
                _report.DecT = ParseDoubleOrNull(DecTTextBox.Text);
                _report.E_Eprime = ParseDoubleOrNull(E_EprimeTextBox.Text);

                // 弁膜評価の更新
                _report.MVP = MVPPositiveRadioButton.IsChecked == true;
                _report.MVPDetails = MVPDetailsTextBox.Text;
                _report.MVCalc = MVCalcPositiveRadioButton.IsChecked == true;
                _report.MVCalcDetails = MVCalcDetailsTextBox.Text;
                _report.MS = MSPositiveRadioButton.IsChecked == true;
                _report.Doomine = DoominePositiveRadioButton.IsChecked == true;
                _report.MVAByPHT = ParseDoubleOrNull(MVAByPHTTextBox.Text);
                _report.MVAByPlanimetry = ParseDoubleOrNull(MVAByPlanimetryTextBox.Text);
                _report.AVCalc = AVCalcPositiveRadioButton.IsChecked == true;
                _report.AVCalcDetails = AVCalcDetailsTextBox.Text;
                _report.AS = ASPositiveRadioButton.IsChecked == true;
                _report.PVE = ParseDoubleOrNull(PVETextBox.Text);
                _report.AVAByPlanimetry = ParseDoubleOrNull(AVAByPlanimetryTextBox.Text);
                _report.AVAByEquation = ParseDoubleOrNull(AVAByEquationTextBox.Text);
                _report.MeanPG = ParseDoubleOrNull(MeanPGTextBox.Text);
                _report.MaxPG = ParseDoubleOrNull(MaxPGTextBox.Text);

                // 逆流評価の更新
                _report.MRGrade = GetRegurgitationGradeString(MRNoneCheckBox, MRTraceCheckBox, MRMildCheckBox,
                                                             MRModerateCheckBox, MRSevereCheckBox, MRNdCheckBox);
                _report.ARGrade = GetRegurgitationGradeString(ARNoneCheckBox, ARTraceCheckBox, ARMildCheckBox,
                                                             ARModerateCheckBox, ARSevereCheckBox, ARNdCheckBox);
                _report.TRGrade = GetRegurgitationGradeString(TRNoneCheckBox, TRTraceCheckBox, TRMildCheckBox,
                                                             TRModerateCheckBox, TRSevereCheckBox, TRNdCheckBox);
                _report.PRGrade = GetRegurgitationGradeString(PRNoneCheckBox, PRTraceCheckBox, PRMildCheckBox,
                                                             PRModerateCheckBox, PRSevereCheckBox, PRNdCheckBox);
                _report.PHT = ParseDoubleOrNull(PHTTextBox.Text);
                _report.PG = ParseDoubleOrNull(PGTextBox.Text);

                // その他所見の更新
                _report.Asynergy = AsynergyPositiveRadioButton.IsChecked == true;
                _report.AsynergyDetails = AsynergyDetailsTextBox.Text;
                _report.PE = PEPositiveRadioButton.IsChecked == true;
                _report.PEDetails = PEDetailsTextBox.Text;
                _report.Shunt = ShuntPositiveRadioButton.IsChecked == true;
                _report.ShuntDetails = ShuntDetailsTextBox.Text;

                // 所見・診断の更新
                _report.Findings = FindingsTextBox.Text;
                _report.Diagnosis = DiagnosisTextBox.Text;
                _report.Recommendations = RecommendationsTextBox.Text;

                if (!_isEditMode)
                {
                    _context.EchoReports.Add(_report);
                }

                _context.SaveChanges();
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"エラーが発生しました: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private string GetRegurgitationGradeString(CheckBox none, CheckBox trace, CheckBox mild,
                                          CheckBox moderate, CheckBox severe, CheckBox nd)
        {
            List<string> grades = new List<string>();

            if (none.IsChecked == true) grades.Add("-");
            if (trace.IsChecked == true) grades.Add("trace");
            if (mild.IsChecked == true) grades.Add("mild");
            if (moderate.IsChecked == true) grades.Add("moderate");
            if (severe.IsChecked == true) grades.Add("severe");
            if (nd.IsChecked == true) grades.Add("n.d.");

            return string.Join("〜", grades);
        }
        private double? ParseDoubleOrNull(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            if (double.TryParse(text, out double result))
                return result;

            return null;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}