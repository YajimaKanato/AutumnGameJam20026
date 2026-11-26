using UnityEngine;

namespace Yajima
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] Vector3 _offset;
        [SerializeField] Transform _player;
        Quaternion _worldRotation;

        private void Awake()
        {
            _worldRotation = transform.rotation;
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
            var playerPosition = _player.position;
            transform.position = new Vector3(
                playerPosition.x + _offset.x,
                _offset.y,
                playerPosition.z + _offset.z);
        }
    }
}
