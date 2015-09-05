﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Wpf;
using Infrastructure;
using Infrastructure.Events;

namespace VideoModule.ViewModels
{
	class CameraDetailsViewModel : SaveCancelDialogViewModel
	{
		public Camera Camera { get; private set; }
		public bool IsEditMode { get; private set; }

		public CameraDetailsViewModel(Camera camera = null)
		{
			if (camera != null)
			{
				Camera = camera;
				Title = "Свойства камеры";
				IsEditMode = true;
				CopyProperties();
			}
			else
			{
				Title = "Создание нового видеоустройства";
				Camera = new Camera();
				Name = "Видеоустройство";
				Address = "172.16.5.201";
				ChannelsCount = 1;
				ChannelNumber = 1;
				IsEditMode = false;
			}
			CanPlay = true;
		}

		void CopyProperties()
		{
			Name = Camera.Name;
			Address = Camera.Ip;
			ChannelNumber = Camera.ChannelNumber + 1;
		}

		string _name;
		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged(() => Name);
			}
		}

		string _address;
		public string Address
		{
			get { return _address; }
			set
			{
				_address = value;
				OnPropertyChanged(() => Address);
			}
		}

		int _channelsCount;
		public int ChannelsCount
		{
			get { return _channelsCount; }
			set
			{
				_channelsCount = value;
				OnPropertyChanged(() => ChannelsCount);
			}
		}

		int _channelNumber;
		public int ChannelNumber
		{
			get { return _channelNumber; }
			set
			{
				_channelNumber = value;
				OnPropertyChanged(() => ChannelNumber);
			}
		}
		bool _canPlay;
		public bool CanPlay
		{
			get { return _canPlay; }
			set
			{
				if (_canPlay == value)
					return;
				_canPlay = value;
				OnPropertyChanged(() => CanPlay);
			}
		}
		public override bool OnClosing(bool isCanceled)
		{
			if (CloseEvent != null)
				CloseEvent();
			return base.OnClosing(isCanceled);
		}

		protected override bool Save()
		{
			Camera.Name = Name;
			Camera.Ip = Address;
			return base.Save();
		}
		public event Action CloseEvent;
	}
}