using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace minecraft_resource_manager
{
	/// <summary>
	/// Interaction logic for GenerateWindow.xaml
	/// </summary>
	public partial class GenerateWindow : Window
	{
		private MainWindow window;
		private string resourceFolder;
		private string templateFolder;

		public GenerateWindow(MainWindow window, string resourceFolder)
		{
			InitializeComponent();

			this.window = window;
			this.resourceFolder = resourceFolder;
			templateFolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\templates\\";

			addEntries(Blockstates, "blockstates");
			addEntries(BlockModels, "block models");
			addEntries(ItemModels, "item models");
			addEntries(Recipes, "recipes");
			addEntries(Advancements, "advancements");
			addEntries(LootTables, "loot tables");
		}

		private void addEntries(ComboBox combo, string folder)
		{
			folder = templateFolder + folder;
			if (Directory.Exists(folder))
			{
				string[] dirs = Directory.GetDirectories(folder);

				foreach (string dir in dirs)
				{
					string name = dir.Substring(dir.LastIndexOf('\\') + 1);
					combo.Items.Add(name);
				}

				string[] files = Directory.GetFiles(folder);

				foreach (string file in files)
				{
					if (file.EndsWith(".json"))
					{
						string name = file.Substring(file.LastIndexOf('\\') + 1);
						name = name.Remove(name.Length - 5);
						combo.Items.Add(name);
					}
				}
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			writeFile(Blockstates, "blockstates", true, "blockstates");
			writeFile(BlockModels, "block models", true, "models\\block");
			writeFile(ItemModels, "item models", true, "models\\item");
			writeFile(Recipes, "recipes", false, "recipes");
			writeFile(Advancements, "advancements", false, "advancements\\recipes");
			writeFile(LootTables, "loot tables", false, "loot_tables\\blocks");

			window.refreshBlockstates(resourceFolder);
			MessageBox.Show("Files generated! Make sure to edit recipes and advancements afterwards.");
		}

		private void writeFile(ComboBox combo, string templates, bool isAsset, string subfolder)
		{
			if (combo.SelectedIndex == 0) { return; }

			string item = combo.SelectedItem.ToString();
			string template = $"{templateFolder + templates}\\{item}";

			// Create folders if they're missing
			string dir = $@"{resourceFolder}\{(isAsset ? "assets" : "data")}\{ModName.Text}\";
			if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }

			dir = dir + subfolder;
			if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }

			if (File.Exists(template + ".json"))
			{
				string contents = File.ReadAllText(template + ".json");

				contents = contents.Replace("$MOD", ModName.Text)
					.Replace("$BLOCK", BlockName.Text)
					.Replace("$ITEM", ItemName.Text)
					.Replace("$ITEM2", ItemAltName.Text)
					.Replace("$TEX", TextureName.Text);

				File.WriteAllText(dir + $@"\{BlockName.Text}.json", contents);
			}
			else if (Directory.Exists(template))
			{
				string[] files = Directory.GetFiles(template);

				foreach (string file in files)
				{
					if (file.EndsWith(".json"))
					{
						string contents = File.ReadAllText(file);
						string name = file.Substring(file.LastIndexOf('\\') + 1).Replace(item, "");

						contents = contents.Replace("$MOD", ModName.Text)
							.Replace("$BLOCK", BlockName.Text)
							.Replace("$ITEM", ItemName.Text)
							.Replace("$ITEM2", ItemAltName.Text)
							.Replace("$TEX", TextureName.Text);

						File.WriteAllText(dir + $@"\{BlockName.Text + name}", contents);
					}
				}
			}
		}
	}
}
