using System;
using System.Linq.Expressions;
namespace XLibrary
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
	public interface IGetValue<out T>
	{
		T Value { get; }
	}
	public interface ISetValue<in T>
	{
		T Value { set; }
	}
	public interface IRefValue<T> : IGetValue<T>, ISetValue<T>
	{
		new T Value { get; set; }
	}
	/// <summary>
	/// 引用的抽象类
	/// </summary>
	public abstract class RefValue<T> : IGetValue<T>, ISetValue<T>, IRefValue<T> {
		/// <summary>
		/// 设置值的函数
		/// </summary>
		public abstract void SetFunc(T value);
		/// <summary>
		/// 获取值的函数
		/// </summary>
		public abstract T GetFunc();
		/// <summary>
		/// 值
		/// </summary>
		public T Value
		{
			get => GetFunc();
			set => SetFunc(value);
		}
	}
	public abstract class GetValue<T> : IGetValue<T>
	{
		/// <summary>
		/// 获取值的函数
		/// </summary>
		public abstract T GetFunc();
		/// <summary>
		/// 值
		/// </summary>
		public T Value
		{
			get => GetFunc();
		}
		public static implicit operator Func<T>(GetValue<T> get) => get.GetFunc;
		public static implicit operator GetValue<T>(Func<T> func)=>(GetByDelegate<T>)func;
		//无法尝试设置隐式类型转化
		//public static implicit operator GetValue<T>(LambdaExpression func)=> (GetByDelegate<T>)(func);
		//public static void T2(Func<int> f2) { 
			
		//}
		//public static void T1() {
		//	GetValue<int> t = (() => 1);
		//	T2(() => 1);
		//}
	}
	public abstract class SetValue<T> :ISetValue<T>
	{
		/// <summary>
		/// 设置值的函数
		/// </summary>
		public abstract void SetFunc(T value);
		/// <summary>
		/// 值
		/// </summary>
		public T Value
		{
			set => SetFunc(value);
		}
		public static implicit operator Action<T>(SetValue<T> set) => set.SetFunc;
		public static implicit operator SetValue<T>(Action<T> action)=>(SetByDelegate<T>)action;
		public static implicit operator SetValue<T>(LambdaExpression func) => (SetByDelegate<T>)((Action<T>)func.Compile());
	}
	/// <summary>
	/// 通过委派函数实现引用
	/// </summary>
	/// <typeparam name="T">引用的对象的类型</typeparam>
	public class RefByDelegate<T> : RefValue<T>
	{
		/// <summary>
		/// 设置值的函数
		/// </summary>
		public readonly Action<T> SetF;
		/// <summary>
		/// 获取值的函数
		/// </summary>
		public readonly Func<T> GetF;
		/// <summary>
		/// 通过函数创建引用
		/// </summary>
		public RefByDelegate(Func<T> GetF, Action<T> SetF)
		{
			this.GetF = GetF;
			this.SetF = SetF;
		}
		public override T GetFunc() => GetF();
		public override void SetFunc(T value) => SetF(value);
		public static explicit operator GetByDelegate<T>(RefByDelegate<T> I) => new GetByDelegate<T>(I.GetF);
		public static explicit operator SetByDelegate<T>(RefByDelegate<T> I) => new SetByDelegate<T>(I.SetF);
	}
	public class GetByDelegate<T> : GetValue<T>
	{
		/// <summary>
		/// 获取值的函数
		/// </summary>
		public readonly Func<T> GetF;
		/// <summary>
		/// 通过函数创建引用
		/// </summary>
		public GetByDelegate(Func<T> GetF)
		{
			this.GetF = GetF;
		}
		public override T GetFunc() => GetF();
		public static implicit operator GetByDelegate<T>(Func<T> func) => new GetByDelegate<T>(func);
		public static implicit operator GetByDelegate<T>(System.Linq.Expressions.LambdaExpression lambda)
		{
			return new GetByDelegate<T>((Func<T>)lambda.Compile());
		}
	}
	public class SetByDelegate<T> : SetValue<T>
	{
		/// <summary>
		/// 设置值的函数
		/// </summary>
		public readonly Action<T> SetF;
		/// <summary>
		/// 通过函数创建引用
		/// </summary>
		public SetByDelegate(Action<T> SetF)
		{
			this.SetF = SetF;
		}
		public override void SetFunc(T value) => SetF(value);
		public static implicit operator SetByDelegate<T>(Action<T> func) => new SetByDelegate<T>(func);
		public static implicit operator SetByDelegate<T>(System.Linq.Expressions.LambdaExpression lambda)
		{
			return new SetByDelegate<T>((Action<T>)lambda.Compile());
		}
	}
	/// <summary>
	/// 使用托管类型表示值
	/// </summary>
	/// <typeparam name="T">类型</typeparam>
	public class ClassValue<T> : IGetValue<T>, ISetValue<T>, IRefValue<T>
	{
		private T value;
		/// <summary>
		/// 值
		/// </summary>
		public T Value
		{
			get => value; set => this.value = value;
		}
		public ClassValue() { }
		public ClassValue(T value) { this.value = value; }
	}
	public static partial class Utils {
		/// <summary><![CDATA[
		/// 从IGetValue<T>到Func<T>]]>
		/// </summary>
		public static Func<T> ToGetDelegate<T>(this IGetValue<T> get) => () => get.Value;
		/// <summary><![CDATA[
		/// 从ISetValue<T>到Action<T>]]>
		/// </summary>
		public static Action<T> ToSetDelegate<T>(this ISetValue<T> set) => (v) => set.Value = v;
		/// <summary><![CDATA[
		/// 从Func<T>到IGetValue<T>]]>
		/// </summary>
		public static GetByDelegate<T> ToGet<T>(this Func<T> func) => (GetByDelegate<T>)func;
		/// <summary><![CDATA[
		/// 从Action<T>到ISetValue<T>]]>
		/// </summary>
		public static SetByDelegate<T> ToSet<T>(this Action<T> action) => (SetByDelegate<T>)action;
		/// <summary>
		/// 将Get和Set结合成Ref
		/// </summary>
		public static RefByDelegate<T> Combine<T>(this IGetValue<T> get, ISetValue<T> set) => new RefByDelegate<T>(get.ToGetDelegate(), set.ToSetDelegate());
		/// <summary>
		/// 将Get和Set结合成Ref
		/// </summary>
		public static RefByDelegate<T> Combine<T>(this ISetValue<T> set, IGetValue<T> get) => new RefByDelegate<T>(get.ToGetDelegate(), set.ToSetDelegate());
	}
}
