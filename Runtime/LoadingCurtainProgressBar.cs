// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.Loading
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(UIDocument))]
	public sealed class LoadingCurtainProgressBar : LoadingCurtainView
	{
		[SerializeField] private string _name = "ProgressBar";

		private ProgressBar _progressBar;
		private LoadingCurtainViewModel _viewModel;

		private void OnDestroy()
		{
			if (_viewModel != null)
			{
				_viewModel.Progress.Changed -= OnProgressChanged;
			}
		}

		public override void Initialize(LoadingCurtainViewModel viewModel)
		{
			var document = GetComponent<UIDocument>();
			var rootElement = document.rootVisualElement;
			_progressBar = rootElement.Q<ProgressBar>(_name);
			_viewModel.Progress.Changed += OnProgressChanged;
		}

		private void OnProgressChanged(float progress) => _progressBar.value = progress;
	}
}