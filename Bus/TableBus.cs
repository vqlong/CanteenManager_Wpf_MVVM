using Interface;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace Bus
{
    public class TableBus
    {
        private TableBus()
        {

        }
        private static readonly TableBus instance = new TableBus();
        public static TableBus Instance => instance;
        ITableData Table => Config.Container.Resolve<ITableData>();

        public bool CombineTable(int firstTableId, int secondTableId) => Table.CombineTable(firstTableId, secondTableId);
        public List<Table> GetListTable() => Table.GetListTable();
        public List<Table> GetListTableUsing() => Table.GetListTableUsing();
        public Table GetTableById(int tableId) => Table.GetTableById(tableId);
        public Table InsertTable() => Table.InsertTable();
        public bool SwapTable(int firstTableId, int secondTableId) => Table.SwapTable(firstTableId, secondTableId);
        public bool UpdateTable(int id, string name, UsingState usingState) => Table.UpdateTable(id, name, usingState);
        public bool DeleteTable(int id) => Table.DeleteTable(id);
    }
}
