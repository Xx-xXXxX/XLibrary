using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLibrary
{
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
	/// <summary>
	/// 对应value的 已启用|已禁用
	/// </summary>
	public class AbleString:IGetValue<string>
	{
		public readonly IGetValue<bool> value;
		public string Value {
			get => value.Value ? Terraria.Localization.Language.GetTextValue($"Mods.XLibrary.Enabled") : Terraria.Localization.Language.GetTextValue($"Mods.XLibrary.Disabled");
		}
		public AbleString(IGetValue<bool> value) { this.value = value; }
	}
	/// <summary>
	/// 对应value的 开|关
	/// </summary>
	public class OnOffString:IGetValue<string>
	{
		public readonly IGetValue<bool> value;
		public string Value
		{
			get => value.Value ? Terraria.Localization.Language.GetTextValue($"Mods.XLibrary.On") : Terraria.Localization.Language.GetTextValue($"Mods.XLibrary.Off");
		}
		public OnOffString(IGetValue<bool> value) { this.value = value; }
	}
}
