using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
using static System.Net.Mime.MediaTypeNames;

namespace XLibrary.UIElements
{
	/// <summary>
	/// 开关
	/// </summary>
	public class UISwitch: UIImageBottomEX, IGetValue<bool>,ISetValue<bool>,IRefValue<bool>
	{
		/// <summary>
		/// 开关图形
		/// </summary>
		public static StaticRefWithF<Texture2D> BottomTexture = new StaticRefWithF<Texture2D>(() => ModContent.Request<Texture2D>("Terraria/Images/UI/Settings_Toggle").Value);
		/// <summary>
		/// 开关图形
		/// </summary>
		public static Texture2DCutted BottomTexture0 => new Texture2DCutted(BottomTexture.Value, new Rectangle(0, 0, 13, 13), new Vector2(0, 0));
		/// <summary>
		/// 开关图形
		/// </summary>
		public static Texture2DCutted BottomTexture1 => new Texture2DCutted(BottomTexture.Value, new Rectangle(15, 0, 13, 13), new Vector2(0, 0));
		/// <summary>
		/// 被控制的值的IGetSetValue
		/// </summary>
		public IRefValue<bool> value;
		/// <summary>
		/// 被控制的值
		/// </summary>
		public bool Value {
			get => value.Value;
			set => this.value.Value = value;
		}
		/// <summary>
		/// 
		/// </summary>
		public UISwitch(IRefValue<bool> value,IGetValue<string> HoverString):base(new GetByDelegate<Texture2DCutted>(()=>value.Value? BottomTexture1: BottomTexture0), HoverString) {
			this.value = value;
			Top.Set(0,0f);
			Left.Set(0, 0f);
			Width.Set(14, 0f);
			Height.Set(14, 0f);
			this.MaxHeight = MinHeight = Height;
			this.MaxWidth = MinWidth = Width;
		}
		/// <summary>
		/// 设置开关中心
		/// </summary>
		public void SetCenter(Vector2 center,Vector2 precent) {
			Left.Set(center.X - 7, precent.X);
			Top.Set(center.Y - 7, precent.Y);
		}
		/// <summary>
		/// 按下
		/// </summary>
		public override void Click(UIMouseEvent evt)
		{
			Value = !Value;
			base.Click(evt);
		}

	}
	/// <summary>
	/// 在文本右边显示开关
	/// </summary>
	public class UILeftNamedSwitch :UIElement
	{
		/// <summary>
		/// 开关
		/// </summary>
		public UISwitch Switch;
		/// <summary>
		/// 文本
		/// </summary>
		public UIText Text; 
		/// <summary>
		/// 名字
		/// </summary>
		public string Name;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
		public UILeftNamedSwitch(string Name,IRefValue<bool> Value,IGetValue<string> HoverString) {
			Switch = new UISwitch(Value, HoverString);
			this.Name = Name;
			Text = new UIText(Name);
			Text.Top.Set(0, 0f);
			Text.Left.Set(0, 0f);
			Text.Width.Set(-14, 1f);
			Text.Height.Set(0, 1f);
			Height = Text.MinHeight;
			Width.Pixels= Text.MinWidth.Pixels+16;
			Switch.SetCenter(new Vector2(-6.5f, 0), new Vector2(1f, 0.5f));
			//Height.Set(0,1f);
			//Width.Set(0,1f);
		}
		public UILeftNamedSwitch(UISwitch Switch, UIText Text)
		{
			this.Switch = Switch;
			this.Text = Text;
			Name = Text.Text;
		}
		public override void OnInitialize()
		{
			Append(Switch);
			Append(Text);
			base.OnInitialize();
		}
	}

	/// <summary>
	/// 在文本右边显示开关
	/// </summary>
	public class UIRightNamedSwitch : UIElement
	{
		/// <summary>
		/// 开关
		/// </summary>
		public UISwitch Switch;
		/// <summary>
		/// 文本
		/// </summary>
		public UIText Text;
		/// <summary>
		/// 名字
		/// </summary>
		public string Name;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
		public UIRightNamedSwitch(string Name, IRefValue<bool> Value, IGetValue<string> HoverString)
		{
			Switch = new UISwitch(Value, HoverString);
			this.Name = Name;
			Text = new UIText(Name);
			Text.Top.Set(0, 0f);
			Text.Left.Set(14, 0f);
			Text.Width.Set(0, 1f);
			Text.Height.Set(0, 1f);
			Height = Text.MinHeight;
			Width.Pixels = Text.MinWidth.Pixels + 16;
			Switch.SetCenter(new Vector2(6.5f, 0), new Vector2(0f, 0.5f));
			//Height.Set(0,1f);
			//Width.Set(0,1f);
		}
		public UIRightNamedSwitch(UISwitch Switch, UIText Text)
		{
			this.Switch = Switch;
			this.Text = Text;
			Name = Text.Text;
		}
		public override void OnInitialize()
		{
			Append(Switch);
			Append(Text);
			base.OnInitialize();
		}
	}
}
