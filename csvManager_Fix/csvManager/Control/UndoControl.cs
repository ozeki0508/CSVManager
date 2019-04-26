using System.Collections.Generic;
using System.Linq;

//前提として、処理をひとつ終えるごとにコマンドを保存する
//そのメソッドをここで作成し、Form側で呼び出す
//Undoボタンを押したら、Undoされるように
//staticが多いので初期化位置が分かりにくい
namespace csvManager
{
    class UndoControl
    {
        #region デリゲート・変数・コンストラクタ

        /// <summary>
        /// デリゲート
        /// </summary>
        public delegate void RecordableAction();
        /// <summary>
        /// すべての状態
        /// </summary>
        private static List<State> States = new List<State>();
        private static object Lock = new object();

        /// <summary>
        /// 静的コンストラクタ
        /// 最初に静的メンバが参照される前に自動で呼び出される
        /// </summary>
        static UndoControl()
        {
            States.Add(new State());
        }
        #endregion

        #region 状態内部クラス
        internal class State
        {
            /// <summary>
            /// 前の状態
            /// </summary>
            public State PrevState { get; set; }
            /// <summary>
            /// 次の状態
            /// </summary>
            public State NextState { get; set; }
            /// <summary>
            /// 前へ戻るコマンド
            /// </summary>
            public Command PrevCommand { get; set; }
            /// <summary>
            /// 次へ進むコマンド
            /// </summary>
            public Command NextCommand { get; set; }
        }
        #endregion

        #region コマンド内部クラス
        //本来はCommandクラスを抽象クラスとして、
        //Commandクラスを継承していろんな要求を実行できるようにするが、
        //今回は必要ないので、抽象クラスを作成しない
        internal class Command
        {
            #region 変数・プロパティ・コンストラクタ
            //イベント
            protected event RecordableAction _Do;
            protected event RecordableAction _Undo;

            /// <summary>
            /// 現在の状態を表すインデックス
            /// </summary>
            private static int _CurrentStateIndex = 0;

            public Command(RecordableAction act, RecordableAction undo)
            {
                _Do = act;
                _Undo = undo;
            }

            /// <summary>
            /// 現在の状態を表すインデックス、プロパティ
            /// </summary>
            private static int CurrentStateIndex
            {
                get
                {
                    return _CurrentStateIndex;
                }
                set
                {
                    if(_CurrentStateIndex != value)
                    {
                        _CurrentStateIndex = value;
                    }
                }
            }
            #endregion

            #region メソッド
            /// <summary>
            /// 現在の状態を取得
            /// </summary>
            private static State CurrentState
            {
                get
                {
                    State result = null;
                    if (States.Count > 0)
                    {
                        result = States.ElementAt(CurrentStateIndex);
                    }
                    return result;
                }
            }
            private void ExecuteAction()
            {
                _Do?.Invoke();
            }
            private void UndoAction()
            {
                _Undo?.Invoke();
            }

            /// <summary>
            /// アクションを記憶する
            /// </summary>
            /// <param name="act">進んだときの一処理</param>
            /// <param name="undo">戻ったときの状態</param>
            public static void Record(RecordableAction act, RecordableAction undo)
            {
                lock(Lock)  //排他制御
                {
                    Command cmd = new Command(act, undo);
                    cmd.ExecuteAction();

                    State newState = new State();
                    //前へ戻る処理に代入
                    newState.PrevCommand = cmd;
                    //追加
                    AddNewState(newState);
                }
            }

            /// <summary>
            /// 引数の状態を更新しカレント状態とする
            /// </summary>
            /// <param name="newState"></param>
            private static void AddNewState(State newState)
            {
                //次の状態を取得
                State nextState = CurrentState?.NextState;
                //次の状態があれば、排除する
                if(nextState != null)
                {
                    EliminateStates(nextState);
                }

                /***状態を新しく更新***/

                CurrentState.NextState = newState;
                //現在の状態のRedo = 次の状態のUndo
                //は同じもの！！！
                CurrentState.NextCommand = newState.PrevCommand;
                //現在の状態は次の状態の前の状態にあたる
                newState.PrevState = CurrentState;
                States.Add(newState);

                //次の状態をカレント状態とする
                CurrentStateIndex = States.IndexOf(newState);
            }

            /// <summary>
            /// 引数にある「状態」から後ろ存在する状態を排除する
            /// </summary>
            private static void EliminateStates(State begin)
            {
                if (begin != null)
                {
                    int beginIndex = States.IndexOf(begin);
                    if (beginIndex > 0)
                    {
                        //状態の個数 - 引数にある「状態」があるインデックス
                        int removeCount = States.Count - beginIndex;
                        //beginIndexが指す位置から指定の個数分を削除
                        States.RemoveRange(beginIndex, removeCount);
                    }
                }
            }
            /// <summary>
            /// 戻る処理
            /// </summary>
            public static void Undo()
            {
                lock(Lock)
                {
                    State prev = CurrentState.PrevState;
                    if(prev != null)
                    {
                        CurrentState.PrevCommand?.UndoAction();
                        //前の状態をカレント状態とする
                        CurrentStateIndex = States.IndexOf(prev);
                    }
                }
            }
            /// <summary>
            /// 進む処理
            /// </summary>
            public static void Redo()
            {
                lock(Lock)
                {
                    State next = CurrentState.NextState;
                    if(next != null)
                    {
                        CurrentState.NextCommand?.ExecuteAction();
                        //次の状態をカレント状態とする
                        CurrentStateIndex = States.IndexOf(next);
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}
