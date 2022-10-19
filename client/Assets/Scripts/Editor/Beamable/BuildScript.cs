using System;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Beamable.Editor
{
    public static class BuildScript
    {
        private static string[] GetActiveScenes()
        {
            var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).Select(scene => scene.path).ToArray();
            return scenes;
        }

        private static string GetBaseBuildPath()
        {
            string path = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName;
            path = string.IsNullOrEmpty(path) ? "/github/workspace/dist" : Path.Combine(path, "dist");

            return path;
        }

        private static string GetBuildPathForTarget(BuildTarget target, string prefix)
        {
            switch (target)
            {
                case UnityEditor.BuildTarget.iOS:
                    return Path.Combine(prefix, "iOS", "iOS");
                case UnityEditor.BuildTarget.Android:
                    return Path.Combine(prefix, "Android");
                case UnityEditor.BuildTarget.StandaloneWindows:
                case UnityEditor.BuildTarget.StandaloneWindows64:
                    return Path.Combine(prefix, "StandaloneWindows");
                case UnityEditor.BuildTarget.StandaloneOSX:
                    return Path.Combine(prefix, "StandaloneOSX");
                case UnityEditor.BuildTarget.WebGL:
                    return Path.Combine(prefix, "WebGL");
                default:
                    throw new Exception(
                        $"Invalid Build Target! Cannot get an output directory for this target. Target=[{target}]");
            }
        }

        private static void BuildTarget(BuildTarget target, bool developmentBuild = false)
        {
            try
            {
                // Clean first
                var basePath = GetBaseBuildPath();
                if (Directory.Exists(basePath))
                {
                    Directory.Delete(basePath, true);
                }

                //Build
                var path = GetBuildPathForTarget(target, basePath);
                var results = BuildPipeline.BuildPlayer(GetActiveScenes(), path, target,
                    developmentBuild ? BuildOptions.Development : BuildOptions.None);

                if (results.summary.result != BuildResult.Succeeded)
                {
                    throw new BuildFailedException("Build failed.");
                }
            }
            catch (BuildFailedException e)
            {
                Debug.LogError(e.StackTrace);
                Debug.LogError(e.Message);
                Debug.LogError(e.Data);
            }
        }

        [MenuItem("Beamable/Debug build")]
        static void DevelopmentBuild()
        {
            BuildTarget(EditorUserBuildSettings.activeBuildTarget);
        }

        [PublicAPI]
        public static void DebugWebGLBuild() => BuildTarget(UnityEditor.BuildTarget.WebGL);
    }
}