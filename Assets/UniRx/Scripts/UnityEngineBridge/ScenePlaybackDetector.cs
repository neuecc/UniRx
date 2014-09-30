#if UNITY_EDITOR

using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;

namespace UniRx
{
    [InitializeOnLoad]
    public class ScenePlaybackDetector
    {
        private static bool _isPlaying = false;

        public static bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                }
            }
        }

        // This callback is notified just before building the scene, before Start().
        [PostProcessScene]
        public static void OnPostprocessScene()
        {
            IsPlaying = true;
        }

        // InitializeOnLoad ensures that this constructor is called when the Unity Editor is started.
        static ScenePlaybackDetector()
        {
            // This callback comes after Start(), it's too late. But it's useful for detecting playback stop.
            EditorApplication.playmodeStateChanged += () => { IsPlaying = EditorApplication.isPlayingOrWillChangePlaymode; };
        }
    }
}

#endif