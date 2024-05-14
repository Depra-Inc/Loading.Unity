// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Depra.Loading.Curtain;
using Depra.Loading.Operations;
using UnityEngine.SceneManagement;

namespace Depra.Loading
{
	public sealed class SceneLoadingCurtain : ILoadingCurtain
	{
		private readonly string _sceneName;
		private readonly OverlayLoadingCurtain _overlayCurtain;

		public SceneLoadingCurtain(string sceneName, OverlayLoadingCurtain overlayCurtain)
		{
			_sceneName = sceneName;
			_overlayCurtain = overlayCurtain;
		}

		public async Task Load(IEnumerable<ILoadingOperation> operations, CancellationToken token)
		{
			await LoadScene(token);
			_overlayCurtain.Completed += Unload;
			await _overlayCurtain.Load(operations, token);
		}

		public void Unload()
		{
			_overlayCurtain.Completed -= Unload;
			_overlayCurtain.Unload();
			_ = UnloadScene(CancellationToken.None);
		}

		private async Task LoadScene(CancellationToken token)
		{
			var loadSceneOperation = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
			if (loadSceneOperation == null)
			{
				throw new InvalidOperationException($"Scene '{_sceneName}' not found.");
			}

			loadSceneOperation.allowSceneActivation = true;
			while (loadSceneOperation.isDone == false)
			{
				if (token.IsCancellationRequested)
				{
					throw new TaskCanceledException();
				}

				await Task.Yield();
			}
		}

		private async Task UnloadScene(CancellationToken token)
		{
			var unloadSceneOperation = SceneManager.UnloadSceneAsync(_sceneName);
			if (unloadSceneOperation == null)
			{
				throw new InvalidOperationException($"Scene '{_sceneName}' not found.");
			}

			while (unloadSceneOperation.isDone == false)
			{
				if (token.IsCancellationRequested)
				{
					throw new TaskCanceledException();
				}

				await Task.Yield();
			}
		}
	}
}