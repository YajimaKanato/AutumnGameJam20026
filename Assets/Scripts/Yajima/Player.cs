using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Yajima
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        [Header("移動速度")]
        [SerializeField] float _speed = 2f;
        [SerializeField] float _lightMoveSpeed = 3f;
        [Header("インタラクト")]
        [SerializeField] float _interactInterval = 0.3f;
        [SerializeField] float _interactScale = 1.0f;
        [SerializeField] Collider _interactCollider;
        [Header("演出パラメータ")]
        [SerializeField] float _moveRotateSpeed = 0.5f;
        [SerializeField] Ease _moveEase = Ease.OutExpo;
        [SerializeField] float _moveUpOffset = 0.2f;
        [SerializeField] float _moveUpHalfSpeed = 0.2f;
        [SerializeField] Ease _moveUpHalfEase = Ease.Linear;
        [SerializeField] float _interactRotateSpeed = 0.3f;
        [SerializeField] float _interactUpOffset = 0.2f;
        [SerializeField] float _interactFreeze = 0.1f;
        [SerializeField] Ease _interactEase = Ease.Linear;
        PlayerInput _input;
        InputAction _move;
        InputAction _interact;
        InputAction _lightMove;
        Rigidbody _rb;
        Tween _moveTween;
        Tween _interactTween;
        Tween _interactColliderTimer;
        Tween _moveUpTween;
        Vector3 _direction;
        float _playerForward = 1;
        float _defaultPositionY;
        float _moveUpHeight;
        float _interactUpHeight;
        bool _isFliped;
        bool _interacted;
        bool _lightMoved;

        public event Action<bool> OnLightMove;

        public bool IsFacingRight => _playerForward > 0f;

        public bool IsInteractCollider(Collider collider)
        {
            return isActiveAndEnabled && collider != null && collider == _interactCollider && collider.enabled;
        }

        private void Awake()
        {
            Init();
        }

        void Init()
        {
            _input = new();
            _move = _input.Ingame.Move;
            _interact = _input.Ingame.Interact;
            _lightMove = _input.Ingame.LightMove;
            _input.Enable();

            _defaultPositionY = transform.position.y;
            _rb = GetComponent<Rigidbody>();

            if (_interactCollider != null)
            {
                _interactCollider.enabled = false;
                _interactCollider.isTrigger = true;
                ((BoxCollider)_interactCollider).size = new Vector3(1, 0.1f, 1) * _interactScale;
            }
        }

        private void OnDisable()
        {
            _interactColliderTimer?.Kill();
            _interactColliderTimer = null;
            if (_interactCollider != null)
                _interactCollider.enabled = false;
        }

        private void Update()
        {
            MoveInput();
            InteractInput();
            LightMoveInput();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void LateUpdate()
        {
            MovePerform();
            InteractPerform();
        }

        #region Input
        void MoveInput()
        {
            // 入力を変換
            var dir = _move.ReadValue<Vector2>();
            _direction = new Vector3(dir.x, 0, dir.y);
            _direction.Normalize();

            // プレイヤーの向く方向を設定
            if (Mathf.Abs(_direction.x) > 0
                && _playerForward * Mathf.Sign(_direction.x) < 0)
            {
                _playerForward = Mathf.Sign(_direction.x);
                _isFliped = true;
            }
        }

        void InteractInput()
        {
            _interacted = _interact.WasPressedThisFrame();
        }

        void LightMoveInput()
        {
            _lightMoved = _lightMove.IsPressed();
            if (_lightMove.WasPressedThisFrame())
            {
                OnLightMove?.Invoke(true);
            }

            if (_lightMove.WasReleasedThisFrame())
            {
                OnLightMove?.Invoke(false);
            }
        }
        #endregion

        #region Move
        void Move()
        {
            // 光速移動かどうかで速度変える
            var velocity = _direction * (_lightMoved ? _lightMoveSpeed : _speed);
            velocity.y = _rb.linearVelocity.y;
            _rb.linearVelocity = velocity;
        }
        #endregion

        #region Performance
        void MovePerform()
        {
            var velocity = _rb.linearVelocity;
            if (_moveUpTween == null && (velocity.x != 0f || velocity.z != 0f))
            {
                _moveUpTween = DOTween.To(() => _moveUpHeight, height =>
                    {
                        _moveUpHeight = height;
                        ApplyHeight(_moveUpHeight);
                    }, _moveUpOffset, _moveUpHalfSpeed)
                    .SetEase(_moveUpHalfEase)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetUpdate(UpdateType.Fixed)
                    .SetLink(gameObject)
                    .OnComplete(() => _moveUpTween = null);
            }

            // インタラクト演出が実行中は無視
            if (_interactTween != null && _interactTween.IsActive()) return;
            if (!_isFliped) return;

            _isFliped = false;

            _moveTween?.Kill();
            var currentY = _rb.rotation.eulerAngles.y;
            // 上から見て、Y軸の正方向は時計回り。
            var rotationY = _playerForward > 0
                ? Mathf.Repeat(-currentY, 360f)
                : -Mathf.Repeat(currentY - 180f, 360f);
            _moveTween = _rb.DORotate(Vector3.up * rotationY, _moveRotateSpeed, RotateMode.WorldAxisAdd)
                .SetEase(_moveEase);

        }

        void ApplyHeight(float height)
        {
            var position = _rb.position;
            position.y = _defaultPositionY + height;
            _rb.position = position;
        }

        void InteractPerform()
        {
            if (!_interacted) return;
            if (_moveTween != null && _moveTween.IsActive()) return;
            if (_interactTween != null && _interactTween.IsActive()) return;

            if (_interactCollider != null && _interactInterval > 0f)
            {
                _interactColliderTimer?.Kill();
                _interactCollider.enabled = true;
                _interactColliderTimer = DOVirtual.DelayedCall(_interactInterval, () =>
                {
                    if (_interactCollider != null)
                        _interactCollider.enabled = false;
                    _interactColliderTimer = null;
                }, ignoreTimeScale: false).SetUpdate(UpdateType.Fixed);
            }

            var sequence = DOTween.Sequence().SetUpdate(UpdateType.Fixed).SetLink(gameObject);
            sequence.Append(_rb
                .DORotate(Vector3.up * 360 * -_playerForward, _interactRotateSpeed, RotateMode.FastBeyond360)
                .SetRelative()
                .SetEase(_interactEase));
            sequence.Join(DOTween.To(() => _interactUpHeight, height =>
                {
                    _interactUpHeight = height;
                    ApplyHeight(_interactUpHeight);
                }, _interactUpOffset, _interactRotateSpeed * 0.5f)
                .SetEase(_moveUpHalfEase)
                .SetLoops(2, LoopType.Yoyo));
            sequence.AppendInterval(_interactFreeze).OnComplete(() => _interactTween = null);

            _interactTween = sequence;
        }
        #endregion
    }
}
