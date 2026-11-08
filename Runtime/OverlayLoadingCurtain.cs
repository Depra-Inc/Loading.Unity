// SPDX-License-Identifier: Apache-2.0
// © 2023-2025 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets;
using Depra.Expectation;
using Depra.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Depra.Loading
{
	public sealed class OverlayLoadingCurtain : ILoadingCurtain
	{
		private readonly IAssetFile<LoadingCurtainViewRoot> _assetFile;

		private int _operationIndex;
		private int _operationsCount;
		private IExpectant _viewReady;
		private LoadingCurtainViewRoot _view;
		private LoadingCurtainViewRoot _original;
		private LoadingCurtainViewModel _viewModel;

		public OverlayLoadingCurtain(IAssetFile<LoadingCurtainViewRoot> assetFile) => _assetFile = assetFile;

		public async ITask Load(Queue<ILoadingOperation> operations, CancellationToken token)
		{
			var operationsReady = new Expectant();
			await Prepare(operations, operationsReady, token);
			await LoadInternal(operations, operationsReady, token);
		}

		internal async ITask Prepare(Queue<ILoadingOperation> operations, Expectant operationsReady, CancellationToken token)
		{
			_original ??= await _assetFile.LoadAsync(cancellation: token);
			_viewModel = new LoadingCurtainViewModel();
			_view = Object.Instantiate(_original);

			var viewExpectant = new GroupExpectant.And()
				.With(operationsReady);

			_view.Initialize(_viewModel, viewExpectant);
			_viewReady = viewExpectant.Build();

			// Otherwise the layout placeholder is visible until the first operation starts.
			if (operations.Count > 0)
			{
				_viewModel.Description.Value = operations.Peek().Description;
			}
		}

		internal async ITask LoadInternal(Queue<ILoadingOperation> operations,
			Expectant operationsReady, CancellationToken token)
		{
			_operationIndex = 0;
			_operationsCount = operations.Count;
			foreach (var operation in operations)
			{
				_viewModel.Description.Value = operation.Description;
				await operation.Load(new Progress<float>(OnProgress), token);
				OnProgress(1f);
				_operationIndex++;
			}

			operationsReady.SetReady();
			await WaitForViewClosed(token);
		}

		internal void MoveViewTo(Scene scene)
		{
			if (_view && scene.IsValid())
			{
				SceneManager.MoveGameObjectToScene(_view.gameObject, scene);
			}
		}

		public ITask Unload(CancellationToken token)
		{
			_operationIndex = 0;
			_operationsCount = 0;
			_viewModel?.Dispose();
			_viewReady?.Dispose();

			if (_view)
			{
				_view.TearDown();
				Object.Destroy(_view.gameObject);
			}

			try
			{
				_assetFile.Unload();
			}
			catch (Exception exception)
			{
				Debug.LogError(exception);
			}

			return Task.CompletedTask.AsITask();
		}

		private void OnProgress(float progress)
		{
			progress = Mathf.Clamp01(progress);
			var normalized = (_operationIndex + progress) / _operationsCount;
			_viewModel.Progress.Value = normalized;
		}

		private async Task WaitForViewClosed(CancellationToken token)
		{
			while (!_viewReady.IsReady())
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