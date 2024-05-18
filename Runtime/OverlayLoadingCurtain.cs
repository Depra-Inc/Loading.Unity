// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Files;
using Depra.Expectation;
using Depra.Loading.Curtain;
using Depra.Loading.Operations;
using UnityEngine;
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

		public async Task Load(Queue<ILoadingOperation> operations, CancellationToken token)
		{
			_operationIndex = 0;
			_operationsCount = operations.Count;

			_original ??= await _assetFile.LoadAsync(cancellationToken: token);
			_viewModel = new LoadingCurtainViewModel();
			_view = Object.Instantiate(_original);

			var operationsReady = new Expectant();
			var viewExpectant = new GroupExpectant.And()
				.With(operationsReady);

			_view.Initialize(_viewModel, viewExpectant);
			_viewReady = viewExpectant.Build();

			foreach (var operation in operations)
			{
				_viewModel.Description.Value = operation.Description;
				await operation.Load(new Progress<float>(OnProgress), token);
				_operationIndex++;
			}

			operationsReady.SetReady();
			await WaitForViewClosed(token);
		}

		public Task Unload(CancellationToken token)
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

			return Task.CompletedTask;
		}

		private void OnProgress(float progress)
		{
			var normalized = (_operationIndex + progress) / _operationsCount;
			if (Mathf.Abs(normalized - 1) < 0.01f)
			{
				normalized = 1;
			}

			_viewModel.Progress.Value = normalized;
		}

		private async Task WaitForViewClosed(CancellationToken token)
		{
			while (_viewReady.IsReady() == false)
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