﻿// SPDX-License-Identifier: Apache-2.0
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

		private int _operationsCount;
		private int _completedOperations;
		private LoadingCurtainViewRoot _view;
		private LoadingCurtainViewRoot _original;
		private LoadingCurtainViewModel _viewModel;

		public OverlayLoadingCurtain(IAssetFile<LoadingCurtainViewRoot> assetFile) => _assetFile = assetFile;

		public async Task Load(Queue<ILoadingOperation> operations, CancellationToken cancellationToken)
		{
			if (_original == null)
			{
				_original = await _assetFile.LoadAsync(cancellationToken: cancellationToken);
			}

			_operationsCount = operations.Count;
			_viewModel = new LoadingCurtainViewModel();
			_view = Object.Instantiate(_original);
			_view.Initialize(_viewModel);

			foreach (var operation in operations)
			{
				_viewModel.Description.Value = operation.Description;
				await operation.Load(OnProgress, cancellationToken);
				_completedOperations++;
			}

			await WaitForViewClosed(cancellationToken);
		}

		public Task Unload(CancellationToken cancellationToken)
		{
			_operationsCount = 0;
			_completedOperations = 0;
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

		private void OnProgress(float progress) =>
			_viewModel.Progress.Value = progress * _completedOperations / _operationsCount;

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