﻿// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using Depra.Expectation;
using UnityEngine;

namespace Depra.Loading
{
	public abstract class LoadingCurtainView : MonoBehaviour
	{
		protected private void OnDestroy() { }

		public abstract void Initialize(LoadingCurtainViewModel viewModel, IGroupExpectant expectant);

		public virtual void TearDown() { }
	}
}