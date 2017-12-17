using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using OrcaMDF.Core.Engine;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace WpfTerminator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public event Action OnRecentlyViewed;
        public MainWindow()
        {
            InitializeComponent();
            ShowRecentFiles();


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
                SaveViewedFiles(textBox.Text);
                Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void loadDemoButton_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text = @"..\..\Resources\AdventureWorks2012.mdf";
            loadButton_Click(sender, e);
        }

        public void SaveViewedFiles(string file)
        {
            
            File.AppendAllText(@"..\..\files.json",JsonConvert.SerializeObject(file)/*+ Environment.NewLine*/);
            

        }
        public List<String> DeserializeFiles()
        {
            List<string> _files = new List<string>();
            using (StreamReader sr = File.OpenText(@"..\..\files.json"))
            {
                JsonSerializer serializer = new JsonSerializer();

                using (JsonTextReader jr = new JsonTextReader(sr))
                {
                    while (jr.Read())
                    {
                        var file = (string)serializer.Deserialize(jr, typeof(string));
                        _files.Add(file);
                    }
                }
            }

            return _files;
         
        }

        public void ShowRecentFiles()
        {
            foreach (var item in DeserializeFiles())
            {
                listBox.Items.Add(item);
            }
        }

        private void listBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            textBox.Text = listBox.SelectedItem as string;
        }
    }
}
