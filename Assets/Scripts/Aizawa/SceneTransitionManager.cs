using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Aizawa
{
    /// <summary>
    /// シーン遷移を管理するマネージャークラス
    /// </summary>
    public static class SceneTransitionManager
    {
        /// <summary>
        /// 指定したシーンに遷移する
        /// </summary>
        /// <param name="scene">遷移先のシーン</param>
        public static async Task TransitionScene(Scene scene)
        {
            await SceneManager.LoadSceneAsync(scene.ToString());
        }
    }

    /// <summary>
    /// シーン指定用enum
    /// </summary>
    public enum Scene
    {
        Title,
        Movie,
        Ingame,
        Result
    }
}