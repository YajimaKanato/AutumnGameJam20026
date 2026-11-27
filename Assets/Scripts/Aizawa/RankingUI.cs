using UnityEngine;

namespace Aizawa
{
    /// <summary>
    /// ランキング表示のUIクラス
    /// </summary>
    public class RankingUI : MonoBehaviour
    {
        /// <summary> 項目を並べるTransform </summary>
        [SerializeField]
        private Transform _content;
        /// <summary> 項目のプレハブ </summary>
        [SerializeField]
        private GameObject _rankingElementPrefab;

        /// <summary>
        /// ランキングを表示する
        /// </summary>
        public void Open()
        {
            //現在表示されているランキングをリセットする
            foreach(Transform child in _content)
            {
                Destroy(child.gameObject);
            }

            //ランキングに基づきデータを表示する
            foreach(var r in ScoreManager.Ranking)
            {
                Instantiate(_rankingElementPrefab, _content).GetComponent<RankingElementUI>().SetRank(r);
            }
        }
    }
}