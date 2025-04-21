using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Microsoft.Win32;
using UCGReportSystem.Models;

namespace UCGReportSystem.Views
{
    public partial class PrintReportWindow : Window
    {
        private readonly EchoReport _report;
        private ReportPreviewWindow _previewWindow;

        public PrintReportWindow(EchoReport report)
        {
            InitializeComponent();
            _report = report;

            // プレビューウィンドウの作成
            _previewWindow = new ReportPreviewWindow(report);

            // 印刷ダイアログを表示
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                PrintReport(printDialog);
            }
            else
            {
                Close();
            }
        }

        private void PrintReport(PrintDialog printDialog)
        {
            try
            {
                // プレビューウィンドウからコンテンツを取得
                FrameworkElement reportPanel = _previewWindow.FindName("ReportPanel") as FrameworkElement;

                if (reportPanel != null)
                {
                    // 印刷のための準備
                    reportPanel.Measure(new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight));
                    reportPanel.Arrange(new Rect(new Point(0, 0), reportPanel.DesiredSize));

                    // 印刷
                    printDialog.PrintVisual(reportPanel, "心エコーレポート");
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"印刷エラー: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // キャンセルボタンのクリックイベントハンドラを追加
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        // PDFへのエクスポート機能（オプション）
        private void ExportToPdf()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDFファイル (*.pdf)|*.pdf",
                    Title = "レポートをPDFに保存"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    // プレビューウィンドウからコンテンツを取得
                    FrameworkElement reportPanel = _previewWindow.FindName("ReportPanel") as FrameworkElement;

                    if (reportPanel != null)
                    {
                        // PDFのためのXPS作成
                        XpsDocument xpsDoc = new XpsDocument(saveFileDialog.FileName, FileAccess.ReadWrite);
                        XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);

                        writer.Write(reportPanel);
                        xpsDoc.Close();

                        MessageBox.Show("PDFエクスポートが完了しました。", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"PDFエクスポートエラー: {ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}