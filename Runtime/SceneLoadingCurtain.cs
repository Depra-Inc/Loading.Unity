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

		public async Task Load(Queue<ILoadingOperation> operations, CancellationToken token)
		{
			await LoadScene(token);
			await _overlayCurtain.Load(operations, token);
		}

		public async Task Unload(CancellationToken cancellationToken)
		{
			await _overlayCurtain.Unload(cancellationToken);
			await UnloadScene(cancellationToken);
		}

		private async Task LoadScene(CancellationToken token)
		{
			var operation = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
			if (operation == null)
			{
				throw new InvalidOperationException($"Scene '{_sceneName}' not found.");
			}

			SceneManager.sceneLoaded += OnSceneLoaded;
			while (operation.isDone == false)
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
			var operation = SceneManager.UnloadSceneAsync(_sceneName);
			if (operation == null)
			{
				throw new InvalidOperationException($"Scene '{_sceneName}' not found.");
			}

			while (operation.isDone == false)
			{
				if (token.IsCancellationRequested)
				{
					throw new TaskCanceledException();
				}

				await Task.Yield();
			}
		}

		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (scene.name != _sceneName)
			{
				return;
			}

			SceneManager.sceneLoaded -= OnSceneLoaded;
			if (scene.IsValid())
			{
				SceneManager.SetActiveScene(scene);
			}
		}
	}
}