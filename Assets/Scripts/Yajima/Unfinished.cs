using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Splines;

namespace Yajima
{
    public class Unfinished : MonoBehaviour
    {
        [Header("初期値")]
        [SerializeField] int _score = 100;
        [SerializeField] float _lifeTime = 5f;
        [SerializeField] float _moveSpeed = 3f;
        [Header("演出")]
        [SerializeField] float _moveUpOffset = 0.2f;
        [SerializeField] float _moveUpHalfSpeed = 0.2f;
        [SerializeField] Ease _moveUpHalfEase = Ease.Linear;
        [SerializeField, Min(0.01f)] float _turnDuration = 0.3f;
        [SerializeField, Min(0f)] float _turnJumpHeight = 0.2f;

        Vector3 _previousPosition;
        Tween _moveUpTween;
        Tween _turnTween;
        Quaternion? _rotationBeforeTurn;
        SplineContainer _moveSplineContainer;
        Spline _moveSpline;
        float _movePhase;
        float _moveUpHeight;
        float _lifeRemaining;
        bool _entered;
        bool _interacted;

        public event Action<int> OnInteractedPlayer;
        public event Action OnEntry;
        public event Action<Unfinished> OnExit;

        private void OnEnable()
        {
            _previousPosition = transform.position;
            _interacted = false;
        }

        private void OnDisable()
        {
            _entered = false;
            _moveUpTween?.Rewind();
            _moveUpTween?.Kill();
            _moveUpTween = null;
            _turnTween?.Kill();
            if (_rotationBeforeTurn.HasValue)
            {
                transform.rotation = _rotationBeforeTurn.Value;
                _rotationBeforeTurn = null;
            }
            SetMoveUpHeight(0f);
        }

        private void Update()
        {
            if (!_entered || _interacted) return;

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
            transform.position = Vector3.Lerp(start, end, progress) + Vector3.up * _moveUpHeight;
        }

        private void LateUpdate()
        {
            if (!_interacted) MovePerform();
        }

        void MovePerform()
        {
            var position = transform.position;
            var isMoving = position.x != _previousPosition.x || position.z != _previousPosition.z;
            _previousPosition = position;

            if (!isMoving || _moveUpTween != null) return;

            _moveUpTween = DOTween.To(() => _moveUpHeight, SetMoveUpHeight, _moveUpOffset, _moveUpHalfSpeed)
                .SetEase(_moveUpHalfEase)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() => _moveUpTween = null);
        }

        void SetMoveUpHeight(float height)
        {
            transform.position += Vector3.up * (height - _moveUpHeight);
            _moveUpHeight = height;
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
            if (_interacted) return;

            Turn(player).OnComplete(Exit);
            OnInteractedPlayer?.Invoke(_score);
        }

        protected Tween Turn(Player player)
        {
            _interacted = true;
            _moveUpTween?.Kill();
            _moveUpTween = null;
            SetMoveUpHeight(0f);

            _rotationBeforeTurn = transform.rotation;
            var rotationY = player.transform.position.x >= transform.position.x ? -180f : 180f;

            var sequence = DOTween.Sequence();
            sequence.Append(transform.DORotate(Vector3.up * rotationY, _turnDuration, RotateMode.WorldAxisAdd)
                .SetEase(Ease.Linear));
            sequence.Join(DOTween.To(() => _moveUpHeight, SetMoveUpHeight, _turnJumpHeight, _turnDuration * 0.5f)
                .SetEase(_moveUpHalfEase)
                .SetLoops(2, LoopType.Yoyo));
            _turnTween = sequence.OnKill(() => _turnTween = null);
            return sequence;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isActiveAndEnabled || _interacted) return;

            var player = other.GetComponentInParent<Player>();
            if (player == null || !player.IsInteractCollider(other)) return;
            InteractExit(player);
        }
    }
}
