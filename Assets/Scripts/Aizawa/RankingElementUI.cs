using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Aizawa
{
    /// <summary>
    /// ランキング項目表示のUIクラス
    /// </summary>
    public class RankingElementUI : MonoBehaviour
    {
        private Text _text;

        public void SetRank(RankingElement element)
        {
            _text = transform.GetChild(0).GetComponent<Text>();
            _text.gameObject.SetActive(false);
            _text.text = $"{element.rank}位 : {element.score}点\n「{element.name}」";

            //回転させる
            transform.localEulerAngles = new(-180, 0, 0);
            transform.DOLocalRotate(new(0, 0, 0), 0.5f);
        }

        void Update()
        {
            //90度になったらテキストを表示
            if(!_text.gameObject.activeSelf && transform.localEulerAngles.x >= -90)
            {
                _text.gameObject.SetActive(true);
            }
        }
    }
}