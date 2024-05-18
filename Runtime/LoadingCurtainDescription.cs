// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Expectation;
using Depra.Loading.Operations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.Loading
{
	[RequireComponent(typeof(UIDocument))]
	public sealed class LoadingCurtainDescription : LoadingCurtainView
	{
		[SerializeField] private string _bindingPath = "Description";

		private Label _label;
		private UIDocument _document;
		private LoadingCurtainViewModel _viewModel;

		public override void Initialize(LoadingCurtainViewModel viewModel, IGroupExpectant expectant)
		{
			_viewModel = viewModel;
			_viewModel.Description.Changed += OnDescriptionChanged;
			_label = GetComponent<UIDocument>().rootVisualElement.Q<Label>(_bindingPath);
		}

		public override void TearDown()
		{
			if (_viewModel != null)
			{
				_viewModel.Description.Changed -= OnDescriptionChanged;
			}
		}

		private void OnDescriptionChanged(OperationDescription description) => _label.text = description.Text;
	}
}