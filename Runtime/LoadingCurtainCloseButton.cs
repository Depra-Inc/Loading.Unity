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

		private void OnDestroy()
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

		public override void Initialize(LoadingCurtainViewModel viewModel, IGroupExpectant expectant)
		{
			_viewModel = viewModel;
			_viewModel.Progress.Changed += OnProgressChanged;
			expectant.With(_buttonClicked = new Expectant());

			var document = GetComponent<UIDocument>();
			var rootElement = document.rootVisualElement;
			_button = rootElement.Q<Button>(_bindingPath);
			if (_button == null)
			{
				return;
			}

			_button.SetEnabled(false);
			_button.clicked += OnClicked;
		}

		private void OnClicked()
		{
			_button.SetEnabled(false);
			_buttonClicked.SetReady();
		}

		private void OnProgressChanged(float progress)
		{
			if (Mathf.Approximately(progress, 1))
			{
				_button.SetEnabled(true);
			}
		}
	}
}