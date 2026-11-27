using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Aizawa
{
    /// <summary>
    /// クレジットのウィンドウクラス
    /// </summary>
    public class CreditWindowPerformance : WindowPerformance
    {
        [SerializeField]
        private Transform _panel;
        /// <summary> 開閉速度 </summary>
        [SerializeField]
        private float _ocSpeed;

        private Vector3 _baseScale;

        void Start()
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
            await _panel.DOScale(_baseScale, _ocSpeed).AsyncWaitForCompletion();
        }
    }
}