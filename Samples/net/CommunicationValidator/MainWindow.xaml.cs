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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var file = GetFile();
                if (file == null) return; // open file aborted

                var rows = ReadLog(file);
                ProcessLog(rows, out Node[] nodes);

                txtLogFile.Text = System.IO.Path.GetFileName(file);
                itmRowViewer.ItemsSource = rows;
                itmNodes.ItemsSource = nodes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading/processing file. Exception: {ex.Message}");
            }
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

            }

            // sample data
            //rows.Add(new Models.LogRow(new byte[] { 0x15 })); // NAK
            //rows.Add(new Models.LogRow(new byte[] { 0x18 })); // CAN
            //rows.Add(new Models.LogRow(new byte[] { 0x06 })); // ACK

            return rows.ToArray();
        }

        private void ProcessLog(LogRow[] rows, out Node[] nodes)
        {
            var foundNodes = new List<Node>();
            // validate Ack on incomming messages
            // check for duplicate incomming messages
            // check for requests without response

            // build a node communication matrix
            nodes = foundNodes.ToArray();
        }
    }
}
