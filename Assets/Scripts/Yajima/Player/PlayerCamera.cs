using UnityEngine;

namespace Yajima
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] Vector3 _offset;
        [SerializeField] Transform _player;
        [SerializeField] Collider[] _stageSurfaces;
        [SerializeField, Min(0f)] float _stageEdgePadding = 1f;
        Quaternion _worldRotation;
        Camera _outputCamera;
        Collider _ground;

        private void Awake()
        {
            _worldRotation = transform.rotation;
            _outputCamera = Camera.main;
            float largestArea = 0f;
            if (_stageSurfaces != null)
            {
                foreach (var surface in _stageSurfaces)
                {
                    if (surface == null) continue;
                    var size = surface.bounds.size;
                    float area = size.x * size.z;
                    if (size.y > 1f || area <= largestArea) continue;
                    largestArea = area;
                    _ground = surface;
                }
            }
            FollowTarget();
        }

        private void LateUpdate()
        {
            transform.rotation = _worldRotation;
            FollowTarget();
        }

        private void OnValidate()
        {
            FollowTarget();
        }

        void FollowTarget()
        {
            if (_player == null) return;
            var playerPosition = ClampPlayerPosition(_player.position);
            transform.position = new Vector3(
                playerPosition.x + _offset.x,
                _offset.y,
                playerPosition.z + _offset.z);
        }

        public Vector3 ClampPlayerPosition(Vector3 position)
        {
            if (!TryGetMovementBounds(out var bounds)) return position;

            position.x = Mathf.Clamp(position.x, bounds.xMin, bounds.xMax);
            position.z = Mathf.Clamp(position.z, bounds.yMin, bounds.yMax);
            return position;
        }

        internal bool TryGetMovementBounds(out Rect bounds)
        {
            bounds = default;
            if (_ground == null || _outputCamera == null) return false;
            var ground = _ground.bounds;
            float height = _offset.y - ground.min.y;
            float vertical = Mathf.Tan(_outputCamera.fieldOfView * Mathf.Deg2Rad * 0.5f);
            float horizontal = vertical * _outputCamera.aspect;
            Vector2 min = Vector2.zero;
            Vector2 max = Vector2.zero;
            for (int corner = 0; corner < 4; corner++)
            {
                float x = (corner & 1) == 0 ? -1f : 1f;
                float y = (corner & 2) == 0 ? -1f : 1f;
                var ray = _worldRotation * new Vector3(x * horizontal, y * vertical, 1f);
                if (ray.y >= 0f || height <= 0f) return false;

                var hit = _offset + ray * (height / -ray.y);
                var footprint = new Vector2(hit.x, hit.z);
                min = Vector2.Min(min, footprint);
                max = Vector2.Max(max, footprint);
            }

            float left = ground.min.x + _stageEdgePadding - min.x;
            float right = ground.max.x - _stageEdgePadding - max.x;
            float bottom = ground.min.z + _stageEdgePadding - min.y;
            float top = ground.max.z - _stageEdgePadding - max.y;
            if (left > right) left = right = (left + right) * 0.5f;
            if (bottom > top) bottom = top = (bottom + top) * 0.5f;
            bounds = Rect.MinMaxRect(left, bottom, right, top);
            return true;
        }
    }
}
