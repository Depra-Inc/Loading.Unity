// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Depra.Expectation;
using Depra.Threading;
using UnityEngine;
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

		public async ITask Load(Queue<ILoadingOperation> operations, CancellationToken token)
		{
			var operationsReady = new Expectant();
			await _overlayCurtain.Prepare(operations, operationsReady, token);
			await LoadScene();
			_overlayCurtain.MoveViewTo(SceneManager.GetSceneByName(_sceneName));
			await _overlayCurtain.LoadInternal(operations, operationsReady, token);
		}

		public async ITask Unload(CancellationToken cancellationToken)
		{
			await _overlayCurtain.Unload(cancellationToken);
			await UnloadScene();
		}

		private async ITask LoadScene()
		{
			if (!Application.CanStreamedLevelBeLoaded(_sceneName))
			{
				throw new InvalidOperationException($"Scene '{_sceneName}' is not in build settings.");
			}

			try
			{
				var operation = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
				await WaitForOperation(operation);
			}
			catch
			{
				await TryUnloadScene();
				CreateEmptyScene();
			}

			var scene = SceneManager.GetSceneByName(_sceneName);
			if (scene.IsValid())
			{
				SceneManager.SetActiveScene(scene);
			}
		}

		private async ITask UnloadScene()
		{
			var scene = SceneManager.GetSceneByName(_sceneName);
			if (!scene.isLoaded)
			{
				return;
			}

			var operation = SceneManager.UnloadSceneAsync(_sceneName);
			if (operation == null)
			{
				throw new InvalidOperationException($"Scene '{_sceneName}' not found.");
			}

			await WaitForOperation(operation);
		}

		private async ITask TryUnloadScene()
		{
			var scene = SceneManager.GetSceneByName(_sceneName);
			if (scene.isLoaded)
			{
				await SceneManager.UnloadSceneAsync(scene);
			}
		}

		private static async ITask WaitForOperation(AsyncOperation operation)
		{
			while (!operation.isDone)
			{
				await Task.Yield();
			}
		}

		private void CreateEmptyScene()
		{
			var scene = SceneManager.CreateScene(_sceneName);
			SceneManager.SetActiveScene(scene);
		}
	}
}
