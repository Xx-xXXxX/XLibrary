using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria.ID;

namespace XLibrary.Behaviors
{
	/// <summary>
	/// 通过组合模式操作behavior
	/// Add应在Initialize前完成，否则应该报错
	/// 否则可能出现联机同步错误
	/// </summary>
	public interface IBehaviorComponent<in RealBehaviorType> : IBehavior
	{
		/// <summary>
		/// 加入成员，应在Initialize前完成
		/// </summary>
		void Add(RealBehaviorType behavior, bool Using);
	}
	/// <summary>
	/// 通过组合模式操作behavior
	/// </summary>
	public abstract class BehaviorComponent<RealBehaviorType> : Behavior, IEnumerable<RealBehaviorType>, IBehaviorComponent<RealBehaviorType>//, IBehaviorComponent<RealBehaviorType>
		where RealBehaviorType : IBehavior
	{
		/// <summary>
		/// 是否同步自己
		/// </summary>
		public abstract bool NetUpdateThis { get; }
		/// <summary>
		/// 如果存在需要同步的组件则同步
		/// </summary>
		public sealed override bool NetUpdate
		{
			get
			{
				if (NetUpdateThis) return true;
				foreach (var i in GetUsings()) if (i.NetUpdate) return true;
				return false;
			}
		}
		/// <summary>
		/// 装有Behavior的容器
		/// </summary>
		//protected List<RealBehaviorType> BehaviorsList = new List<RealBehaviorType>();
		protected abstract IList<RealBehaviorType> BehaviorsList { get; }
		/// <summary>
		/// 下一个可用ID
		/// </summary>
		public int NextID => BehaviorsList.Count;
		/// <summary>
		/// 获取Behavior
		/// </summary>
		public RealBehaviorType this[int id] => BehaviorsList[id];
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
		public virtual void Add(RealBehaviorType behavior, bool Using = true)
		{
			//if (Initialized) throw new InvalidOperationException();
			int id = NextID;
			BehaviorsList.Add(behavior);
			//BehaviorsUsing.Add(Using);
			OnAdd(behavior, id);
		}
		/// <summary>
		/// 在加入成员时（已经加入BehaviorsList）
		/// </summary>
		public virtual void OnAdd(RealBehaviorType behavior, int id)
		{

		}
		//public virtual bool BehaviorUsing(int id) => true;
		//public bool BehaviorUse(int id)
		//{
		//	BehaviorsUsing[id] = true;
		//	if (Active) return BehaviorsList[id].TryActivate();
		//	return false;
		//}
		//public bool BehaviorUnUse(int id)
		//{
		//	BehaviorsUsing[id] = false;
		//	if (Active) return BehaviorsList[id].TryPause();
		//	return false;
		//}
		public bool ActivateBehavior(int id) {
			IBehavior behavior = BehaviorsList[id];
			if (behavior == null) return false;
			if (!behavior.CanActivate()) return false;
			behavior.OnActivate();
			Usings.Add(id);
			return true;
		}
		public bool PauseBehavior(int id)
		{
			IBehavior behavior = BehaviorsList[id];
			if (behavior == null) return false;
			if (!behavior.CanPause()) return false;
			behavior.OnPause();
			Usings.Remove(id);
			return true;
		}

		public override bool CanPause()
		{
			bool can = base.CanPause();
			if (!can) return false;
			foreach (var i in GetUsings())
			{
				can &= i.CanPause();
				if (!can) return false;
			}
			return true;
		}

		public override void OnPause()
		{
			foreach (var i in GetUsings())
			{
				i.OnPause();
			}
			base.OnPause();
		}

		public override bool CanActivate()
		{
			bool can = base.CanActivate();
			if (!can) return false;
			foreach (var i in GetUsings())
			{
				can &= i.CanActivate();
				if (!can) return false;
			}
			return true;
		}

		public override void OnActivate()
		{
			foreach (var i in GetUsings())
			{
				i.OnActivate();
			}
			base.OnActivate();
		}

		public override void Initialize()
		{
			foreach (var i in BehaviorsList)
			{
				i.Initialize();
			}
		}

		public override void Dispose()
		{
			foreach (var i in BehaviorsList)
			{
				i.Dispose();
			}
		}

		public sealed override void NetUpdateSend(BinaryWriter writer)
		{
			bool All = Utils.ExistNewPlayer();
			if (NetUpdateThis)
			{
				writer.Write(true);
				OnNetUpdateSend(writer);
			}
			else writer.Write(false);
			writer.Write(BehaviorsList.Count);
			for (int i = 0; i < BehaviorsList.Count; ++i)
			{
				RealBehaviorType behavior = BehaviorsList[i];
				Terraria.BitsByte bits = 0;
				bits[0] = behavior.Active;
				bool NetUpdate = bits[1] = All || behavior.NetUpdate;
				writer.Write(bits);
				if (NetUpdate)
					behavior.NetUpdateSend(writer);
			}
		}
		public virtual void OnNetUpdateSend(BinaryWriter writer) { }
		public sealed override void NetUpdateReceive(BinaryReader reader)
		{
			base.NetUpdateReceive(reader);
			bool netUpdateThis = reader.ReadBoolean();
			if (netUpdateThis) OnNetUpdateReceive(reader);
			int Count = reader.ReadInt32();
			for (int i = 0; i < Count; ++i)
			{
				int id = i;
				Terraria.BitsByte bits = reader.ReadByte();
				bool active = bits[0];
				bool NetUpdate = bits[1];
				IBehavior behavior = BehaviorsList[id];
				if (active)
					ActivateBehavior(id);
				else
					PauseBehavior(id);
				if (NetUpdate)
					behavior.NetUpdateReceive(reader);
			}
		}
		public virtual void OnNetUpdateReceive(BinaryReader reader) { }
		public override object Call(params object[] vs)
		{
			return null;
		}
		protected SetEx<int> Usings = new();
		public IEnumerator<RealBehaviorType> GetEnumerator()
		{
			foreach (var i in BehaviorsList)
			{
				RealBehaviorType b = i;
				if (b.Active) yield return b;
			}
		}
		public virtual IEnumerable<RealBehaviorType> GetUsings()
		{
			foreach (var i in Usings) {
				yield return BehaviorsList[i];
			}
		}
		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<RealBehaviorType>)this).GetEnumerator();
		}
	}

}
