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
            int frameIndex = element.rank - 1;
            var image = GetComponent<Image>();
            if (image != null && _rankFrames != null && frameIndex >= 0 && frameIndex < _rankFrames.Length
                && _rankFrames[frameIndex] != null)
                image.sprite = _rankFrames[frameIndex];
            _text.richText = false;
            _text.text = $"{element.rank}. {element.name}  {element.score:N0}点";

            //回転させる
            transform.localEulerAngles = new(-180, 0, 0);
            transform.DOLocalRotate(new(0, 0, 0), 0.5f);
        }

        void Update()
        {
            //90度になったらテキストを表示
            if (_text != null && !_text.gameObject.activeSelf && Mathf.Abs(Mathf.DeltaAngle(0f, transform.localEulerAngles.x)) <= 90f)
            {
                _text.gameObject.SetActive(true);
            }
        }
    }
}
