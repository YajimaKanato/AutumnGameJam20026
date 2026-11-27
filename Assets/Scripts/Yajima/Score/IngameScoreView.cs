using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Yajima
{
    public class IngameScoreView : IngameBehaviour
    {
        [SerializeField] TextMeshProUGUI _scoreText;
        [SerializeField] TextMeshProUGUI _comboText;
        [SerializeField, Min(0f)] float _countDuration = 0.5f;
        int _displayedScore;
        int _targetScore;
        Tween _scoreTween;

        protected override void OnPaused()
        {
            _scoreTween?.Pause();
        }

        protected override void OnResumed()
        {
            _scoreTween?.Play();
        }

        public void GetScore(int score)
        {
            _targetScore = score;
            _scoreTween?.Kill();
            _scoreTween = null;
            if (!isActiveAndEnabled || _countDuration <= 0f || score == _displayedScore)
            {
                DisplayScore(score);
                return;
            }

            _scoreTween = DOTween.To(() => _displayedScore, DisplayScore, score, _countDuration)
                .SetEase(Ease.OutQuad)
                .OnKill(() => _scoreTween = null);
            if (IsStopped) _scoreTween.Pause();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _scoreTween?.Kill();
            DisplayScore(_targetScore);
        }

        void DisplayScore(int score)
        {
            _displayedScore = score;
            _scoreText.text = score.ToString("N0");
        }

        public void AddCombo(int combo)
        {
            _comboText.text = combo > 0 ? $"{combo} COMBO" : "";
        }
    }
}
