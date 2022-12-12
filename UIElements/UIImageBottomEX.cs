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
namespace XLibrary.UIElements
{
	/// <summary>
	/// 带图的按钮
	/// </summary>
	public class UIImageBottomEX:UIElement
	{
		/// <summary>
		/// 按钮的图像，自动放缩
		/// </summary>
		public IGetValue<Texture2DCutted> texture;
		/// <summary>
		/// 鼠标悬浮时的显示字符串
		/// </summary>
		public IGetValue<string> hoverString;
		/// <summary>
		/// 鼠标悬浮时透明度
		/// </summary>
		public float visibilityActive = 1f;
		/// <summary>
		/// 不活跃（鼠标没有悬浮时）时透明度
		/// </summary>
		public float visibilityInactive = 0.4f;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
		public UIImageBottomEX(IGetValue<Texture2DCutted> texture, IGetValue<string> hoverString=null) {
			this.texture = texture;
			this.hoverString = hoverString;
		}
		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			texture.Value.Draw(spriteBatch, GetDimensions().ToRectangle(), Color.White * (base.IsMouseHovering ? visibilityActive : visibilityInactive), 0, SpriteEffects.None);
			if (IsMouseHovering && hoverString != null)
			{
				Main.hoverItemName = hoverString.Value;
			}
		}
		public void SetVisibility(float whenActive, float whenInactive)
		{
			visibilityActive = MathHelper.Clamp(whenActive, 0f, 1f);
			visibilityInactive = MathHelper.Clamp(whenInactive, 0f, 1f);
		}
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
	}
}
