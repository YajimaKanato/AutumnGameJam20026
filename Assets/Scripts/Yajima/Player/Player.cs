using DG.Tweening;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Yajima
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : IngameBehaviour
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
        bool _isPause;
        RigidbodyConstraints _constraintsBeforePause;
        float _verticalVelocityBeforePause;

        readonly ReactiveProperty<bool> _lightMoved = new(false);
        public ReadOnlyReactiveProperty<bool> LightMove => _lightMoved;
        readonly ReactiveProperty<bool> _openedMenu = new(false);
        public ReadOnlyReactiveProperty<bool> OpenedMenu => _openedMenu;

        public bool IsFacingRight => _playerForward > 0f;

        public bool IsInteractCollider(Collider collider)
        {
            return isActiveAndEnabled && !_isPause && collider != null && collider == _interactCollider && collider.enabled;
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

            _isPause = false;
        }

        protected override void OnEnable()
        {
            IngameManager.RegisterObj(this);
        }

        protected override void OnDisable()
        {
            _interactColliderTimer?.Kill();
            _interactColliderTimer = null;
            if (_interactCollider != null)
                _interactCollider.enabled = false;
            IngameManager.UnregisterObj(this);
            Resume();
            _input?.Disable();
        }

        private void Update()
        {
            OpenMenu();
            if (_isPause) return;

            MoveInput();
            InteractInput();
            LightMoveInput();
        }

        private void FixedUpdate()
        {
            if (!_isPause) Move();
        }

        private void LateUpdate()
        {
            if (_isPause || _paperPuppet == null || !_paperPuppet.isActiveAndEnabled) return;

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
            _lightMoved.Value = _lightMove.IsPressed();
        }

        void OpenMenu()
        {
            if (IngameManager.HasStarted && !IsFinished && _menu.WasPressedThisFrame())
                _openedMenu.Value = !_openedMenu.Value;
        }
        #endregion

        #region Move
        void Move()
        {
            // 光速移動かどうかで速度変える
            var velocity = _direction * (_lightMoved.Value ? _lightMoveSpeed : _speed);
            velocity.y = _rb.linearVelocity.y;
            _rb.linearVelocity = velocity;
        }

        protected override void OnPaused()
        {
            if (_isPause) return;

            _isPause = true;
            _direction = Vector3.zero;
            _interacted = false;
            _interactColliderTimer?.Pause();
            _constraintsBeforePause = _rb.constraints;
            _verticalVelocityBeforePause = _rb.linearVelocity.y;
            _rb.linearVelocity = Vector3.zero;
            _rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        protected override void OnResumed()
        {
            if (!_isPause) return;

            _isPause = false;
            _rb.constraints = _constraintsBeforePause;
            _rb.linearVelocity = Vector3.up * _verticalVelocityBeforePause;
            _interactColliderTimer?.Play();
        }
        #endregion
    }
}
