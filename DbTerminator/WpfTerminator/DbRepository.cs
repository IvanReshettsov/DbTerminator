using OrcaMDF.Core.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfTerminator
{
    public class DbRepository
    {
        private Database _db;

        public DbRepository(Database db)
        {
            _db = db;

        }

        public void addTablesNode(System.Windows.Controls.TreeView rootNode, System.Windows.Controls.ContextMenu tableMenu)
        {
            var treeViewItem = new System.Windows.Controls.TreeViewItem();
            treeViewItem.Header = "Tables";
            rootNode.Items.Add(treeViewItem);
            var tables = _db.Dmvs.Tables.OrderBy(t => t.Name);

            foreach (var t in tables)
            {
                var tableNode = new System.Windows.Controls.TreeViewItem();
                tableNode.Header = t.Name;
                treeViewItem.Items.Add(tableNode);
                tableNode.ContextMenu = tableMenu;
                // Add columns
                var tableColumnsNode = new System.Windows.Controls.TreeViewItem();
                tableColumnsNode.Header = "Columns"; 
                tableNode.Items.Add(tableColumnsNode);
                var columns = _db.Dmvs.Columns
                    .Where(c => c.ObjectID == t.ObjectID)
                    .OrderBy(c => c.Name);

                foreach (var c in columns)
                {
                    var mainColumn = _db.Dmvs.Columns.Where(x => x.ColumnID == c.ColumnID && x.ObjectID == c.ObjectID).Single();
                    var type = _db.Dmvs.Types.Where(x => x.SystemTypeID == mainColumn.SystemTypeID).First();

                    tableColumnsNode.Items.Add(c.Name + " (" + type.Name + "[" + type.MaxLength + "])");
                }

                // Add indexes
                var tableIndexesNode = new System.Windows.Controls.TreeViewItem();
                tableIndexesNode.Header = "Indexes";
                tableNode.Items.Add(tableIndexesNode);
                var indexes = _db.Dmvs.Indexes
                    .Where(i => i.ObjectID == t.ObjectID && i.IndexID > 0)
                    .OrderBy(i => i.Name);

                foreach (var i in indexes)
                {
                    var indexNode = new System.Windows.Controls.TreeViewItem();
                    tableIndexesNode.Items.Add(i.Name);

                    // Add index columns
                    var indexColumns = _db.Dmvs.IndexColumns
                        .Where(ic => ic.ObjectID == t.ObjectID && ic.IndexID == i.IndexID);

                    foreach (var ic in indexColumns)
                    {
                        var mainColumn = _db.Dmvs.Columns.Where(x => x.ColumnID == ic.ColumnID && x.ObjectID == ic.ObjectID).Single();
                        var type = _db.Dmvs.Types.Where(x => x.SystemTypeID == mainColumn.SystemTypeID).First();

                        indexNode.Items.Add(columns.Where(c => c.ColumnID == ic.ColumnID).Single().Name + " (" + type.Name + "[" + type.MaxLength + "])");
                    }
                }
            }
        }

        public void addViewsNode(System.Windows.Controls.TreeView rootNode, System.Windows.Controls.ContextMenu viewMenu)
        {
            var viewsNode = new System.Windows.Controls.TreeViewItem();
            viewsNode.Header = "Views";
            rootNode.Items.Add(viewsNode);
            var views = _db.Dmvs.Views.OrderBy(v => v.Name);

            foreach (var view in views)
            {
                var viewNode = new System.Windows.Controls.TreeViewItem();
                viewNode.Header = view.Name;
                viewsNode.Items.Add(viewNode);
                viewNode.ContextMenu = viewMenu;
            }
        }

        public void addProgrammabilityNode(System.Windows.Controls.TreeView rootNode, System.Windows.Controls.ContextMenu procedureMenu)
        {
            var proceduresNode = new System.Windows.Controls.TreeViewItem();
            proceduresNode.Header = "Stored Procedures";
            rootNode.Items.Add(proceduresNode);
            var procedures = _db.Dmvs.Procedures.OrderBy(p => p.Name);

            foreach (var item in procedures)
            {
                var procNode = new System.Windows.Controls.TreeViewItem();
                procNode.Header = item.Name;
                proceduresNode.Items.Add(procNode);
                procNode.ContextMenu = procedureMenu;
            } 
        }
        
    }
}
