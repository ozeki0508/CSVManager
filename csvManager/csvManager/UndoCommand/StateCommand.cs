using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csvManager.UndoCommand
{
    /// <summary>
    /// 状態保持用
    /// </summary>
    public class StateCommand : Command
    {
        public DataTable F_DataTable { get; set; }

        /// <summary>
        /// 現在の状態を保持しておくスタック
        /// </summary>
        public Stack<Command> CurrentCommands;

        /// <summary>
        /// 元に戻した状態を保持しておくスタック
        /// </summary>
        public Stack<Command> UndoCommands;

        public StateCommand()
        {
            this.CurrentCommands = new Stack<Command>();
            this.UndoCommands = new Stack<Command>();
        }

        /// <summary>
        /// 実行
        /// </summary>
        public void Execute()
        {
            var firstCmd = this.CurrentCommands.FirstOrDefault();
            firstCmd?.Execute();
        }

        /// <summary>
        /// 追加
        /// </summary>
        public void Append(Command cmd)
        {
            if(cmd != null && cmd != this)
            {
                this.CurrentCommands.Push(cmd);
            }
        }

        /// <summary>
        /// 元に戻す
        /// </summary>
        public void Undo()
        {
            if (this.CurrentCommands.Any())// Any()がなかった場合、例外
            {
                var undoCmd = this.CurrentCommands.Pop();

                if (undoCmd != null && undoCmd != this)
                {
                    this.UndoCommands.Push(undoCmd);
                }
            }
        }

        /// <summary>
        /// 前に進む
        /// </summary>
        public void Redo()
        {
            if(this.UndoCommands.Any())
            {
                var redoCmd = this.UndoCommands.Pop();

                if (redoCmd != null && redoCmd != this)
                {
                    this.CurrentCommands.Push(redoCmd);
                }
            }
        }
    }
}
