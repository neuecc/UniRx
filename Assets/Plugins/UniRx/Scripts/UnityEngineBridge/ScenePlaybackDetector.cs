#if UNITY_EDITOR

using UnityEditor;

namespace UniRx
{
    [InitializeOnLoad]
    public class ScenePlaybackDetector
    {
        private static bool _isPlaying = false;


        public static bool IsPlaying
        {
            get => _isPlaying;
            private set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                }
            }
        }


        [InitializeOnEnterPlayMode]
        public static void OnDidReloadScriptsA()
        {
            IsPlaying = true;
        }

        // InitializeOnLoad ensures that this constructor is called when the Unity Editor is started.
        static ScenePlaybackDetector()
        {
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += e =>
#else
            EditorApplication.playmodeStateChanged += () =>
#endif
            {
                // Before scene start:          isPlayingOrWillChangePlaymode = false;  isPlaying = false
                // Pressed Playback button:     isPlayingOrWillChangePlaymode = true;   isPlaying = false
                // Playing:                     isPlayingOrWillChangePlaymode = false;  isPlaying = true
                // Pressed stop button:         isPlayingOrWillChangePlaymode = true;   isPlaying = true

                if (!EditorApplication.isPlaying)
                {
                    IsPlaying = false;
                }
            };
        }
    }
}

#endif