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
    }
}
