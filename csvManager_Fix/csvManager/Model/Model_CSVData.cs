using System.Collections.Generic;
using System.Text;
using System.Data;

namespace csvManager
{
    #region CSV全データクラス

    class CSVAllData : List<CSVColumnData>
    {
        /// <summary>
        /// すべての行で、一番カラムが多い行のカラム数
        /// カラム数として設定するため
        /// </summary>
        public int MaxColumnsInAllRows { get; set; }
    }
    #endregion

    #region CSVデータのColumm(縦)用クラス[1行分]

    class CSVColumnData : List<string>
    {
        private const char comma = ',';

        /// <summary>
        /// 区切り文字付加して一行分取得
        /// </summary>
        /// <returns></returns>
        public string MakeLineByThisDatas()
        {
            StringBuilder sb = new StringBuilder();
            int cnt = 0;
            foreach (string columnItem in this)
            {
                cnt++;
                sb.Append(columnItem);
                if (cnt < this.Count)//最後以外カンマを付加
                {
                    sb.Append(comma);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 1行を区切り文字で区切ってList化
        /// </summary>
        /// <param name="line">1行分データ</param>
        public void MakeListBy(string line)
        {
            this.AddRange(line.Split(comma));
        }
    }
    #endregion

    #region UndoRedo用記録データ
    public class RecordedDataTables : List<DataTable>
    {
        private new const int Capacity = 3;

        public new void Add(DataTable d)
        {
            if(this.Count >= Capacity)
            {
                //0番目にあるデータを削除し、前につめた後、最後尾にデータを追加
                //最後尾が最新データとなる
                base.RemoveAt(0);
                for(int i = 0; i < this.Count; i++)
                {
                    this[i] = this[i + 1];
                }
            }
            base.Add(d);
        }
    }
    #endregion
}
