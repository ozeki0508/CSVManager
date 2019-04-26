using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace csvManager
{
    class Control
    {
        #region 変数・プロパティ・コンストラクタ

        /// <summary>
        /// ファイル名
        /// </summary>
        private string FileName;
        /// <summary>
        /// CSVデータプロパティ
        /// </summary>
        public CSVAllData CSVAllData{ get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Control()
        {
            CSVAllData = new CSVAllData();
            FileName = string.Empty;
        }
        #endregion

        #region メソッド
        /// <summary>
        /// コピー
        /// </summary>
        private void Copy(object sender)
        {
            DataGridView gridView = sender as DataGridView;
            if (gridView != null)
            {
                StringBuilder sb = new StringBuilder();

                //順に文字列結合して取得
                //(GetDataする際にひとつのデータのほうが扱える)
                foreach (DataGridViewCell cell in gridView.SelectedCells)
                {
                    sb.Append(cell.Value.ToString());
                }
                Clipboard.SetDataObject(sb.ToString());
            }
        }

        /// <summary>
        /// ペースト
        /// </summary>
        private void Paste(object sender)
        {
            DataGridView gridView = sender as DataGridView;
            if (gridView != null)
            {
                string[] pasteStr = this.MakePasteStr();

                //gridView.SelectedCellsを変数cellsに一旦入れて、
                //左上→右下の順にソート
                List<DataGridViewCell> cells = new List<DataGridViewCell>();
                foreach (DataGridViewCell cell in gridView.SelectedCells)
                {
                    cells.Add(cell);
                }
                this.CellsSort(cells);

                int cnt = 0;
                //順に貼り付ける
                foreach (DataGridViewCell cell in cells)
                {
                    if (cnt >= pasteStr.Length) break;  //選択セルのほうが多いならbreak
                    cell.Value = pasteStr[cnt];
                    cnt++;
                }
                //フォーカスを外して値を確定
                gridView.CurrentCell = null;
            }
        }

        /// <summary>
        /// ペーストする値を取得(左上→右下の順になっている)
        /// </summary>
        /// <returns></returns>
        private string[] MakePasteStr()
        {
            IDataObject idata = Clipboard.GetDataObject();
            string pasteVal = (string)idata.GetData(DataFormats.Text);
            string[] pasteStr = pasteVal.Split(new string[] { "\t", "\r\n" }, StringSplitOptions.None);
            return pasteStr;
        }
        /// <summary>
        /// 指定のセルを左上→右下にかけて順にソート
        /// SelectedCellsを指定の順にする
        /// </summary>
        private void CellsSort(List<DataGridViewCell> list)
        {
            for (int i = 1; i < list.Count; i++)
            {
                DataGridViewCell tmp = list[i];
                int j = i;
                while (j > 0 && this.CompareIndex(tmp, list[j - 1]))
                {
                    list[j] = list[j - 1];  //大きい方の値を一つ後ろにずらす
                    j--;    //手前の方へ確認していく
                }
                list[j] = tmp;  //インサート
            }
        }

        /// <summary>
        /// 2つのセルの列・行インデックスを比較し、cell1の方が小さければtrue
        /// 左上→右下の順
        /// </summary>
        /// <param name="cell1"></param>
        /// <param name="cell2"></param>
        /// <returns></returns>
        private bool CompareIndex(DataGridViewCell cell1, DataGridViewCell cell2)
        {
            bool result = false;
            //行インデックス
            if (cell1.RowIndex < cell2.RowIndex)
            {
                result = true;
            }
            else if (cell1.RowIndex == cell2.RowIndex)
            {
                //列インデックス
                if (cell1.ColumnIndex < cell2.ColumnIndex)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Undo処理
        /// </summary>
        private void Undo(object sender)
        {
            DataGridView gridView = sender as DataGridView;
            if (gridView != null)
            {
                //フォーカスを外して、値を確定
                gridView.CurrentCell = null;
                UndoControl.Command.Undo();
            }
        }

        private void Redo(object sender)
        {
            DataGridView gridView = sender as DataGridView;
            if (gridView != null)
            {
                //フォーカスを外して、値を確定
                gridView.CurrentCell = null;
                UndoControl.Command.Redo();
            }
        }

        /// <summary>
        /// キーを押下したときの処理
        /// </summary>
        /// <param name="e"></param>
        public void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                if (e.KeyCode == Keys.C)
                {
                    this.Copy(sender);
                }
                else if (e.KeyCode == Keys.V)
                {
                    this.Paste(sender);
                }
                else if(e.KeyCode == Keys.Z)
                {
                    this.Undo(sender);
                }
                else if(e.KeyCode == Keys.Y)
                {
                    this.Redo(sender);
                }
            }
        }

        /// <summary>
        /// ファイルがロックされているか
        /// </summary>
        /// <returns>true:されている</returns>
        private bool IsLockFile()
        {
            bool result = false;
            try
            {
                if (File.Exists(this.FileName))
                {
                    using (FileStream fs = new FileStream(this.FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
                }
            }
            catch (IOException)
            {
                result = true;
            }
            return result;
        }
        /// <summary>
        /// ファイルから読み込み
        /// </summary>
        public void InputFile(string fileName)
        {
            this.FileName = fileName;
            this.Read();
        }

        /// <summary>
        /// ファイル読み込み、実処理
        /// </summary>
        private void Read()
        {
            if (File.Exists(this.FileName))
            {
                if (!this.IsLockFile())
                {
                    using (FileStream fs = new FileStream(this.FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                    //using (StreamReader sr = new StreamReader(fs, new UTF8Encoding(false)))//BOMなし
                    using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("Shift_JIS")))
                    {
                        int maxVal = 0;
                        while (sr.Peek() >= 0)
                        {
                            string lineStr = sr.ReadLine();
                            if (string.IsNullOrEmpty(lineStr)) continue;//改行などはとばす

                            CSVColumnData itemsList = new CSVColumnData();
                            itemsList.MakeListBy(lineStr);
                            this.CSVAllData.Add(itemsList);  //一行分を追加

                            //一番多いカラム数を設定
                            if (maxVal < itemsList.Count) maxVal = itemsList.Count;
                        }
                        CSVAllData.MaxColumnsInAllRows = maxVal;
                    }
                }
            }
        }

        /// <summary>
        /// ファイルへ書き込み
        /// </summary>
        /// <returns>true:成功</returns>
        public bool OutputFile()
        {
            return this.Write();
        }
        /// <summary>
        /// ファイル書き込み、実処理
        /// </summary>
        /// <returns>true:成功</returns>
        private bool Write()
        {
            bool result = false;
            if (File.Exists(this.FileName))
            {
                if (!this.IsLockFile())
                {
                    //Create：上書きさせるようにする
                    using (FileStream fs = new FileStream(this.FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
                    //using (StreamWriter sw = new StreamWriter(fs, new UTF8Encoding(false)))//BOMなし
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("Shift_JIS")))
                    {
                        foreach (CSVColumnData lineData in this.CSVAllData)
                        {
                            string oneLine = lineData.MakeLineByThisDatas();
                            sw.WriteLine(oneLine);
                        }
                        sw.Flush();
                    }
                    result = true;
                }
            }
            return result;
        }
        #endregion
    }
}
