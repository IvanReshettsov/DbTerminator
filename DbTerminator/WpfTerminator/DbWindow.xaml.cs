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

        private void ShowRows(IEnumerable<Row> rows)
        {
            dataGrid.ItemsSource = null;
            dataGrid.Items.Clear();
            if (rows!=null && rows.Count() > 0)
                {
                    var ds = new DataSet();
                    var tbl = new DataTable();
                    ds.Tables.Add(tbl);

                    var firstRow = rows.First();

                    foreach (var col in firstRow.Columns)
                        tbl.Columns.Add(col.Name);

                    foreach (var scannedRow in rows)
                    {
                        var row = tbl.NewRow();

                        foreach (var col in scannedRow.Columns)
                            row[col.Name] = scannedRow[col];

                        tbl.Rows.Add(row);
                    }

                    dataGrid.ItemsSource = tbl.AsDataView();
                }
            
            
        }

        private void treeView_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var t = sender.GetType();
            if (sender is System.Windows.Controls.TreeView && treeView.SelectedItem as TreeViewItem!=null)
            {
                ShowRows(_dbRepository.LoadTable((treeView.SelectedItem as TreeViewItem).Header.ToString()));
            }
        }
    }
}
