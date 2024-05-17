// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Expectation;
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

		private float _target;
		private float _velocity;
		private ProgressBar _bar;
		private Expectant _filled;
		private LoadingCurtainViewModel _viewModel;

		private void Awake() => enabled = false;

		private void Update()
		{
			_bar.value = Mathf.SmoothDamp(_bar.value, _target, ref _velocity, _smoothTime);
			if (_bar.value >= 1)
			{
				MarkAsFilled();
			}
		}

		private void OnDestroy()
		{
			_filled?.Dispose();
			if (_viewModel != null)
			{
				_viewModel.Progress.Changed -= OnProgressChanged;
			}
		}

		public override void Initialize(LoadingCurtainViewModel viewModel, IGroupExpectant expectant)
		{
			_viewModel = viewModel;
			_viewModel.Progress.Changed += OnProgressChanged;

			expectant.With(_filled = new Expectant());
			_bar = GetComponent<UIDocument>().rootVisualElement.Q<ProgressBar>(_bindingPath);

			enabled = true;
		}

		private void MarkAsFilled()
		{
			enabled = false;
			_filled.SetReady();
		}

		private void OnProgressChanged(float value) => _target = value;
	}
}