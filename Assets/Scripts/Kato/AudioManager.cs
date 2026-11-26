using System.Collections.Generic;
using UnityEngine;

namespace Kato
{
    [System.Serializable]
    public class ClipData
    {
        public string id;
        public AudioClip clip;
        public ClipType type;
    }

    public enum ClipType
    {
        BGM = 0,
        LoopSE = 1,
        OneShotSE = 2,
    }

    public class AudioManager : MonoBehaviour
    {
        [SerializeField, Header("AudioClipİ’è")] private ClipData[] _clip;
        [SerializeField] AudioSource[] audioSources;

        private Dictionary<string, ClipData> _dict;

        static AudioManager instance { get; set; }

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

            InitDictionary();//Dictionary‰Šú‰»
        }

        /// <summary>
        /// Dictionary‰Šú‰»
        /// </summary>
        private void InitDictionary()
        {
            _dict = new Dictionary<string, ClipData>();
            foreach (ClipData data in _clip)
            {
                _dict[data.id] = data;
            }
        }

        /// <summary>
        /// ID‚ÉŠî‚Ã‚«Clip‚ğæ“¾
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClipData GetClip(string id)
        {
            return _dict.TryGetValue(id, out ClipData data) ? data : null;
        }

        /// <summary>
        /// AudioÄ¶
        /// </summary>
        /// <param name="id"></param>
        public void PlayAudio(string id)
        {
            AudioSource source = audioSources[(int)GetClip(id).type];

            switch (GetClip(id).type)
            {
                case ClipType.BGM:
                    source.Play();
                    break;
                case ClipType.LoopSE:
                    source.Play();
                    break;
                case ClipType.OneShotSE:
                    source.PlayOneShot(GetClip(id).clip);
                    break;
            }
        }
    }
}