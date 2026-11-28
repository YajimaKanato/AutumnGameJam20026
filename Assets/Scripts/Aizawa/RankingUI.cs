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
            if (_content == null || _rankingElementPrefab == null) return;

            StopAllCoroutines();
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
            var ranking = ScoreManager.Ranking;
            for (int i = 0; i < ScoreManager.RankingLimit; i++)
            {
                var entry = i < ranking.Count ? ranking[i] : new RankingElement(i + 1, "---", 0);
                Instantiate(_rankingElementPrefab, _content).GetComponent<RankingElementUI>()?.SetRank(entry);
                yield return new WaitForSeconds(_openSpeed);
            }
        }
    }
}
