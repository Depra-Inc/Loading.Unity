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
		[Min(0)] [SerializeField] private float _maxDelta = 0.01f;
		[SerializeField] private string _bindingPath = "ProgressBar";

		private float _target;
		private ProgressBar _bar;
		private Expectant _filled;
		private LoadingCurtainViewModel _viewModel;

		private void Awake() => enabled = false;

		private void Update()
		{
			if (Mathf.Approximately(_bar.value, 1) || _bar.value >= 1)
			{
				enabled = false;
				_filled.SetReady();
			}

			var delta = (_target - _bar.value) / 2;
			_bar.value = delta > _maxDelta ? _bar.value + delta : _target;
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

		private void OnProgressChanged(float value) => _target = value;
	}
}