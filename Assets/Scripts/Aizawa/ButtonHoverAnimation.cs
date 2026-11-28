using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Aizawa
{
    [RequireComponent(typeof(Button))]
    public sealed class ButtonHoverAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] Image _art;
        [SerializeField] Sprite[] _frames;
        [SerializeField, Min(1f)] float _framesPerSecond = 30f;
        [SerializeField] Sprite _highResolutionHover;
        [SerializeField] Vector2 _hoverSizeRatio = Vector2.one;
        [SerializeField] Vector2 _hoverOffsetRatio;
        [SerializeField] float[] _hoverScales;

        Button _button;
        Tween _playback;
        Sprite _idleSprite;
        Vector2 _idleSize;
        Vector2 _idlePosition;
        float _framePosition;

        void Awake()
        {
            int frameCount = _highResolutionHover != null ? _hoverScales?.Length ?? 0 : _frames?.Length ?? 0;
            if (_art == null || frameCount < 2) return;

            _button = GetComponent<Button>();
            _idleSprite = _art.sprite;
            _idleSize = _art.rectTransform.sizeDelta;
            _idlePosition = _art.rectTransform.anchoredPosition;
            _playback = DOTween.To(() => _framePosition, ShowFrame, frameCount - 1,
                    (frameCount - 1) / _framesPerSecond)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .SetAutoKill(false)
                .Pause();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_playback != null && _button.IsInteractable()) _playback.PlayForward();
        }

        public void OnPointerExit(PointerEventData eventData) => _playback?.PlayBackwards();

        void OnDisable()
        {
            if (_playback == null) return;
            _playback.Rewind();
            ShowFrame(0f);
        }

        void OnDestroy() => _playback?.Kill();

        void ShowFrame(float framePosition)
        {
            _framePosition = framePosition;
            int frame = Mathf.FloorToInt(framePosition);
            if (_highResolutionHover != null && framePosition > 0f)
            {
                int next = Mathf.Min(frame + 1, _hoverScales.Length - 1);
                float scale = Mathf.Lerp(_hoverScales[frame], _hoverScales[next], framePosition - frame);
                _art.sprite = _highResolutionHover;
                _art.rectTransform.sizeDelta = Vector2.Scale(_idleSize, _hoverSizeRatio) * scale;
                var hoverPosition = _idlePosition + Vector2.Scale(_idleSize, _hoverOffsetRatio);
                _art.rectTransform.anchoredPosition = hoverPosition * scale;
                return;
            }

            bool showFrame = _highResolutionHover == null && frame > 0 && _frames[frame] != null;
            _art.sprite = showFrame ? _frames[frame] : _idleSprite;
            _art.rectTransform.sizeDelta = _idleSize;
            _art.rectTransform.anchoredPosition = _idlePosition;
        }
    }
}
