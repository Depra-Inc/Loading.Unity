// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.Loading
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(UIDocument))]
	public sealed class LoadingCurtainBackground : LoadingCurtainView
	{
		[CanBeNull] [SerializeField] private Sprite _background;

		public override void Initialize(LoadingCurtainViewModel viewModel)
		{
			if (_background == false)
			{
				return;
			}

			var document = GetComponent<UIDocument>();
			var rootElement = document.rootVisualElement;
			rootElement.style.backgroundImage = new StyleBackground(_background);
		}
	}
}