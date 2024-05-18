// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Expectation;
using UnityEngine;

namespace Depra.Loading
{
	[DisallowMultipleComponent]
	public sealed class LoadingCurtainViewRoot : MonoBehaviour
	{
		[SerializeField] private LoadingCurtainView[] _views;

		public void Initialize(LoadingCurtainViewModel viewModel, IGroupExpectant expectant)
		{
			foreach (var view in _views)
			{
				view.Initialize(viewModel, expectant);
			}
		}

		public void TearDown()
		{
			foreach (var view in _views)
			{
				view.TearDown();
			}
		}
#if UNITY_EDITOR
		[ContextMenu(nameof(Refill))]
		private void Refill()
		{
			_views = GetComponentsInChildren<LoadingCurtainView>();
			UnityEditor.EditorUtility.SetDirty(this);
		}
#endif
	}
}