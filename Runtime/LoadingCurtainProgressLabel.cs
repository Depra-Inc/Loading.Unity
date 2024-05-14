﻿// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.Loading
{
	[RequireComponent(typeof(UIDocument))]
	public sealed class LoadingCurtainProgressLabel : LoadingCurtainView
	{
		[SerializeField] private string _format = "{0}%";
		[SerializeField] private string _bindingPath = "ProgressText";

		private Label _label;
		private LoadingCurtainViewModel _viewModel;

		private void OnDestroy()
		{
			if (_viewModel != null)
			{
				_viewModel.Progress.Changed -= OnProgressChanged;
			}
		}

		public override void Initialize(LoadingCurtainViewModel viewModel)
		{
			_viewModel = viewModel;
			_viewModel.Progress.Changed += OnProgressChanged;
			_label = GetComponent<UIDocument>().rootVisualElement.Q<Label>(_bindingPath);
		}

		private void OnProgressChanged(float value) =>
			_label.text = string.Format(_format, Mathf.RoundToInt(value * 100));
	}
}