using System.Collections;
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
        [SerializeField]
        private float _openSpeed;

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
            StartCoroutine(SetRankingElements());
        }

        /// <summary>
        /// ランキングを順々に表示する
        /// </summary>
        IEnumerator SetRankingElements()
        {
            foreach(var r in ScoreManager.Ranking)
            {
                Instantiate(_rankingElementPrefab, _content).GetComponent<RankingElementUI>().SetRank(r);
                yield return new WaitForSeconds(_openSpeed);
            }
        }
    }
}