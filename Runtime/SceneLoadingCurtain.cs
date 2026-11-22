// SPDX-License-Identifier: Apache-2.0
// © 2023-2026 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.IO;
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
		private readonly string _scenePath;
		private readonly OverlayLoadingCurtain _overlayCurtain;

		private Scene _scene;

		public SceneLoadingCurtain(string scenePath, OverlayLoadingCurtain overlayCurtain)
		{
			_scenePath = scenePath;
			_overlayCurtain = overlayCurtain;
		}

		public async ITask Load(Queue<ILoadingOperation> operations, CancellationToken token)
		{
			var operationsReady = new Expectant();
			await _overlayCurtain.Prepare(operations, operationsReady, token);
			await LoadScene();
			_overlayCurtain.MoveViewTo(_scene);
			await _overlayCurtain.LoadInternal(operations, operationsReady, token);
		}

		public async ITask Unload(CancellationToken cancellationToken)
		{
			await _overlayCurtain.Unload(cancellationToken);
			await UnloadScene();
		}

		private async ITask LoadScene()
		{
			if (!Application.CanStreamedLevelBeLoaded(_scenePath))
			{
				throw new InvalidOperationException($"Scene '{_scenePath}' is not in build settings.");
			}

			try
			{
				await WaitForOperation(SceneManager.LoadSceneAsync(_scenePath, LoadSceneMode.Additive));
				_scene = SceneManager.GetSceneByPath(_scenePath);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				_scene = SceneManager.GetSceneByPath(_scenePath);
				await UnloadScene();
				_scene = SceneManager.CreateScene(Path.GetFileNameWithoutExtension(_scenePath));
			}

			if (_scene.IsValid())
			{
				SceneManager.SetActiveScene(_scene);
			}
		}

		private async ITask UnloadScene()
		{
			var scene = _scene;
			_scene = default;
			if (!scene.IsValid() || !scene.isLoaded)
			{
				return;
			}

			await WaitForOperation(SceneManager.UnloadSceneAsync(scene));
		}

		private static async ITask WaitForOperation(AsyncOperation operation)
		{
			if (operation == null)
			{
				throw new InvalidOperationException("Scene operation could not be started.");
			}

			while (!operation.isDone)
			{
				await Task.Yield();
			}
		}
	}
}
