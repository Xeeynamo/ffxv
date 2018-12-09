using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using FFXV.Services;
using FFXV.Tools.MsgViewer.Models;
using Xe.Tools;
using Xe.Tools.Wpf.Commands;
using Xe.Tools.Wpf.Dialogs;
using Xe.Tools.Wpf.Models;

namespace FFXV.Tools.MsgViewer.ViewModels
{
	public class MessageListViewModel : GenericListModel<MessageModel>
	{
		private readonly IEnumerable<MessageModel> _msgs;

		public string CurrentMessage
		{
			get => SelectedItem?.Text;
			set
			{
				if (IsItemSelected)
				{
					SelectedItem.Text = value;
				}
			}
		}

		public MessageListViewModel(IEnumerable<MessageModel> list) :
			base(list)
		{
			_msgs = list;
		}

		protected override MessageModel OnNewItem()
		{
			return new MessageModel();
		}

		protected override void OnSelectedItem(MessageModel item)
		{
			OnPropertyChanged(nameof(CurrentMessage));
			base.OnSelectedItem(item);
		}
	}

	public class MsgViewerViewModel : BaseNotifyPropertyChanged
	{
		private string _filter;

		public string Title => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName;

		public MessageListViewModel MessageList { get; private set; }

		public RelayCommand OpenCommand { get; }

		public RelayCommand AboutCommand { get; }

		public string Filter
		{
			get => _filter;
			set
			{
				_filter = value;
				MessageList.Filter(x => x.Id.ToString().Contains(_filter) ||
				                        x.Text.Contains(_filter));
			}
		}

		public MsgViewerViewModel()
		{
			OpenCommand = new RelayCommand(o =>
			{
				var fd = FileDialog.Factory(null, FileDialog.Behavior.Open, ("Final Fantasy XV messages file", "*.msgbin"));
				if (fd.ShowDialog() == true)
				{
					Open(fd.FileName);
				}
			}, x => true);

			AboutCommand = new RelayCommand(x =>
			{
				new AboutDialog(Assembly.GetExecutingAssembly()).ShowDialog();
			}, x => true);
		}

		public void Open(string fileName)
		{
			using (var stream = File.OpenRead(fileName))
			{
				var messages = Msg.Open(stream)
					.Select(x => new MessageModel
					{
						Id = x.Key,
						Text = x.Value
					})
					.ToList();
				MessageList = new MessageListViewModel(messages);
				OnPropertyChanged(nameof(MessageList));
			}
		}
	}
}
