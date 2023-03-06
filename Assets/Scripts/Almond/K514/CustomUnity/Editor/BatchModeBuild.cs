using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace k514
{
    public class BatchModeBuild
    {
        /// <summary>
        /// C:\ SomeWhere \Editor >> Unity.exe -batchmode -quit -logFile [log Path] -projectPath [project Path] -batchmode -executeMethod k514.BatchModeBuild.Build
        ///
        /// log Path는 파일명을 포함하며, 해당 패스의 루트는 Unity.exe가 위치하는 디렉터리이고 해당 로그 파일이 없다면 오류가 발생하므로 사전에 만들어주어야만 한다.
        /// </summary>
        private static void Build()
        {
            var outputPath = Path.Combine(Environment.CurrentDirectory, "Build");
            if (Directory.Exists(outputPath))
            {
                Directory.Delete(outputPath, true);
            }
            Directory.CreateDirectory(outputPath);

            var buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = SystemMaintenance.GetBuiltInScenePathSet();
            buildPlayerOptions.locationPathName = Path.Combine("Build", "BuildResult.x86_64");
            buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
            buildPlayerOptions.options = BuildOptions.EnableHeadlessMode;

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;

            switch (summary.result)
            {
                case BuildResult.Unknown:
                    break;
                case BuildResult.Succeeded:
                    Debug.LogError($"k514, Build Success [{summary.totalSize} Bytes]");
                    break;
                case BuildResult.Failed:
                    Debug.LogError("k514, [Error] Build failed");
                    break;
                case BuildResult.Cancelled:
                    Debug.LogError("k514, [Error] Build Cancelled");
                    break;
            }
        }
    }
}
