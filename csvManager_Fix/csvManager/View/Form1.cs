using System;
using System.Data;
using System.Windows.Forms;

namespace csvManager
{
    public partial class Form1 : Form
    {
        #region 変数・プロパティ・コンストラクタ

        /// <summary>
        /// コントローラ
        /// </summary>
        private Control F_Control;
        ///// <summary>
        ///// データテーブル(GridDataViewと紐付ける)
        ///// </summary>
        //public DataTable DataTableForGrid { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form1()
        {
            F_Control = new Control();
            InitializeComponent();
        }
        #endregion

        #region メソッド

        /// <summary>
        /// 開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemOpen_Click(object sender, EventArgs e)
        {
            this.Open();
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemSave_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        /// <summary>
        /// 開く、実処理
        /// </summary>
        private void Open()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "CSVファイル(*.csv)|*.csv";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                F_Control.InputFile(dialog.FileName);
                this.Display();
            }
        }
        /// <summary>
        /// 保存、実処理
        /// </summary>
        private void Save()
        {
            //以下のようにNullにすると、セルからフォーカスがはずれ編集中の値が確定する
            this.dataGridView1.CurrentCell = null;

            this.MakeAllData(this.dataGridView1);
            if (F_Control.OutputFile()) MessageBox.Show("保存しました。");
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            F_Control.KeyDown(sender, e);
        }
        #endregion

        #region メソッド　表示用
        /// <summary>
        /// Gridへ表示
        /// DataGridViewのDataSourceに紐付けると表示可能
        /// </summary>
        private void Display()
        {
            CSVAllData allData = F_Control.CSVAllData;
            //カラム設定
            this.SetColumnIndex(allData.MaxColumnsInAllRows);

            //レコード追加
            foreach (CSVColumnData lineData in allData)
            {
                string[] rowData = lineData.ToArray();
                F_Control.DataTableForGrid.Rows.Add(rowData);
            }

            //DataSourceへ設定
            //表示したDataTableを記録し、最新データをDataSourceに登録
            RecordedDataTables rcdData = F_Control.RecordedData;

            rcdData.Add(F_Control.DataTableForGrid);
            dataGridView1.DataSource = rcdData[rcdData.Count - 1];
        }

        /// <summary>
        /// カラム設定、数値を入れる
        /// </summary>
        /// <param name="cnt"></param>
        private void SetColumnIndex(int cnt)
        {
            F_Control.DataTableForGrid = new DataTable();
            for (int i = 1; i <= cnt; i++)
            {
                F_Control.DataTableForGrid.Columns.Add(i.ToString());
            }
        }

        ////例外の確認

        /// <summary>
        /// グリッドのすべてのデータを作成
        /// </summary>
        /// <param name="dataGridView"></param>
        private void MakeAllData(DataGridView dataGridView)
        {
            if (F_Control.DataTableForGrid != null)
            {
                F_Control.CSVAllData = new CSVAllData();
                foreach (DataRow row in F_Control.DataTableForGrid.Rows)
                {
                    F_Control.CSVAllData.Add(this.MakeCSVColumnData(row));
                }
            }
        }
        /// <summary>
        /// </summary>
        /// 1行分のグリッドに入っている文字列を取得し、CSVColumnDataオブジェクト作成
        /// <returns></returns>
        private CSVColumnData MakeCSVColumnData(DataRow row)
        {
            CSVColumnData csvData = new CSVColumnData();
            //columnには列インデックスが入っている
            foreach (DataColumn column in F_Control.DataTableForGrid.Columns)
            {
                string cellVal = row[column] == null ? string.Empty : row[column].ToString();
                csvData.Add(cellVal);
            }
            return csvData;
        }

        //CellLeaveイベント....セルを移動しただけで走る
        //CellEndEditイベント..編集中でなくなったら入力操作がなくても走る
        //以下のイベント.......編集中でなくなったら入力操作があった場合走る
        private void dataGridView1_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            F_Control.RecodeGridData(sender);
        }
        #endregion
    }
}
