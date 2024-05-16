// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Expectation;
using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.Loading
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(UIDocument))]
	public sealed class LoadingCurtainBackground : LoadingCurtainView
	{
		[SerializeField] private Sprite _image;
		[SerializeField] private bool _colorize;
		[SerializeField] private Color _color;

		public override void Initialize(LoadingCurtainViewModel viewModel, IGroupExpectant expectant)
		{
			var style = GetComponent<UIDocument>().rootVisualElement.style;
			style.backgroundImage = new StyleBackground(_image);

			if (_colorize)
			{
				style.backgroundColor = _color;
			}
		}
	}
}