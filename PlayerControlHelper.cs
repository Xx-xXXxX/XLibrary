using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
namespace XLibrary
{
	/// <summary>
	/// 用于处理玩家操作
	/// </summary>
	public class PlayerControlHelper:ICloneable
	{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
		public enum ControlEnum {
			MouseLeft, MouseRight, MoveFront, MoveBack, MoveUp, MoveDown, MoveJump
		}
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
		/// <summary>
		/// 玩家操作的标识，用|合并
		/// </summary>
		public static class ControlFlag {
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
			public const int MouseLeft =1<< (int)ControlEnum.MouseLeft;
			/// <summary>
			/// 对物品的右键，与左键无关
			/// </summary>
			public const int MouseRight =1<< (int)ControlEnum.MouseRight;
			public const int MoveFront = 1 << (int)ControlEnum.MoveFront;
			public const int MoveBack = 1 << (int)ControlEnum.MoveBack;
			public const int MoveUp = 1 << (int)ControlEnum.MoveUp;
			public const int MoveDown = 1 << (int)ControlEnum.MoveDown;
			public const int MoveJump = 1 << (int)ControlEnum.MoveJump;
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
			/// <summary>
			/// 鼠标左右键移动上下键组合
			/// </summary>
			public const int MouseLeftRightMoveUpDown = MoveUp | MoveDown | MouseLeft | MouseRight;
			/// <summary>
			/// 左右键组合
			/// </summary>
			public const int LeftRight= MouseLeft | MouseRight;
		}
		/// <summary>
		/// 玩家
		/// </summary>
		public Player player;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
		public bool MouseLeft => player.controlUseItem;
		/// <summary>
		/// 对物品的右键，与左键无关
		/// </summary>
		public bool MouseRight => player.controlUseItemRight();
		public bool MoveFront => (player.direction == 1) ? player.controlRight : player.controlLeft;
		public bool MoveBack => (player.direction == 1) ? player.controlLeft : player.controlRight;
		public bool MoveUp => player.controlUp;
		public bool MoveDown => player.controlDown;
		public bool MoveJump => player.controlJump;
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
		/// <summary>
		/// 现在玩家的控制的状态
		/// </summary>
		public BitsByte ControlNow;
		/// <summary>
		/// 正在使用的玩家的控制的状态，开始时决定
		/// </summary>
		public BitsByte ControlWorking;
		/// <summary>
		/// 上一次连点时（短按）使用的玩家的控制的状态，结束时决定
		/// </summary>
		public BitsByte ControlWorkingOld;
		/// <summary>
		/// 被考虑的操作
		/// </summary>
		public readonly BitsByte ControlConsidered;
		/// <summary>
		/// 被考虑的主要操作，当主要操作被按下时计时，计时结束决定ControlNow
		/// </summary>
		public readonly BitsByte ControlConsideredMain;
		/// <summary>
		/// 按下/松开 到现在的时间
		/// </summary>
		public int ControlDelay =0;
		/// <summary>
		/// 连点判定时间
		/// </summary>
		public int ControlDelayMax =8;
		/// <summary>
		/// 连点次数
		/// </summary>
		public byte ControlTimes = 0;
		/// <summary>
		/// 从 第一个主要操作 到 开始判断的时间间隔
		/// </summary>
		public byte ControlMainDelayMax = 2;
		/// <summary>
		/// 从 第一个主要操作 到 开始判断的时间
		/// </summary>
		public byte ControlMainDelay = 0;
		/// <summary>
		/// 
		/// </summary>
		public PlayerControlHelper(Player player, int ControlConsidered, int ControlConsideredMain)
		{
			this.player = player;
			this.ControlConsidered =(BitsByte) ControlConsidered;
			this.ControlConsideredMain = (BitsByte)ControlConsideredMain;
			ControlWorking = 0;
			ControlNow = 0;
		}
		/// <summary>
		/// 在对应操作激活时，传入ControlTimes，返回false阻止激活
		/// </summary>
		public Func<Player,int,bool>[] ActionsStart = new Func<Player, int, bool>[256];
		/// <summary>
		/// 在对应操作运行时，传入ControlDelay，返回false结束操作
		/// </summary>
		public Func<Player, int, bool>[] ActionsUpdate = new Func<Player, int, bool>[256];
		/// <summary>
		/// 在对应操作结束时，传入ControlTimes，返回true阻止结束操作
		/// </summary>
		public Func<Player, int, bool>[] ActionsEnd = new Func<Player, int, bool>[256];
		/// <summary>
		/// 先于ActionsStart，传入ControlNow，ControlTimes，返回false阻止ActionsStart和激活
		/// </summary>
		public Func<Player, int, int, bool> GlobalActionStart;
		/// <summary>
		/// 先于ActionsUpdate，传入ControlWorking，ControlDelay，返回false阻止ActionsUpdate并结束操作
		/// </summary>
		public Func<Player, int, int, bool> GlobalActionUpdate;
		/// <summary>
		/// 先于ActionsEnd，传入ControlWorking，ControlTimes，返回true阻止ActionsEnd,不结束
		/// </summary>
		public Func<Player, int, int, bool> GlobalActionEnd;
		/// <summary>
		/// 没有操作时
		/// </summary>
		public Action<int> NoControlUpdate;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
		/// <summary>
		/// 在对应操作激活时，传入ControlTimes，返回false阻止激活
		/// </summary>
		public void AddActionStart(int Flag, Func<Player, int, bool> action) {
			if (ActionsStart[Flag] == null) ActionsStart[Flag] = action;
			else ActionsStart[Flag] += action;
		}
		public void RemoveActionStart(int Flag, Func<Player, int, bool> action)
		{
			if (ActionsStart[Flag] == null) return;
			else ActionsStart[Flag] -= action;
		}
		/// <summary>
		/// 在对应操作运行时，传入ControlDelay，返回true结束操作
		/// </summary>
		public void AddActionUpdate(int Flag, Func<Player, int, bool> action)
		{
			if (ActionsUpdate[Flag] == null) ActionsUpdate[Flag] = action;
			else ActionsUpdate[Flag] += action;
		}
		public void RemoveActionUpdate(int Flag, Func<Player, int, bool> action)
		{
			if (ActionsUpdate[Flag] == null) return;
			else ActionsUpdate[Flag] -= action;
		}
		/// <summary>
		/// 在对应操作结束时，传入ControlTimes，返回false阻止结束操作
		/// </summary>
		public void AddActionEnd(int Flag, Func<Player, int, bool> action)
		{
			if (ActionsEnd[Flag] == null) ActionsEnd[Flag] = action;
			else ActionsEnd[Flag] += action;
		}
		public void RemoveActionEnd(int Flag, Func<Player, int, bool> action)
		{
			if (ActionsEnd[Flag] == null) return;
			else ActionsEnd[Flag] -= action;
		}
		public BitsByte GetControl()=> (BitsByte)
				((MouseLeft ? ControlFlag.MouseLeft : 0) |
				(MouseRight ? ControlFlag.MouseRight : 0) |
				(MoveFront ? ControlFlag.MoveFront : 0) |
				(MoveBack ? ControlFlag.MoveBack : 0) |
				(MoveUp ? ControlFlag.MoveUp : 0) |
				(MoveDown ? ControlFlag.MoveDown : 0) |
				(MoveJump ? ControlFlag.MoveJump : 0));
		/// <summary>
		/// 每帧进行
		/// </summary>
		public void Update() {
			ControlNow = GetControl();
			ControlNow &= ControlConsidered;
			if ((ControlWorking & ControlConsideredMain) != 0)
			{
				if ((ControlNow & ControlConsideredMain) == 0 || ((!GlobalActionUpdate?.Invoke(player,ControlWorking,ControlDelay)) ?? false) || ((!ActionsUpdate[ControlWorking]?.Invoke(player, ControlDelay))??false))
				{
					if ((!GlobalActionEnd?.Invoke(player, ControlWorking,ControlTimes)??true)&&(!ActionsEnd[ControlWorking]?.Invoke(player, ControlTimes) ??true))
					{
						ControlWorkingOld = ControlWorking;
						ControlWorking = 0;
						if (ControlDelay > ControlDelayMax) {
							ControlTimes = 0;
							ControlWorkingOld = 0;
						}
						ControlDelay = 0;
					}
				}
				ControlDelay += 1;
			}
			else {
				if ((ControlNow & ControlConsideredMain) != 0)
				{
					ControlMainDelay += 1;
					if (ControlMainDelay > ControlMainDelayMax)
					{
						if (ControlNow == ControlWorkingOld && ControlDelay <= ControlDelayMax) ControlTimes += 1;
						else ControlTimes = 0;
						if ((GlobalActionStart?.Invoke(player, ControlNow, ControlTimes) ?? true) && (ActionsStart[ControlNow]?.Invoke(player, ControlTimes) ?? true))
						{
							ControlWorking = ControlNow;
							ControlDelay = 0;
						}
						else
						{
							if (ControlNow == ControlWorkingOld && ControlDelay <= ControlDelayMax) ControlTimes -= 1;
						}
						ControlMainDelay = 0;
					}
					else NoControlUpdate?.Invoke(ControlDelay);
				}
				else { 
					NoControlUpdate?.Invoke(ControlDelay); 
					ControlMainDelay = 0;
					ControlDelay += 1;
				}
			}
		}
		/// <summary>
		/// 重置，会执行ActionEnd(无视GlobalActionEnd的true)
		/// </summary>
		public void Reset(Player player) 
		{
			if ((ControlWorking & ControlConsideredMain) != 0) {
				GlobalActionEnd?.Invoke(player, ControlWorking, ControlTimes);
				ActionsEnd[ControlWorking]?.Invoke(player, ControlTimes);
			}
			ControlDelay = 0;
			ControlMainDelay = 0;
			ControlNow = 0;
			ControlTimes = 0;
			ControlWorking = 0;
			ControlWorkingOld = 0;
			this.player = player;
		}
		/// <summary>
		/// 尝试重置，如果相同则不重置
		/// </summary>
		public bool TryReset(Player player) {
			if (!object.ReferenceEquals(player,(this.player))) { Reset(player); return true; }
			return false;
		}
		public PlayerControlHelper Clone() => (PlayerControlHelper)this.MemberwiseClone();
		object ICloneable.Clone()=> this.MemberwiseClone();
	}
}
