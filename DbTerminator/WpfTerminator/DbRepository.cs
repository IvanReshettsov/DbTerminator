using OrcaMDF.Core.Engine;
using System;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using OrcaMDF.Core.MetaData;
using System.Collections.Generic;

namespace WpfTerminator
{
    public class DbRepository
    {
        private Database _db;

        public DbRepository(Database db)
        {
            _db = db;
        }

        public TreeViewItem createTablesNode()
        {
            var treeViewItem = new TreeViewItem();
            treeViewItem.Header = "Tables";
            var tables = _db.Dmvs.Tables.OrderBy(t => t.Name);

            foreach (var t in tables)
            {
                var tableNode = new TreeViewItem();
                tableNode.Header = t.Name;
                treeViewItem.Items.Add(tableNode);
                // Add columns
                var tableColumnsNode = new TreeViewItem();
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
                var tableIndexesNode = new TreeViewItem();
                tableIndexesNode.Header = "Indexes";
                tableNode.Items.Add(tableIndexesNode);
                var indexes = _db.Dmvs.Indexes
                    .Where(i => i.ObjectID == t.ObjectID && i.IndexID > 0)
                    .OrderBy(i => i.Name);

                foreach (var i in indexes)
                {
                    var indexNode = new TreeViewItem();
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
            return treeViewItem;
        }

        public TreeViewItem createViewsNode()
        {
            var viewsNode = new TreeViewItem();
            viewsNode.Header = "Views";
            var views = _db.Dmvs.Views.OrderBy(v => v.Name);

            foreach (var view in views)
            {
                var viewNode = new TreeViewItem();
                viewNode.Header = view.Name;
                viewsNode.Items.Add(viewNode);
            }
            return viewsNode;
        }

        public TreeViewItem createProgrammabilityNode()
        {
            var proceduresNode = new TreeViewItem();
            proceduresNode.Header = "Stored Procedures";
            var procedures = _db.Dmvs.Procedures.OrderBy(p => p.Name);

            foreach (var item in procedures)
            {
                var procNode = new TreeViewItem();
                procNode.Header = item.Name;
                proceduresNode.Items.Add(procNode);
            }
            return proceduresNode;
        }

        public IEnumerable<Row> LoadTable(string table)
        {
            IEnumerable<Row> rows = null;
            try
            {
                var scanner = new DataScanner(_db);
                rows = scanner.ScanTable(table).Take(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return rows;
        }
    }
}
