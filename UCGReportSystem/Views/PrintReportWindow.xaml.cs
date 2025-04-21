using System;
using System.IO;
using System.Printing;
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

            // ウィンドウが読み込まれたときに印刷ダイアログを表示する
            this.Loaded += PrintReportWindow_Loaded;
        }

        private void PrintReportWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // このイベントハンドラは一度だけ実行されるようにする
            this.Loaded -= PrintReportWindow_Loaded;

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
                    // 現在のサイズを保存
                    double originalWidth = reportPanel.Width;

                    // プリンターの印刷可能領域に合わせる
                    reportPanel.Width = printDialog.PrintableAreaWidth;

                    // 印刷前に再レイアウト
                    reportPanel.Measure(new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight));
                    reportPanel.Arrange(new Rect(new Point(0, 0), reportPanel.DesiredSize));

                    // 印刷
                    printDialog.PrintVisual(reportPanel, "心エコーレポート");

                    // 元のサイズに戻す
                    reportPanel.Width = originalWidth;
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
    public class ReportPaginator : DocumentPaginator
    {
        private FrameworkElement _element;
        private Size _pageSize;
        private Thickness _margin;
        private int _pageCount;

        public ReportPaginator(FrameworkElement element, Size pageSize, Thickness margin)
        {
            _element = element;
            _pageSize = pageSize;
            _margin = margin;

            // Calculate how many pages we need
            double totalHeight = element.DesiredSize.Height;
            double printableHeight = pageSize.Height - margin.Top - margin.Bottom;
            _pageCount = (int)Math.Ceiling(totalHeight / printableHeight);
        }

        public override DocumentPage GetPage(int pageNumber)
        {
            if (pageNumber >= _pageCount)
                throw new ArgumentOutOfRangeException("pageNumber");

            // Create a visual for this specific page
            var printableHeight = _pageSize.Height - _margin.Top - _margin.Bottom;
            var printableWidth = _pageSize.Width - _margin.Left - _margin.Right;

            // Create a container for our content
            FixedPage page = new FixedPage();
            page.Width = _pageSize.Width;
            page.Height = _pageSize.Height;

            // Clone the element for this page
            FrameworkElement pageElement = CloneElement(_element);
            pageElement.Width = printableWidth;

            // Position the content for this specific page
            Canvas.SetLeft(pageElement, _margin.Left);
            Canvas.SetTop(pageElement, _margin.Top - (pageNumber * printableHeight));

            // Add content to the page
            page.Children.Add(pageElement);
            page.Measure(new Size(_pageSize.Width, _pageSize.Height));
            page.Arrange(new Rect(0, 0, _pageSize.Width, _pageSize.Height));

            return new DocumentPage(page);
        }

        // A simple method to clone the visual element
        private FrameworkElement CloneElement(FrameworkElement original)
        {
            // For this implementation, we'll create a container and use a VisualBrush
            // to "clone" the visual appearance of the original element
            Border container = new Border();
            container.Width = original.ActualWidth;
            container.Height = original.ActualHeight;
            container.Background = new VisualBrush(original);
            return container;
        }

        public override bool IsPageCountValid => true;
        public override int PageCount => _pageCount;
        public override Size PageSize
        {
            get => _pageSize;
            set => _pageSize = value;
        }
        public override IDocumentPaginatorSource Source => null;
    }
}