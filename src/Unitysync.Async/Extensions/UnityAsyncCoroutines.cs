using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unitysync.Async
{
	internal static class UnityAsyncCoroutines
	{
		internal static IEnumerator UnityAsyncCoroutine<T>(this Task<T> future, Action<T> continuation)
		{
			if (future == null) throw new ArgumentNullException(nameof(future));
			if (continuation == null) throw new ArgumentNullException(nameof(continuation));

			yield return new WaitForFuture(future);

			ThrowIfIsCanceledOrErrored(future);

			//Result will throw if we encounted exceptions but it will be aggregate exception
			continuation(future.Result);
		}

		/// <summary>
		/// Throws exceptions if the task is canceled or errored.
		/// </summary>
		/// <param name="future">The future to check.</param>
		private static void ThrowIfIsCanceledOrErrored(Task future)
		{
			if(future.IsCanceled)
				throw new TaskCanceledException(future);

			if (future.IsFaulted)
				if(future.Exception != null)
					throw future.Exception;
				else
					throw new InvalidOperationException($"Task: {future} failed to complete execution.");
		}

		//You may wonder why this overload exists. It's for efficiency reasons so we don't need to wrap the a single continuation
		//in a concated enumerable.
		internal static IEnumerator UnityAsyncCoroutine<T>(this Task<T> future, Action<T> continuation, IEnumerable<Action<T>> continuations)
		{
			if (future == null) throw new ArgumentNullException(nameof(future));
			if (continuation == null) throw new ArgumentNullException(nameof(continuation));
			if (continuations == null) throw new ArgumentNullException(nameof(continuations));

			yield return new WaitForFuture(future);

			ThrowIfIsCanceledOrErrored(future);

			//Result will throw if we encounted exceptions but it will be aggregate exception
			//We call the first continuation
			continuation(future.Result);

			continuations.DispatchContinuations(future.Result);
		}

		internal static IEnumerator UnityAsyncCoroutine(this Task future, Action continuation)
		{
			if (future == null) throw new ArgumentNullException(nameof(future));
			if (continuation == null) throw new ArgumentNullException(nameof(continuation));

			yield return new WaitForFuture(future);

			//Result will throw if we encounted exceptions but it will be aggregate exception
			continuation();
		}

		//You may wonder why this overload exists. It's for efficiency reasons so we don't need to wrap the a single continuation
		//in a concated enumerable.
		internal static IEnumerator UnityAsyncCoroutine(this Task future, Action continuation, IEnumerable<Action> continuations)
		{
			if (future == null) throw new ArgumentNullException(nameof(future));
			if (continuation == null) throw new ArgumentNullException(nameof(continuation));
			if (continuations == null) throw new ArgumentNullException(nameof(continuations));

			yield return new WaitForFuture(future);

			ThrowIfIsCanceledOrErrored(future);

			//Result will throw if we encounted exceptions but it will be aggregate exception
			//We call the first continuation
			continuation();

			continuations.DispatchContinuations();
		}

		internal static IEnumerator UnityAsyncCoroutine<T, TResult>(this Task<T> future, Func<T, Task<TResult>> continuation, TaskCompletionSource<TResult> result)
		{
			if (future == null) throw new ArgumentNullException(nameof(future));
			if (continuation == null) throw new ArgumentNullException(nameof(continuation));

			yield return new WaitForFuture(future);

			ThrowIfIsCanceledOrErrored(future);

			Task<TResult> resultValue;
			try
			{
				//Result will throw if we encounted exceptions but it will be aggregate exception
				resultValue = continuation(future.Result);
			}
			catch (Exception e)
			{
				result.SetException(e);
				yield break;
			}

			//Now unlike the non task func we must wait for the new task to finish before we set its completion source
			yield return new WaitForFuture(resultValue);

			ThrowIfIsCanceledOrErrored(resultValue);

			result.SetResult(resultValue.Result);
		}

		internal static IEnumerator UnityAsyncCoroutine<T, TResult>(this Task<T> future, Func<T, TResult> continuation, TaskCompletionSource<TResult> result)
		{
			if (future == null) throw new ArgumentNullException(nameof(future));
			if (continuation == null) throw new ArgumentNullException(nameof(continuation));

			yield return new WaitForFuture(future);

			ThrowIfIsCanceledOrErrored(future);

			TResult resultValue;
			try
			{
				//Result will throw if we encounted exceptions but it will be aggregate exception
				resultValue = continuation(future.Result);
			}
			catch(Exception e)
			{
				result.SetException(e);

				yield break;
			}

			result.SetResult(resultValue);
		}

		internal static IEnumerator UnityAsyncCoroutine<T>(this Task<T> future, Func<T, Task> continuation, TaskCompletionSource<object> result)
		{
			if(future == null) throw new ArgumentNullException(nameof(future));
			if(continuation == null) throw new ArgumentNullException(nameof(continuation));

			yield return new WaitForFuture(future);

			ThrowIfIsCanceledOrErrored(future);

			Task resultValue = null;
			try
			{
				//Result will throw if we encounted exceptions but it will be aggregate exception
				resultValue = continuation(future.Result);
			}
			catch(Exception e)
			{
				result.SetException(e);

				//Set the exception but don't break
				//We want to wait for it to finish AND
				//throw if it failed below.
			}

			//Result value should not be null here
			yield return new WaitForFuture(resultValue);

			ThrowIfIsCanceledOrErrored(resultValue);

			result.SetResult(null);
		}

		internal static IEnumerator UnityAsyncCoroutine<T, TResult>(this Task<T> future, Func<Task<TResult>> continuation, TaskCompletionSource<TResult> result)
		{
			if (future == null) throw new ArgumentNullException(nameof(future));
			if (continuation == null) throw new ArgumentNullException(nameof(continuation));

			yield return new WaitForFuture(future);

			ThrowIfIsCanceledOrErrored(future);

			Task<TResult> resultValue;
			try
			{
				//Result will throw if we encounted exceptions but it will be aggregate exception
				resultValue = continuation();
			}
			catch (Exception e)
			{
				result.SetException(e);
				yield break;
			}

			//Now unlike the non task func we must wait for the new task to finish before we set its completion source
			yield return new WaitForFuture(resultValue);

			ThrowIfIsCanceledOrErrored(resultValue);

			result.SetResult(resultValue.Result);
		}

		internal static IEnumerator UnityAsyncCoroutine<T, TResult>(this Task<T> future, Func<TResult> continuation, TaskCompletionSource<TResult> result)
		{
			if (future == null) throw new ArgumentNullException(nameof(future));
			if (continuation == null) throw new ArgumentNullException(nameof(continuation));

			yield return new WaitForFuture(future);

			ThrowIfIsCanceledOrErrored(future);

			TResult resultValue;
			try
			{
				//Result will throw if we encounted exceptions but it will be aggregate exception
				resultValue = continuation();
			}
			catch (Exception e)
			{
				result.SetException(e);
				yield break;
			}

			result.SetResult(resultValue);
		}

		internal static IEnumerator UnityAsyncCoroutine<TResult>(this Task future, Func<Task<TResult>> continuation, TaskCompletionSource<TResult> result)
		{
			if (future == null) throw new ArgumentNullException(nameof(future));
			if (continuation == null) throw new ArgumentNullException(nameof(continuation));

			yield return new WaitForFuture(future);

			ThrowIfIsCanceledOrErrored(future);

			Task<TResult> resultValue;
			try
			{
				//Result will throw if we encounted exceptions but it will be aggregate exception
				resultValue = continuation();
			}
			catch (Exception e)
			{
				result.SetException(e);
				yield break;
			}

			//Now unlike the non task func we must wait for the new task to finish before we set its completion source
			yield return new WaitForFuture(resultValue);

			ThrowIfIsCanceledOrErrored(resultValue);

			result.SetResult(resultValue.Result);
		}

		internal static IEnumerator UnityAsyncCoroutine<TResult>(this Task future, Func<TResult> continuation, TaskCompletionSource<TResult> result)
		{
			if (future == null) throw new ArgumentNullException(nameof(future));
			if (continuation == null) throw new ArgumentNullException(nameof(continuation));

			yield return new WaitForFuture(future);

			ThrowIfIsCanceledOrErrored(future);

			TResult resultValue;
			try
			{
				//Result will throw if we encounted exceptions but it will be aggregate exception
				resultValue = continuation();
			}
			catch (Exception e)
			{
				result.SetException(e);
				yield break;
			}

			result.SetResult(resultValue);
		}
	}
}