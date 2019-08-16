using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csvManager.UndoCommand
{ 
    public interface Command
    {
        /// <summary>
        /// データテーブル
        /// </summary>
        DataTable F_DataTable { get; set; }

        /// <summary>
        /// コマンドの実行
        /// </summary>
        void Execute();
    }
}
