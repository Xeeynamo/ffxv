using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FFXV.Tools.MsgViewer.ViewModels;

namespace FFXV.Tools.MsgViewer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			DataContext = new MsgViewerViewModel();
			(DataContext as MsgViewerViewModel).Open(@"D:\Hacking\FFXV\ps4-duscae\app0-unpack\data\message\bin\us\message.win32.msgbin");
		}
	}
}
