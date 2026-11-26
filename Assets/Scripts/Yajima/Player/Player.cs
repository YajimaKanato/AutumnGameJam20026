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
        [SerializeField] PaperPuppet _paperPuppet;
        PlayerInput _input;
        InputAction _move;
        InputAction _interact;
        InputAction _lightMove;
        InputAction _menu;
        Rigidbody _rb;
        Tween _interactColliderTimer;
        Vector3 _direction;
        float _playerForward = 1;
        bool _interacted;
        bool _lightMoved;
        bool _openedMenu;

        public event Action<bool> OnLightMove;
        public event Action<bool> OnOpenMenu;

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
            _menu = _input.Ingame.Menu;
            _input.Enable();

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
            OpenMenu();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void LateUpdate()
        {
            if (_paperPuppet == null || !_paperPuppet.isActiveAndEnabled) return;

            var velocity = _rb.linearVelocity;
            _paperPuppet.Move(velocity.x != 0f || velocity.z != 0f, IsFacingRight);
            if (_interacted) _paperPuppet.Interact(IsFacingRight ? -360f : 360f);
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
            }
        }

        void InteractInput()
        {
            _interacted = _interact.WasPressedThisFrame();

            if (_interacted)
            {
                if (_interactCollider != null && _interactInterval > 0f)
                {
                    if (_interactColliderTimer == null)
                    {
                        _interactCollider.enabled = true;
                        _interactColliderTimer = DOVirtual.DelayedCall(_interactInterval, () =>
                        {
                            if (_interactCollider != null)
                                _interactCollider.enabled = false;
                            _interactColliderTimer = null;
                        }, ignoreTimeScale: false).SetUpdate(UpdateType.Fixed);
                    }
                }
            }
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

        void OpenMenu()
        {
            if (_menu.WasPressedThisFrame())
            {
                _openedMenu = !_openedMenu;
                OnOpenMenu?.Invoke(_openedMenu);
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
    }
}
