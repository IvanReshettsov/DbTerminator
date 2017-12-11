using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using OrcaMDF.Core.Engine;

namespace WpfTerminator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
    }
}
