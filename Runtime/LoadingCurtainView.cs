// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using UnityEngine;

namespace Depra.Loading
{
	public abstract class LoadingCurtainView : MonoBehaviour
	{
		public abstract void Initialize(LoadingCurtainViewModel viewModel);
	}
}