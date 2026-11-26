using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Aizawa
{
    /// <summary>
    /// <para>ボタンに機能を追加するクラス</para>
    /// <para>ボタンのUIオブジェクトに後付けするだけで良い</para>
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ButtonExpansion : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary> 未選択時の画像 </summary>
        [SerializeField]
        private Sprite _notSelected;
        /// <summary> 選択時の画像 </summary>
        [SerializeField]
        private Sprite _selected;
        /// <summary> キーボード選択移行時最初に選択されるかどうか </summary>
        [SerializeField]
        private bool _isFirstSelecting;

        /// <summary> 現在キーボードで選択する状態か </summary>
        private bool _isKeyboardControlle;

        private Button _button;
        private InputAction _buttonSelectingAction = new("ButtonSelect", InputActionType.Button);

        void Start()
        {
            _button = GetComponent<Button>();

            //上下左右を一つのInputActionにまとめる
            //あとでInputSystem_Actionsに突っ込むかも
            _buttonSelectingAction.AddBinding("<Keyboard>/upArrow");
            _buttonSelectingAction.AddBinding("<Keyboard>/downArrow");
            _buttonSelectingAction.AddBinding("<Keyboard>/rightArrow");
            _buttonSelectingAction.AddBinding("<Keyboard>/leftArrow");
            _buttonSelectingAction.Enable();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //マウス選択モードのみ
            if(!_isKeyboardControlle)
            {
                //フォーカスされたら選択時の画像に
                _button.image.sprite = _selected;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //マウス選択モードのみ
            if(!_isKeyboardControlle)
            {
                //フォーカスが取れたら未選択時の画像に
                _button.image.sprite = _notSelected;
            }
        }

        void Update()
        {
            //キーボード選択モードのみ
            if(_isKeyboardControlle)
            {
                //マウスが動いたらマウス選択モードへ移行
                if(Mouse.current.delta.magnitude > 0)
                {
                    _isKeyboardControlle = false;
                    EventSystem.current.SetSelectedGameObject(null);
                }

                //このボタンが選択中なら選択中の画像、そうでなければ未選択時の画像にする
                if(EventSystem.current.currentSelectedGameObject == gameObject)
                {
                    _button.image.sprite = _selected;
                }
                else
                {
                    _button.image.sprite = _notSelected;
                }
            }
            //マウス選択モードのみ
            else
            {
                //上下左右どれかが押されたらキーボード選択モードに移行
                if(_buttonSelectingAction.WasPressedThisFrame())
                {
                    _isKeyboardControlle = true;

                    //これが最初に選択される設定なら、選択状態にしておく
                    if(_isFirstSelecting)
                    {
                        _button.Select();
                    }
                }
            }
        }
}
}