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
		[SerializeField] private float _speed = 0.5f;
		[Min(0)] [SerializeField] private float _maxValue = 1;
		[SerializeField] private string _bindingPath = "ProgressBar";

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
			_viewModel = viewModel;
			_viewModel.Progress.Changed += OnProgressChanged;
			_progressBar = GetComponent<UIDocument>().rootVisualElement.Q<ProgressBar>(_bindingPath);
		}

		private void OnProgressChanged(float progress)
		{
			var target = _maxValue / 0.9f;
			var maxDelta = _speed * Time.deltaTime;
			_progressBar.value = Mathf.MoveTowards(progress, target, maxDelta);
		}
	}
}