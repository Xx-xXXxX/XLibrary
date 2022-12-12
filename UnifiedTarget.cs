using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Terraria;
namespace XLibrary
{
	/// <summary><code><![CDATA[
	/// 用short表示玩家,NPC,Projectile,或无
	/// 值data
	/// [1,256) player的ID-1
	/// [301,501) npc的ID-301
	/// [600,1600) projectile的ID-600
	/// 0 null
	/// ]]></code>
	/// </summary>
	public struct UnifiedTarget
	{

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
		public override string ToString()
		{
			if (IsNPC) return $"npc:{npc}";
			else if (IsPlayer) return $"player:{player}";
			else if (IsProj) return $"projectile:{projectile}";
			else return $"null";
		}
		private const int PlayerSectionLeft = 1;
		private const int PlayerSectionLength = 255;
		private const int NPCSectionLeft = 301;
		private const int NPCSectionLength = 200;
		private const int ProjSectionLeft = 600;
		private const int ProjSectionLength = 1000;
		/// <summary>
		/// 确认
		/// </summary>
		public void Check() {
			if (!IsPlayer && !IsNPC && !IsProj) IsNull = true;
		}
		/// <summary>
		/// 目标的值
		/// </summary>
		private short data;
		/// <summary>
		/// 目标是否为npc
		/// </summary>
		public bool IsNPC {
			get => NPCSectionLeft <= data && data <= NPCSectionLeft+ NPCSectionLength;
		}
		/// <summary>
		/// 目标是否为player
		/// </summary>
		public bool IsPlayer {
			get => data >= PlayerSectionLeft && data <= PlayerSectionLeft+ PlayerSectionLength;
		}
		/// <summary>
		/// 目标是否为空
		/// </summary>
		public bool IsNull {
			get { Check();return data == 0; }
			set => data = 0;
		}
		/// <summary>
		/// 目标是否为Proj
		/// </summary>
		public bool IsProj {
			get => ProjSectionLeft <= data && data <= ProjSectionLeft+ ProjSectionLength;
		}
		/// <summary>
		/// 获取所表示的NPC
		/// </summary>
		public NPC GetNPC()
		{
			if (!IsNPC) return null;
			return Main.npc[NPCID];
		}
		/// <summary>
		/// 获取所表示的player
		/// </summary>
		public Player GetPlayer()
		{
			if (!IsPlayer) return null;
			return Main.player[PlayerID];
		}
		/// <summary>
		/// 获取所表示的Proj
		/// </summary>
		public Projectile GetProj()
		{
			if (!IsProj) return null;
			return Main.projectile[ProjID];
		}
		/// <summary>
		/// 所表示的NPC
		/// </summary>
		public NPC npc
		{
			get => GetNPC();
			set => Set(value);
		}
		/// <summary>
		/// 所表示的player
		/// </summary>
		public Player player {
			get => GetPlayer();
			set => Set(value);
		}
		/// <summary>
		/// 所表示的proj
		/// </summary>
		public Projectile projectile
		{
			get => GetProj();
			set => Set(value);
		}
		/// <summary>
		/// 所表示的npc的id
		/// </summary>
		public int NPCID
		{
			get
			{
				if (IsNPC) return data - NPCSectionLeft;
				else return -1;
			}
			set { data = (short)(value + NPCSectionLeft); Check(); }
		}
		/// <summary>
		/// 所表示的player的id
		/// </summary>
		public int PlayerID{
			get {
				if (IsPlayer) return data- PlayerSectionLeft;
				else return -1;
			}
			set { data = (short)(value + PlayerSectionLeft); Check(); }
		}
		/// <summary>
		/// 所表示的projectile的id
		/// </summary>
		public int ProjID {
			get {
				if (IsPlayer) return data - ProjSectionLeft;
				else return -1;
			}
			set { data = (short)(value + ProjSectionLeft); Check(); }
		}
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
		public UnifiedTarget(short data=0) { this.data = data; Check(); }
		public static explicit operator short(UnifiedTarget A) { return A.data; }
		public static explicit operator UnifiedTarget(short A) { return new UnifiedTarget(A); }
		public static explicit operator UnifiedTarget(int A) { return new UnifiedTarget((short)A); }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
		//public void WriteBinary(BinaryWriter writer) { writer.Write(data); }
		//public void ReadBinary(BinaryReader reader) { data = reader.ReadInt16(); }
		/// <summary>
		/// 将UnifiedTarget写入流
		/// </summary>
		public static void WriteBinary(BinaryWriter writer, UnifiedTarget i) { writer.Write(i.data); }
		/// <summary>
		/// 从流读取UnifiedTarget
		/// </summary>
		public static UnifiedTarget ReadBinary(BinaryReader reader) {
			return new UnifiedTarget(reader.ReadInt16());
		}
		/// <summary>
		/// 生成对应NPC的UnifiedTarget
		/// </summary>
		public static UnifiedTarget MakeNPC(int NPCID) {
			return new UnifiedTarget() { NPCID = NPCID };
		}
		/// <summary>
		/// 生成对应player的UnifiedTarget
		/// </summary>
		public static UnifiedTarget MakePlayer(int PlayerID)
		{
			return new UnifiedTarget() { PlayerID = PlayerID };
		}
		/// <summary>
		/// 生成对应proj的UnifiedTarget
		/// </summary>
		public static UnifiedTarget MakeProj(int ProjID)
		{
			return new UnifiedTarget() { ProjID = ProjID };
		}
		/// <summary>
		/// 生成对应npc.target的UnifiedTarget
		/// </summary>
		public static UnifiedTarget MakeNPCTarget(NPC npc)
		{
			return new UnifiedTarget() { NPCTarget = npc.target };
		}
		/// <summary>
		/// 空对象
		/// </summary>
		public static UnifiedTarget Null => new UnifiedTarget();
		/// <summary>
		/// 生成对应NPC的id的UnifiedTarget
		/// </summary>
		public static UnifiedTarget MakeNPC(NPC npc)
		{
			return new UnifiedTarget() { NPCID = npc.whoAmI };
		}
		/// <summary>
		/// 生成对应player的id的UnifiedTarget
		/// </summary>
		public static UnifiedTarget MakePlayer(Player player)
		{
			return new UnifiedTarget() { PlayerID = player.whoAmI };
		}
		/// <summary>
		/// 生成对应proj的id的UnifiedTarget
		/// </summary>
		public static UnifiedTarget MakeProj(Projectile projectile)
		{
			return new UnifiedTarget() { ProjID = projectile.whoAmI };
		}
		/// <summary>
		/// 生成对应NPC的id的UnifiedTarget
		/// </summary>
		public static UnifiedTarget Make(NPC npc)
		{
			return new UnifiedTarget() { NPCID = npc.whoAmI };
		}
		/// <summary>
		/// 生成对应player的id的UnifiedTarget
		/// </summary>
		public static UnifiedTarget Make(Player player)
		{
			return new UnifiedTarget() { PlayerID = player.whoAmI };
		}
		/// <summary>
		/// 生成对应proj的id的UnifiedTarget
		/// </summary>
		public static UnifiedTarget Make(Projectile projectile)
		{
			return new UnifiedTarget() { ProjID = projectile.whoAmI };
		}
		/// <summary>
		/// 生成空对象
		/// </summary>
		public static UnifiedTarget Make()
		{
			return new UnifiedTarget();
		}
		/// <summary>
		/// 生成对应npc.target的UnifiedTarget
		/// </summary>
		public static UnifiedTarget MakeNPCTarget(int target)
		{
			return new UnifiedTarget() { NPCTarget = target };
		}
		/// <summary>
		/// 设置表示npc
		/// </summary>
		public void Set(NPC npc) { NPCID = npc.whoAmI; }
		/// <summary>
		/// 设置表示player
		/// </summary>
		public void Set(Player player) { PlayerID = player.whoAmI; }
		/// <summary>
		/// 设置表示proj
		/// </summary>
		public void Set(Projectile projectile) { ProjID = projectile.whoAmI; }
		/// <summary>
		/// 设置表示npc
		/// </summary>
		public void SetNPC(int id) => NPCID = id;
		/// <summary>
		/// 设置表示player
		/// </summary>
		public void SetPlayer(int id) => PlayerID = id;
		/// <summary>
		/// 设置表示proj
		/// </summary>
		public void SetProj(int id) => ProjID = id;
		/// <summary>
		/// 与npc.target对应
		/// </summary>
		public int NPCTarget {
			get {
				return data - 1;
			}
			set {
				data = (short)(value + 1);
				Check();
			}
		}
		/// <summary>
		/// 设置表示npc.target
		/// </summary>
		public void SetNPCTarget(NPC npc) => NPCTarget = npc.target;
		/// <summary>
		/// 设置表示npc.target
		/// </summary>
		public void SetNPCTarget(int target)=> NPCTarget = target;
		/// <summary>
		/// 对应的Entity
		/// </summary>
		public Entity entity {
			get {
				if (IsNPC) return npc;
				else if (IsPlayer) return player;
				else if (IsProj) return projectile;
				else return null;
			}
		}
	}
	
}
