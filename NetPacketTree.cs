
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ModLoader;

using static XLibrary.BinaryIOFunc;
namespace XLibrary
{
	/// <summary>
	/// NetPacketTree的借口
	/// </summary>
	public interface INetPacketTree
	{
		/// <summary>
		/// 获取到该节点的Packet
		/// </summary>
		/// <returns></returns>
		ModPacket GetPacket();
		/// <summary>
		/// 到该节点时执行的函数
		/// </summary>
		/// <param name="reader">Packet</param>
		/// <param name="whoAmI">发出Packet的玩家</param>
		void Handle(BinaryReader reader,int whoAmI);
	}
	/// <summary>
	/// 作为Packet操作的后续结点接口
	/// </summary>
	public interface INetPacketTreeChild: INetPacketTree {
		/// <summary>
		/// 该节点的父亲
		/// </summary>
		NetPacketTreeFather Father { get; set; }
		/// <summary>
		/// 自己在父亲中的Key
		/// </summary>
		short ChildKey { get; set; }
		
	}
	
	/// <summary>
	/// 作为Packet操作的前驱结点类
	/// 用AddChild加入子节点，会自动设置Binary来传给对应的Child
	/// </summary>
	public abstract class NetPacketTreeFather : INetPacketTree {
		/// <summary>
		/// 存储子节点的表
		/// </summary>
		public SortedList<short, WeakReference< INetPacketTreeChild> > NetPacketTreeChilds = new SortedList<short, WeakReference<INetPacketTreeChild> >();
		/// <summary>
		/// 获取用于子节点的Packet
		/// </summary>
		/// <param name="childKey">子节点的Key</param>
		/// <returns></returns>
		public ModPacket GetPacketChild(short childKey) {
			ModPacket mp = GetPacket();
			WriteBinaryShort(mp, childKey);
			return mp;
		}
		/// <summary>
		/// 获取到该节点的Packet
		/// </summary>
		public abstract ModPacket GetPacket();
		/// <summary>
		/// 传输Packet到对应Key的Child
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="whoAmI"></param>
		public void Handle(BinaryReader reader, int whoAmI) {
			INetPacketTreeChild  child;
			short key= ReadBinaryShort(reader);
			NetPacketTreeChilds[key].TryGetTarget(out child);
			if (child != null) child.Handle(reader, whoAmI);
			else NetPacketTreeChilds.Remove(key);
		}
		/// <summary>
		/// 新增Child
		/// </summary>
		/// <param name="Child">子节点</param>
		/// <param name="childKey">子节点的Key</param>
		public void AddChild(INetPacketTreeChild Child, short childKey) {
			NetPacketTreeChilds.Add(childKey, new WeakReference<INetPacketTreeChild >(Child));
			Child.ChildKey = childKey;
			Child.Father = this;
		}
		/// <summary>
		/// 删除字节点
		/// </summary>
		public void RemoveChild( short childKey)
		{
			NetPacketTreeChilds.Remove(childKey);
		}
	}
	/// <summary>
	/// 进行Packet操作的根节点
	/// </summary>
	public class NetPacketTreeMain : NetPacketTreeFather {
		/// <summary>
		/// 该mod
		/// </summary>
		public Mod mod;
		/// <summary>
		///  mod.GetPacket()
		/// </summary>
		public override ModPacket GetPacket() { return mod.GetPacket(); }
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="mod">该mod</param>
		public NetPacketTreeMain(Mod mod){
			this.mod = mod;
		}
		/// <summary>
		/// ~NetPacketTreeMain
		/// </summary>
		~NetPacketTreeMain() {
		}
	}
	/// <summary>
	/// 进行Packet操作的分支结点
	/// </summary>
	public class NetPacketTreeNode : NetPacketTreeFather, INetPacketTreeChild {
		internal short _childKey;
		/// <summary>
		/// 作为子节点在父节点中的Key
		/// </summary>
		public short ChildKey
		{
			get { return _childKey; }
			set { _childKey = value; }
		}
		internal WeakReference<NetPacketTreeFather> _Father;
		/// <summary>
		/// 作为子节点的父节点
		/// </summary>
		public NetPacketTreeFather Father
		{
			get {
				if (_Father.TryGetTarget(out var t))
					return t;
				else return null;
			}
			set { _Father.SetTarget(value); }
		}
		/// <summary>
		/// 从父亲获取到此节点的Packet;
		/// </summary>
		public override ModPacket GetPacket()
		{
			return Father.GetPacketChild(ChildKey);
		}
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="Father">父亲</param>
		/// <param name="childKey">该节点的Key</param>
		public NetPacketTreeNode(NetPacketTreeFather Father, short childKey)
		{
			_Father = new WeakReference<NetPacketTreeFather>(Father);
			this.Father.AddChild(this, childKey);
			this.ChildKey = childKey;
		}
		/// <summary>
		/// ~NetPacketTreeNode
		/// </summary>
		~NetPacketTreeNode()
		{
			Father?.NetPacketTreeChilds.Remove(ChildKey);
		}
	}
	/// <summary>
	/// 进行Packet操作的叶子结点，进行操作
	/// </summary>
	public class NetPacketTreeLeaf : INetPacketTreeChild {
		internal short _childKey;
		/// <summary>
		/// 作为子节点在父节点中的Key
		/// </summary>
		public short ChildKey
		{
			get { return _childKey; }
			set { _childKey = value; }
		}
		internal WeakReference<NetPacketTreeFather> _Father;
		/// <summary>
		/// 作为子节点的父节点
		/// </summary>
		public NetPacketTreeFather Father
		{
			get
			{
				if (_Father.TryGetTarget(out var t))
					return t;
				else return null;
			}
			set { _Father.SetTarget(value); }
		}
		/// <summary>
		/// HandleFunction的委派
		/// </summary>
		public delegate void DHandleFunction(BinaryReader reader, int whoAmI);
		/// <summary>
		/// 进行操作的函数
		/// </summary>
		public DHandleFunction HandleFunction;
		/// <summary>
		/// 使用委派
		/// </summary>
		public void Handle(BinaryReader reader, int whoAmI) {
			HandleFunction(reader, whoAmI);
		}
		/// <summary>
		/// 从父节点获取Packet
		/// </summary>
		public ModPacket GetPacket() {
			return Father.GetPacketChild(ChildKey);
		}
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="HandleFunction">进行操作的函数</param>
		/// <param name="Father">父节点</param>
		/// <param name="childKey">在父节点中的Key</param>
		/// <param name="AutoDoFunc">自动操作函数</param>
		public NetPacketTreeLeaf(DHandleFunction HandleFunction, NetPacketTreeFather Father, short childKey, Action<ModPacket> AutoDoFunc=null)
		{
			_Father = new WeakReference<NetPacketTreeFather>(Father);
			this.HandleFunction = HandleFunction;
			this.Father.AddChild(this, childKey);
			this.ChildKey = childKey;
			this.AutoDoFunc = AutoDoFunc;
		}
		/// <summary>
		///~NetPacketTreeLeaf
		/// </summary>
		~NetPacketTreeLeaf(){
			Father?.NetPacketTreeChilds.Remove(ChildKey);
		}
		/// <summary>
		/// 用AutoDoFunc自动完成并发送
		/// </summary>
		public void AutoDo(int toClient = -1, int ignoreClient = -1) {
			ModPacket mp = GetPacket();
			AutoDoFunc(mp);
			mp.Send(toClient, ignoreClient);
		}
		/// <summary>
		/// 用于自动完成ModPacket的函数
		/// </summary>
		public Action<ModPacket> AutoDoFunc;
	}

	public static class BinaryIOFunc
	{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
		public static void WriteBinaryShort(BinaryWriter writer, short data) { writer.Write(data); }
		public static short ReadBinaryShort(BinaryReader reader) { return reader.ReadInt16(); }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
	}
}
