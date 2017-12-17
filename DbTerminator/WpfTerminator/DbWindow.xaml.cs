using System.Windows;
using System.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OrcaMDF.Core.Engine;
using OrcaMDF.Core.MetaData;
using System.Windows.Controls;
using ModelTerminator;

namespace WpfTerminator
{
    /// <summary>
    /// Логика взаимодействия для DbWindow.xaml
    /// </summary>
    
    public partial class DbWindow : Window
    {

        private DbRepository _dbRepository;
        
        public DbWindow(DbRepository dbRepository)
        {       
            InitializeComponent();    
            _dbRepository = dbRepository;
            UpdateTreeView();
        }

        public void UpdateTreeView()
        {
            treeView.Items.Clear();
            treeView.Items.Add(_dbRepository.createTablesNode());
            treeView.Items.Add(_dbRepository.createViewsNode());
            treeView.Items.Add(_dbRepository.createProgrammabilityNode());
        }

        private void ShowTable(DataView table)
        {
            dataGrid.ItemsSource = null;
            dataGrid.Items.Clear();

            dataGrid.ItemsSource = table;   
        }

        private void treeView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.TreeView && treeView.SelectedItem as TreeViewItem != null)
            {
                var tvi = treeView.SelectedItem as TreeViewItem;

                if (tvi.Tag != null)
                {
                    if (tvi.Tag.ToString() == "table")
                    {
                        ShowTable(_dbRepository.LoadTable((treeView.SelectedItem as TreeViewItem).Header.ToString()));
                    }
                    else if (tvi.Tag.ToString() == "view")
                    {
                        // ex. vSalesPerson, Views.vPersonDemographics in Adventure Works
                        ShowTable(_dbRepository.LoadViewCode((treeView.SelectedItem as TreeViewItem).Header.ToString()));
                    }
                    else if (tvi.Tag.ToString() == "procedure")
                    {
                        ShowTable(_dbRepository.LoadProcedureCode((treeView.SelectedItem as TreeViewItem).Header.ToString()));
                    }
                }
            }
        }
    }
}
