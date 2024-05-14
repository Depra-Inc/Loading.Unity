// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;

namespace Depra.Loading
{
	[DisallowMultipleComponent]
	public sealed class LoadingCurtainViewRoot : LoadingCurtainView
	{
		[SerializeField] private LoadingCurtainView[] _views;

		public override void Initialize(LoadingCurtainViewModel viewModel)
		{
			foreach (var view in _views)
			{
				view.Initialize(viewModel);
			}
		}
	}
}