using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Elements;
namespace XLibrary.UIElements
{
	/// <summary>
	/// 操作可拖动UI
	/// uI.Append(uIDrag);
	/// </summary>
	public class UIDrag:UIElement
	{

		/// <summary>
		/// 相对位置
		/// </summary>
		public Vector2 Offset;
		/// <summary>
		/// 是否正在拖动
		/// </summary>
		public bool Dragging;
		/// <summary>
		/// 是否启用，不受Activate影响
		/// </summary>
		public bool Active {
			get => active;
			set {
				if (value && !active) { 
					if (leftMouse) AddEventLeft();
					if (rightMouse) AddEventRight();
					if (middleMouse) AddEventMiddle();
				}
				if (!value && active) {
					if (leftMouse) DelEventLeft();
					if (rightMouse) DelEventRight();
					if (middleMouse) DelEventMiddle();
				}
				active = value;
			}
		}
		private bool active;
		private bool leftMouse = true;
		/// <summary>
		/// 是否启用左键
		/// </summary>
		public bool LeftMouse {
			get => leftMouse;
			set {
				if (active) { 
					if(value&&!leftMouse) AddEventLeft();
					if (!value && leftMouse) DelEventLeft();
				}
				leftMouse = value;
			}
		}
		private bool rightMouse = false;
		/// <summary>
		/// 是否启用右键
		/// </summary>
		public bool RightMouse
		{
			get => rightMouse;
			set
			{
				if (active)
				{
					if (value && !rightMouse) AddEventRight();
					if (!value && rightMouse) DelEventRight();
				}
				rightMouse = value;
			}
		}
		private bool middleMouse = false;
		/// <summary>
		/// 是否启用中键
		/// </summary>
		public bool MiddleMouse
		{
			get => middleMouse;
			set
			{
				if (active)
				{
					if (value && !middleMouse) AddEventMiddle();
					if (!value && middleMouse) DelEventMiddle();
				}
				middleMouse = value;
			}
		}
		/// <summary>
		/// 即使按到这些元素也启用拖拽
		/// </summary>
		public List<UIElement> ThroughElements;
		/// <summary>
		/// 无论按到哪些元素都启用拖拽
		/// </summary>
		public bool ThroughAll = false;
		private void AddEventLeft()
		{
			Parent.OnMouseDown += DragStart;
			Parent.OnMouseUp += DragEnd;
		}
		private void DelEventLeft()
		{
			Parent.OnMouseDown -= DragStart;
			Parent.OnMouseUp -= DragEnd;
		}
		private void AddEventRight()
		{
			Parent.OnRightMouseDown += DragStart;
			Parent.OnRightMouseUp += DragEnd;
		}
		private void DelEventRight()
		{
			Parent.OnRightMouseDown -= DragStart;
			Parent.OnRightMouseUp -= DragEnd;
		}
		private void AddEventMiddle()
		{
			Parent.OnMiddleMouseDown += DragStart;
			Parent.OnMiddleMouseUp += DragEnd;
		}
		private void DelEventMiddle()
		{
			Parent.OnMiddleMouseDown -= DragStart;
			Parent.OnMiddleMouseUp -= DragEnd;
		}
		private void DragStart(UIMouseEvent evt, UIElement listeningElement)
		{
			if (listeningElement != Parent) return;
			if (Dragging) return;
			if (!ThroughAll) {
				UIElement uIElement = Parent.GetElementAt(evt.MousePosition);
				if (uIElement != Parent)
					if (ThroughElements == null || !ThroughElements.Contains(uIElement))
						return;
			}
			Offset = new Vector2(evt.MousePosition.X - Parent.Left.Pixels, evt.MousePosition.Y - Parent.Top.Pixels);
			Dragging = true;
		}
		private void DragEnd(UIMouseEvent evt, UIElement listeningElement)
		{
			if (listeningElement != Parent) return;
			if (!Dragging) return;
			if (!ThroughAll)
			{
				UIElement uIElement = Parent.GetElementAt(evt.MousePosition);
				if (uIElement != Parent)
					if (ThroughElements == null || !ThroughElements.Contains(uIElement))
						return;
			}
			Vector2 end = evt.MousePosition;
			Dragging = false;
			Parent.Left.Set(end.X - Offset.X, Parent.Left.Percent);
			Parent.Top.Set(end.Y - Offset.Y, Parent.Top.Percent);
			Parent.Recalculate();
		}
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
		public override void Update(GameTime gameTime)
		{
			if (Dragging)
			{
				Parent.Left.Set(Main.mouseX - Offset.X, Parent.Left.Percent); // Main.MouseScreen.X and Main.mouseX are the same.
				Parent.Top.Set(Main.mouseY - Offset.Y, Parent.Top.Percent);
				Parent.Recalculate();
			}
			base.Update(gameTime);
		}
		public override void OnInitialize()
		{
			//this.u
		}
		//private void DragMoving(UIMouseEvent evt, UIElement listeningElement)
		//{

		//	if (Dragging)
		//	{
		//		Parent.Left.Set(evt.MousePosition.X - Offset.X, Left.Percent); // Main.MouseScreen.X and Main.mouseX are the same.
		//		Parent.Top.Set(evt.MousePosition.Y - Offset.Y, Top.Percent);
		//		Parent.Recalculate();
		//	}
		//}
		/// <summary>
		/// 为uI生成UIDrag 穿透右键移动
		/// </summary>
		/// <param name="uI"></param>
		public static void SetRightDrag(UIElement uI) {
			UIDrag drag = new UIDrag();
			uI.Append(drag);
			drag.LeftMouse = false;
			drag.RightMouse = true;
			drag.ThroughAll = true;
			drag.Active = true;
		}
		/// <summary>
		/// 为uI生成UIDrag 穿透中键移动
		/// </summary>
		/// <param name="uI"></param>
		public static void SetMiddleDrag(UIElement uI)
		{
			UIDrag drag = new UIDrag();
			uI.Append(drag);
			drag.LeftMouse = false;
			drag.MiddleMouse = true;
			drag.ThroughAll = true;
			drag.Active = true;
		}
	}
}
