using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Aizawa
{
    /// <summary>
    /// ランキング項目表示のUIクラス
    /// </summary>
    public class RankingElementUI : MonoBehaviour
    {
        [SerializeField]
        private Sprite[] _rankFrames = new Sprite[10];

        private TextMeshProUGUI _text;

        public void SetRank(RankingElement element)
        {
            _text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            _text.gameObject.SetActive(false);
            GetComponent<Image>().sprite = _rankFrames[element.rank - 1];
            _text.text = $"{element.score:00000}p :「{element.name}」";

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