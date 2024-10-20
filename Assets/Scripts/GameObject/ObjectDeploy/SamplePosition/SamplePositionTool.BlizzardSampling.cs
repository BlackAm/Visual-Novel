using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlackAm
{
    public partial class SamplePositionTool
    {        
        public const int __BLIZZARD_SAMPLING_MAX_SCALE = 7;
        public const int __BLIZZARD_SAMPLING_MAX_SCALE_SQR = __BLIZZARD_SAMPLING_MAX_SCALE * __BLIZZARD_SAMPLING_MAX_SCALE;
        public readonly static int[,] __BLIZZARD_SAMPLING_SEED_MAP = new int[__BLIZZARD_SAMPLING_MAX_SCALE, __BLIZZARD_SAMPLING_MAX_SCALE];
        public readonly static Dictionary<int, Vector3> __Inv_BLIZZARD_SAMPLING_SEED_MAP = new Dictionary<int, Vector3>();
        private readonly static Dictionary<BlizzardSamplingIndexType, Dictionary<int, (int t_Start, int t_End)>> BlizzardSamplingIndexTable = new Dictionary<BlizzardSamplingIndexType, Dictionary<int, (int t_Start, int t_End)>>();
        
        public enum BlizzardSamplingIndexType
        {
            PointUp, PointDown, SquareOut, SquareIn
        }

        private static void Initialize_BlizzardSampling()
        {
            var isEven = __BLIZZARD_SAMPLING_MAX_SCALE % 2 == 0;
            var startX = isEven ? __BLIZZARD_SAMPLING_MAX_SCALE / 2 - 1 : __BLIZZARD_SAMPLING_MAX_SCALE / 2;
            var startY = isEven ? startX + 1 : startX;
            var curX = startX;
            var curY = startY;
            var direction = 0;
            var count = Mathf.Pow(__BLIZZARD_SAMPLING_MAX_SCALE, 2);
            var currentProgress = 0;
            var progressScale = 1;
            var switchTrigger = 0;
            
            for (int i = 0; i < count; i++)
            {
                __BLIZZARD_SAMPLING_SEED_MAP[curX, curY] = i;
                __Inv_BLIZZARD_SAMPLING_SEED_MAP.Add(i, new Vector3(curX - startX, 0f, curY - startY));
                
                // 0 : ↓
                // 1 : →
                // 2 : ↑
                // 3 : ←
                switch (direction)
                {
                    case 3 :
                        curX--;
                        break;
                    case 2 :
                        curY++;
                        break;
                    case 1 :
                        curX++;
                        break;
                    default:
                    case 0 :
                        curY--;
                        break;
                }

                currentProgress++;
                if (currentProgress == progressScale)
                {
                    currentProgress = 0;
                    direction++;
                    if (direction > 3)
                    {
                        direction = 0;
                    }
                    switchTrigger++;
                    if (switchTrigger == 2)
                    {
                        switchTrigger = 0;
                        progressScale++;
                    }
                }
            }

            var indexTypeEnumerator =
                SystemTool.GetEnumEnumerator<BlizzardSamplingIndexType>(SystemTool.GetEnumeratorType.ExceptNone);

            foreach (var indexType in indexTypeEnumerator)
            {
                var indexTable = new Dictionary<int, (int t_Start, int t_End)>();
                BlizzardSamplingIndexTable.Add(indexType, indexTable);
                switch (indexType)
                {
                    case BlizzardSamplingIndexType.PointUp:
                    case BlizzardSamplingIndexType.PointDown:
                        break;
                    case BlizzardSamplingIndexType.SquareOut:
                    {
                        var startIndex = 0;
                        for (int i = 0; i < startX; i++)
                        {
                            var seed = CustomMath.PowInt(2 * i + 1, 2);
                            indexTable.Add(i, (startIndex, seed));
                            startIndex = seed;
                        }
                    }
                        break;
                    case BlizzardSamplingIndexType.SquareIn:
                    {
                        var traceTable = BlizzardSamplingIndexTable[BlizzardSamplingIndexType.SquareOut];
                        for (int i = 0; i < startX; i++)
                        {
                            var invIndex = startX - i - 1;
                            indexTable.Add(i, traceTable[invIndex]);
                        }
                    }
                        break;
                }
            }
        }

        public static void GetBlizzardSampling(ObjectDeployTimeline p_ObjectDeployTimeline, BlizzardSamplingIndexType p_Type, TransformTool.AffineCachePreset p_StartPivot, float p_Predelay, ObjectDeployTimelinePreset p_ObjectDeployTimelinePreset)
        {
            var count = Mathf.Min(p_ObjectDeployTimelinePreset.Count, __BLIZZARD_SAMPLING_MAX_SCALE_SQR);
            var interval = Mathf.Max(p_ObjectDeployTimelinePreset.Interval, CustomMath.Epsilon);
            var scale = p_ObjectDeployTimelinePreset.Scale * p_StartPivot.ScaleFactor;

            var sampleMap = __Inv_BLIZZARD_SAMPLING_SEED_MAP;
            switch (p_Type)
            {
                case BlizzardSamplingIndexType.PointUp:
                {
                    for (int i = 0; i < count; i++)
                    {
                        var tryTimeStamp = i * interval + p_Predelay;
                        var vectorScale = sampleMap[i];
                        var tryAffine = p_StartPivot.TransformPosition(scale * vectorScale);
                        p_ObjectDeployTimeline.AddTimelineAffine(tryTimeStamp, tryAffine);
                    }
                }
                    break;
                case BlizzardSamplingIndexType.PointDown:
                {
                    for (int i = 0; i < count; i++)
                    {
                        var tryTimeStamp = i * interval + p_Predelay;
                        var vectorScale = sampleMap[count - i - 1];
                        var tryAffine = p_StartPivot.TransformPosition(scale * vectorScale);
                        p_ObjectDeployTimeline.AddTimelineAffine(tryTimeStamp, tryAffine);
                    }
                }
                    break;
                case BlizzardSamplingIndexType.SquareOut:
                case BlizzardSamplingIndexType.SquareIn:
                    var seedIndex = 0;
                    var targetTable = BlizzardSamplingIndexTable[p_Type];
                    for (int i = 0; i < count;)
                    {
                        var tryTimeStamp = seedIndex * interval + p_Predelay;
                        var indexTuple = targetTable[seedIndex];
                        for (int j = indexTuple.t_Start; i < count && j < indexTuple.t_End; i++, j++)
                        {
                            var vectorScale = sampleMap[j];
                            var tryAffine = p_StartPivot.TransformPosition(scale * vectorScale);
                            p_ObjectDeployTimeline.AddTimelineAffine(tryTimeStamp, tryAffine);
                        }

                        seedIndex++;
                    }
                    break;
            }
        }

#if UNITY_EDITOR
        public static void DebugBlizzardSamplingArray()
        {
            for (var j = __BLIZZARD_SAMPLING_MAX_SCALE - 1; j > -1; j--)
            {
                var str = string.Empty;
                for (var i = 0; i < __BLIZZARD_SAMPLING_MAX_SCALE; i++)
                {
                    str += $"[{__BLIZZARD_SAMPLING_SEED_MAP[i, j]}] ";
                }
                Debug.Log(str);
            }
        }
#endif
    }
}