namespace UniRx.InternalUtil
{
	using System;

	static class ExceptionExtensions
	{
		public static void Rethrow(this Exception exception)
		{
#if NET_4_6
			System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(exception).Throw();
#endif
		}
	}
}
