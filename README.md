# Unisync.Async

Unisync.Async is an async extension library that allows you to use Task and Task<T> with continuations in Unity3D.

## How to Use

If you have an async method that returns a Task or Task<T> then you can register a continuation with an extension method. You can provide various continuation types such as: Action, Action<T>, Func<T, TResult>, Func<T, Task<TResult>> and of course supports method delegates/lambdas.

The way this works is it starts a coroutine on the current Monobehaviour, using the Monobehaviour as a context for the continuation, and fires off the delegate once the async method has completed and the Task completition state is complete.

```csharp
Service.DoSomethingAsync(someData, someMoreData)
	.UnityAsyncContinueWith(this, LogAndDipatchResult);
```

```csharp
Service.DoSomethingAsync(someData, someMoreData)
	.UnityAsyncContinueWith(this, result => SomeEntity.SetResult(result.Something));
```

You can schedule multiple continuations on a single Task but the order of their execution is currently undefined if you schedule multiple on the same task.

You can even do a continuation into an async method which will of course return a Task<T> which can be continued on again, in a fluent method chaining pattern, and this execution order is defined since they are different tasks.

## Setup

To compile or open TypeSafe.Http.Net project you'll first need a couple of things:

* Visual Studio 2017
* Unity 2017.1 with .NET 4.6 enabled

## Builds

NuGet: [Unisync.Async](https://www.nuget.org/packages/Unisync.Async/)

Myget: [![hellokitty MyGet Build Status](https://www.myget.org/BuildSource/Badge/hellokitty?identifier=f89338ba-bf06-478a-9131-ab52b4855aa5)](https://www.myget.org/)

## Tests

TODO actual tests
