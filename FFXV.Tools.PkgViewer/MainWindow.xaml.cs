using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Xml.Linq;
using FFXV.Models;
using FFXV.Services;
using FFXV.Services.Extensions;
using FFXV.Tools.PkgViewer.Controls;
using SQEX.Ebony;
using Path = System.IO.Path;

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
			[typeof(TextBox)] = TextBox.TextProperty,
			[typeof(ComboBox)] = Selector.SelectedValueProperty
		};

		private static readonly List<Type> PrimitiveTypes = new List<Type>
		{
			typeof(bool),
			typeof(sbyte),
			typeof(byte),
			typeof(char),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(string),
			typeof(float),
			typeof(double),
			typeof(Float2),
			typeof(Float3),
			typeof(Float4),
			typeof(Color),
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
			//Load(@"D:\Hacking\FFXV\ps4-duscae\app0-unpack\level\debugsystem\wm\debug_wm.xml");
			Load(@"D:\Hacking\FFXV\ps4-duscae\app0-unpack\level\debugsystem\event\debug_event.xml");
			base.OnInitialized(e);
		}

		private void Load(string fileName)
		{
			switch (Path.GetExtension(fileName))
			{
				case ".exml":
					LoadXmb(fileName);
					break;
				case ".xml":
					LoadXml(fileName);
					break;
			}
		}

		private void LoadXml(string fileName)
		{
			XDocument doc;
			using (var stream = File.Open(fileName, FileMode.Open))
			{
				doc = XDocument.Load(stream);
			}

			Open(doc);
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

		private static UIElement CreateUiElementGrid(PackageObject pkgObject)
		{
			if (pkgObject.Item == null)
				return null;

			return CreateUiElementGrid(pkgObject.Item);
		}

		private static UIElement CreateUiElementGrid(object obj)
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

		private static void AddUiElement(Grid grid, int index, object obj, PropertyInfo propertyInfo)
		{
			grid.RowDefinitions.Add(new RowDefinition
			{
				Height = new GridLength(1, GridUnitType.Auto)
			});
			grid.RowDefinitions.Add(new RowDefinition
			{
				Height = new GridLength(5, GridUnitType.Pixel)
			});

			var value = propertyInfo.GetValue(obj);

			if (value != null)
			{
				if (IsPrimitiveType(propertyInfo.PropertyType))
				{
					var element = CreateUiElement(value);
					BindProperty(element, propertyInfo.Name, obj);

					AddUiFrameworkElement(grid, index, new TextBlock
					{
						Text = propertyInfo.Name
					}, element);
				}
				else if (IsCollection(propertyInfo.PropertyType))
				{
					var element = CreateUiElementsArrayGroup(propertyInfo.Name, value);
					BindProperty(element, propertyInfo.Name, obj);
					AddUiFrameworkElement(grid, index, element);
				}
				else if (IsEnum(propertyInfo.PropertyType))
				{
					var element = CreateUiElementEnum(value);
					BindProperty(element, propertyInfo.Name, obj);
					AddUiFrameworkElement(grid, index, new TextBlock
					{
						Text = propertyInfo.Name
					}, element);
				}
				else
				{
					var element = CreateUiElementsGroup(propertyInfo.Name, value);
					AddUiFrameworkElement(grid, index, element);
				}
			}
			else
			{
				// Null values should be handled in some way.
			}
		}

		private static void BindProperty(FrameworkElement element, string name, object dataContext)
		{
			if (BindingProperties.TryGetValue(element.GetType(), out var dependencyProperty))
			{
				element.DataContext = dataContext;
				element.SetBinding(dependencyProperty, new Binding(name)
				{
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
					Mode = BindingMode.TwoWay
				});
			}
		}

		private static void AddUiFrameworkElement(Grid grid, int index, FrameworkElement element)
		{
			grid.Children.Add(element);
			Grid.SetColumn(element, 0);
			Grid.SetColumnSpan(element, 3);
			Grid.SetRow(element, index * 2);
		}

		private static void AddUiFrameworkElement(Grid grid, int index, FrameworkElement elementName, FrameworkElement elementValue)
		{
			grid.Children.Add(elementName);
			Grid.SetColumn(elementName, FirstColumn);
			Grid.SetRow(elementName, index * 2);

			grid.Children.Add(elementValue);
			Grid.SetColumn(elementValue, SecondColumn);
			Grid.SetRow(elementValue, index * 2);
		}

		private static bool IsPrimitiveType(Type type)
		{
			return PrimitiveTypes.Contains(type);
		}

		private static bool IsCollection(Type type)
		{
			return type.GetInterfaces()
				.Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
		}

		private static bool IsEnum(Type type)
		{
			return type.IsEnum;
		}

		private static FrameworkElement CreateUiElement(object value)
		{
			if (value is string ||
			    value is int ||
			    value is double ||
			    value is float)
				return CreateUiElementAs(value);

			if (value is bool bval)
				return CreateUiElementAs(value, bval);

			return CreateUiElementAs(value, value);
		}

		private static FrameworkElement CreateUiElementEnum(object value)
		{
			var comboBox = new ComboBox();
			var enumerator = value.GetType().GetEnumValues().GetEnumerator();

			while (enumerator.MoveNext())
			{
				comboBox.Items.Add(enumerator.Current);
			}

			return comboBox;
		}


		private static FrameworkElement CreateUiElementAs(object context, object value)
		{
			return new TextBlock
			{
				Text = value?.ToString() ?? "<null>"
			};
		}

		private static FrameworkElement CreateUiElementAs(object context)
		{
			return new TextBox();
		}

		private static FrameworkElement CreateUiElementAs(object context, bool value)
		{
			return new CheckBox
			{
				IsChecked = value,
			};
		}

		private static FrameworkElement CreateUiElementsGroup(string name, object context)
		{
			return new SpecialGroupBox
			{
				Header = name,
				Content = CreateUiElementGrid(context)
			};
		}

		private static FrameworkElement CreateUiElementsArrayGroup(string name, object context)
		{
			var stackLayout = new StackPanel();
			var index = 0;

			foreach (var item in context as IEnumerable)
			{
				stackLayout.Children.Add(new SpecialGroupBox
				{
					Header = $"Item {index++}",
					IsContentVisible = false,
					Content = CreateUiElementGrid(item)
				});
			}

			return new SpecialGroupBox
			{
				Header = name,
				Content = stackLayout
			};
		}
	}
}
