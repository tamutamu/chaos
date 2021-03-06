using System;
using System.Collections.Generic;
using System.Text;

namespace Yanesdk.Ytl
{
	/// <summary>
	/// シンプルなcache system
	/// </summary>
	/// <remarks>
	/// 数量固定でcacheする。
	/// このクラスは解放するときに、S.Disposeを自動的に呼び出す。
	/// </remarks>
	/// <typeparam name="T">key</typeparam>
	/// <typeparam name="S">cacheしているobject。IDisposableを持っているなら
	/// このcacheから追い出されるときに自動的にDisposeが呼び出される。
	/// </typeparam>
	public class SimpleCacheSystem<T,S> : IDisposable
		where S : class //  , IDisposable
	{

		/// <summary>
		/// size == cacheのサイズ
		/// </summary>
		/// <param name="size">size == 0は許容しない</param>
		public SimpleCacheSystem(int size)
		{
			cache = new Pair<T , S>[size];
		}

		/// <summary>
		/// size == 32 のcache
		/// </summary>
		public SimpleCacheSystem()
		{
			cache = new Pair<T , S>[32];
		}

		/// <summary>
		/// keyに対応するオブジェクトを取得
		/// </summary>
		/// <param name="key">非nullであること。</param>
		/// <returns>cache内に存在しなければnullを返す。</returns>
		public S GetValue(T key)
		{
			for ( int i = 0 ; i < index ; ++i )
			{
				T t = cache[i].First;
				if ( t != null && t.Equals(key) )
				{
					// 見つかったので、これをqueueの一番右に移動させ、secondを返す
					Pair<T,S> f = cache[i];
					cache[i] = cache[index - 1];
					cache[index - 1] = f;

					return f.Second;
				}
			}
			return null; // not found
		}
		
		/// <summary>
		///	cacheオブジェクトに追加
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Insert(T key , S value)
		{
			{
				S s = GetValue(key);
				if ( s != null )
					// あってはならないのだが…
					return;
			}

			if ( index == Capacity )
			{
				// 一番古いのを削除
				S s = cache[0].Second;
				if ( s != null )
				{
					IDisposable ds = s as IDisposable;
					if ( ds != null )
						ds.Dispose();
				}

				--index;
				for ( int i = 0 ; i < index ; ++i )
					cache[i] = cache[i + 1];
			}

			cache[index].First = key;
			cache[index].Second = value;
			
			++index;
		}

		/// <summary>
		/// 保持しているcacheをすべて解放
		/// </summary>
		public void Dispose()
		{
			for ( int i = 0 ; i < index ; ++i )
			{
				S s = cache[i].Second;
				if ( s != null )
				{
					IDisposable ds = s as IDisposable;
					if ( ds != null )
						ds.Dispose();
				}

				cache[i].First = default(T);
				cache[i].Second = null;
			}
			index = 0;
		}

		/// <summary>
		/// cacheのサイズ
		/// </summary>
		public int Capacity
		{
			get { return cache.Length; }
		}

		/// <summary>
		/// cacheしているオブジェクトのサイズ
		/// </summary>
		public int Count
		{
			get { return index; }
		}

		/// <summary>
		/// 次にデータを書き込むべきmarker
		/// </summary>
		private int index;

		/// <summary>
		/// cacheしているオブジェクト
		/// </summary>
		private Pair<T , S>[] cache;
	}

	/// <summary>
	/// ただのpair struct
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="S"></typeparam>
	public struct Pair<T , S>
	{
		public T First;
		public S Second;

		public Pair(T first , S second)
		{
			this.First = first;
			this.Second = second;
		}
	}

	/*  --- test code

			SimpleCacheSystem<string , string> c = new SimpleCacheSystem<string , string>(2);
			c.Insert("abc" , "def");
			c.Insert("bbc" , "aaa");
			c.Insert("dbc" , "aaa");
			string cc = c.GetValue("bbc");
	*/
}
