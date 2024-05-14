// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.Loading
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(UIDocument))]
	public sealed class LoadingCurtainBackground : LoadingCurtainView
	{
		[SerializeField] private Sprite _image;
		[SerializeField] private Color _color;

		public override void Initialize(LoadingCurtainViewModel viewModel)
		{
			var style = GetComponent<UIDocument>().rootVisualElement.style;
			style.backgroundImage = new StyleBackground(_image);
			style.unityBackgroundImageTintColor = _color;
		}
	}
}