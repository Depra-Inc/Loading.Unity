// SPDX-License-Identifier: Apache-2.0
// © 2023-2024 Nikolay Melnikov <n.melnikov@depra.org>

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Depra.Loading
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(UIDocument))]
	public sealed class LoadingCurtainProgressBar : LoadingCurtainView
	{
		[Min(0)] [SerializeField] private float _smoothTime = 0.1f;
		[SerializeField] private string _bindingPath = "ProgressBar";

		private float _velocity;
		private Coroutine _coroutine;
		private ProgressBar _progressBar;
		private LoadingCurtainViewModel _viewModel;

		private void OnDestroy()
		{
			if (_coroutine == null)
			{
				return;
			}

			StopCoroutine(_coroutine);
			_coroutine = null;
		}

		public override void Initialize(LoadingCurtainViewModel viewModel)
		{
			_viewModel = viewModel;
			_progressBar = GetComponent<UIDocument>().rootVisualElement.Q<ProgressBar>(_bindingPath);

			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
			}

			_coroutine = StartCoroutine(UpdateProgressBar());
		}

		private IEnumerator UpdateProgressBar()
		{
			var current = _progressBar.value;
			var target = _viewModel.Progress.Value;
			_progressBar.value = Mathf.SmoothDamp(current, target, ref _velocity, _smoothTime);

			if (Mathf.Abs(_progressBar.value - target) < 0.01f)
			{
				_progressBar.value = target;
				StopCoroutine(_coroutine);
				_coroutine = null;
			}

			yield return null;
		}
	}
}