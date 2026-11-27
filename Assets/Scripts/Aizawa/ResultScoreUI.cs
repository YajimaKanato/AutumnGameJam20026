using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Aizawa
{
    /// <summary>
    /// リザルトスコア表示のUIクラス
    /// </summary>
    public class ResultScoreUI : MonoBehaviour
    {
        private char[] _scoreChars;
        private int _showedDigits;

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
            //スコアをchar配列にして、桁進め用のコルーチンを走らせる
            _scoreChars = ScoreManager.CurrentScore.ToString().ToCharArray();
            StartCoroutine(DigitStep());
        }

        void Update()
        {
            //スコア表示演出中なら
            if(_scoreChars != null)
            {
                _text.text = "";

                //桁ごとに表示処理
                for(int i = 0; i < _scoreChars.Length; i++)
                {
                    //桁が表示されているなら対応する数字を、表示前ならランダムな数字を出す
                    _text.text += _showedDigits > i ? _scoreChars[i] - '0' : Random.Range(0, 10);
                }

                _text.text += "点";

                //全ての桁が表示されていたら演出を止める
                if(_scoreChars.Length <= _showedDigits)
                {
                    _scoreChars = null;
                }
            }
        }

        /// <summary>
        /// 表示する桁を徐々に進ませるコルーチン
        /// </summary>
        IEnumerator DigitStep()
        {
            //演出が終わったら自動で止まる
            while(_scoreChars != null)
            {
                yield return new WaitForSeconds(0.5f);

                _showedDigits++;
            }
        }
    }
}