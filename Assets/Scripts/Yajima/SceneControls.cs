using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Yajima
{
    internal static class SceneControls
    {
        public static RectTransform CreateCanvas(string name, Transform owner, int sortingOrder = 20)
        {
            var root = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            SceneManager.MoveGameObjectToScene(root, owner.gameObject.scene);
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = sortingOrder;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            return (RectTransform)root.transform;
        }

        public static void EnsureEventSystem(Transform owner)
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null) return;
            var events = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            events.transform.SetParent(owner, false);
        }

        public static RectTransform CreateRect(string name, Transform parent, Vector2 anchor, Vector2 position, Vector2 size)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = anchor;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            return rect;
        }

        public static Text AddText(RectTransform rect, string caption, int size = 28)
        {
            var text = rect.gameObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
            text.raycastTarget = false;
            text.text = caption;
            return text;
        }

        public static Text AddButton(Transform parent, string caption, Vector2 anchor, Vector2 position, UnityAction action)
        {
            var rect = CreateRect(caption, parent, anchor, position, new Vector2(210f, 48f));
            var image = rect.gameObject.AddComponent<Image>();
            image.color = new Color(0.10f, 0.16f, 0.22f, 0.95f);
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(action);
            var label = CreateRect("Label", rect, Vector2.one * 0.5f, Vector2.zero, rect.sizeDelta);
            return AddText(label, caption, 22);
        }
    }
}
