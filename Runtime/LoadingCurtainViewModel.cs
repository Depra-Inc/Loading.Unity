// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System;
using Depra.Loading.Operations;
using Depra.Observables;

namespace Depra.Loading
{
	public sealed class LoadingCurtainViewModel : IDisposable
	{
		public readonly IReactiveProperty<float> Progress = new ReactiveProperty<float>();
		public readonly IReactiveProperty<OperationDescription> Description = new ReactiveProperty<OperationDescription>();

		public void Dispose()
		{
			Progress.Dispose();
			Description.Dispose();
		}

		internal bool NeedToClose { get; private set; }

		public void Close() => NeedToClose = true;
	}
}