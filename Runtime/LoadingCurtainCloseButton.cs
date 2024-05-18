// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Expectation;
using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.Loading
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(UIDocument))]
	internal sealed class LoadingContinueButton : LoadingCurtainView
	{
		[SerializeField] private string _bindingPath = "ClickToContinue";

		private Button _button;
		private Expectant _buttonClicked;
		private LoadingCurtainViewModel _viewModel;

		public override void Initialize(LoadingCurtainViewModel viewModel, IGroupExpectant expectant)
		{
			_viewModel = viewModel;
			_viewModel.Progress.Changed += OnProgressChanged;

			expectant.With(_buttonClicked = new Expectant());
			_button = GetComponent<UIDocument>().rootVisualElement.Q<Button>(_bindingPath);
			if (_button == null)
			{
				return;
			}

			_button.SetEnabled(false);
			_button.clicked += OnClicked;
		}

		public override void TearDown()
		{
			_buttonClicked?.Dispose();

			if (_button != null)
			{
				_button.clicked -= OnClicked;
			}

			if (_viewModel != null)
			{
				_viewModel.Progress.Changed -= OnProgressChanged;
			}
		}

		private void OnClicked()
		{
			_button.clicked -= OnClicked;
			_button.SetEnabled(false);
			_buttonClicked.SetReady();
		}

		private void OnProgressChanged(float progress)
		{
			var completed = Mathf.Approximately(progress, 1) || progress >= 1;
			if (_button.enabledSelf == false && completed)
			{
				_button.SetEnabled(true);
			}
		}
	}
}