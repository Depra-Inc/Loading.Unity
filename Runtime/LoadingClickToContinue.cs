using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.Loading
{
	[RequireComponent(typeof(UIDocument))]
	public sealed class LoadingClickToContinue : LoadingCurtainViewFinal
	{
		[SerializeField] private string _name = "ClickToContinue";

		private VisualElement _clickToContinue;
		private LoadingCurtainViewModel _viewModel;

		private void Update()
		{
			if (_clickToContinue is { enabledInHierarchy: true } && Input.GetMouseButtonDown(0))
			{
				_clickToContinue.SetEnabled(false);
				_viewModel.Complete();
			}
		}

		private void OnDestroy() => _viewModel.Progress.Changed -= OnProgressChanged;

		public override void Initialize(LoadingCurtainViewModel viewModel)
		{
			var document = GetComponent<UIDocument>();
			var rootElement = document.rootVisualElement;
			_clickToContinue = rootElement.Q<Label>(_name);
			_clickToContinue.SetEnabled(false);

			_viewModel = viewModel;
			_viewModel.Progress.Changed += OnProgressChanged;
		}

		private void OnProgressChanged(float progress)
		{
			if (Mathf.Approximately(progress, 1))
			{
				_clickToContinue.SetEnabled(true);
			}
		}
	}
}