using Terraria.ModLoader;
using Terraria.Localization;
using System.Linq;
using System.Linq.Expressions;

namespace XLibrary
{
	public class XLibrary : Mod
	{
		private static XLibrary instance=null;

		public static XLibrary Instance => instance;
		public XLibrary()
		{
			instance = this;
			
		}

		public override void Load()
		{

			ModTranslation Translations = LocalizationLoader.CreateTranslation(this, "Disabled");
			Translations.SetDefault("Disabled");
			Translations.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "已禁用");
			LocalizationLoader.AddTranslation(Translations);

			Translations = LocalizationLoader.CreateTranslation(this, "Enabled");
			Translations.SetDefault("Enabled");
			Translations.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "已启用");
			LocalizationLoader.AddTranslation(Translations);

			Translations = LocalizationLoader.CreateTranslation(this, "On");
			Translations.SetDefault("On");
			Translations.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "开");
			LocalizationLoader.AddTranslation(Translations);


			Translations = LocalizationLoader.CreateTranslation(this, "Off");
			Translations.SetDefault("Off");
			Translations.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "关");
			LocalizationLoader.AddTranslation(Translations);
		}
		public override void Unload()
		{
			instance = null;
			UnloadDoHolder.Unload();
		}
	}
}