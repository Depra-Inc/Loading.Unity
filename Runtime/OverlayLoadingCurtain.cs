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

		public OverlayLoadingCurtain(IAssetFile<LoadingCurtainViewRoot> assetFile) => _assetFile = assetFile;

		public LoadingCurtainViewModel ViewModel { get; private set; }

		public async Task Load(IEnumerable<ILoadingOperation> operations, CancellationToken token)
		{
			if (_original == null)
			{
				_original = await _assetFile.LoadAsync(cancellationToken: token);
			}

			ViewModel = new LoadingCurtainViewModel();
			_view = Object.Instantiate(_original);
			_view.Initialize(ViewModel);

			foreach (var operation in operations)
			{
				ViewModel.Initialize(operation);
				await operation.Load(OnProgress, token);
			}

			if (_view != null)
			{
				Object.Destroy(_view.gameObject);
			}

			return;

			void OnProgress(float progress) => ViewModel.Progress.Value = progress;
		}

		public void Unload()
		{
			ViewModel.Dispose();
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
	}
}