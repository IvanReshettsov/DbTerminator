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
using System.Windows.Shapes;

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
            
            _dbRepository.addTablesNode(treeView, treeView.ContextMenu);
            _dbRepository.addViewsNode(treeView, treeView.ContextMenu);
            _dbRepository.addProgrammabilityNode(treeView, treeView.ContextMenu);

        }
    }
}
