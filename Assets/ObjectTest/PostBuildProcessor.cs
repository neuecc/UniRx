#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public static class PostBuildProcessor
{

#if UNITY_CLOUD_BUILD
    /*
    This methods are per platform post export methods. They can be added additionally to the post process attributes in the Advanced Features Settings on UCB using
    - PostBuildProcessor.OnPostprocessBuildiOS
    - PostBuildProcessor.OnPostprocessBuildAndroid
    depending on the platform they should be executed.
 
    Here is the basic order of operations (e.g. with iOS operation)
    - Unity Cloud Build Pre-export methods run
    - Export process happens
    - Methods marked the built-in PostProcessBuildAttribute are called
    - Unity Cloud Build Post-export methods run
    - [unity ends]
    - (iOS) Xcode processes project
    - Done!
    More information can be found on http://forum.unity3d.com/threads/solved-ios-build-failed-pushwoosh-dependency.293192/
    */
    public static void OnPostprocessBuildiOS (string exportPath)
    {
        Debug.Log("[UCB] OnPostprocessBuildiOS");
        ProcessPostBuild(BuildTarget.iOS,exportPath);
    }
    public static void OnPostprocessBuildAndroid (string exportPath)
    {
        Debug.Log("[UCB] OnPostprocessBuildAndroid");
        ProcessPostBuild(BuildTarget.Android,exportPath);
    }
#endif

    // a normal post process method which is executed by Unity
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
#if !UNITY_CLOUD_BUILD
        Debug.Log("[UCB] OnPostprocessBuild");
        ProcessPostBuild(buildTarget, path);
#endif
    }

    private static void ProcessPostBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject proj = new PBXProject();
            string nativeTarget = proj.TargetGuidByName(UnityEditor.iOS.Xcode.PBXProject.GetUnityTargetName());
            string testTarget = proj.TargetGuidByName(UnityEditor.iOS.Xcode.PBXProject.GetUnityTestTargetName());
            string[] buildTargets = new string[] { nativeTarget, testTarget };
            proj.ReadFromString(File.ReadAllText(projPath));
            proj.SetBuildProperty(buildTargets, "ENABLE_BITCODE", "NO");
            File.WriteAllText(projPath, proj.WriteToString());
        }
    }

}

#endif