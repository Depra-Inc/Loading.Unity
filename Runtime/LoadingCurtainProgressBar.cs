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
		[Min(0)] [SerializeField] private float _smoothTime = 0.1f;
		[SerializeField] private string _bindingPath = "ProgressBar";

		private float _velocity;
		private ProgressBar _progressBar;
		private LoadingCurtainViewModel _viewModel;

		private void Awake() => enabled = false;

		private void Update()
		{
			var current = _progressBar.value;
			var target = _viewModel.Progress.Value;
			_progressBar.value = Mathf.SmoothDamp(current, target, ref _velocity, _smoothTime);
		}

		public override void Initialize(LoadingCurtainViewModel viewModel)
		{
			_viewModel = viewModel;
			_progressBar = GetComponent<UIDocument>().rootVisualElement.Q<ProgressBar>(_bindingPath);
			enabled = true;
		}
	}
}