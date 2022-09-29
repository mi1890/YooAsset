﻿using UnityEngine;

namespace YooAsset
{
	internal class YooAssetDriver : MonoBehaviour
	{
		void Update()
		{
			YooAssets.Update();
		}

		void OnDestroy()
		{
			YooAssets.Destroy();
		}

		void OnApplicationQuit()
		{
			YooAssets.Destroy();
		}
	}
}