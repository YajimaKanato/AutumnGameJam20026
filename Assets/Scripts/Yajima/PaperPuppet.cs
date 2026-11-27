using DG.Tweening;
using UnityEngine;

namespace Yajima
{
    public class PaperPuppet : IngameBehaviour
    {
        [Header("移動演出")]
        [SerializeField] float _moveRotateSpeed = 0.5f;
        [SerializeField] Ease _moveEase = Ease.OutExpo;
        [SerializeField] float _moveUpOffset = 0.2f;
        [SerializeField] float _moveUpHalfSpeed = 0.2f;
        [SerializeField] Ease _moveUpHalfEase = Ease.Linear;

        [Header("インタラクト演出")]
        [SerializeField] float _interactRotateSpeed = 0.3f;
        [SerializeField] float _interactUpOffset = 0.2f;
        [SerializeField] float _interactFreeze = 0.1f;
        [SerializeField] Ease _interactEase = Ease.Linear;

        Tween _moveTween;
        Tween _moveUpTween;
        Tween _interactTween;
        Vector3 _initialPosition;
        Quaternion _initialRotation;
        float _turnAngle;
        float _moveUpHeight;
        float _interactUpHeight;
        bool _facingRight = true;

        private void Awake()
        {
            _initialPosition = transform.localPosition;
            _initialRotation = transform.localRotation;
        }

        protected override void OnDisable()
        {
            _moveTween?.Kill();
            _moveUpTween?.Kill();
            _interactTween?.Kill();
            _moveTween = _moveUpTween = _interactTween = null;
            _turnAngle = _moveUpHeight = _interactUpHeight = 0f;
            _facingRight = true;
            transform.localPosition = _initialPosition;
            transform.localRotation = _initialRotation;
            base.OnDisable();
        }

        public void Move(bool isMoving, bool facingRight)
        {
            if (IsStopped) return;

            Move(isMoving);

            if (_interactTween != null || _facingRight == facingRight) return;
            _facingRight = facingRight;

            _moveTween?.Kill();
            var delta = facingRight
                ? Mathf.Repeat(-_turnAngle, 360f)
                : -Mathf.Repeat(_turnAngle - 180f, 360f);
            _moveTween = DOTween.To(() => _turnAngle, ApplyRotation, _turnAngle + delta, _moveRotateSpeed)
                .SetEase(_moveEase)
                .OnKill(() => _moveTween = null);
        }

        public void Move(bool isMoving)
        {
            if (!IsStopped && isMoving) PlayMoveUp();
        }

        private void LateUpdate()
        {
            if (IsFinished) PlayMoveUp();
        }

        void PlayMoveUp()
        {
            if (_moveUpTween != null || _moveUpHalfSpeed <= 0f || _moveUpOffset == 0f) return;

            _moveUpTween = DOTween.To(() => _moveUpHeight, height =>
                {
                    _moveUpHeight = height;
                    ApplyHeight();
                }, _moveUpOffset, _moveUpHalfSpeed)
                .SetEase(_moveUpHalfEase)
                .SetLoops(2, LoopType.Yoyo)
                .SetUpdate(UpdateType.Fixed)
                .OnKill(() => _moveUpTween = null);
        }

        public void StopMovement()
        {
            _moveTween?.Kill();
            _moveUpTween?.Kill();
            _moveUpHeight = 0f;
            ApplyHeight();
        }

        public Tween Interact(float angle)
        {
            if (IsStopped || _moveTween != null || _interactTween != null) return null;

            float targetAngle = _turnAngle + angle;
            var sequence = DOTween.Sequence().SetUpdate(UpdateType.Fixed);
            sequence.Append(DOTween.To(() => _turnAngle, ApplyRotation, targetAngle, _interactRotateSpeed)
                .SetEase(_interactEase));
            sequence.Join(DOTween.To(() => _interactUpHeight, height =>
                {
                    _interactUpHeight = height;
                    ApplyHeight();
                }, _interactUpOffset, _interactRotateSpeed * 0.5f)
                .SetEase(_moveUpHalfEase)
                .SetLoops(2, LoopType.Yoyo));
            sequence.AppendInterval(_interactFreeze);
            _interactTween = sequence.OnKill(() => _interactTween = null);
            return sequence;
        }

        void ApplyHeight()
        {
            transform.localPosition = _initialPosition + Vector3.up * (_moveUpHeight + _interactUpHeight);
        }

        void ApplyRotation(float angle)
        {
            _turnAngle = angle;
            transform.localRotation = Quaternion.AngleAxis(angle, Vector3.up) * _initialRotation;
        }

        protected override void OnPaused()
        {
            _moveTween?.Pause();
            _moveUpTween?.Pause();
            _interactTween?.Pause();
        }

        protected override void OnResumed()
        {
            _moveTween?.Play();
            _moveUpTween?.Play();
            _interactTween?.Play();
        }

        protected override void OnFinished()
        {
            _moveUpTween?.Play();
        }
    }
}
