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
                var csvData = F_Control.InputFile(dialog.FileName);
                this.LoadData(csvData);
            }
        }
        /// <summary>
        /// 保存、実処理
        /// </summary>
        private void Save()
        {
            //以下のようにNullにすると、セルからフォーカスがはずれ編集中の値が確定する
            this.dataGridView1.CurrentCell = null;

            var dataTable = this.dataGridView1.DataSource as DataTable;
            var csvData = this.GetAllData(dataTable);
            if (F_Control.OutputFile(csvData)) MessageBox.Show("保存しました。");
        }

        /// <summary>
        /// キー押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            F_Control.KeyDown(sender, e);
        }
        #endregion

        #region メソッド　表示用
        /// <summary>
        /// グリッドデータをロード
        /// </summary>
        private void LoadData(CSVAllData csvData)
        {
            var table = new DataTable();

            //カラム設定
            this.SetColumnIndex(csvData.MaxColumnsInAllRows, table);

            //レコード追加
            foreach (CSVColumnData lineData in csvData)
            {
                string[] rowData = lineData.ToArray();
                table.Rows.Add(rowData);
            }

            this.dataGridView1.DataSource = table;
            F_Control.SetViewMemento(this.dataGridView1);
        }

        /// <summary>
        /// カラム設定、数値を入れる
        /// </summary>
        /// <param name="cnt"></param>
        private void SetColumnIndex(int cnt, DataTable table)
        {
            if (table == null) table = new DataTable();

            for (int i = 1; i <= cnt; i++)
            {
                table.Columns.Add(i.ToString());
            }
        }

        ////例外の確認

        /// <summary>
        /// グリッドのすべてのデータを作成
        /// </summary>
        private CSVAllData GetAllData(DataTable table)
        {
            var csvData = new CSVAllData();

            if (table != null)
            {
                foreach (DataRow row in table.Rows)
                {
                    csvData.Add(this.GetCSVColumnData(row, table));
                }
            }
            return csvData;
        }
        /// <summary>
        /// </summary>
        /// 1行分のグリッドに入っている文字列を取得し、CSVColumnDataオブジェクト作成
        /// <returns></returns>
        private CSVColumnData GetCSVColumnData(DataRow row, DataTable table)
        {
            CSVColumnData csvColData = new CSVColumnData();
            //columnには列インデックスが入っている
            foreach (DataColumn column in table.Columns)
            {
                string cellVal = row[column] == null ? string.Empty : row[column].ToString();
                csvColData.Add(cellVal);
            }
            return csvColData;
        }

        //CellLeaveイベント....セルを移動しただけで走る
        //CellEndEditイベント..編集中でなくなったら入力操作がなくても走る
        //以下のイベント.......入力操作があったら走る
        private void dataGridView1_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
        }
        #endregion

        //CellEndEditイベント..編集中でなくなったら入力操作がなくても走る
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (sender is DataGridView)
            {
                var gridView = sender as DataGridView;
                F_Control.SetViewMemento(gridView);
            }
        }
    }
}
