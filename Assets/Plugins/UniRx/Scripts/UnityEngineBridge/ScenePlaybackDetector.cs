#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace UniRx
{
    [InitializeOnLoad]
    public class ScenePlaybackDetector
    {
        private static bool _isPlaying = false;

        private static bool AboutToStartScene
        {
            get
            {
                return EditorPrefs.GetBool("AboutToStartScene");
            }
            set
            {
                EditorPrefs.SetBool("AboutToStartScene", value);
            }
        }

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

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void OnRuntimeInitializeOnLoadMethod()
        {
            if (AboutToStartScene)
            {
                IsPlaying = true;
            }
        }

        // This callback is notified after scripts have been reloaded.
        [DidReloadScripts]
        public static void OnDidReloadScripts()
        {
            // Filter DidReloadScripts callbacks to the moment where playmodeState transitions into isPlaying.
            if (AboutToStartScene)
            {
                IsPlaying = true;
            }
        }

        // InitializeOnLoad ensures that this constructor is called when the Unity Editor is started.
        static ScenePlaybackDetector()
        {
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += e =>
            {
                if (e == PlayModeStateChange.ExitingEditMode)
                {
                    AboutToStartScene = true;
                }
                else
                {
                    AboutToStartScene = false;
                }

                if (e == PlayModeStateChange.ExitingPlayMode)
                {
                    IsPlaying = false;
                }
            };
#else
            EditorApplication.playmodeStateChanged += () =>
            {
                // Before scene start:          isPlayingOrWillChangePlaymode = false;  isPlaying = false
                // Pressed Playback button:     isPlayingOrWillChangePlaymode = true;   isPlaying = false
                // Playing:                     isPlayingOrWillChangePlaymode = false;  isPlaying = true
                // Pressed stop button:         isPlayingOrWillChangePlaymode = true;   isPlaying = true
                if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
                {
                    AboutToStartScene = true;
                }
                else
                {
                    AboutToStartScene = false;
                }

                // Detect when playback is stopped.
                if (!EditorApplication.isPlaying)
                {
                    IsPlaying = false;
                }
            };
#endif
        }
    }
}

#endif