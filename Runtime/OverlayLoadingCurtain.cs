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

		private LoadingCurtainViewRoot _view;
		private LoadingCurtainViewRoot _original;
		private LoadingCurtainViewModel _viewModel;

		public event Action Completed;

		public OverlayLoadingCurtain(IAssetFile<LoadingCurtainViewRoot> assetFile) => _assetFile = assetFile;


		public async Task Load(IEnumerable<ILoadingOperation> operations, CancellationToken token)
		{
			if (_original == null)
			{
				_original = await _assetFile.LoadAsync(cancellationToken: token);
			}

			_viewModel = new LoadingCurtainViewModel();
			_viewModel.Completed += OnCompleted;

			_view = Object.Instantiate(_original);
			_view.Initialize(_viewModel);

			foreach (var operation in operations)
			{
				_viewModel.Initialize(operation);
				await operation.Load(OnProgress, token);
			}

			if (_view != null)
			{
				Object.Destroy(_view.gameObject);
			}

			return;

			void OnProgress(float progress) => _viewModel.Progress.Value = progress;
		}

		public void Unload()
		{
			if (_viewModel != null)
			{
				_viewModel.Completed -= OnCompleted;
				_viewModel.Dispose();
			}

			if (_original)
			{
				_original = null;
			}

			try
			{
				_assetFile.Unload();
			}
			catch (Exception exception)
			{
				Debug.LogError(exception);
			}
		}

		private void OnCompleted() => Completed?.Invoke();
	}
}