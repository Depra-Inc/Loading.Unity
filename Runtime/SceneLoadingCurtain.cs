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
	public sealed class UnityLoadingCurtain : ILoadingCurtain
	{
		private readonly IAssetFile<LoadingCurtainView> _assetFile;

		private LoadingCurtainView _view;
		private LoadingCurtainView _original;

		public UnityLoadingCurtain(IAssetFile<LoadingCurtainView> assetFile) => _assetFile = assetFile;

		async Task ILoadingCurtain.Load(IEnumerable<ILoadingOperation> operations,
			CancellationToken token)
		{
			if (_original == null)
			{
				_original = await _assetFile.LoadAsync(cancellationToken: token);
			}

			using var viewModel = new LoadingCurtainViewModel();
			_view = Object.Instantiate(_original);
			_view.Initialize(viewModel);

			foreach (var operation in operations)
			{
				viewModel.Initialize(operation);
				await operation.Load(OnProgress, token);
			}

			if (_view != null)
			{
				Object.Destroy(_view.gameObject);
			}

			return;

			void OnProgress(float progress) => viewModel.Progress.Value = progress;
		}

		void ILoadingCurtain.Unload()
		{
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