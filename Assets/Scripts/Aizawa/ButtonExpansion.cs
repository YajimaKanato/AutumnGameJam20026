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
    public class ButtonExpansion : MonoBehaviour
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

        void Start()
        {
            _button = GetComponent<Button>();

            //画像をアサインする
            _button.transition = Selectable.Transition.SpriteSwap;
            var sprites = _button.spriteState;
            sprites.disabledSprite = _notSelected;
            sprites.pressedSprite = _selected;
            sprites.highlightedSprite = _selected;
            _button.spriteState = sprites;
            ChangeSpriteSet();
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

                    //選択モードに合わせて画像セットを変更する
                    ChangeSpriteSet();
                }
            }
            //マウス選択モードのみ
            else
            {
                //何らかのキーボードが押されたらキーボード操作モードへ移行
                if(Keyboard.current.anyKey.wasPressedThisFrame)
                {
                    _isKeyboardControlle = true;

                    //選択モードに合わせて画像セットを変更する
                    ChangeSpriteSet();

                    //これが最初に選択される設定なら、選択状態にしておく
                    if(_isFirstSelecting)
                    {
                        _button.Select();
                    }
                }
            }
        }

        /// <summary>
        /// 現在の操作モードに合わせて、選択時の画像を変更する
        /// </summary>
        private void ChangeSpriteSet()
        {
            var sprites = _button.spriteState;
            sprites.selectedSprite = _isKeyboardControlle ? _selected : _notSelected;
            _button.spriteState = sprites;
        }
    }
}