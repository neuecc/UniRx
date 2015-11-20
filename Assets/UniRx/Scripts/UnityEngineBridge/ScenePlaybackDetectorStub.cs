using System;
using System.Linq;
using System.Reflection;

namespace UniRx
{
    public static class ScenePlaybackDetectorStub
    {
        private static Type scenePlaybackDetectorType = null;

        private static PropertyInfo isPlayingProperty = null;

        static ScenePlaybackDetectorStub()
        {
            scenePlaybackDetectorType = TypeLoader.GetType("ScenePlaybackDetector");
            if (scenePlaybackDetectorType != null)
            {
                isPlayingProperty = scenePlaybackDetectorType.GetProperty(
                    "IsPlaying",
                    BindingFlags.Public | BindingFlags.Static);
            }
        }

        public static bool IsPlaying
        {
            get
            {
                if (scenePlaybackDetectorType == null)
                {
                    // always playing in player
                    return true;
                }

                return (bool)isPlayingProperty.GetValue(null, null);
            }
        }
    }
}