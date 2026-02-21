using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using System.IO;

public class BuildScript
{
    [MenuItem("Tools/Build Windows Game")]
    public static void BuildGame()
    {
        string buildPath = "Builds/Windows/Tetris.exe";
        string dir = Path.GetDirectoryName(buildPath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        string[] scenes = { "Assets/Scenes/SampleScene.unity" };
        
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = buildPath;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        Debug.Log("Starting Windows build to " + buildPath + " ...");
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        
        Debug.Log("Build Result: " + report.summary.result.ToString());
    }
}