using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDataAccess
{
    public abstract class DataAccess
    {
        public override string ToString()
        {
            return base.ToString() + $" {GetHashCode()}";
        }
    }
}
