using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
                    //var db = new Database(files);
                    textBox.Text = String.Join(", ", files.ToArray());
                    //refreshTreeview();
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
                this.Close();
            }
            catch
            {
                Console.WriteLine("Error!");
            }
        }
    }
}
