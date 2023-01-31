using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using UniFramework.Module;
using System.Reflection;
using System;
using YooAsset;

/// <summary>
/// 流程更新完毕
/// </summary>
internal class FsmPatchDone : IStateNode
{
	void IStateNode.OnCreate(StateMachine machine)
	{
	}
	void IStateNode.OnEnter()
	{
		PatchEventDefine.PatchStatesChange.SendEventMessage("开始游戏！");
		//加载dll 执行gamestart
#if UNITY_EDITOR
        Assembly ass = Assembly.Load("SpaceShooterSample");
#else
		//从你的资源管理系统中获得热更新dll的数据
		var handle = YooAssets.LoadAssetSync<TextAsset>("SpaceShooterSample.dll");
		handle.WaitForAsyncComplete();
        byte[] assemblyData = (handle.AssetObject as TextAsset).bytes;
		Assembly ass = Assembly.Load(assemblyData);
#endif

        Type entryType = ass.GetType("GameManager");
		Debug.Log($"entryType:{entryType}");
        MethodInfo method = entryType.GetMethod("GameStart");
        Action mainFunc = (Action)Delegate.CreateDelegate(typeof(Action), method);
        mainFunc();

    }
	void IStateNode.OnUpdate()
	{
	}
	void IStateNode.OnExit()
	{
	}
}