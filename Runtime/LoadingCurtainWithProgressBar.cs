// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Loading.Operations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.Loading
{
	public sealed class LoadingCurtainWithProgressBar : LoadingCurtainView
	{
		[SerializeField] private UIDocument _document;
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
			_viewModel.Description.Changed += OnDescriptionChanged;
		}

		public override void Initialize(LoadingCurtainViewModel viewModel)
		{
			_viewModel = viewModel;
			var rootElement = _document.rootVisualElement;
			_descriptionLabel = rootElement.Q<Label>(_descriptionName);
			_progressBar = rootElement.Q<ProgressBar>(_progressBarName);

			_viewModel.Progress.Changed += OnProgressChanged;
			_viewModel.Description.Changed += OnDescriptionChanged;
		}

		private void OnDescriptionChanged(OperationDescription description) =>
			_descriptionLabel.text = description.Text;

		private void OnProgressChanged(float progress) => _progressBar.value = progress;
	}
}