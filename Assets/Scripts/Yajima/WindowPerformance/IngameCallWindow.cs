using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Yajima
{
    public class IngameCallWindow : WindowPerformance
    {
        [SerializeField] TextMeshProUGUI _firstCall;
        [SerializeField] TextMeshProUGUI _secondCall;
        [SerializeField] TextMeshProUGUI _thirdCall;
        [SerializeField] TextMeshProUGUI _fourthCall;
        [SerializeField, Min(1f)] float _entryScale = 1.45f;
        [SerializeField, Min(0.01f)] float _entryDuration = 0.25f;
        [SerializeField, Min(0f)] float _callInterval = 0.4f;
        [SerializeField, Min(0.01f)] float _fadeDuration = 0.25f;
        [SerializeField, Min(0f)] float _fourthHold = 0.45f;
        [SerializeField] UnfinishedType _finishedType;
        [SerializeField] Dictionary<UnfinishedType, string> _callDict;

        Sequence _sequence;
        TMP_FontAsset _thirdCallRuntimeFont;
        public event Action OnFourthCall;

        public void SetThirdCall(UnfinishedType type)
        {
            if (_thirdCall == null) return;

            if (!_callDict.TryGetValue(type, out var text))
            {
                text = _callDict.GetValueOrDefault(_finishedType);
            }

            if (_thirdCall.font == null || !_thirdCall.font.HasCharacters(text, out uint[] _, true, true))
            {
                if (_thirdCallRuntimeFont == null)
                    _thirdCallRuntimeFont = TMP_FontAsset.CreateFontAsset("Yu Gothic", "Regular");
                if (_thirdCallRuntimeFont != null)
                    _thirdCall.font = _thirdCallRuntimeFont;
            }

            _thirdCall.text = text;
        }

        private void OnDestroy()
        {
            if (_thirdCallRuntimeFont == null) return;

            foreach (var atlas in _thirdCallRuntimeFont.atlasTextures)
                Destroy(atlas);
            Destroy(_thirdCallRuntimeFont.material);
            Destroy(_thirdCallRuntimeFont);
        }

        public override UniTask OpenWindow() => PlayCalls(destroyCancellationToken);

        public override UniTask CloseWindow()
        {
            ResetCalls();
            return UniTask.CompletedTask;
        }

        private void OnDisable() => ResetCalls();

        public async UniTask PlayCalls(CancellationToken cancellationToken)
        {
            ResetCalls();
            var sequence = DOTween.Sequence().SetUpdate(true).Pause();
            _sequence = sequence;
            TextMeshProUGUI[] firstCalls = { _firstCall, _secondCall, _thirdCall };

            foreach (var call in firstCalls)
            {
                if (call == null) continue;

                PrepareCall(call, 1f);
                sequence.AppendCallback(() => call.gameObject.SetActive(true));
                sequence.Append(call.rectTransform.DOScale(Vector3.one, _entryDuration).SetEase(Ease.OutCubic));
                sequence.AppendInterval(_callInterval);
            }

            float fadeStart = sequence.Duration();
            foreach (var call in firstCalls)
            {
                if (call != null) sequence.Insert(fadeStart, Fade(call, 0f, _fadeDuration));
            }

            if (_fourthCall != null) PrepareCall(_fourthCall, 0f);
            sequence.AppendCallback(() =>
            {
                foreach (var call in firstCalls) Hide(call);
                if (_fourthCall != null) _fourthCall.gameObject.SetActive(true);
                OnFourthCall?.Invoke();
            });

            if (_fourthCall != null)
            {
                sequence.Append(_fourthCall.rectTransform.DOScale(Vector3.one, _entryDuration).SetEase(Ease.OutCubic));
                sequence.Join(Fade(_fourthCall, 1f, _entryDuration));
                sequence.AppendInterval(_fourthHold);
                sequence.Append(Fade(_fourthCall, 0f, _fadeDuration));
            }

            // A callback-only sequence needs a duration when no call text is assigned.
            if (sequence.Duration() <= 0f) sequence.AppendInterval(_entryDuration);
            sequence.Play();
            try
            {
                await UniTask.WaitUntil(() => !sequence.IsActive(), cancellationToken: cancellationToken);
            }
            finally
            {
                if (_sequence == sequence) ResetCalls();
            }
        }

        void PrepareCall(TextMeshProUGUI call, float alpha)
        {
            call.rectTransform.localScale = Vector3.one * _entryScale;
            call.alpha = alpha;
        }

        static Tween Fade(TextMeshProUGUI call, float alpha, float duration)
            => DOTween.To(() => call.alpha, value => call.alpha = value, alpha, duration).SetEase(Ease.Linear);

        static void Hide(TextMeshProUGUI call)
        {
            if (call != null) call.gameObject.SetActive(false);
        }

        void ResetCalls()
        {
            _sequence?.Kill();
            _sequence = null;
            Hide(_firstCall);
            Hide(_secondCall);
            Hide(_thirdCall);
            Hide(_fourthCall);
        }
    }
}
