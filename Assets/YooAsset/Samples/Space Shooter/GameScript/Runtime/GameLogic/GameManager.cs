﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Event;
using UniFramework.Machine;
using UniFramework.Module;
using YooAsset;

public class GameManager : ModuleSingleton<GameManager>, IModule
{
	public static void GameStart()
	{
		byte[] LoadMetaFormAot(string dllname)
		{
			Debug.Log($"补充元数据:{dllname}");
            var handle = YooAssets.LoadRawFileSync($"{dllname}.dll");
            handle.WaitForAsyncComplete();
            byte[] assemblyData = handle.GetRawFileData();
			return assemblyData;
        }
		//Aot
		HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(LoadMetaFormAot("mscorlib"), HybridCLR.HomologousImageMode.SuperSet);
        HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(LoadMetaFormAot("System"), HybridCLR.HomologousImageMode.SuperSet);
        HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(LoadMetaFormAot("UniTask"), HybridCLR.HomologousImageMode.SuperSet);
        HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(LoadMetaFormAot("YooAsset"), HybridCLR.HomologousImageMode.SuperSet);


        // 创建游戏管理器
        UniModule.CreateModule<GameManager>();

        // 开启游戏流程
        GameManager.Instance.Run();
    }


	private bool _isRun = false;
	private EventGroup _eventGroup = new EventGroup();
	private StateMachine _machine;

	void IModule.OnCreate(object createParam)
	{
	}
	void IModule.OnDestroy()
	{
		_eventGroup.RemoveAllListener();
	}
	void IModule.OnUpdate()
	{
		if (_machine != null)
			_machine.Update();
	}

	public void Run()
	{
		if (_isRun == false)
		{
			_isRun = true;

			// 注册监听事件
			_eventGroup.AddListener<SceneEventDefine.ChangeToHomeScene>(OnHandleEventMessage);
			_eventGroup.AddListener<SceneEventDefine.ChangeToBattleScene>(OnHandleEventMessage);

			Debug.Log("开启游戏流程...");
			_machine = new StateMachine(this);
			_machine.AddNode<FsmInitGame>();
			_machine.AddNode<FsmSceneHome>();
			_machine.AddNode<FsmSceneBattle>();
			_machine.Run<FsmInitGame>();
		}
		else
		{
			Debug.LogWarning("补丁更新已经正在进行中!");
		}
	}

	/// <summary>
	/// 接收事件
	/// </summary>
	private void OnHandleEventMessage(IEventMessage message)
	{
		if(message is SceneEventDefine.ChangeToHomeScene)
		{
			_machine.ChangeState<FsmSceneHome>();
		}
		else if(message is SceneEventDefine.ChangeToBattleScene)
		{
			_machine.ChangeState<FsmSceneBattle>();
		}
	}
}