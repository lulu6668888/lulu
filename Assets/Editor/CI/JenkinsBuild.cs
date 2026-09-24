using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class JenkinsBuild
{
    public static void Build()
    {
        string platform = GetArg("-platform", "Android");
        string appVersion = GetArg("-appVersion", "1.0.0");
        int buildNumber = GetIntArg("-buildNumber", 1);

        Debug.Log("====================================");
        Debug.Log("Jenkins Unity Build");
        Debug.Log($"Platform: {platform}");
        Debug.Log($"Version : {appVersion}");
        Debug.Log($"Build   : {buildNumber}");
        Debug.Log("====================================");

        if (platform.Equals(
                "Android",
                StringComparison.OrdinalIgnoreCase))
        {
            BuildAndroid(appVersion, buildNumber);
        }
        else if (platform.Equals(
                     "iOS",
                     StringComparison.OrdinalIgnoreCase))
        {
            BuildIOS(appVersion, buildNumber);
        }
        else
        {
            throw new Exception(
                $"Unknown platform: {platform}");
        }
    }

    private static void BuildAndroid(
        string appVersion,
        int buildNumber)
    {
        EditorUserBuildSettings.buildAppBundle = false;

        PlayerSettings.bundleVersion = appVersion;

        PlayerSettings.Android.bundleVersionCode =
            buildNumber;

        string outputDir =
            Path.GetFullPath(
                "Build/Android");

        Directory.CreateDirectory(outputDir);

        string output =
            Path.Combine(
                outputDir,
                $"Game_{appVersion}_{buildNumber}.apk");

        Build(output, BuildTarget.Android);

        Debug.Log(
            $"Android APK: {output}");
    }

    private static void BuildIOS(
        string appVersion,
        int buildNumber)
    {
        PlayerSettings.bundleVersion =
            appVersion;

        PlayerSettings.iOS.buildNumber =
            buildNumber.ToString();

        string outputDir =
            Path.GetFullPath(
                "Build/iOS");

        Directory.CreateDirectory(outputDir);

        string output =
            Path.Combine(
                outputDir,
                "XcodeProject");

        Build(output, BuildTarget.iOS);

        Debug.Log(
            $"iOS Xcode Project: {output}");
    }

    private static void Build(
        string outputPath,
        BuildTarget target)
    {
        string[] scenes =
            GetBuildScenes();

        BuildPlayerOptions options =
            new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = outputPath,
                target = target,
                targetGroup =
                    BuildPipeline.GetBuildTargetGroup(target),
                options = BuildOptions.None
            };

        BuildReport report =
            BuildPipeline.BuildPlayer(options);

        if (report.summary.result !=
            BuildResult.Succeeded)
        {
            throw new Exception(
                $"Build failed: {report.summary.result}");
        }

        Debug.Log(
            $"Build success: {outputPath}");
    }

    private static string[] GetBuildScenes()
    {
        return new[]
        {
            // "Assets/Scenes/Boot.unity",
            // "Assets/Scenes/Login.unity",
            "Assets/Scenes/SampleScene.unity"
        };
    }

    private static string GetArg(
        string name,
        string defaultValue)
    {
        string[] args =
            Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == name)
                return args[i + 1];
        }

        return defaultValue;
    }

    private static int GetIntArg(
        string name,
        int defaultValue)
    {
        string value =
            GetArg(
                name,
                defaultValue.ToString());

        return int.TryParse(
            value,
            out int result)
            ? result
            : defaultValue;
    }
}