using UnityEngine;

namespace Depra.Loading
{
	public sealed class LoadingCurtainAutoFinal : LoadingCurtainViewFinal
	{
		private LoadingCurtainViewModel _viewModel;

		public override void Initialize(LoadingCurtainViewModel viewModel)
		{
			_viewModel = viewModel;
			_viewModel.Progress.Changed += OnProgressChanged;
		}

		private void OnProgressChanged(float progress)
		{
			if (Mathf.Approximately(progress, 1))
			{
				_viewModel.Complete();
			}
		}
	}
}