using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Xml.Linq;
using FFXV.Models;
using FFXV.Services;
using FFXV.Services.Extensions;

namespace FFXV.Tools.PkgViewer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		const int FirstColumn = 0;
		const int SecondColumn = 1;

		private static readonly Dictionary<Type, DependencyProperty> BindingProperties = new Dictionary<Type, DependencyProperty>
		{
			[typeof(TextBox)] = TextBox.TextProperty
		};

		public MainWindow()
		{
			InitializeComponent();

			PkgTree.SelectedItemChanged += (sender, args) =>
			{
				if (args.NewValue is TreeViewItem treeViewItem)
				{
					if (treeViewItem.DataContext is PackageObject pkgObject)
					{
						SelectItem(pkgObject);
					}
				}
			};
		}

		protected override void OnInitialized(EventArgs e)
		{
			//LoadXmb(@"ps4-duscae\app0-unpack\quest\cleigne\cl_qt007\cl_qt007.exml");
			//LoadXmb(@"ps4-duscae\app0-unpack\character\nh\common\script\aigraph\nh_enemy_ai_000.exml");
			base.OnInitialized(e);
		}

		private void LoadXmb(string fileName)
		{
			Xmb xmb;
			using (var stream = File.Open(fileName, FileMode.Open))
			{
				xmb = new Xmb(new BinaryReader(stream));
			}

			Open(xmb);
		}

		public void Open(Xmb xmb)
		{
			Open(xmb.Document);
		}

		public void Open(XDocument document)
		{
			var pkg = PackageService.Open(document.Root);
			FeedPkgTree(pkg.ToPackageObject());
		}

		private void SelectItem(PackageObject pkgObject)
		{
			LayoutContent.Children.Clear();

			LayoutContent.Children.Add(new TextBlock
			{
				Text = pkgObject.Object.Type
			});
			LayoutContent.Children.Add(new Separator
			{
				Margin = new Thickness(2, 0, 2, 5)
			});

			var uiElement = CreateUiElementGrid(pkgObject);
			if (uiElement != null)
			{
				LayoutContent.Children.Add(uiElement);
			}
			else
			{
				LayoutContent.Children.Add(new TextBlock
				{
					Text = "Not implemented yet"
				});
			}
		}

		private void FeedPkgTree(PackageObject pkgObject)
		{
			var item = CreateTreeViewItem(pkgObject);
			PkgTree.Items.Clear();
			PkgTree.Items.Add(item);
		}

		private static TreeViewItem CreateTreeViewItem(PackageObject pkgObject)
		{
			var treeViewItem = new TreeViewItem
			{
				DataContext = pkgObject,
				Header = pkgObject.ToString(),
			};

			if (pkgObject.Children != null)
			{
				foreach (var item in pkgObject.Children)
				{
					treeViewItem.Items.Add(CreateTreeViewItem(item));
				}
			}

			return treeViewItem;
		}

		private UIElement CreateUiElementGrid(PackageObject pkgObject)
		{
			if (pkgObject.Item == null)
				return null;

			return CreateUiElementGrid(pkgObject.Item);
		}

		private UIElement CreateUiElementGrid(object obj)
		{
			var grid = new Grid
			{
				ColumnDefinitions =
				{
					new ColumnDefinition
					{
						Width = new GridLength(1, GridUnitType.Star)
					},
					new ColumnDefinition
					{
						Width = new GridLength(2, GridUnitType.Star)
					},
				}
			};

			var properties = obj.GetType().GetProperties();
			for (var i = 0; i < properties.Length; i++)
			{
				AddUiElement(grid, i, obj, properties[i]);
			}

			return grid;
		}

		private void AddUiElement(Grid grid, int index, object obj, PropertyInfo propertyInfo)
		{
			grid.RowDefinitions.Add(new RowDefinition
			{
				Height = new GridLength(1, GridUnitType.Auto)
			});
			grid.RowDefinitions.Add(new RowDefinition
			{
				Height = new GridLength(5, GridUnitType.Pixel)
			});

			var elementName = new TextBlock
			{
				Text = propertyInfo.Name
			};
			grid.Children.Add(elementName);
			Grid.SetColumn(elementName, FirstColumn);
			Grid.SetRow(elementName, index * 2);

			var elementValue = CreateUiElement(propertyInfo.GetValue(obj));
			//if (BindingProperties.TryGetValue(elementValue.GetType(), out var dependencyProperty))
			//{
			//	elementValue.DataContext = obj;
			//	elementValue.SetBinding(dependencyProperty, new Binding(propertyInfo.Name)
			//	{
			//		UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
			//		Mode = BindingMode.TwoWay
			//	});
			//}

			grid.Children.Add(elementValue);
			Grid.SetColumn(elementValue, SecondColumn);
			Grid.SetRow(elementValue, index * 2);
		}

		private static FrameworkElement CreateUiElement(object value)
		{
			if (value is string str)
				return CreateUiElementAs(value, str);
			if (value is bool bval)
				return CreateUiElementAs(value, bval);
			return CreateUiElementAs(value, value);
		}


		private static FrameworkElement CreateUiElementAs(object context, object value)
		{
			return new TextBlock
			{
				Text = value?.ToString() ?? "<null>"
			};
		}

		private static FrameworkElement CreateUiElementAs(object context, string value)
		{
			return new TextBox
			{
				Text = value,
			};
		}

		private static FrameworkElement CreateUiElementAs(object context, bool value)
		{
			return new CheckBox
			{
				IsChecked = value,
			};
		}
	}
}
