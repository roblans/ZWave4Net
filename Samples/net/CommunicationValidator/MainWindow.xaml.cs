using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunicationValidator.Models;
using Microsoft.Win32;

namespace CommunicationValidator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private LogRow[] rows = null;

        public MainWindow()
        {
            InitializeComponent();

            this.SizeChanged += MainWindow_SizeChanged;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var file = GetFile();
                if (file == null) return; // open file aborted

                rows = ReadLog(file);
                ProcessLog(rows, out Node[] nodes);

                txtStatus.Text = $"{rows.Count(r => r.IsNakOrCan)} fails detected";
                txtLogFile.Text = System.IO.Path.GetFileName(file);
                itmRowViewer.ItemsSource = rows;
                itmNodes.ItemsSource = nodes;

                StatusIndicatorBar.Content = new LineStatusIndicator(rows, StatusIndicatorBar.ActualHeight);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading/processing file. Exception: {ex.Message}");
            }
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (rows != null)
                StatusIndicatorBar.Content = new LineStatusIndicator(rows, StatusIndicatorBar.ActualHeight);
        }

        private string GetFile()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = false
                // todo: dialog.Filter
            };

            if (dialog.ShowDialog() != true)
                return null;
            return dialog.FileName;
        }

        private Models.LogRow[] ReadLog(string file)
        {
            var lines = System.IO.File.ReadLines(file);
            var rows = new List<Models.LogRow>();

            foreach (var line in lines)
            {
                rows.Add(new LogRow(line));
            }

            return rows.ToArray();
        }

        private void ProcessLog(LogRow[] rows, out Node[] nodes)
        {
            // validate Ack on incomming messages
            // check for duplicate incomming messages
            // check for requests without response

            var foundNodes = new List<Node>();
            // build a node communication matrix
            foreach (var node in rows.Where(el => el.NodeID.HasValue && !string.IsNullOrEmpty(el.Command)).GroupBy(el => el.NodeID.Value))
            {
                foundNodes.Add(new Node()
                {
                    NodeID = node.Key,
                    CommandClassCommunications = node.GroupBy(el => el.Command).OrderBy(el => el.Key).Select(c => new CommandClassCommunication()
                    {
                        CommandClass = c.Key,
                        EventReceived = c.Count(el => el.Mode == CommunicationMode.Received),
                        RequestsSucceeded = c.Count(el => el.Mode == CommunicationMode.Send)
                    }).ToArray()
                });
            }
            nodes = foundNodes.ToArray();
        }
    }
}
