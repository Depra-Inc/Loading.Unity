// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Loading.Operations;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.Loading
{
	[DisallowMultipleComponent]
	public sealed class LoadingCurtainWithProgressBar : LoadingCurtainView
	{
		[SerializeField] private UIDocument _document;
		[CanBeNull] [SerializeField] private Sprite _background;
		[SerializeField] private string _descriptionName = "Description";
		[SerializeField] private string _progressBarName = "ProgressBar";

		private Label _descriptionLabel;
		private ProgressBar _progressBar;
		private LoadingCurtainViewModel _viewModel;

		private void OnDestroy()
		{
			if (_viewModel == null)
			{
				return;
			}

			_viewModel.Progress.Changed -= OnProgressChanged;
			_viewModel.Description.Changed -= OnDescriptionChanged;
		}

		public override void Initialize(LoadingCurtainViewModel viewModel)
		{
			var rootElement = _document.rootVisualElement;
			_descriptionLabel = rootElement.Q<Label>(_descriptionName);
			_progressBar = rootElement.Q<ProgressBar>(_progressBarName);

			if (_background)
			{
				rootElement.style.backgroundImage = new StyleBackground(_background);
			}

			_viewModel = viewModel;
			_viewModel.Progress.Changed += OnProgressChanged;
			_viewModel.Description.Changed += OnDescriptionChanged;
		}

		private void OnDescriptionChanged(OperationDescription description) =>
			_descriptionLabel.text = description.Text;

		private void OnProgressChanged(float progress) => _progressBar.value = progress;
	}
}