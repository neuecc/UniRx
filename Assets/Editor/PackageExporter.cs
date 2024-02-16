﻿#if UNITY_EDITOR

using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class PackageExporter
{
    [MenuItem("Tools/Export Unitypackage")]
    public static void Export()
    {
        // configure
        var root = "Plugins/UniRx";
        var exportPath = "./UniRx.unitypackage";

        var path = Path.Combine(Application.dataPath, root);
        var assets = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
            .Where(x => Path.GetExtension(x) == ".cs" || Path.GetExtension(x) == ".asmdef" || Path.GetExtension(x) == ".json" || Path.GetExtension(x) == ".unity" || Path.GetExtension(x) == ".meta")
            .Select(x => "Assets" + x.Replace(Application.dataPath, "").Replace(@"\", "/"))
            .ToArray();

        Debug.Log("Export below files" + Environment.NewLine + string.Join(Environment.NewLine, assets));

        AssetDatabase.ExportPackage(
            assets,
            exportPath,
            ExportPackageOptions.Default);

        Debug.Log("Export complete: " + Path.GetFullPath(exportPath));
    }
}

#endif