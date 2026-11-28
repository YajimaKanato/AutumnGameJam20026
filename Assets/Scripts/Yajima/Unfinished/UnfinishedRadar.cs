using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Yajima
{
    public class UnfinishedRadar : MonoBehaviour
    {
        [SerializeField] Transform _player;
        [SerializeField] UnfinishedPool _pool;
        [SerializeField, Min(0.1f)] float _range = 12f;
        [SerializeField, Min(1f)] float _radius = 74f;
        [SerializeField] RectTransform _display;
        [SerializeField] RectTransform _markerPrefab;

        readonly List<RectTransform> _markers = new();

        internal void Initialize(RectTransform canvas, Transform player, UnfinishedPool pool)
        {
            _player = player;
            _pool = pool;
            _display = SceneControls.CreateRect("Unfinished Radar", canvas, Vector2.one,
                new Vector2(-112f, -114f), new Vector2(170f, 170f));
            var background = _display.gameObject.AddComponent<Image>();
            background.color = new Color(0.05f, 0.12f, 0.16f, 0.8f);
            background.raycastTarget = false;
            var center = SceneControls.CreateRect("Player", _display, Vector2.one * 0.5f, Vector2.zero, Vector2.one * 8f);
            var centerImage = center.gameObject.AddComponent<Image>();
            centerImage.color = Color.white;
            centerImage.raycastTarget = false;
        }

        private void LateUpdate()
        {
            if (_player == null || _pool == null || _display == null) return;

            int index = 0;
            foreach (var target in _pool.ActiveTargets())
            {
                var offset = target.position - _player.position;
                var planar = new Vector2(offset.x, offset.z);
                if (planar.sqrMagnitude > _range * _range) continue;

                var marker = index < _markers.Count ? _markers[index] : CreateMarker();
                marker.anchoredPosition = planar * (_radius / _range);
                marker.gameObject.SetActive(true);
                index++;
            }
            for (; index < _markers.Count; index++) _markers[index].gameObject.SetActive(false);
        }

        RectTransform CreateMarker()
        {
            var marker = _markerPrefab != null
                ? Instantiate(_markerPrefab, _display)
                : SceneControls.CreateRect("Target", _display, Vector2.one * 0.5f, Vector2.zero, Vector2.one * 9f);
            if (_markerPrefab == null)
            {
                var image = marker.gameObject.AddComponent<Image>();
                image.color = new Color(0.37f, 0.93f, 0.73f);
                image.raycastTarget = false;
            }
            _markers.Add(marker);
            return marker;
        }
    }
}
