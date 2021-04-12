using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace minecraft_resource_manager
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private string resourceFolder;
		private string selectedMod;
		private string selectedBlock;

		private Window generateWindow;

		private List<string> blockstates = new List<string>();

		private string BlockstateFile => $@"{resourceFolder}\assets\{selectedMod}\blockstates\{selectedBlock}.json";
		private string ItemModelFile => $@"{resourceFolder}\assets\{selectedMod}\models\item\{selectedBlock}.json";
		private string RecipeFile => $@"{resourceFolder}\data\{selectedMod}\recipes\{selectedBlock}.json";
		private string AdvancementFile => $@"{resourceFolder}\data\{selectedMod}\advancements\recipes\{selectedBlock}.json";
		private string LootTableFile => $@"{resourceFolder}\data\{selectedMod}\loot_tables\blocks\{selectedBlock}.json";

		public MainWindow()
		{
			InitializeComponent();
			Blockstates.ItemsSource = blockstates;
			CollectionView view = CollectionViewSource.GetDefaultView(Blockstates.ItemsSource) as CollectionView;
			view.Filter = CustomerFilter;
		}

		private void Load_Click(object sender, RoutedEventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
			if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
			{
				// Check if assets folder exists
				if (Directory.Exists(dialog.FileName + "\\assets"))
				{
					refreshBlockstates(dialog.FileName);
				}
			}
		}

		public void refreshBlockstates(string resourceFolder)
		{
			// Prep and go through each mod folder
			blockstates.Clear();
			this.resourceFolder = resourceFolder;
			string[] dirs = Directory.GetDirectories(resourceFolder + "\\assets");

			foreach (string dir in dirs)
			{
				// Check if blockstates folder exists
				string mod = dir.Substring(dir.LastIndexOf('\\') + 1);
				string newDir = dir + "\\blockstates";

				if (Directory.Exists(newDir))
				{
					// Add all blockstates
					string[] files = Directory.GetFiles(newDir);

					foreach (string file in files)
					{
						if (file.EndsWith(".json"))
						{
							string block = file.Substring(file.LastIndexOf('\\') + 1);
							block = block.Remove(block.Length - 5);
							blockstates.Add(mod + ":" + block);
						}
					}
				}
			}

			// Cleanup and enable buttons
			CollectionViewSource.GetDefaultView(Blockstates.ItemsSource).Refresh();
			Generate.IsEnabled = true;
		}

		private void Generate_Click(object sender, RoutedEventArgs e)
		{
			if (generateWindow == null)
			{
				generateWindow = new GenerateWindow(this, resourceFolder);
				generateWindow.Show();
			}
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e) => CollectionViewSource.GetDefaultView(Blockstates.ItemsSource).Refresh();

		private bool CustomerFilter(object item) => string.IsNullOrEmpty(TextBox.Text) || item.ToString().Contains(TextBox.Text);

		private void Blockstates_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			object item = (sender as ListBox).SelectedItem;
			if (item == null)
			{
				Blockstate.IsEnabled = false;
				BlockModel.IsEnabled = false;
				ItemModel.IsEnabled = false;
				Recipe.IsEnabled = false;
				Advancement.IsEnabled = false;
				LootTable.IsEnabled = false;
				BlockModels.Items.Clear();
				return;
			}

			string str = item.ToString();
			int i = str.IndexOf(':');
			selectedMod = str.Remove(i);
			selectedBlock = str.Substring(i + 1);

			string file = File.ReadAllText(BlockstateFile);

			MatchCollection matches = Regex.Matches(file, "\"model\":\\s*\"(.+?)\"");
			BlockModels.Items.Clear();
			List<string> added = new List<string>();

			foreach (Match match in matches)
			{
				string model = match.Groups[1].Value;

				if (!added.Contains(model))
				{
					added.Add(model);
					BlockModels.Items.Add(model);
				}
			}

			// Check if some files exist
			Blockstate.IsEnabled = true;
			//BlockModel.IsEnabled = true;
			ItemModel.IsEnabled = File.Exists(ItemModelFile);
			Recipe.IsEnabled = File.Exists(RecipeFile);
			Advancement.IsEnabled = File.Exists(AdvancementFile);
			LootTable.IsEnabled = File.Exists(LootTableFile);
		}

		private void Blockstate_Click(object sender, RoutedEventArgs e) => openFile(BlockstateFile);
		private void BlockModel_Click(object sender, RoutedEventArgs e) { }
		private void ItemModel_Click(object sender, RoutedEventArgs e) => openFile(ItemModelFile);
		private void Recipe_Click(object sender, RoutedEventArgs e) => openFile(RecipeFile);
		private void Advancement_Click(object sender, RoutedEventArgs e) => openFile(AdvancementFile);
		private void LootTable_Click(object sender, RoutedEventArgs e) => openFile(LootTableFile);
		private void openFile(string file)
		{
			if (File.Exists(file)) { Process.Start(file); }
		}

		private void BlockModels_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			object item = (sender as ListBox).SelectedItem;
			if (item == null) { return; }

			string id = item.ToString();
			int i = id.IndexOf(':');
			string mod = id.Remove(i);
			string block = id.Substring(i + 1);

			openFile($@"{resourceFolder}\assets\{mod}\models\{block}.json");
		}
	}
}
