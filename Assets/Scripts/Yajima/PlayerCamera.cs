using UnityEngine;

namespace Yajima
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] Vector3 _offset;
        Transform _camera;
        float? _defaultPositionY;

        // Update is called once per frame
        void Update()
        {
            FollowTarget();
        }

        private void OnValidate()
        {
            FollowTarget();
        }

        void FollowTarget()
        {
            if (Camera.main == null) return;
            _camera ??= Camera.main.transform;
            _defaultPositionY ??= transform.position.y;
            var cameraPos = _camera.position;
            cameraPos = transform.position;
            cameraPos.y = _defaultPositionY.Value;
            _camera.localPosition = cameraPos + _offset;
        }
    }
}
