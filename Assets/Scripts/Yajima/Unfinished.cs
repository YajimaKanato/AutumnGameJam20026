using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Splines;

namespace Yajima
{
    public class Unfinished : IngameBehaviour
    {
        [Header("初期値")]
        [SerializeField] int _score = 100;
        [SerializeField] float _lifeTime = 5f;
        [SerializeField] float _moveSpeed = 3f;
        [SerializeField] PaperPuppet _paperPuppet;

        protected Vector3 _previousPosition;
        protected SplineContainer _moveSplineContainer;
        protected Spline _moveSpline;
        protected float _movePhase;
        protected float _lifeRemaining;
        protected bool _entered;
        protected bool _interacted;

        public event Action<int> OnInteractedPlayer;
        public event Action OnEntry;
        public event Action<Unfinished> OnExit;

        protected override void OnEnable()
        {
            _previousPosition = transform.position;
            _interacted = false;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            _entered = false;
            base.OnDisable();
        }

        private void Update()
        {
            if (IsStopped || !_entered || _interacted) return;

            _lifeRemaining -= Time.deltaTime;
            if (_lifeRemaining <= 0f)
            {
                Exit();
                return;
            }

            if (_moveSplineContainer == null || _moveSpline == null || _moveSpline.Count < 2 || _moveSpeed <= 0f) return;

            _movePhase = Mathf.Repeat(_movePhase + _moveSpeed * Time.deltaTime, 1f);
            var progress = Mathf.PingPong(_movePhase * 2f, 1f);
            var start = GetSplineEndpoint(_moveSplineContainer, _moveSpline, 0);
            var end = GetSplineEndpoint(_moveSplineContainer, _moveSpline, _moveSpline.Count - 1);
            transform.position = Vector3.Lerp(start, end, progress);
        }

        private void LateUpdate()
        {
            if (!_interacted) MovePerform();
        }

        void MovePerform()
        {
            if (IsStopped) return;

            var position = transform.position;
            var isMoving = position.x != _previousPosition.x || position.z != _previousPosition.z;
            _previousPosition = position;

            if (_paperPuppet != null && _paperPuppet.isActiveAndEnabled)
                _paperPuppet.Move(isMoving);
        }

        internal static Vector3 GetSplineEndpoint(SplineContainer container, Spline spline, int knotIndex)
        {
            return container.transform.TransformPoint((Vector3)spline[knotIndex].Position);
        }

        internal void SetMoveSpline(SplineContainer container, Spline spline)
        {
            _moveSplineContainer = container;
            _moveSpline = spline;
            _movePhase = 0f;
        }

        public void Init(int score, float lifeTime, float moveSpeed)
        {
            _score = score;
            _lifeTime = lifeTime;
            _moveSpeed = moveSpeed;
        }

        internal void UseDefaultSettings(Unfinished prefab)
        {
            Init(prefab._score, prefab._lifeTime, prefab._moveSpeed);
        }

        // これをオーバーライドで拡張
        public virtual async void Entry()
        {
            _lifeRemaining = _lifeTime;
            _entered = true;
            OnEntry?.Invoke();
        }

        // これをオーバーライドで拡張
        public virtual async void Exit()
        {
            _entered = false;
            OnExit?.Invoke(this);
        }

        public virtual void InteractExit(Player player)
        {
            if (IsStopped || _interacted) return;

            var turn = Turn(player);
            turn?.OnComplete(Exit);
            OnInteractedPlayer?.Invoke(_score);
            if (turn == null && isActiveAndEnabled) Exit();
        }

        protected Tween Turn(Player player)
        {
            _interacted = true;
            if (_paperPuppet == null || !_paperPuppet.isActiveAndEnabled) return null;

            _paperPuppet.StopMovement();
            var angle = player.transform.position.x < transform.position.x ? 180f : -180f;
            return _paperPuppet.Interact(angle);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isActiveAndEnabled || IsStopped || _interacted) return;

            var player = other.GetComponentInParent<Player>();
            if (player == null || !player.IsInteractCollider(other)) return;
            InteractExit(player);
        }

    }
}
