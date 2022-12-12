using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XLibrary
{
	internal static class UnloadDoHolder
	{
		internal static LinkedList<object> staticRefItems;
		public static LinkedList<object> StaticRefItems
		{
			get
			{
				staticRefItems ??= new LinkedList<object>();
				return staticRefItems;
			}
		}
		internal static LinkedList<Action> unloadDoes;
		public static LinkedList<Action> UnloadDoes
		{
			get
			{
				unloadDoes ??= new LinkedList<Action>();
				return unloadDoes;
			}
		}
		internal static bool Loaded = false;
		public static void Load()
		{
			Loaded = true;
		}
		public static void Unload()
		{
			if (Loaded)
			{
				Loaded = false;
				staticRefItems = null;
				if(unloadDoes!=null)
				foreach (var i in UnloadDoes)
				{
					i?.Invoke();
				}
				unloadDoes = null;
			}
		}
	}
	public abstract class StaticRef<T> : IRefValue<T>
		where T:class
	{
		abstract protected T GetNew();
		private WeakReference<T> valueWR;
		private WeakReference<LinkedListNode<object>> valueNodeWR;
		private T SetNew(T value)
		{
			if (valueNodeWR == null)
				valueNodeWR = new(UnloadDoHolder.staticRefItems.AddLast(value));
			else if (valueNodeWR.TryGetTarget(out LinkedListNode<object> Node))
				Node.Value = value;
			else 
				valueNodeWR.SetTarget(UnloadDoHolder.staticRefItems.AddLast(value));

			if (valueWR == null)
				valueWR = new(value);
			else
				valueWR.SetTarget(value);

			return value;
		}
		public T Value {
			get {
				if (valueWR == null || !valueWR.TryGetTarget(out T value))
					value = SetNew(GetNew());
				return value;
			}
			set {
				SetNew(value);
			}
		}
	}
	/// <summary><![CDATA[
	/// 通过一个弱引用和一个列表引用完成可以在XLibrary.Unload自动释放的引用
	/// 通过CtorF创建对象
	/// 在内存中剩下一个WeakReference<T>和委派构造函数(重复使用)]]>
	/// </summary>
	public class StaticRefWithF<T> : StaticRef<T>
		where T : class
	{
		private Func<T> ctorf;

		public StaticRefWithF(Func<T> ctorf)
		{
			this.ctorf = ctorf;
		}

		protected override T GetNew() => ctorf();
	}
	/// <summary><![CDATA[
	/// 通过一个弱引用和一个列表引用完成可以在XLibrary.Unload自动释放的引用
	/// 通过new T()创建对象
	/// 在内存中剩下一个WeakReference<T>(重复使用)]]>
	/// </summary>
	public class StaticRefWithNew<T> : StaticRef<T>
		where T : class,new()
	{
		public StaticRefWithNew()
		{ }

		protected override T GetNew() => new();
	}
}
