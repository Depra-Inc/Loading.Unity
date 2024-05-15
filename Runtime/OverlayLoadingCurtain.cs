// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Depra.Assets.Files;
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
		private LoadingCurtainViewRoot _view;
		private LoadingCurtainViewRoot _original;
		private LoadingCurtainViewModel _viewModel;

		public OverlayLoadingCurtain(IAssetFile<LoadingCurtainViewRoot> assetFile) => _assetFile = assetFile;

		public async Task Load(Queue<ILoadingOperation> operations, CancellationToken cancellationToken)
		{
			_operationIndex = 0;
			_operationsCount = operations.Count;

			if (_original == null)
			{
				_original = await _assetFile.LoadAsync(cancellationToken: cancellationToken);
			}

			_viewModel = new LoadingCurtainViewModel();
			_view = Object.Instantiate(_original);
			_view.Initialize(_viewModel);

			foreach (var operation in operations)
			{
				_viewModel.Description.Value = operation.Description;
				await operation.Load(OnProgress, cancellationToken);
				_operationIndex++;
			}

			await WaitForViewClosed(cancellationToken);
		}

		public Task Unload(CancellationToken cancellationToken)
		{
			_operationIndex = 0;
			_operationsCount = 0;
			_viewModel?.Dispose();

			if (_view != null)
			{
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

		private void OnProgress(float progress) => _viewModel.Progress.Value = NormalizeProgress(progress);

		private float NormalizeProgress(float progress)
		{
			var normalizedProgress = (_operationIndex + progress) / _operationsCount;
			if (Mathf.Abs(normalizedProgress - 1) < 0.01f)
			{
				normalizedProgress = 1;
			}

			return normalizedProgress;
		}

		private async Task WaitForViewClosed(CancellationToken token)
		{
			while (_viewModel.NeedToClose == false)
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