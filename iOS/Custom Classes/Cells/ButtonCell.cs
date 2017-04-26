﻿using System;

using Foundation;
using UIKit;

namespace Commercially.iOS
{
	public partial class ButtonCell : UITableViewCell
	{
		public static readonly NSString Key = new NSString("ButtonCell");
		public static readonly UINib Nib;

		static ButtonCell()
		{
			Nib = UINib.FromName(Key, NSBundle.MainBundle);
		}

		protected ButtonCell(IntPtr handle) : base(handle) { }

		FlicButton _Button;
		public FlicButton Button {
			get {
				return _Button;
			}
			set {
				_Button = value;
				ButtonLabel.Text = value.bluetooth_id;
				var tmpClient = Client.FindClient(Button.clientId, Session.Clients);
				ClientLabel.Text = tmpClient != null && tmpClient.friendlyName != null ? tmpClient.friendlyName : value.clientId;
				ClientLabel.Hidden = string.IsNullOrWhiteSpace(value.clientId);
				DescriptionLabel.Text = value.description;
				LocationLabel.Text = value.room;
			}
		}
	}
}
