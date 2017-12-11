using OrcaMDF.Core.Engine;
using System;
using System.Windows.Controls;
using System.Linq;
using OrcaMDF.Core.MetaData;
using System.Collections.Generic;
using System.Data;

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
                tableNode.Tag = "table";
                tableNode.Header = t.Name;
                treeViewItem.Items.Add(tableNode);

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
                viewNode.Tag = "view";
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
                var procedureNode = new TreeViewItem();
                procedureNode.Tag = "procedure";
                procedureNode.Header = item.Name;
                proceduresNode.Items.Add(procedureNode);
            }
            return proceduresNode;
        }

        public DataView LoadTable(string table)
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

            if (rows != null && rows.Count() > 0)
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

                return tbl.AsDataView();
            } else {
                return null;
            }
        }

        public DataView LoadViewCode(string viewName)
        {
            string view = null;

            try
            {
                int objID = _db.Dmvs.Views
                .Where(p => p.Name == viewName)
                .Select(p => p.ObjectID)
                .Single();

                view = _db.Dmvs.SqlModules
                    .Where(m => m.ObjectID == objID)
                    .Select(m => m.Definition)
                    .Single();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return ConvertToDataView(view);
        }

        public DataView LoadProcedureCode(string procedureName)
        {
            string procedure = null;

            try
            {
                int objID = _db.Dmvs.Procedures
                .Where(p => p.Name == procedureName)
                .Select(p => p.ObjectID)
                .Single();

                procedure = _db.Dmvs.SqlModules
                    .Where(m => m.ObjectID == objID)
                    .Select(m => m.Definition)
                    .Single();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return ConvertToDataView(procedure);
        }

        private DataView ConvertToDataView(string code)
        {
            var tbl = new DataTable();
            tbl.Columns.Add("Code");
            tbl.Rows.Add(code == null ? "The code can not be shown for this item." : code);
            return tbl.AsDataView();
        }
    }
}
