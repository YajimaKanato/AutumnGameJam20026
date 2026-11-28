using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Aizawa
{
    /// <summary>
    /// ランキングのウィンドウクラス
    /// </summary>
    public class RankingWindowPerformance : WindowPerformance
    {
        [SerializeField]
        private Transform _panel;
        /// <summary> 開閉速度 </summary>
        [SerializeField]
        private float _ocSpeed;
        [SerializeField] RankingUI _rankingUI;

        private Vector3 _baseScale;

        void Awake()
        {
            _baseScale = _panel.localScale;
        }

        public async void Open()
        {
            await OpenWindow();
        }

        public async void Close()
        {
            await CloseWindow();
        }

        public override async UniTask CloseWindow()
        {
            await _panel.DOScale(Vector3.zero, _ocSpeed).AsyncWaitForCompletion();
            _panel.gameObject.SetActive(false);
        }

        public override async UniTask OpenWindow()
        {
            _panel.localScale = Vector3.zero;
            _panel.gameObject.SetActive(true);
            _rankingUI?.Open();
            await _panel.DOScale(_baseScale, _ocSpeed).AsyncWaitForCompletion();
        }
    }
}
