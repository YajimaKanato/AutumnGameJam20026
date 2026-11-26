using UnityEngine;
using UnityEngine.UI;

namespace Aizawa
{
    /// <summary>
    /// リザルトスコア表示のUIクラス
    /// </summary>
    public class ResultScoreUI : MonoBehaviour
    {
        private Text _text;

        void Start()
        {
            _text = transform.GetChild(0).GetComponent<Text>();
        }

        /// <summary>
        /// スコアを表示する
        /// </summary>
        public void Open()
        {
            _text.text = $"{ScoreManager.CurrentScore}点";
        }
    }
}