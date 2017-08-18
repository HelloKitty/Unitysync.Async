using System;
using System.Collections;
using System.Collections.Generic;

namespace Unitysync.Async
{
	public static class UnityAsyncHelperExtensions
	{
		/// <summary>
		/// Dispatches the provided <see cref="continuations"/> using the provided
		/// <see cref="value"/> as the <see cref="Action{T}"/> parameter.
		/// </summary>
		/// <typeparam name="T">The Type of the value.</typeparam>
		/// <param name="value">The value.</param>
		/// <param name="continuations">The continuations to dispatch.</param>
		public static void DispatchContinuations<T>(this IEnumerable<Action<T>> continuations, T value)
		{
			if (continuations == null) throw new ArgumentNullException(nameof(continuations));
			if (value == null) throw new ArgumentNullException(nameof(value));

			foreach (Action<T> a in continuations)
			{
				if (a == null)
					throw new InvalidOperationException($"Provided {nameof(Action<T>)} in {nameof(continuations)} was null.");

				a(value);
			}
		}

		/// <summary>
		/// Dispatches the provided <see cref="continuations"/>.
		/// </summary>
		/// <param name="continuations">The continuations to dispatch.</param>
		public static void DispatchContinuations(this IEnumerable<Action> continuations)
		{
			if (continuations == null) throw new ArgumentNullException(nameof(continuations));

			foreach (Action a in continuations)
			{
				if (a == null)
					throw new InvalidOperationException($"Provided {nameof(Action)} in {nameof(continuations)} was null.");

				a();
			}
		}
	}
}