﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

namespace MonitorLib.GOT.Editor
{
    public class AllFunctionDatas
    {
        public Dictionary<string, List<FunctionMonitorDatas>> ProfilerFunctionDatas;
    }

    public static class HookUtil
    {
        static Dictionary<string, List<FunctionMonitorDatas>> profilersDatas = new Dictionary<string, List<FunctionMonitorDatas>>();
        static Thread mainThread = Thread.CurrentThread;
        public static void Begin(string methodName)
        {
            //一些方法不能再Unity主线程调用
            if (Thread.CurrentThread == mainThread)
            {

            }
            FunctionMonitorDatas data = new FunctionMonitorDatas()
            {
                BeginTotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong(),
                BeginTime = Time.realtimeSinceStartup
            };
            if (profilersDatas.ContainsKey(methodName))
            {
                var datas = profilersDatas[methodName];
                data.Index = datas.Count + 1;
                datas.Add(data);
            }
            else
            {
                var methodProfileDatas = new List<FunctionMonitorDatas>();
                data.Index = 1;
                methodProfileDatas.Add(data);
                profilersDatas.Add(methodName, methodProfileDatas);
            }
        }

        public static void End(string methodName)
        {
            if (!profilersDatas.ContainsKey(methodName))
            {
                Debug.LogError($"没有注册方法{methodName}");
                return;
            }
            var lastMethodProfileData = profilersDatas[methodName].Last();
            lastMethodProfileData.EndTotalAllocatedMemory = Profiler.GetTotalAllocatedMemoryLong();
            lastMethodProfileData.DeltaAllocatedMemory = lastMethodProfileData.EndTotalAllocatedMemory - lastMethodProfileData.BeginTotalAllocatedMemory;
            lastMethodProfileData.EndTime = Time.realtimeSinceStartup;
            lastMethodProfileData.DeltaTime = lastMethodProfileData.EndTime - lastMethodProfileData.BeginTime;

            //需要屏蔽
            Debug.LogError($"{methodName}   " + lastMethodProfileData.ToString());
        }

        public static void BeginSample(string methodName)
        {
            Profiler.BeginSample(methodName);
        }

        public static void EndSample()
        {
            Profiler.EndSample();
        }
    }
}
