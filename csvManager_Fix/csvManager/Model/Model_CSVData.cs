using System.Collections.Generic;
using System.Text;

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
}
