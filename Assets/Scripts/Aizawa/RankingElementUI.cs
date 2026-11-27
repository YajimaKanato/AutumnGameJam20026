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
<<<<<<< HEAD
            GetComponent<Image>().sprite = _rankFrames[element.rank - 1];
            _text.text = $"{element.score}p : {element.name}";
=======
            _text.text = $"{element.rank}位 : {element.score}点\n「{element.name}」";
>>>>>>> 793f82a033f8aae48b79f6a8dbfc1249d64ddf48

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