// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.Loading
{
	[RequireComponent(typeof(UIDocument))]
	internal sealed class LoadingContinueButton : LoadingCurtainClose
	{
		[SerializeField] private string _bindingPath = "ClickToContinue";

		private Button _button;
		private LoadingCurtainViewModel _viewModel;

		private void OnDestroy()
		{
			if (_button != null)
			{
				_button.clicked -= OnClicked;
			}

			if (_viewModel != null)
			{
				_viewModel.Progress.Changed -= OnProgressChanged;
			}
		}

		public override void Initialize(LoadingCurtainViewModel viewModel)
		{
			_viewModel = viewModel;
			_viewModel.Progress.Changed += OnProgressChanged;

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
			_viewModel.Close();
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