using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
namespace XLibrary
{
	/// <summary>
	/// 随机数接口
	/// </summary>
	public interface IRandom
	{
		/// <summary>
		/// [0,1)double样本
		/// </summary>
		double Sample();
		/// <summary>
		/// [0,int.MaxValue)整数样本
		/// </summary>
		int IntSample();
	}
	/// <summary>
	/// 通过委派实现随机数接口
	/// </summary>
	public class IRandomByDelegate : IRandom
	{
		readonly Func<double> sample;
		readonly Func<int> intSample;
		/// <summary>
		/// 通过委派获取样本
		/// </summary>
		public double Sample() => sample();
		/// <summary>
		/// 通过委派获取样本
		/// </summary>
		public int IntSample() => intSample();
		/// <summary>
		/// 通过委派实现随机数接口
		/// </summary>
		public IRandomByDelegate(Func<double> sampleFunc) { sample = sampleFunc;intSample = this.IntSampleFromSample; }
		/// <summary>
		/// 通过委派实现随机数接口
		/// </summary>
		public IRandomByDelegate(Func<int> sampleFunc) { intSample = sampleFunc; sample = this.SampleFromIntSample; }
		/// <summary>
		/// 通过委派实现随机数接口
		/// </summary>
		public IRandomByDelegate(Func<double> sampleFunc, Func<int> sampleFunc2) { sample = sampleFunc; intSample = sampleFunc2; }
	}
	/// <summary>
	/// 用一个ulong完成随机数
	/// </summary>
	public class RandomULong1: IRandom
	{
		/// <summary>
		/// 值
		/// </summary>
		public readonly IRefValue<ulong> Value1;
		/// <summary>
		/// 操作value1生成随机数
		/// </summary>
		public RandomULong1(IRefValue<ulong> value1) { Value1 = value1; }
		/// <summary>
		/// IntSample
		/// </summary>
		public int IntSample() {
			Value1.Value = Terraria.Utils.RandomNextSeed(Value1.Value);
			return (int)Value1.Value;
		}
		/// <summary>
		/// Sample
		/// </summary>
		public double Sample() => this.SampleFromIntSample();
		/// <summary>
		/// 加入一个数以增强随机
		/// </summary>
		/// <param name="v"></param>
		public void AddRand(ulong v)
		{
			Value1.Value = Terraria.Utils.RandomNextSeed(Value1.Value ^ Terraria.Utils.RandomNextSeed(v));
		}
	}
	/// <summary>
	/// 用1个int完成随机数
	/// </summary>
	public class RandomInt1 : IRandom
	{
		/// <summary>
		/// 值
		/// </summary>
		public readonly IRefValue<int> Value1;
		/// <summary>
		/// 操作value1生成随机数
		/// </summary>
		public RandomInt1(IRefValue<int> value1) { Value1 = value1; }
		/// <summary>
		/// IntSample
		/// </summary>
		public int IntSample()
		{
			Value1.Value = IRandomUtils.NextIntSeed(Value1.Value);
			return Value1.Value.ToNonnegative();
		}
		/// <summary>
		/// Sample
		/// </summary>
		public double Sample() => this.SampleFromIntSample();
		/// <summary>
		/// 加入一个数以增强随机
		/// </summary>
		public void AddRand(int v) {
			Value1.Value = IRandomUtils.NextIntSeed(Value1.Value^ IRandomUtils.NextIntSeed(v));
		}
	}
	/// <summary>
	/// 用2个int完成随机数
	/// </summary>
	public class RandomInt2 : IRandom
	{
		/// <summary>
		/// 值
		/// </summary>
		public readonly IRefValue<int> Value1;
		/// <summary>
		/// 值
		/// </summary>
		public readonly IRefValue<int> Value2;
		/// <summary>
		/// 操作value1生成随机数
		/// </summary>
		public RandomInt2(IRefValue<int> value1, IRefValue<int> value2) { Value1 = value1;Value2 = value2; }
		/// <summary>
		/// IntSample
		/// </summary>
		public int IntSample()
		{
			Value1.Value = IRandomUtils.NextIntSeed(Value1.Value-Value2.Value);
			Value2.Value = IRandomUtils.NextIntSeed(Value1.Value+Value2.Value);
			return (Value1.Value^Value2.Value).ToNonnegative();
		}
		/// <summary>
		/// Sample
		/// </summary>
		public double Sample() => this.SampleFromIntSample();
		/// <summary>
		/// 加入一个数以增强随机
		/// </summary>
		/// <param name="value"></param>
		public void AddRand(int value) {
			Value1.Value = IRandomUtils.NextIntSeed(Value1.Value - Value2.Value+ value);
			Value2.Value = IRandomUtils.NextIntSeed(Value1.Value + Value2.Value- value);
		}
	}
	/// <summary>
	/// 随机数接口的方法
	/// </summary>
	public static class IRandomUtils
	{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
		/// <summary>
		/// 对该seed生成下一个seed，可以小于0
		/// </summary>
		public static int NextIntSeed(int seed) {
			return (int)((uint)seed * 65537 + 9586585221L);
		}
		/// <summary>
		/// 对该seed生成下一个seed
		/// </summary>
		public static uint NextUIntSeed(uint seed)
		{
			return (uint)(seed * 65537 + 9586585221L);
		}
		public static int ToNonnegative(this int v) {
			if (v < 0) v += int.MaxValue;
			if (v == int.MaxValue) v -= 1;
			return v;
		}
		/// <summary>
		/// 将Terraria.Utilities.UnifiedRandom转为IRandom
		/// </summary>
		public static IRandomByDelegate ToIRandom(this Terraria.Utilities.UnifiedRandom random) {
			return new IRandomByDelegate( random.NextDouble,random.Next);
		}
		public static int IntSampleFromSample(this IRandom random) => (int)(random.Sample() * int.MaxValue);
		public static double SampleFromIntSample(this IRandom random) => random.IntSample() * 4.6566128752457969E-10;
		public static int Next(this IRandom random) {
			return (int)(random.Sample()*int.MaxValue);
		}
		public static int Next(this IRandom random, int maxValue) {
			if (maxValue < 0)
			{
				throw new ArgumentOutOfRangeException("maxValue", "maxValue must be positive.");
			}

			return (int)(random.Sample() * (double)maxValue);
		}
		public static int Next(this IRandom random,int minValue, int maxValue)
		{
			if (minValue > maxValue)
			{
				throw new ArgumentOutOfRangeException("minValue", "minValue must be less than maxValue");
			}

			long num = (long)maxValue - (long)minValue;
			if (num <= int.MaxValue)
			{
				return (int)(random.Sample() * (double)num) + minValue;
			}

			return (int)((long)(random.GetSampleForLargeRange() * (double)num) + minValue);
		}
		public static double GetSampleForLargeRange(this IRandom random)
		{
			int num = random.IntSample();
			if (random.IntSample() % 2 == 0)
			{
				num = -num;
			}
			return ((double)num + 2147483646.0) / 4294967293.0;
		}
		public static double NextDouble(this IRandom random) {
			return random.Sample();
		}
		public static float NextFloat(this IRandom random) {
			return (float)(random.Sample());
		}
		public static float NextFloat(this IRandom random, float maxValue)
		{
			return random.NextFloat() * maxValue;
		}
		public static float NextFloat(this IRandom random, float minValue, float maxValue) {
			if (minValue > maxValue)
			{
				throw new ArgumentOutOfRangeException(nameof(minValue), "minValue must be less than maxValue");
			}
			return random.NextFloat() * (maxValue - minValue) + minValue;
		}
		public static float NextFloatDirection(this IRandom r)
		{
			return (float)r.NextDouble() * 2f - 1f;
		}
		public static float NextAngle(this IRandom random) => random.NextFloat((float)Math.PI*2);
		public static void NextBytes(this IRandom random,byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException(nameof(buffer));
			}

			for (int i = 0; i < buffer.Length; i++)
			{
				buffer[i] = (byte)(random.IntSample() % 256);
			}
		}
		/// <summary>
		/// 获取在Range内的向量
		/// </summary>
		public static Vector2 NextVectorInRect(this IRandom random, Rectangle Range) {
			return new Vector2(random.NextFloat(Range.Left,Range.Right), random.NextFloat(Range.Top, Range.Bottom));
		}
		/// <summary>
		/// 获取XY在[min,max)的向量
		/// </summary>
		public static Vector2 NextVector2Square(this IRandom random, float minValue, float maxValue)
		{
			return new Vector2((maxValue - minValue) * (float)random.NextDouble() + minValue, (maxValue - minValue) * (float)random.NextDouble() + minValue);
		}
		/// <summary>
		/// 获取单位向量
		/// </summary>
		public static Vector2 NextVector2Unit(this IRandom random, float startRotation = 0f, float rotationRange = (float)Math.PI * 2f)
		{
			return (startRotation + rotationRange * random.NextFloat()).ToRotationVector2();
		}
		/// <summary>
		/// 获取在椭圆x^2/a^2+y^2/b^2=1内的向量
		/// </summary>
		public static Vector2 NextVector2Ellipse(this IRandom random, float a, float b)
		{
			return random.NextVector2Unit() * new Vector2(a, b) * random.NextFloat();
		}
		/// <summary>
		/// 获取在椭圆x^2/a^2+y^2/b^2=1上的向量
		/// </summary>
		public static Vector2 NextVector2EllipseEdge(this IRandom random, float a, float b)
		{
			return random.NextVector2Unit() * new Vector2(a, b);
		}
		/// <summary>
		/// 获取在圆内的向量
		/// </summary>
		public static Vector2 NextVector2Circular(this IRandom random, float r)
		{
			return random.NextVector2Unit() * new Vector2(r, r) * random.NextFloat();
		}
		/// <summary>
		/// 获取在圆上的向量
		/// </summary>
		public static Vector2 NextVector2CircularEdge(this IRandom random, float r)
		{
			return random.NextVector2Unit() * new Vector2(r, r);
		}
		/// <summary>
		/// 获取符合正态分布的随机数
		/// </summary>
		public static double NextGaussian(this IRandom random)
		{
			double u = -2 * Math.Log(random.NextDouble());
			double v = 2 * Math.PI * random.NextDouble();
			return (Math.Sqrt(u) * Math.Cos(v));
		}
		/// <summary>
		/// 获取符合正态分布的随机数
		/// </summary>
		public static double NextGaussian(this IRandom random, double mu, double sigma) => random.NextGaussian() * sigma + mu;
		public static T Next<T>(this IRandom random, T[] array)
		{
			return array[random.Next(array.Length)];
		}
		public static T Next<T>(this IRandom random, IList<T> list)
		{
			return list[random.Next(list.Count)];
		}
		public static bool NextBool(this IRandom random)
		{
			return random.NextDouble() < 0.5;
		}
		public static bool NextBool(this IRandom random, float probability)
		{
			return random.NextFloat()< probability;
		}
	}
}
