using UnityEngine;

namespace Kato
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance { get; set; }

        private void Awake()
        {
            #region ƒVƒ“ƒOƒ‹ƒgƒ“‰»
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            #endregion
        }

        public void PopWindow(GameObject ui)
        {
            ui.gameObject.SetActive(true);
        }

        public void CloseWindow(GameObject ui)
        {
            ui.gameObject.SetActive(false);
        }
    }
}
