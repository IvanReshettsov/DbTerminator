using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using OrcaMDF.Core.Engine;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ModelTerminator;

namespace WpfTerminator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Repository _repository;

        public MainWindow()
        {
            InitializeComponent();

            _repository = new Repository();
            ShowViewedFiles(_repository.LoadViewedFiles());
        }
        
        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            var openDatabaseDialog = new OpenFileDialog();
            openDatabaseDialog.AddExtension = false;
            openDatabaseDialog.Filter = "SQL Server data files|*.mdf;*.ndf";
            openDatabaseDialog.Multiselect = true;

            var result = openDatabaseDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                try
                {                  
                    var files = openDatabaseDialog.FileNames;
                    textBox.Text = String.Join(", ", files.ToArray());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }


        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var db = new Database(textBox.Text);
                var dbWindow = new DbWindow(new DbRepository(db));
                dbWindow.Show();
                dbWindow.Title = db.Name;
                _repository.SaveViewedFile(textBox.Text);

                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void ShowViewedFiles(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                listBox.Items.Add(file);
            }
        }

        private void listBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (listBox.SelectedItems[0].ToString() == "Demo (adventure works 2012)") {
                textBox.Text = @"..\..\Resources\AdventureWorks2012.mdf";
            } else
            {
                textBox.Text = listBox.SelectedItem.ToString();
            }
        }
    }
}
