// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Loading.Operations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.Loading
{
	[RequireComponent(typeof(UIDocument))]
	public sealed class LoadingCurtainDescription : LoadingCurtainView
	{
		[SerializeField] private string _bindingPath = "Description";

		private UIDocument _document;
		private Label _descriptionLabel;
		private LoadingCurtainViewModel _viewModel;

		private void OnDestroy()
		{
			if (_viewModel != null)
			{
				_viewModel.Description.Changed -= OnDescriptionChanged;
			}
		}

		public override void Initialize(LoadingCurtainViewModel viewModel)
		{
			_viewModel = viewModel;
			_viewModel.Description.Changed += OnDescriptionChanged;
			_descriptionLabel = GetComponent<UIDocument>().rootVisualElement.Q<Label>(_bindingPath);
		}

		private void OnDescriptionChanged(OperationDescription description) =>
			_descriptionLabel.text = description.Text;
	}
}