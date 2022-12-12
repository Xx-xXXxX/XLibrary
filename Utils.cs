using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using Terraria.ID;

namespace XLibrary
{
	/// <summary>
	/// 方法
	/// </summary>
	public static partial class Utils
	{
		/// <summary>
		/// 启用Effect(Immediate)
		/// </summary>
		public static void SpriteBatchUsingEffect(SpriteBatch spriteBatch)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}
		/// <summary>
		/// 结束启用Effect(Deferred)
		/// </summary>
		public static void SpriteBatchEndUsingEffect(SpriteBatch spriteBatch)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}
		/// <summary>
		/// 在其他边不变的情况下设置左边界
		/// </summary>
		public static Rectangle SetLeft(this Rectangle rect, int L)
		{
			rect.Width += L - rect.X;
			rect.X = L;
			return rect;
		}
		/// <summary>
		/// 在其他边不变的情况下设置右边界
		/// </summary>
		public static Rectangle SetRight(this Rectangle rect, int R)
		{
			rect.Width = R - rect.X; return rect;
		}
		/// <summary>
		/// 在其他边不变的情况下设置上边界
		/// </summary>
		public static Rectangle SetTop(this Rectangle rect, int T)
		{
			rect.Height += T - rect.Y;
			rect.Y = T;
			return rect;
		}
		/// <summary>
		/// 在其他边不变的情况下设置下边界
		/// </summary>
		public static Rectangle SetBottom(this Rectangle rect, int B)
		{
			rect.Height = B - rect.X; return rect;
		}
		/// <summary>
		/// 在右下角不变的情况下设置左上角
		/// </summary>
		public static Rectangle SetLT(this Rectangle rect, Point point)
		{
			rect.SetLeft(point.X);
			rect.SetTop(point.Y);
			return rect;
		}
		/// <summary>
		/// 在左上角不变的情况下设置右下角
		/// </summary>
		public static Rectangle SetRB(this Rectangle rect, Point point)
		{
			rect.SetRight(point.X);
			rect.SetBottom(point.Y);
			return rect;
		}
		/// <summary>
		/// 移动
		/// </summary>
		public static Rectangle MoveBy(this Rectangle rect, Point point)
		{
			rect.X += point.X;
			rect.Y += point.Y;
			return rect;
		}
		/// <summary>
		/// 判断npc是否活动
		/// </summary>
		public static bool NPCCanUse(this NPC npc) => npc.active;
		/// <summary>
		/// 判断player是否活动
		/// </summary>
		public static bool PlayerCanUse(this Player player) => player.active && !player.dead && !player.ghost;
		/// <summary>
		/// 判断npc是否可以追踪
		/// </summary>
		public static bool NPCCanFind(this NPC npc) => NPCCanUse(npc) && npc.CanBeChasedBy();
		/// <summary>
		/// 判断npc是否可以追踪，避开Tile
		/// </summary>
		public static bool NPCCanFindNoTile(this NPC npc, Vector2 pos) => NPCCanFind(npc) && Terraria.Collision.CanHitLine(npc.Center, 1, 1, pos, 1, 1);
		/// <summary>
		/// 判断玩家是否可以追踪
		/// </summary>
		public static bool PlayerCanFind(this Player player) => PlayerCanUse(player);
		/// <summary>
		/// 判断npc是否可以追踪，避开Tile
		/// </summary>
		public static bool PlayerCanFindNoTile(this Player player, Vector2 pos) => PlayerCanUse(player) && Terraria.Collision.CanHitLine(player.Center, 1, 1, pos, 1, 1);
		/// <summary>
		/// 将n限制在[l,r)
		/// </summary>
		public static int Limit(int n, int l, int r)
		{
			if (n < l) n = l;
			if (n >= r) n = r;
			return n;
		}
		/// <summary>
		/// 返回n在[l,r)中循环的结果
		/// </summary>
		public static int LimitLoop(int n, int l, int r)
		{
			int d = r - l;
			n = ((n - l) % d)+l;
			if (n < l) n += d;
			return n;
		}
		/// <summary>
		/// 返回n在[l,r)中循环的结果
		/// </summary>
		public static double LimitLoop(double n, double l, double r)
		{
			double d = r - l;
			n = ((n - l) % d) + l;
			if (n < l) n += d;
			return n;
		}
		/// <summary>
		/// 返回n在[l,r)中循环的结果
		/// </summary>
		public static float LimitLoop(float n, float l, float r)
		{
			float d = r - l;
			n = ((n - l) % d) + l;
			if (n < l) n += d;
			return n;
		}
		/// <summary>
		/// 用于计算的方法
		/// </summary>
		public static class CalculateUtils
		{
			#region MathFunc
			/// <summary>
			/// 获取一个随n缓慢减小的值,从1到0
			/// <code>Sqrt(n + 1) - Sqrt(n)</code>
			/// </summary>
			public static double SlowlyDecreaseLim1To0(double n) => Math.Sqrt(n + 1) - Math.Sqrt(n);
			/// <summary>
			/// 获取一个随n缓慢减小的值,从1到0,SpeedParameter绝定该函数增长的速度。SpeedParameter越小，减小越慢。SpeedParameter应大于1
			/// <code>Math.Pow(n+1, 1 / SpeedParameter) - Math.Pow(n, 1 / SpeedParameter)</code>
			/// </summary>
			public static double SlowlyDecreaseLim1To0(double n, double SpeedParameter) => Math.Pow(n + 1, 1 / SpeedParameter) - Math.Pow(n, 1 / SpeedParameter);
			/// <summary>
			/// 用SlowlyDecreaseLim1To0获取一个随n缓慢增长的值,从0到1
			/// <code>1-SlowlyDecreaseLim1To0(n)</code>
			/// </summary>
			public static double SlowlyIncreaseLim0To1(double n) => 1 - SlowlyDecreaseLim1To0(n);
			/// <summary>
			/// 用SlowlyIncreaseLim0To1获取一个随n缓慢增长的值,从l到r
			/// </summary>
			public static double SlowlyIncreaseLim(double n, double l, double r) => SlowlyIncreaseLim0To1(n) * (r - l) + l;
			/// <summary><![CDATA[
			/// SlowlyIncrease 的原始函数
			/// 获取一个增长的值，n为参数，SpeedParameter绝定该函数增长的速度。SpeedParameter越大，增长越慢。
			/// SpeedParameter>e时会比log(n+1)小，SpeedParameter<1时比n大]]>
			/// <code>Exp(Pow(Ln(n+1), 1 / SpeedParameter));</code>
			/// </summary>
			public static double SlowlyIncreaseRaw(double n, double SpeedParameter) => Math.Exp(Math.Pow(Math.Log(n + 1), 1 / SpeedParameter));
			/// <summary>
			/// 获取一个缓慢增长的值，n为参数，SpeedParameter绝定该函数增长的速度。SpeedParameter越大，增长越慢。比log(n+1)大。SpeedParameter应大于0
			/// <code>Exp(Pow(Ln(n+1), 1/SlowlyIncreaseLim(SpeedParameter,1,E)));</code>
			/// </summary>
			public static double SlowlyIncrease(double n, double SpeedParameter) => Math.Exp(Math.Pow(Math.Log(n + 1), 1 / SlowlyIncreaseLim(SpeedParameter, 1, Math.E)));
			/// <summary>
			/// 获取一个快速增长的值，在n取1时达到正无穷，n为参数
			/// <code>Tan(n*PI/2)</code>
			/// </summary>
			public static double FastlyIncreaseToInf(double n) => Math.Tan(n * Math.PI / 2);
			#endregion
			#region Colliding
			/// <summary>
			/// 获取 在Box中 到Point最近的点，可用于判断碰撞
			/// </summary>
			public static Vector2 GetNearestPoint(Rectangle Box, Vector2 Point)
			{
				Vector2 NearestPoint = Point;
				if (NearestPoint.X < Box.Left) NearestPoint.X = Box.Left;
				if (NearestPoint.X > Box.Right) NearestPoint.X = Box.Right;
				if (NearestPoint.Y < Box.Top) NearestPoint.Y = Box.Top;
				if (NearestPoint.Y > Box.Bottom) NearestPoint.Y = Box.Bottom;
				return NearestPoint;
			}
			/// <summary>
			/// 获取 在Box中 到Point最远的点，可用于判断碰撞
			/// </summary>
			public static Vector2 GetFarestPoint(Rectangle Box, Vector2 Point)
			{
				Vector2 NearestPoint = Point;
				if (Box.Right - NearestPoint.X > NearestPoint.X - Box.Left) NearestPoint.X = Box.Right;
				else NearestPoint.X = Box.Left;
				if (Box.Bottom - NearestPoint.Y > NearestPoint.Y - Box.Top) NearestPoint.Y = Box.Bottom;
				else NearestPoint.Y = Box.Top;
				return NearestPoint;
			}
			/// <summary>
			/// 判断点是否在圆内
			/// </summary>
			/// <param name="Pos">圆心</param>
			/// <param name="R">半径</param>
			/// <param name="Point">目标点</param>
			/// <returns></returns>
			public static bool CheckPointInCircle(Vector2 Pos, float R, Vector2 Point) => (Point - Pos).LengthSquared() <= R * R;
			/// <summary>
			/// 判断Box与 Pos为圆心，R为半径的圆 是否碰撞
			/// </summary>
			public static bool CheckAABBvCircleColliding(Rectangle Box, Vector2 Pos, float R)
			{
				Vector2 P = GetNearestPoint(Box, Pos);
				return CheckPointInCircle(Pos, R, P);
			}
			/// <summary>
			/// 判断Box与 Pos为圆心，半径MinR到MaxR的圆环 是否碰撞
			/// </summary>
			public static bool CheckAABBvAnnulusColliding(Rectangle Box, Vector2 Pos, float MaxR, float MinR)
			{
				Vector2 PMin = GetNearestPoint(Box, Pos);
				Vector2 PMax = GetFarestPoint(Box, Pos);
				return CheckPointInCircle(Pos, MaxR, PMin) && !CheckPointInCircle(Pos, MinR, PMax);
			}
			#endregion
			/// <summary>
			/// 计算二维向量叉乘的长
			/// </summary>
			public static float CrossProduct(Vector2 v1, Vector2 v2) => v1.X * v2.Y - v1.Y * v2.X;
			/// <summary>
			/// 根据相对位置，相对速度，固定发射速度进行预判
			/// </summary>
			/// <param name="OffsetPos">相对位置</param>
			/// <param name="OffsetVel">相对速度</param>
			/// <param name="Speed">固定发射速度</param>
			public static float? PredictWithVel(Vector2 OffsetPos, Vector2 OffsetVel, float Speed)
			{
				float D = CrossProduct(OffsetPos, OffsetVel) / (Speed * OffsetPos.Length());
				if (D > 1 || D < -1) return null;
				else return (float)Math.Asin(D) + OffsetPos.ToRotation();
			}
			/// <summary>
			/// 预判，返回速度，如果没有，返回Vector2.Normalize(OffsetVel) * Speed)
			/// </summary>
			/// <param name="OffsetPos"></param>
			/// <param name="OffsetVel"></param>
			/// <param name="Speed"></param>
			/// <returns></returns>
			public static Vector2 PredictWithVelDirect(Vector2 OffsetPos, Vector2 OffsetVel, float Speed)
			{
				//float? D= PredictWithVel(OffsetPos, OffsetVel, Speed);
				//return (D.HasValue ? D.Value.ToRotationVector2() * Speed : Vector2.Normalize(OffsetVel) * Speed);
				return (PredictWithVel(OffsetPos, OffsetVel, Speed) ?? OffsetVel.ToRotation()).ToRotationVector2() * Speed;
			}
			///// <summary>
			///// 以npc与Pos的距离为价值
			///// <code>(npc.Center - Pos).Length() - npc.Size.Length()</code>
			///// </summary>
			//public static float NPCFindValue(NPC npc, Vector2 Pos) => (npc.Center - Pos).Length() - npc.Size.Length() / 2;
			///// <summary>
			///// 以player与Pos的距离为价值
			///// <code>(player.Center - Pos).Length() - player.Size.Length()</code>
			///// </summary>
			//public static float PlayerFindValue(Player player, Vector2 Pos) => (player.Center - Pos).Length() - player.Size.Length() / 2;
			
			///// <summary>
			///// 根据价值搜索目标，找到价值最低的目标
			///// </summary>
			///// <param name="Pos">搜索位置</param>
			///// <param name="DefValue">初始价值</param>
			///// <param name="FindFriendly">将player和友好的npc作为可选目标</param>
			///// <param name="FindHostile">将敌对的npc作为可选目标</param>
			///// <param name="NPCCanFindFunc">确认npc可以作为目标</param>
			///// <param name="PlayerCanFindFunc">确认player可以作为目标</param>
			///// <param name="NPCFindValueFunc">npc的价值，默认NPCFindValue</param>
			///// <param name="PlayerFindValueFunc">player的价值，默认PlayerFindValue</param>
			///// <returns></returns>
			//public static UnifiedTarget FindTargetClosest(Vector2 Pos, float DefValue, bool FindFriendly, bool FindHostile, Func<NPC, bool> NPCCanFindFunc = null, Func<Player, bool> PlayerCanFindFunc = null, Func<NPC, float> NPCFindValueFunc = null, Func<Player, float> PlayerFindValueFunc = null)
			//{
			//	UnifiedTarget T = new UnifiedTarget();
			//	float Value = DefValue;
			//	if (NPCCanFindFunc == null) NPCCanFindFunc = NPCCanFind;
			//	if (PlayerCanFindFunc == null) PlayerCanFindFunc = PlayerCanFind;
			//	if (NPCFindValueFunc == null) NPCFindValueFunc = (npc) => NPCFindValue(npc, Pos);
			//	if (PlayerFindValueFunc == null) PlayerFindValueFunc = (player) => PlayerFindValue(player, Pos);
			//	foreach (var i in Main.npc)
			//	{
			//		if (NPCCanFindFunc(i))
			//		{
			//			if ((FindFriendly && i.friendly) || (FindHostile && !i.friendly))
			//			{
			//				float newValue = NPCFindValueFunc(i);
			//				if (newValue < Value)
			//				{
			//					T.Set(i);
			//					Value = newValue;
			//				}
			//			}
			//		}
			//	}
			//	if (FindFriendly)
			//	{
			//		foreach (var i in Main.player)
			//		{
			//			if (PlayerCanFindFunc(i))
			//			{
			//				float newValue = PlayerFindValueFunc(i);
			//				if (newValue < Value)
			//				{
			//					T.Set(i);
			//					Value = newValue;
			//				}

			//			}
			//		}
			//	}
			//	return T;
			//}
			/// <summary>
			/// 加权选择
			/// </summary>
			/// <returns>
			/// 返回选中的第几项
			/// 返回-1表示I超过权的和</returns>
			public static int WeightedChoose(int I, params int[] values)
			{
				for (int i = 0; i < values.Length; ++i)
				{
					if (I < values[i]) return i;
					I -= values[i];
				}
				return -1;
			}
			/// <summary>
			/// 计算以origin为起始点，向direction方向移动，直到碰到方块或到达最远距离的距离，不考虑斜方块和半砖
			/// </summary>
			public static float CanHitLineDistance(Vector2 origin, float direction, float MaxDistance = 2200f,bool fallThroughPlasform=true)
			{
				
				if (MaxDistance == 0) return 0f;
				bool first = true;
				Vector2 Offset =  direction.ToRotationVector2() * MaxDistance;
				Vector2 Unit = Vector2.Normalize(Offset);
				foreach (var i in EnumTilesInLine(origin, origin + Offset)) {
					Tile tile = Main.tile[i.X, i.Y];
					if (tile.HasTile&& Main.tileSolid[tile.TileType]&&(Main.tileSolidTop[tile.TileType] ? !fallThroughPlasform:true)) {
						if (first) return 0f;
						Vector2 tilePos = i.ToVector2() * 16;
						Vector2 HitT =Unit*( tilePos.Y - origin.Y)/Unit.Y;
						Vector2 HitB = Unit * (tilePos.Y + 16 - origin.Y) / Unit.Y;
						Vector2 HitL = Unit * (tilePos.X - origin.X) / Unit.X;
						Vector2 HitR = Unit * (tilePos.X + 16 - origin.X) / Unit.X;
						float HitTD = Vector2.Dot(HitT,Unit);
						float HitBD = Vector2.Dot(HitB, Unit);
						float HitLD = Vector2.Dot(HitL, Unit);
						float HitRD = Vector2.Dot(HitR, Unit);
						List<float> vs = new List<float>() { HitBD, HitLD, HitRD };
						//vs.Sort();
						//return vs[1];
						float vs1= HitTD; float vs2= HitBD;
						foreach (var j in vs) {
							if (j < vs1)
							{
								vs2 = vs1; vs1 = j;
							}
							else if (j < vs2) {
								vs2 = j;
							}
						}
						return vs2;
					}
					first = false;
				}
				return MaxDistance;
			}
			/// <summary>
			/// 计算以origin为起始点，向direction方向移动，直到碰到方块或到达最远距离的距离，考虑斜方块和半砖
			/// </summary>
			public static float CanHitLineDistancePerfect(Vector2 origin, float direction, float MaxDistance = 2200f, bool fallThroughPlasform = true)
			{
				
				if (MaxDistance == 0) return 0f;
				bool first = true;
				Vector2 Offset = direction.ToRotationVector2() * MaxDistance;
				Vector2 Unit = Vector2.Normalize(Offset);
				foreach (var i in EnumTilesInLine(origin, origin + Offset))
				{
					Tile tile = Main.tile[i.X, i.Y];
					
					if (tile.HasTile&& Main.tileSolid[tile.TileType] && (Main.tileSolidTop[tile.TileType] ? !fallThroughPlasform : true))
					{
						if (first) return 0f;
						Vector2 tilePos = i.ToVector2() * 16;
						List<Vector2> Points=new List<Vector2>();
						if (tile.IsHalfBlock)
						{
							Points.Add(tilePos + new Vector2(0, 8));
							Points.Add(tilePos + new Vector2(16, 8));
							Points.Add(tilePos + new Vector2(16, 16));
							Points.Add(tilePos + new Vector2(0, 16));
						}
						else
						{
							switch ((int)tile.Slope)
							{
								case 0:
									{
										Points.Add(tilePos + new Vector2(0, 0));
										Points.Add(tilePos + new Vector2(16, 0));
										Points.Add(tilePos + new Vector2(16, 16));
										Points.Add(tilePos + new Vector2(0, 16));
									}
									break;
								case 1:
									{
										Points.Add(tilePos + new Vector2(0, 0));
										Points.Add(tilePos + new Vector2(16, 16));
										Points.Add(tilePos + new Vector2(0, 16));
									}
									break;
								case 2:
									{
										Points.Add(tilePos + new Vector2(16, 0));
										Points.Add(tilePos + new Vector2(16, 16));
										Points.Add(tilePos + new Vector2(0, 16));
									}
									break;
								case 3:
									{
										Points.Add(tilePos + new Vector2(0, 0));
										Points.Add(tilePos + new Vector2(16, 0));
										Points.Add(tilePos + new Vector2(0, 16));
									}
									break;
								case 4:
									{
										Points.Add(tilePos + new Vector2(0, 0));
										Points.Add(tilePos + new Vector2(16, 0));
										Points.Add(tilePos + new Vector2(16, 16));
									}
									break;
							}
						}
						//List<float> Collidings = new List<float>();
						//for (int k = 0; k < Points.Count-1; ++k) {
						//	Collidings.Add(LineCollitionDistance(origin, direction, Points[k], (Points[k+1]-Points[k]).ToRotation()));
						//}
						//Collidings.Add(LineCollitionDistance(origin, direction, Points[Points.Count - 1], (Points[Points.Count - 1] - Points[0]).ToRotation()));
						//Collidings.Sort();
						//return Collidings[Collidings.Count-3];
						List<Vector2> Collidings = new List<Vector2>();
						Vector2 End = direction.ToRotationVector2() * MaxDistance + origin;
						foreach (var k in Points.EnumPairs()) {
							Collidings.AddRange(Terraria.Collision.CheckLinevLine(origin, End,k.Item1,k.Item2));
						}
						if (Collidings.Count <= 0) continue;
						float Distance = MaxDistance;
						foreach (var k in Collidings) {
							float NowDistance = (k - origin).Length();
							if (NowDistance < Distance) Distance = NowDistance;
						}
						return Distance;
					}
					first = false;
				}
				return MaxDistance;
			}
			/// <summary>
			/// 线1 过原点，方向Direction1 与 线2 过Point2 ，方向Direction2 交点到原点的距离（反向为负）
			/// </summary>
			public static float LineCollitionDistance(float Direction1, Vector2 Point2, float Direction2) {
				float P2Angle = Point2.ToRotation();
				float Angle1 = P2Angle - Direction1;
				float Angle2 = (float)Math.PI - P2Angle + Direction2;
				float Angle3 = (float)Math.PI - Angle1 - Angle2;
				float Distance =(float)( Point2.Length() / Math.Sin(Angle3) * Math.Sin(Angle2));
				return Distance;
			}
			/// <summary>
			/// 线1 过Point1，方向Direction1 与 线2 过Point2 ，方向Direction2 交点到Point1的距离（反向为负）
			/// </summary>
			public static float LineCollitionDistance(Vector2 Point1, float Direction1, Vector2 Point2, float Direction2) => LineCollitionDistance(Direction1,Point2-Point1,Direction2);
			/// <summary>
			/// 线1 过原点，方向Direction1 与 线2 过Point2 ，方向Direction2 交点到原点的距离（反向为负）
			/// </summary>
			public static float LineCollitionDistance(Vector2 Direction1, Vector2 Point2, Vector2 Direction2) {
				return Direction1.Length() * CrossProduct(Point2, Direction2) / CrossProduct(Direction1, Direction2);
			}
			/// <summary>
			/// 线1 Point1a，Point1b 与 线2 Point2a ，Point2b 交点到Point1a的距离（反向为负）
			/// </summary>
			public static float LineCollitionDistance(Vector2 Point1a, Vector2 Point1b, Vector2 Point2a, Vector2 Point2b)
			{
				return LineCollitionDistance(Point1b - Point1a,Point2a-Point1a,Point2b-Point2a);
			}
			/// <summary>
			/// 计算碰撞箱以Velocity速度移动，碰撞后最终速度
			/// </summary>
			/// <param name="rect"></param>
			/// <param name="Velocity"></param>
			/// <param name="StopWhenHit">是否在碰到物块时结束计算</param>
			/// <param name="fallThrough">是否穿过平台</param>
			/// <param name="fall2">在Velocity.Y>1时穿过平台</param>
			/// <param name="gravDir"></param>
			/// <returns></returns>
			public static Vector2 TileCollisionPerfect(Rectangle rect, Vector2 Velocity, bool StopWhenHit = false, bool fallThrough = false, bool fall2 = false, int gravDir = 1)
			{
				Vector2 Position = rect.Location.ToVector2();
				int Width = rect.Width;
				int Height = rect.Height;
				Vector2 NewPosition = Position;
				float PL = Math.Min(Width, Height);
				Vector2 PV = Vector2.Normalize(Velocity) * PL;
				Vector2 PV0 = PV;
				int i = 1;
				bool RealFall = fallThrough || (Velocity.Y > 1 && fall2);
				for (; PL * i < Velocity.Length(); ++i)
				{
					Vector2 NPL = Collision.TileCollision(NewPosition, PV, Width, Height, RealFall, false, gravDir);
					NewPosition += NPL;
					if (NPL != PV && StopWhenHit) return NewPosition - Position;
					PV = NPL;
				}
				NewPosition += Collision.TileCollision(NewPosition, (PV / PV0.Length()) * (Velocity.Length() - PL * (i - 1)), Width, Height, RealFall, false, gravDir);
				return NewPosition - Position;
			}
			/// <summary>
			/// 对蠕虫的头执行，加载全身，需要npc最后的速度
			/// </summary>
			/// <param name="npc">头</param>
			/// <param name="Nextnpc">下一个npc</param>
			/// <param name="End">是否结束</param>
			/// <param name="projLength">体节的长度，应比npc的长度稍小</param>
			/// <param name="AngleAmount">体节转向角度缩小比例</param>
			public static void UpdateWornPositionNPC(NPC npc, Func<NPC, NPC> Nextnpc, Func<NPC, bool> End, float projLength, float AngleAmount)
			{
				while (End(npc))
				{
					NPC Nownpc = Nextnpc(npc);
					Vector2 ToPos = npc.Center + npc.velocity + new Vector2(-0.5f * projLength, 0).RotatedBy(npc.rotation);
					Vector2 OffsetPos = ToPos - Nownpc.Center;
					float OffsetAngle = npc.rotation.AngleLerp(OffsetPos.ToRotation(), 1 - AngleAmount);
					Nownpc.Center = ToPos - OffsetAngle.ToRotationVector2() * projLength / 2;
					Nownpc.rotation = OffsetAngle;
					Nownpc.velocity = Vector2.Zero;
					npc = Nownpc;
				}
			}
			/// <summary>
			/// 对蠕虫的头执行，加载全身，需要npc最后的速度
			/// </summary>
			/// <param name="npc">头</param>
			/// <param name="Nextnpc">下一个npc</param>
			/// <param name="End">是否结束</param>
			/// <param name="projLength">体节的长度，应比npc的长度稍小</param>
			public static void UpdateWornPositionNPC(NPC npc, Func<NPC, NPC> Nextnpc, Func<NPC, bool> End, float projLength)
			{
				while (End(npc))
				{
					NPC Nownpc = Nextnpc(npc);
					Vector2 ToPos = npc.Center + npc.velocity + new Vector2(-0.5f * projLength, 0).RotatedBy(npc.rotation);
					Vector2 OffsetPos = ToPos - Nownpc.Center;
					float OffsetAngle = OffsetPos.ToRotation();
					Nownpc.Center = ToPos - OffsetAngle.ToRotationVector2() * projLength / 2;
					Nownpc.rotation = OffsetAngle;
					Nownpc.velocity = Vector2.Zero;
					npc = Nownpc;
				}
			}
			/// <summary>
			/// 对蠕虫的头执行，加载全身，需要projectile最后的速度
			/// </summary>
			/// <param name="projectile">头</param>
			/// <param name="NextProj">下一个proj</param>
			/// <param name="End">是否结束</param>
			/// <param name="projLength">体节的长度，应比proj的长度稍小</param>
			/// <param name="AngleAmount">体节转向角度缩小比例</param>
			public static void UpdateWornPositionProj(Projectile projectile, Func<Projectile, Projectile> NextProj, Func<Projectile, bool> End, float projLength, float AngleAmount)
			{
				while (!End(projectile))
				{
					Projectile NowProj1 = NextProj(projectile);
					Vector2 ToPos = projectile.Center + projectile.velocity + new Vector2(-0.5f * projLength, 0).RotatedBy(projectile.rotation);
					Vector2 OffsetPos = ToPos - NowProj1.Center;
					float OffsetAngle = projectile.rotation.AngleLerp(OffsetPos.ToRotation(), 1 - AngleAmount);
					NowProj1.Center = ToPos - OffsetAngle.ToRotationVector2() * projLength / 2;
					NowProj1.rotation = OffsetAngle;
					NowProj1.velocity = Vector2.Zero;
					projectile = NowProj1;
				}
			}
			/// <summary>
			/// 对蠕虫的头执行，加载全身，需要projectile最后的速度
			/// </summary>
			/// <param name="projectile">头</param>
			/// <param name="NextProj">下一个proj</param>
			/// <param name="End">是否结束</param>
			/// <param name="projLength">体节的长度，应比proj的长度稍小</param>
			public static void UpdateWornPositionProj(Projectile projectile, Func<Projectile, Projectile> NextProj, Func<Projectile, bool> End, float projLength)
			{
				while (!End(projectile))
				{
					Projectile NowProj1 = NextProj(projectile);
					Vector2 ToPos = projectile.Center + projectile.velocity + new Vector2(-0.5f * projLength, 0).RotatedBy(projectile.rotation);
					Vector2 OffsetPos = ToPos - NowProj1.Center;
					float OffsetAngle = OffsetPos.ToRotation();
					NowProj1.Center = ToPos - OffsetAngle.ToRotationVector2() * projLength / 2;
					NowProj1.rotation = OffsetAngle;
					NowProj1.velocity = Vector2.Zero;
					projectile = NowProj1;
				}
			}
			/// <summary>
			/// 获取两个向量的夹角的cos值
			/// </summary>
			public static float AngleCos(Vector2 a, Vector2 b) => (Vector2.Dot(a, b)) / (a.Length() * b.Length());
			/// <summary>
			/// 获取两个向量的夹角的sin值
			/// </summary>
			public static float AngleSin(Vector2 a, Vector2 b) => CrossProduct(a,b) / (a.Length() * b.Length());
			/// <summary>
			/// 枚举线上的物块
			/// </summary>
			public static IEnumerable<Point> EnumTilesInLine(Vector2 Start, Vector2 End)
			{
				if (((int)(Start.Y/16))== (int)(End.Y / 16))
				{
					int Y = (int)(Start.Y / 16);
					if (Y < 1 || Y >= Main.maxTilesY) yield break;
					int L = (int)(Start.X / 16);
					int R = (int)(End.X / 16);
					if (L > R)
					{
						if (L < 1) L = 1;
						if (R >= Main.maxTilesX) R = Main.maxTilesX - 1;
						for (int i = L; i <= R; ++i)
							yield return new Point(i, Y);
						yield break;
					}
					else {
						if (R < 1) R = 1;
						if (L >= Main.maxTilesX) L = Main.maxTilesX - 1;
						for (int i = R; i >= L; --i)
							yield return new Point(i, Y);
						yield break;
					}
				}
				//Vector2 Point = Start;
				Vector2 Offset = End - Start;
				Vector2 Unit = Vector2.Normalize(Offset);
				bool GoRight = Unit.X > 0;
				bool GoDown = Unit.Y > 0;
				Vector2 UnitY16 = Unit * (16 / Unit.Y);
				float FromX = Start.X;
				float FromY = Start.Y;
				int FromXP = (int)(FromX / 16);
				int FromYP = (int)(FromY / 16);
				int NextYP;
				float NextX;
				int NextXP;
				int EndYP = (int)(End.Y / 16f);
				if (GoDown)
				{
					if (EndYP < 0) yield break;
					if (EndYP >= Main.maxTilesY) EndYP = Main.maxTilesY - 1;
					NextYP = FromYP + 1;
					NextX = FromX + UnitY16.X * (NextYP * 16 - FromY) / 16;
					NextXP = (int)(NextX / 16f);
					while (FromYP < EndYP)
					{
						if (FromYP >= 0)
						{
							if (GoRight)
							{
								if (FromXP < 0) FromXP = 0;
								if (NextXP >= Main.maxTilesX) NextXP = Main.maxTilesX - 1;
								for (int i = FromXP; i <= NextXP; ++i)
									yield return new Point(i, FromYP);
							}
							else
							{
								if (NextXP < 0) NextXP = 0;
								if (FromXP >= Main.maxTilesX) FromXP = Main.maxTilesX - 1;
								for (int i = FromXP; i >= NextXP; --i)
									yield return new Point(i, FromYP);
							}
						}
						FromX = NextX;
						FromXP = (int)(FromX / 16);
						FromYP = NextYP;
						NextYP = FromYP + 1;
						NextX = FromX + UnitY16.X;
						NextXP = (int)(NextX / 16f);
					}
					NextX = End.X;
					NextXP = (int)(NextX / 16f);
					if (FromYP >= 0 && FromYP < Main.maxTilesY)
					{
						if (GoRight)
						{
							if (FromXP < 0) FromXP = 0;
							if (NextXP >= Main.maxTilesX) NextXP = Main.maxTilesX - 1;
							for (int i = FromXP; i <= NextXP; ++i)
								yield return new Point(i, FromYP);
						}
						else
						{
							if (NextXP < 0) NextXP = 0;
							if (FromXP >= Main.maxTilesX) FromXP = Main.maxTilesX - 1;
							for (int i = FromXP; i >= NextXP; --i)
								yield return new Point(i, FromYP);
						}
					}
				}
				else
				{
					if (EndYP >= Main.maxTilesY) yield break;
					if (EndYP < 0) EndYP =0;
					NextYP = FromYP;
					NextX = FromX + UnitY16.X * (NextYP * 16 - FromY) / 16;
					NextXP = (int)(NextX / 16f);
					while (FromYP > EndYP)
					{
						if (FromYP < Main.maxTilesY)
						{
							if (GoRight)
							{
								if (FromXP < 0) FromXP = 0;
								if (NextXP >= Main.maxTilesX) NextXP = Main.maxTilesX - 1;
								for (int i = FromXP; i <= NextXP; ++i)
									yield return new Point(i, FromYP);
							}
							else
							{
								if (NextXP < 0) NextXP = 0;
								if (FromXP >= Main.maxTilesX) FromXP = Main.maxTilesX - 1;
								for (int i = FromXP; i >= NextXP; --i)
									yield return new Point(i, FromYP);
							}
						}
						FromX = NextX;
						FromXP = (int)(FromX / 16);
						FromYP = NextYP - 1;
						NextYP = FromYP;
						NextX = FromX - UnitY16.X;
						NextXP = (int)(NextX / 16f);
					}
					NextX = End.X;
					NextXP = (int)(NextX / 16f);
					if (FromYP>=0&& FromYP < Main.maxTilesY)
					{
						if (GoRight)
						{
							if (FromXP < 0) FromXP = 0;
							if (NextXP >= Main.maxTilesX) NextXP = Main.maxTilesX - 1;
							for (int i = FromXP; i <= NextXP; ++i)
								yield return new Point(i, FromYP);
						}
						else
						{
							if (NextXP < 0) NextXP = 0;
							if (FromXP >= Main.maxTilesX) FromXP = Main.maxTilesX - 1;
							for (int i = FromXP; i >= NextXP; --i)
								yield return new Point(i, FromYP);
						}
					}
				}
			}
			/// <summary>
			/// 枚举线上的物块，从上往下返回每层的区间范围
			/// </summary>
			/// <returns>(纵坐标，横坐标左端，横坐标右端)</returns>
			public static IEnumerable<(int, int, int)> EnumTileInLineYLR(Vector2 Start, Vector2 End)
			{
				if (((int)(Start.Y / 16)) == (int)(End.Y / 16))
				{
					int Y = (int)(Start.Y / 16);
					if (Y < 1 || Y >= Main.maxTilesY) yield break;
					int L = (int)(Start.X / 16);
					int R = (int)(End.X / 16)+1;
					if (L > R) Terraria.Utils.Swap(ref L, ref R);
					if (L < 1) L = 1;
					if (L >= Main.maxTilesX) L = Main.maxTilesX - 1;
					if (R < 0) R = 0;
					if (R  >= Main.maxTilesX) R = Main.maxTilesX - 1;
					yield return (Y, L, R);
					yield break;
				}
				if (End.Y < Start.Y) Terraria.Utils.Swap(ref End, ref Start);
				//Vector2 Point = Start;
				Vector2 Offset = End - Start;
				Vector2 Unit = Vector2.Normalize(Offset);
				bool GoRight = Unit.X > 0;
				bool GoDown = Unit.Y > 0;
				Vector2 UnitY16 = Unit * (16 / Unit.Y);
				float FromX = Start.X;
				float FromY = Start.Y;
				int FromXP = (int)(FromX / 16);
				int FromYP = (int)(FromY / 16);
				int NextYP;
				float NextX;
				int NextXP;
				int EndYP = (int)(End.Y / 16f);
				if (EndYP < 0) yield break;
				if (EndYP >= Main.maxTilesY) EndYP = Main.maxTilesY - 1;
				NextYP = FromYP + 1;
				NextX = FromX + UnitY16.X * (NextYP * 16 - FromY) / 16;
				NextXP = (int)(NextX / 16f);
				while (FromYP < EndYP)
				{
					if (FromYP >= 0)
					{
						if (GoRight)
						{
							if (FromXP < 0) FromXP = 0;
							if (NextXP >= Main.maxTilesX) NextXP = Main.maxTilesX - 1;
							yield return (FromYP, FromXP, NextXP);
						}
						else
						{
							if (NextXP < 0) NextXP = 0;
							if (FromXP >= Main.maxTilesX) FromXP = Main.maxTilesX - 1;
							yield return (FromYP, NextXP, FromXP);
						}
					}
					FromX = NextX;
					FromXP = (int)(FromX / 16);
					FromYP = NextYP;
					NextYP = FromYP + 1;
					NextX = FromX + UnitY16.X;
					NextXP = (int)(NextX / 16f);
				}
				NextX = End.X;
				NextXP = (int)(NextX / 16f);
				if (FromYP >= 0 && FromYP < Main.maxTilesY)
				{
					if (GoRight)
					{
						if (FromXP < 0) FromXP = 0;
						if (NextXP >= Main.maxTilesX) NextXP = Main.maxTilesX - 1;
						yield return (FromYP, FromXP, NextXP);
					}
					else
					{
						if (NextXP < 0) NextXP = 0;
						if (FromXP >= Main.maxTilesX) FromXP = Main.maxTilesX - 1;
						yield return (FromYP, NextXP, FromXP);
					}
				}
			}
			/// <summary>
			/// 枚举有宽度的线上的物块，从上往下
			/// </summary>
			public static IEnumerable<Point> EnumTilesInWideLine(Vector2 Start, Vector2 End, float Width) {
				Vector2 v = Vector2.Normalize(End - Start).RotatedBy((float)Math.PI/2) * Width/2;
				return EnumTilesInConvexPolygon(Start+v, End+v,End-v,Start-v);
			}
			/// <summary>
			/// 枚举凸多边形内的物块，从上往下
			/// </summary>
			public static IEnumerable<Point> EnumTilesInConvexPolygon(params Vector2[] Points) {
				int MaxY=(int) (Points[0].Y/16f), MinY = (int)(Points[0].Y / 16f), DistanceY=0;
				foreach (var i in Points) {
					MaxY.ToMax((int)(i.Y / 16f));
					MinY.ToMin((int)(i.Y / 16f));
				}
				DistanceY = MaxY - MinY+1;
				List<(int, int)?> YLRs=new List<(int, int)?>(DistanceY);
				for (int i = 0; i < DistanceY; ++i) YLRs.Add(null);
				foreach (var i in EnumPairs(Points)) {
					foreach (var j in EnumTileInLineYLR(i.Item1, i.Item2)) {
						int I = j.Item1 - MinY;
						if (YLRs[I].HasValue)
							YLRs[I] = (Min(j.Item2, YLRs[I].Value.Item1), Max(j.Item3, YLRs[I].Value.Item2));
						else
							YLRs[I] = (j.Item2, j.Item3);
					}
				}
				for (int i = 0; i < DistanceY; ++i) {
					if(YLRs[i].HasValue)
						for (int k = YLRs[i].Value.Item1; k <= YLRs[i].Value.Item2; ++k) 
							yield return new Point( k, i + MinY);
				}
			}
			/// <summary>
			/// 确定点是否在线的上方（世界上线的下方）
			/// </summary>
			public static bool PointAboveLine(Vector2 Point, Vector2 Start, Vector2 End)
			{
				Vector2 Unit = Vector2.Normalize(End - Start);
				Vector2 OffsetPoint = Point - Start;
				Vector2 PointXOnLine = Unit / Unit.X * OffsetPoint.X;
				return OffsetPoint.Y > PointXOnLine.Y;
			}
			/// <summary>
			/// 确定点是否在线的上方（世界上线的下方）
			/// </summary>
			public static bool PointAboveLine(Vector2 Point, Vector2 Line)
			{
				Vector2 Unit = Vector2.Normalize(Line);
				Vector2 OffsetPoint = Point;
				Vector2 PointXOnLine = Unit / Unit.X * OffsetPoint.X;
				return OffsetPoint.Y > PointXOnLine.Y;
			}
			/// <summary>
			/// 枚举被Rect接触的物块Point
			/// </summary>
			public static IEnumerable<Point> EnumTilesInRect(Rectangle rectangle) {
				for (int x = rectangle.Left / 16; x <= rectangle.Right / 16; ++x) {
					for (int y = rectangle.Top / 16; y <= rectangle.Bottom / 16; ++y)
					{
						if (x < 0) x = 0;if (y < 0) y = 0;
						if (x >= Main.maxTilesX) yield break;
						if (y >= Main.maxTilesY) continue;
						yield return new Point(x,y);
					}
				}
			}
			/// <summary>
			/// 枚举圆上的物块
			/// </summary>
			public static IEnumerable<Point> EnumTilesInCircle(Vector2 Pos, float R) {
				Point PosW = (Pos / 16).ToPoint();
				int r = (int)(R) / 16 + 1;
				int i;
				int wL;
				int wR;
				for (i = Math.Max(PosW.Y - r, 0); i < PosW.Y; ++i) {
					if (i >= Main.maxTilesY) yield break;
					float d = Pos.Y - (i * 16 + 16);
					if (R < d) continue;
					float dl = (float)Math.Sqrt(R * R - d * d);
					int dL = (int)((dl + 16) / 16);
					wL = PosW.X - dL;
					wR = PosW.X + dL;
					if (wL < 0) wL = 0;
					if (wR >= Main.maxTilesX) wR = Main.maxTilesX - 1;
					for (int j = wL; j <= wR; ++j) yield return new Point(j, i);
				}
				i = PosW.Y;
				if (i >= Main.maxTilesY) yield break;
				if (i >= 0)
				{
					wL = PosW.X - r;
					wR = PosW.X + r;
					if (wL < 0) wL = 0;
					if (wR >= Main.maxTilesX) wR = Main.maxTilesX - 1;
					for (int j = wL; j <= wR; ++j) yield return new Point(j, i);
				}
				for (i = Math.Max(PosW.Y+1,0); i <= PosW.Y+r; ++i)
				{
					if (i >= Main.maxTilesY) yield break;
					float d = i * 16-Pos.Y;
					if (R < d) break;
					float dl = (float)Math.Sqrt(R * R - d * d);
					int dL = (int)((dl + 16) / 16);
					wL = PosW.X - dL;
					wR = PosW.X + dL;
					if (wL < 0) wL = 0;
					if (wR >= Main.maxTilesX) wR = Main.maxTilesX - 1;
					for (int j = wL; j <= wR; ++j) yield return new Point(j, i);
				}
			}
		}
		/// <summary>
		/// 生成方法
		/// </summary>
		public static class SummonUtils
		{
			///// <summary>
			///// 生成作为弹幕的爆炸
			///// </summary>
			///// <param name="Position">爆炸位置</param>
			///// <param name="radius">爆炸半径</param>
			///// <param name="friendlyDamage">对hostile的npc的伤害</param>
			///// <param name="hostileDamage">对player和友好npc的伤害</param>
			///// <param name="color_">爆炸颜色</param>
			///// <param name="Owner">弹幕的所有者</param>
			///// <param name="npcProj">弹幕是否为npc的弹幕，注意npc的弹幕的所有者为Main.myplayer</param>
			///// <returns>爆炸弹幕的id</returns>
			//public static int SummonProjExplosion(Vector2 Position, float radius, int friendlyDamage, int hostileDamage, Color? color_, int Owner, bool npcProj) => Projectiles.ProjExplosion.SummonProjExplosion(Position, radius, friendlyDamage, hostileDamage, color_, Owner, npcProj);
			///// <summary>
			///// 生成作为弹幕，陷阱的爆炸
			///// </summary>
			///// <param name="Position">爆炸位置</param>
			///// <param name="radius">爆炸半径</param>
			///// <param name="friendlyDamage">对hostile的npc的伤害</param>
			///// <param name="hostileDamage">对player和友好npc的伤害</param>
			///// <param name="color_">爆炸颜色</param>
			///// <returns>爆炸弹幕的id</returns>
			//public static int SummonProjExplosionTrap(Vector2 Position, float radius, int friendlyDamage, int hostileDamage, Color? color_) => Projectiles.ProjExplosion.SummonProjExplosionTrap(Position, radius, friendlyDamage, hostileDamage, color_);
			static SummonUtils()
			{
				//ModTranslation DRS = XDef.Instance.CreateTranslation("KilledBySummonDustExplosion");
				//DRS.SetDefault("was killed by an explosion");
				//DRS.AddTranslation(GameCulture.Chinese, "被炸死了");
				//XDef.Instance.AddTranslation(DRS);
				//DRS = XDef.Instance.CreateTranslation("Killed");
				//DRS.SetDefault("was killed");
				//DRS.AddTranslation(GameCulture.Chinese, "被杀死了");
				//XDef.Instance.AddTranslation(DRS);
			}
			/// <summary>
			/// 对半径中的对象造成伤害，并产生粒子
			/// </summary>
			/// <param name="Position">爆炸位置</param>
			/// <param name="radius">爆炸半径</param>
			/// <param name="friendlyDamage">对hostile的npc的伤害</param>
			/// <param name="hostileDamage">对player和友好npc的伤害</param>
			/// <param name="DustType">粒子类型</param>
			/// <param name="DustCircleNumber">粒子在爆炸圆中的数量</param>
			/// <param name="DustSpreadNumber">粒子在中心扩散的数量</param>
			/// <param name="DustSpeed">扩散的粒子的速度</param>
			/// <param name="DustDo">对每个生成的粒子的操作</param>
			/// <param name="MakeDeathReason">杀死玩家的原因，默认为 player.name + " " + Language.GetTextValue("Mods.XDef.KilledBySummonDustExplosion") </param>
			public static void SummonDustExplosion(Vector2 Position, float radius, int friendlyDamage, int hostileDamage, int DustType, int DustCircleNumber, int DustSpreadNumber, float DustSpeed, Action<Dust> DustDo = null, Func<Player, Terraria.DataStructures.PlayerDeathReason> MakeDeathReason = null)
			{
				for (int i = 0; i < DustSpreadNumber; i++)
				{
					Dust dust = Dust.NewDustPerfect(Position, DustType, Main.rand.NextVector2Circular(DustSpeed, DustSpeed));
					DustDo?.Invoke(dust);
				}
				for (int i = 0; i < DustCircleNumber; i++)
				{
					Vector2 NewPos = Position + Main.rand.NextVector2Circular(radius, radius);
					Dust dust = Dust.NewDustPerfect(NewPos, DustType, Vector2.Zero);
					DustDo?.Invoke(dust);
				}
				if (friendlyDamage > 0 || hostileDamage > 0)
				{
					foreach (var i in Main.npc)
					{
						if (i.active && !i.dontTakeDamage)
						{
							if (!i.friendly && friendlyDamage > 0 && CalculateUtils.CheckAABBvCircleColliding(i.Hitbox, Position, radius)) i.StrikeNPC(friendlyDamage, 0, 0);
							if (i.friendly && !i.dontTakeDamageFromHostiles && hostileDamage > 0 && CalculateUtils.CheckAABBvCircleColliding(i.Hitbox, Position, radius)) i.StrikeNPC(hostileDamage, 0, 0);
						}
					}
				}
				if (hostileDamage > 0)
				{
					if (MakeDeathReason == null)
					{
						MakeDeathReason = (player) => Terraria.DataStructures.PlayerDeathReason.ByCustomReason(player.name + " " + Language.GetTextValue("Mods.XDef.KilledBySummonDustExplosion"));
					}
					foreach (var i in Main.player)
					{
						if (i.active && CalculateUtils.CheckAABBvCircleColliding(i.Hitbox, Position, radius)) i.Hurt(MakeDeathReason(i), hostileDamage, 0);
					}
				}
			}
			/// <summary>
			/// 生成GoreExplosion
			/// </summary>
			//public static Gore NewGoreExplosion(Terraria.DataStructures.IEntitySource source,Vector2 Pos, float R, Color? color = null)=>Gores.GoreExplosion.NewGoreExplosion(source,Pos, R, color);
		}
		/// <summary>
		/// 在范围内
		/// </summary>
		public static bool InRange(this float obj, float value = 0, float range = float.Epsilon)
		{
			float d = (float)Math.Abs(obj - value);
			return d < range;
		}
		/// <summary>
		/// 在范围内
		/// </summary>
		public static bool InRange(this double obj, double value = 0, double range = float.Epsilon)
		{
			double d = (double)Math.Abs(obj - value);
			return d < range;
		}
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
		public static T Max<T>(params T[] vs)
			where T:IComparable
		{
			T V = vs[0];
			foreach (var i in vs) if (i.CompareTo(V)>0) V = i;
			return V;
		}
		public static T Min<T>(params T[] vs)
			where T : IComparable
		{
			T V = vs[0];
			foreach (var i in vs) if (i.CompareTo(V) < 0) V = i;
			return V;
		}
		public static T SecondMax<T>(params T[] vs)
			where T:IComparable
		{
			T MaxV = vs[0];
			T MaxV2 = vs[1];
			foreach (var i in vs) {
				if (i.CompareTo(MaxV) > 0)
				{
					MaxV2 = MaxV;
					MaxV = i;
				}
				else if (i.CompareTo(MaxV2) > 0) {
					MaxV2 = i;
				}
			}
			return MaxV2;
		}
		public static T SecondMin<T>(params T[] vs)
			where T : IComparable
		{
			T V = vs[0];
			T V2 = vs[1];
			foreach (var i in vs)
			{
				if (i.CompareTo(V) < 0)
				{
					V2 = V;
					V = i;
				}
				else if (i.CompareTo(V2) < 0)
				{
					V2 = i;
				}
			}
			return V2;
		}
		public static bool ToMax<T>(this ref T v, params T[] vs)
			where T :struct, IComparable
		{
			bool Changed = false;
			foreach (var i in vs) {
				if (i.CompareTo(v) > 0) { v = i;Changed = true; }
			}
			return Changed;
		}
		public static bool ToMin<T>(this ref T v, params T[] vs)
			where T : struct, IComparable
		{
			bool Changed = false;
			foreach (var i in vs)
			{
				if (i.CompareTo(v) < 0) { v = i; Changed = true; }
			}
			return Changed;
		}
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
		/// <summary>
		/// 获取Player的位置的IGetValue
		/// </summary>
		public static IGetValue<Vector2> GetPlayerCenter(int id, Vector2 Offset = default)
		{
			return (GetValue<Vector2>)(() => Main.player[id].Center + Offset);
		}
		/// <summary>
		/// 获取Player的位置的IGetValue
		/// </summary>
		public static IGetValue<Vector2> GetPlayerCenter(Player player, Vector2 Offset = default)
		{
			return (GetValue<Vector2>)(() => player.Center + Offset);
		}
		/// <summary>
		/// 获取NPC的位置的IGetValue
		/// </summary>
		public static IGetValue<Vector2> GetNPCCenter(int id, Vector2 Offset = default)
		{
			return (GetValue<Vector2>)(() => Main.npc[id].Center + Offset);
		}
		/// <summary>
		/// 获取NPC的位置的IGetValue
		/// </summary>
		public static IGetValue<Vector2> GetNPCCenter(NPC npc, Vector2 Offset = default)
		{
			return (GetValue<Vector2>)(() => npc.Center + Offset);
		}
		/// <summary>
		/// 获取Proj的位置的IGetValue
		/// </summary>
		public static IGetValue<Vector2> GetProjCenter(int id, Vector2 Offset = default)
		{
			return (GetValue<Vector2>)(() => Main.projectile[id].Center + Offset);
		}
		/// <summary>
		/// 获取Proj的位置的IGetValue
		/// </summary>
		public static IGetValue<Vector2> GetProjCenter(Projectile projectile, Vector2 Offset = default)
		{
			return (GetValue<Vector2>)(() => projectile.Center + Offset);
		}
		/// <summary>
		/// 获取NPC的目标位置的IGetValue
		/// </summary>
		public static IGetValue<Vector2> GetNPCTargetCenter(int id, IGetValue<Vector2> Default, Vector2 Offset = default)
		{
			return (GetValue<Vector2>)(() =>
			{
				UnifiedTarget target = new UnifiedTarget() { NPCTarget = Main.npc[id].target };
				if (target.IsPlayer) return target.player.Center + Offset;
				else if (target.IsNPC) return target.npc.Center + Offset;
				else return Default.Value;
			});
		}
		/// <summary>
		/// 获取NPC的目标位置的IGetValue
		/// </summary>
		public static IGetValue<Vector2> GetNPCTargetCenter(NPC npc, IGetValue<Vector2> Default, Vector2 Offset = default)
		{
			return (GetValue<Vector2>)(() =>
			{
				UnifiedTarget target = new UnifiedTarget() { NPCTarget = npc.target };
				if (target.IsPlayer) return target.player.Center + Offset;
				else if (target.IsNPC) return target.npc.Center + Offset;
				else return Default.Value;
			});
		}
		/// <summary>
		/// 获取entity的位置的IGetValue
		/// </summary>
		public static IGetValue<Vector2> GetEntityCenter(Entity entity, Vector2 Offset = default)
		{
			return (GetValue<Vector2>)(() => entity.Center + Offset);
		}
		/// <summary>
		/// 获取表示NPC的目标的UnifiedTarget
		/// </summary>
		public static IRefValue<UnifiedTarget> GetNPCTarget(NPC npc) => new RefByDelegate<UnifiedTarget>(
			() => new UnifiedTarget() { NPCTarget = npc.target },
			(v) => npc.target = v.NPCTarget
			);
		/// <summary>
		/// 获取表示NPC的目标的UnifiedTarget
		/// </summary>
		public static IRefValue<UnifiedTarget> GetNPCTarget(int id) => new RefByDelegate<UnifiedTarget>(
			() => new UnifiedTarget() { NPCTarget = Main.npc[id].target },
			(v) => Main.npc[id].target = v.NPCTarget
			);
		/// <summary>
		/// 设置Vector2的长度
		/// </summary>
		public static Vector2 SetLength(this Vector2 vector, float Length)
		{
			return vector *= (Length / vector.Length());
		}
		/// <summary>
		/// 保持entity在世界中（否则会直接被active=false或报错）
		/// </summary>
		/// <param name="entity"></param>
		public static void SetInWorld(Entity entity) {

			if (entity.position.Y + entity.velocity.Y < 16)
				entity.position.Y = 16 - entity.velocity.Y;

			if (entity.position.X + entity.velocity.X < 16)
				entity.position.X = 16 - entity.velocity.X;

			if (entity.position.Y+entity.height + entity.velocity.Y >Main.maxTilesY*16- 16)
				entity.position.Y = Main.maxTilesY * 16 - 16 - entity.velocity.Y - entity.height;

			if (entity.position.X + entity.width + entity.velocity.X > Main.maxTilesX * 16 - 16)
				entity.position.X = Main.maxTilesX * 16 - 16 - entity.velocity.X -entity.width;
		}
		/// <summary>
		/// 从相对位置到世界位置
		/// </summary>
		public static Vector2 OffsetToWorld(this Vector2 offset, Vector2 center, float rotation, int direction) { 
			return (offset * new Vector2(direction, 1)).RotatedBy(rotation) + center;
		}
		/// <summary>
		/// 从相对entity的中心的位置到世界位置
		/// </summary>
		public static Vector2 OffsetToWorld(this Vector2 offset, NPC entity) {
			return (offset * new Vector2(entity.direction, 1)).RotatedBy(entity.rotation) + entity.Center;
		}
		/// <summary>
		/// 从相对entity的中心的位置到世界位置
		/// </summary>
		public static Vector2 OffsetToWorld(this Vector2 offset, Projectile entity)
		{
			return (offset * new Vector2(entity.direction, 1)).RotatedBy(entity.rotation) + entity.Center;
		}
		/// <summary>
		/// 遍历枚举全部相邻两元素组成的二元组，包括尾和首
		/// 每个元素都会出现两次
		/// </summary>
		public static IEnumerable<(T,T)> EnumPairs<T>(this IEnumerable<T> ts) {
			List < T > l= ts.ToList();
			for (int i = 0; i < l.Count - 1; ++i) yield return (l[i],l[i+1]);
			yield return (l[^1], l[0]);
		}
		/// <summary>
		/// 画激光
		/// </summary>
		public static void DrawLaser(SpriteBatch spriteBatch, Vector2 Start, Vector2 End, float WidthScale, Texture2DCutted Body, Texture2DCutted Head, Texture2DCutted Tail,Func<Vector2,float,Vector2,bool > DrawBody = null, Func<Vector2, float, Vector2, bool> DrawHead = null, Func<Vector2, float, Vector2, bool> DrawTail=null) {
			Vector2 Scale = new Vector2(1f, WidthScale);
			float HeadLength = Head.RealSize().X * 0.9f;
			float BodyLength = Body.RealSize().X * 0.9f;
			float TailLength = Tail.RealSize().X * 0.9f;
			Vector2 O;
			Vector2 Offset = End - Start;
			float Distance = Offset.Length();
			float Rotation = Offset.ToRotation();
			Color color = Color.White;
			for (float i = HeadLength / 2f; i < Distance - TailLength / 2f; i += BodyLength)
			{
				O = Start + new Vector2(i, 0).RotatedBy(Rotation);
				if (DrawBody == null || DrawBody(O, Rotation, Scale))
					Body.Draw(spriteBatch, O - Main.screenPosition, color, Rotation, Scale, SpriteEffects.None);
			}
			O = Start + new Vector2(Distance - TailLength / 2f, 0).RotatedBy(Rotation);
			if (DrawBody == null || DrawBody(O, Rotation, Scale))
				Body.Draw(spriteBatch, O - Main.screenPosition, color, Rotation, Scale, SpriteEffects.None);
			if (DrawTail == null || DrawTail(Start, Rotation, Scale))
				Tail.Draw(spriteBatch, Start - Main.screenPosition, color, Rotation, Scale, SpriteEffects.None);
			if (DrawHead == null || DrawHead(End, Rotation, Scale))
				Head.Draw(spriteBatch, End - Main.screenPosition, color, Rotation, Scale, SpriteEffects.None);
		}
		/// <summary>
		/// 玩家是否在使用对物品的右键
		/// </summary>
#pragma warning disable IDE1006 // 命名样式
		public static bool controlUseItemRight2(this Player player) => player.selectedItem != 58&& player.controlUseTile && !player.tileInteractionHappened && player.releaseUseItem && !player.controlUseItem && !player.mouseInterface && !Terraria.Graphics.Capture.CaptureManager.Instance.Active && !Main.HoveringOverAnNPC && !Main.SmartInteractShowingGenuine;
		/// <summary>
		/// 玩家是否在使用对物品的右键
		/// </summary>
		public static bool controlUseItemRight(this Player player) => player.selectedItem != 58&& player.controlUseTile && !player.tileInteractionHappened && !player.mouseInterface && !Terraria.Graphics.Capture.CaptureManager.Instance.Active && !Main.HoveringOverAnNPC && !Main.SmartInteractShowingGenuine;
#pragma warning restore IDE1006 // 命名样式
		#region OldAction
		/// <summary>
		/// 挖墙
		/// </summary>
		public static int HitWall(Player player, Point point, int hammerPower) {
			HitTile hitTile = player.hitTile;
			int tileId = hitTile.HitObject(point.X, point.Y, 2);
			hammerPower = (int)((float)hammerPower * 1.5f);
			if (hitTile.AddDamage(tileId, hammerPower) >= 100)
			{
				hitTile.Clear(tileId);
				WorldGen.KillWall(point.X, point.Y);
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 2, point.X, point.Y);
				}
			}
			else
			{
				WorldGen.KillWall(point.X, point.Y, fail: true);
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 2, point.X, point.Y, 1f);
				}
			}
			if (hammerPower != 0)
			{
				hitTile.Prune();
			}
			return tileId;
		}
		///// <summary>
		///// 判断是否暴击
		///// </summary>
		//public static Func<bool> GetCrit(Player player,int crit,DamageClass damageClass,Func<bool> Other=null) {
		//	return () => (Other?.Invoke()??false) 
		//	|| (Main.rand.Next(1, 101) / 100 <= crit)
		//	|| (melee&&(Main.rand.Next(1,101)/100<=player.meleeCrit))
		//	|| (ranged && (Main.rand.Next(1, 101) / 100 <= player.rangedCrit))
		//	|| (magic && (Main.rand.Next(1, 101) / 100 <= player.magicCrit))
		//	|| (thrown && (Main.rand.Next(1, 101) / 100 <= player.thrownCrit));
		//}
		///// <summary>
		///// 获取伤害
		///// </summary>
		//public static int GetDamage(Player player, int damage, bool melee = false, bool ranged = false, bool magic = false, bool thrown = false, bool summon = false) {
		//	float add = player.allDamage;
		//	float mult = player.allDamageMult;
		//	if (melee)
		//	{
		//		add += player.meleeDamage - 1f;
		//		mult *= player.meleeDamageMult;
		//	}
		//	if (ranged)
		//	{
		//		add += player.rangedDamage - 1f;
		//		mult *= player.rangedDamageMult;
		//	}
		//	if (magic)
		//	{
		//		add += player.magicDamage - 1f;
		//		mult *= player.magicDamageMult;
		//	}
		//	if (summon)
		//	{
		//		add += player.minionDamage - 1f;
		//		mult *= player.minionDamageMult;
		//	}
		//	if (thrown)
		//	{
		//		add += player.thrownDamage - 1f;
		//		mult *= player.thrownDamageMult;
		//	}
		//	damage = (int)((float)damage * add * mult + 5E-06f);
		//	return Math.Max(0, damage);
		//}
		///// <summary>
		///// 判断物品是否可以对NPC造成伤害，不考虑是否碰撞
		///// </summary>
		//public static bool ItemCanHitNPC(Item item, Player player, NPC npc) { 
		//	return 
		//		item.damage > 0 &&
		//		npc.active && !npc.dontTakeDamage &&
		//		(ItemLoader.CanHitNPC(item, player, npc) ?? true) &&
		//		(NPCLoader.CanBeHitByItem(npc, player, item) ?? true) &&
		//		(PlayerLoader.CanHitNPC(player, item, npc) ?? true) &&
		//		(!npc.friendly || (npc.type == NPCID.Guide && player.killGuide) || (npc.type == NPCID.Clothier && player.killClothier));
		//}
		///// <summary>
		///// 物品对NPC造成伤害，无无敌帧，有击退，不考虑是否可以
		///// </summary>
		///// <returns>伤害</returns>
		//public static int ItemHitNPC(Item item,Player player, NPC npc) {
		//	#region crit
		//	float critChance = 0;
		//	critChance = player.GetCritChance(item.DamageType);
		//	//if (item.melee)
		//	//{
		//	//	critChance = player.meleeCrit;
		//	//}
		//	//else if (item.ranged)
		//	//{
		//	//	critChance = player.rangedCrit;
		//	//}
		//	//else if (item.magic)
		//	//{
		//	//	critChance = player.magicCrit;
		//	//}
		//	//else if (item.thrown)
		//	//{
		//	//	critChance = player.thrownCrit;
		//	//}
		//	//else if (!item.summon)
		//	//{
		//	//	critChance = item.crit;
		//	//}
		//	ItemLoader.GetWeaponCrit(item, player, ref critChance);
		//	PlayerLoader.GetWeaponCrit(player, item, ref critChance);
		//	bool crit= (critChance >= 100 || Main.rand.Next(1, 101) <= critChance);
		//	#endregion
		//	int damage = player.GetWeaponDamage(item);
		//	#region Banner
		//	int Banner = Item.NPCtoBanner(npc.BannerID());
		//	if (Banner > 0 && player.NPCBannerBuff[Banner])
		//	{
		//		damage = ((!Main.expertMode) ? ((int)((float)damage * ItemID.Sets.BannerStrength[Item.BannerToItem(Banner)].NormalDamageDealt)) : ((int)((float)damage * ItemID.Sets.BannerStrength[Item.BannerToItem(Banner)].ExpertDamageDealt)));
		//	}
		//	#endregion
		//	damage = Main.DamageVar(damage);
		//	#region knockBack
		//	float knockBack = item.knockBack;
		//	float knockBackMul = 1f;
		//	if (player. kbGlove)
		//	{
		//		knockBackMul += 1f;
		//	}
		//	if (player.kbBuff)
		//	{
		//		knockBackMul += 0.5f;
		//	}
		//	knockBack *= knockBackMul;
		//	ItemLoader.GetWeaponKnockback(item, player, ref knockBack);
		//	PlayerHooks.GetWeaponKnockback(player, item, ref knockBack);
		//	#endregion
		//	#region Modify
		//	ItemLoader.ModifyHitNPC(item, player, npc, ref damage, ref knockBack, ref crit);
		//	NPCLoader.ModifyHitByItem(npc, player, item, ref damage, ref knockBack, ref crit);
		//	PlayerHooks.ModifyHitNPC(player, item, npc, ref damage, ref knockBack, ref crit);
		//	#endregion
		//	player.StatusNPC(item.type, npc.whoAmI);
		//	player.OnHit(npc.Center.X, npc.Center.Y, npc);
		//	int realdmg = (int)npc.StrikeNPC(damage, knockBack, player.direction, crit);
		//	#region meleeEnchant
		//	if (player.meleeEnchant == 7)
		//	{
		//		Projectile.NewProjectile(npc.Center, npc.velocity, ProjectileID.ConfettiMelee, 0, 0f,player. whoAmI);
		//	}
		//	#endregion
		//	#region coins
		//	if (npc.value > 0f &&player.coins && Main.rand.NextBool(5))
		//	{
		//		int type11 = 71;
		//		if (Main.rand.NextBool(10))
		//		{
		//			type11 = 72;
		//		}
		//		if (Main.rand.NextBool(100))
		//		{
		//			type11 = 73;
		//		}
		//		int num318 = Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, type11);
		//		Main.item[num318].stack = Main.rand.Next(1, 11);
		//		Main.item[num318].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
		//		Main.item[num318].velocity.X = (float)Main.rand.Next(10, 31) * 0.2f * (float)player. direction;
		//		if (Main.netMode == NetmodeID.MultiplayerClient)
		//		{
		//			NetMessage.SendData(MessageID.SyncItem, -1, -1, null, num318);
		//		}
		//	}
		//	#endregion
		//	#region On
		//	ItemLoader.OnHitNPC(item, player, npc, realdmg, knockBack, crit);
		//	NPCLoader.OnHitByItem(npc, player, item, realdmg, knockBack, crit);
		//	PlayerHooks.OnHitNPC(player, item, npc, realdmg, knockBack, crit);
		//	#endregion
		//	#region NetMessage
		//	if (Main.netMode != NetmodeID.SinglePlayer)
		//	{
		//		if (crit)
		//		{
		//			NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI, damage, knockBack, player. direction, 1);
		//		}
		//		else
		//		{
		//			NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI, damage, knockBack, player.direction);
		//		}
		//	}
		//	#endregion
		//	#region DreamCatcher
		//	if (player. accDreamCatcher)
		//	{
		//		player.addDPS(realdmg);
		//	}
		//	#endregion
		//	return realdmg;
		//}
		///// <summary>
		///// npc对target造成伤害，不考虑是否可以
		///// </summary>
		//public static double NPCHitNPC(NPC npc, NPC target) {
		//	int damage = npc.damage;
		//	float knockback = 0;
		//	int HitDirection = 1;
		//	if (target.position.X + target.width / 2 > npc.position.X + npc.width / 2)
		//	{
		//		HitDirection = -1;
		//	}
		//	bool crit = false;
		//	NPCLoader.ModifyHitNPC(npc, target, ref damage, ref knockback, ref crit);
		//	double realdmg = target.StrikeNPCNoInteraction(damage,knockback,HitDirection,crit);
		//	if (Main.netMode != NetmodeID.SinglePlayer)
		//	{
		//		NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI, damage, knockback, HitDirection);
		//	}
		//	target.netUpdate = true;
		//	npc.immune[255] = 30;
		//	NPCLoader.OnHitNPC(npc, target, damage, knockback, crit);
		//	return realdmg;
		//}
		///// <summary>
		///// 判断projectile能否对npc造成伤害，不考虑碰撞
		///// </summary>
		//public static bool ProjectileCanHitNPC(Projectile projectile, NPC npc) =>
		//	((npc.active && !npc.dontTakeDamage &&
		//	(!projectile.usesLocalNPCImmunity || projectile.localNPCImmunity[npc.whoAmI] == 0) &&
		//	(!projectile.usesIDStaticNPCImmunity || !Projectile.IsNPCImmune(projectile.type, npc.whoAmI)) &&
		//	(ProjectileLoader.CanHitNPC(projectile, npc) ?? true) &&
		//	(NPCLoader.CanBeHitByProjectile(npc, projectile) ?? true) &&
		//	(PlayerHooks.CanHitNPCWithProj(projectile, npc) ?? true) &&
		//	((projectile.friendly && !npc.friendly) || (projectile.hostile && npc.friendly)) &&
		//	(!npc.trapImmune || !projectile.trap)&& (!npc.immortal || !projectile.npcProj)) || projectile.friendly && (!npc.friendly || projectile.type == 318 || npc.type == NPCID.Guide && projectile.owner < 255 && Main.player[projectile.owner].killGuide || npc.type == NPCID.Clothier && projectile.owner < 255 && Main.player[projectile.owner].killClothier) || projectile.hostile && npc.friendly && !npc.dontTakeDamageFromHostiles) && (projectile.owner < 0 || npc.immune[projectile.owner] == 0 || projectile.maxPenetrate == 1)
		//	;
		///// <summary>
		///// projectile对npc造成伤害，不考虑是否可以
		///// </summary>
		//public static int ProjectileHitNPC(Projectile projectile, NPC npc) {
		//	Player player = Main.player[projectile.owner];
		//	int damage = projectile.damage;
		//	damage = Main.DamageVar(damage);
		//	if (projectile.trap && NPCID.Sets.BelongsToInvasionOldOnesArmy[npc.type])
		//	{
		//		damage /= 2;
		//	}
		//	#region Banner
		//	int Banner = Item.NPCtoBanner(npc.BannerID());
		//	if (Banner > 0 && player.NPCBannerBuff[Banner])
		//	{
		//		damage = ((!Main.expertMode) ? ((int)((float)damage * ItemID.Sets.BannerStrength[Item.BannerToItem(Banner)].NormalDamageDealt)) : ((int)((float)damage * ItemID.Sets.BannerStrength[Item.BannerToItem(Banner)].ExpertDamageDealt)));
		//	}
		//	#endregion
		//	float knockBack = projectile.knockBack;
		//	int hitDirection = projectile.direction;
		//	bool playerProj = !projectile.npcProj && !projectile.trap;
		//	bool crit = playerProj&& GetCrit(player,0,projectile.melee,projectile.ranged,projectile.magic,projectile.thrown)();
		//	#region Modify
		//	ProjectileLoader.ModifyHitNPC(projectile, npc, ref damage, ref knockBack, ref crit, ref hitDirection);
		//	NPCLoader.ModifyHitByProjectile(npc, projectile, ref damage, ref knockBack, ref crit, ref hitDirection);
		//	PlayerHooks.ModifyHitNPCWithProj(projectile, npc, ref damage, ref knockBack, ref crit, ref hitDirection);
		//	#endregion
		//	projectile.StatusNPC(npc.whoAmI);
		//	if(playerProj) player.OnHit(npc.Center.X, npc.Center.Y, npc);
		//	int realdmg = (!playerProj) ? ((int)npc.StrikeNPCNoInteraction(damage, knockBack, hitDirection, crit)) : ((int)npc.StrikeNPC(damage, knockBack, hitDirection, crit));
		//	#region DreamCatcher
		//	if (player.accDreamCatcher)
		//	{
		//		player.addDPS(realdmg);
		//	}
		//	#endregion
		//	#region meleeEnchant
		//	if (playerProj&&projectile.melee&& player.meleeEnchant == 7)
		//	{
		//		Projectile.NewProjectile(npc.Center, npc.velocity, ProjectileID.ConfettiMelee, 0, 0f, player.whoAmI);
		//	}
		//	#endregion
		//	#region NetMessage
		//	if (Main.netMode != NetmodeID.SinglePlayer)
		//	{
		//		if (crit)
		//		{
		//			NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI, damage, knockBack, player.direction, 1);
		//		}
		//		else
		//		{
		//			NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI, damage, knockBack, player.direction);
		//		}
		//	}
		//	#endregion
		//	#region Immunity
		//	if (projectile.usesIDStaticNPCImmunity)
		//	{
		//		npc.immune[projectile.owner] = 0;
		//		Projectile.perIDStaticNPCImmunity[projectile.type][npc.whoAmI] = (uint)((int)Main.GameUpdateCount + projectile.idStaticNPCHitCooldown);
		//	}
		//	else if (projectile.usesLocalNPCImmunity && projectile.localNPCHitCooldown != -2)
		//	{
		//		npc.immune[projectile.owner] = 0;
		//		projectile.localNPCImmunity[npc.whoAmI] = projectile.localNPCHitCooldown;
		//	}
		//	else if (projectile.penetrate != 1)
		//	{
		//		npc.immune[projectile.owner] = 10;
		//	}
		//	#endregion
		//	#region on
		//	ProjectileLoader.OnHitNPC(projectile, npc, realdmg, knockBack, crit);
		//	NPCLoader.OnHitByProjectile(npc, projectile, realdmg, knockBack, crit);
		//	PlayerHooks.OnHitNPCWithProj(projectile, npc, realdmg, knockBack, crit);
		//	#endregion
		//	projectile.numHits++;
		//	return realdmg;
		//}
		///// <summary>
		///// 判断projectile能否对player造成伤害，不考虑碰撞
		///// </summary>
		///// <param name="projectile"></param>
		///// <param name="player"></param>
		///// <returns></returns>
		//public static bool ProjectileCanHitPlayer(Projectile projectile, Player player) => projectile.hostile && PlayerCanUse(player) && !player.immune &&
		//	(ProjectileLoader.CanHitPlayer(projectile, player) && PlayerHooks.CanBeHitByProjectile(player, projectile));
		///// <summary>
		///// projectile对player造成伤害，不考虑是否可以
		///// </summary>
		//public static int ProjectileHitPlayer(Projectile projectile, Player player) {
		//	int hitDirection = ((!(player.position.X + (float)(player.width / 2) < projectile.position.X + (float)(projectile.width / 2))) ? 1 : (-1));
		//	int damage = Main.DamageVar(projectile.damage);
		//	bool crit = false;
		//	ProjectileLoader.ModifyHitPlayer(projectile, player, ref damage, ref crit);
		//	PlayerHooks.ModifyHitByProjectile(player, projectile, ref damage, ref crit);
		//	if (!player.immune)
		//	{
		//		projectile.StatusPlayer(player.whoAmI);
		//	}
		//	if (player.resistCold && projectile.coldDamage)
		//	{
		//		damage = (int)((float)damage * 0.7f);
		//	}
		//	if (Main.expertMode)
		//	{
		//		damage = (int)((float)damage * Main.expertDamage);
		//	}
		//	int realdmg = (int)player.Hurt(Terraria.DataStructures.PlayerDeathReason.ByProjectile(-1, projectile.whoAmI), damage * 2, hitDirection, pvp: false, quiet: false, Crit: false, projectile.modProjectile?.cooldownSlot??-1);
		//	if (projectile.trap)
		//	{
		//		player.trapDebuffSource = true;
		//		if (player.dead)
		//		{
		//			Terraria.GameContent.Achievements. AchievementsHelper.HandleSpecialEvent(player, 4);
		//		}
		//	}
		//	ProjectileLoader.OnHitPlayer(projectile, player, realdmg, crit);
		//	PlayerHooks.OnHitByProjectile(player, projectile, realdmg, crit);
		//	return realdmg;
		//}
		#endregion
		public enum RocketType {
			Rocket,
			Grenade,
			Mine,
			Snowman
		}
		public static int GetAmmoShootType(this Item item, RocketType SpecialRocketType=RocketType.Rocket) {
			int type = item.shoot;
			if (item.ammo == AmmoID.Solution)
				type += ProjectileID.PureSpray;
			if (item.ammo == AmmoID.Rocket)
			{
				switch (SpecialRocketType) {
					case RocketType.Grenade: {
							AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[758].TryGetValue(item.type, out type);
							break;
						}
					case RocketType.Rocket:{
							AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[759].TryGetValue(item.type, out type);
							break;
						}
					case RocketType.Mine: {
							AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[760].TryGetValue(item.type, out type);
							break;
						}
					case RocketType.Snowman: {
							AmmoID.Sets.SpecificLauncherAmmoProjectileMatches[1946].TryGetValue(item.type, out type);
							break;
						}
					default:
						type += ProjectileID.RocketI;
						break;
				}
			}
			return type;
		}
		public static IEnumerable<bool> DecomposeBin(long i) {
			while (i != 0) {
				yield return (i & 0x1) == 1;
				i >>= 1;
			}
		}

		public static string Repeat(string str, int count) {
			if (count <= 0) return "";
			string res = "";
			foreach (var i in DecomposeBin(count)) {
				if (i) res += str;
				str += str;
			}
			return res;
		}

		public static bool ExistNewPlayer() {
			if (Main.netMode == NetmodeID.Server)
			{
				foreach (var i in Netplay.Clients)
				{
					if (i != null && i.IsActive && i.State == 3)
					{
						return true;//存在需要同步的端
					}
				}
			}
			return false;
		}
	}
}
