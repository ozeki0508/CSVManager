using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csvManager.UndoCommand
{
    /// <summary>
    /// 画面状態用
    /// </summary>
    public class ViewCommand : Command
    {
        public DataTable F_DataTable { get; set; }

        /// <summary>
        /// 入力項目を編集
        /// </summary>
        public void Execute()
        {
        }
    }
}
