namespace BuildAutomation
{
	using System;
	using System.Collections.ObjectModel;

	public class FuncKeyedCollection<TKey, TValue> : KeyedCollection<TKey, TValue>
	{
		/// <remarks>
		/// delegate TResult Func&lt;in T, out TResult&gt; (T arg);
		/// </remarks>
		Func<TValue, TKey> extractor_func;

		public FuncKeyedCollection (Func<TValue, TKey> extractor_func)
		{
			this.extractor_func = extractor_func;
		}

		protected override TKey GetKeyForItem (TValue item)
		{
			return extractor_func(item);
		}
	}
}

