using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Commercially.Droid
{
	public static class ActivityExtensions
	{

		public static void CreateMainOptionsMenu(this Activity activity, IMenu menu, int currItem)
		{
			activity.MenuInflater.Inflate(Resource.Menu.TopMenu, menu);
			var rIds = new int[] { Resource.Id.DashboardIcon, Resource.Id.ButtonIcon, Resource.Id.UserIcon, Resource.Id.ClientIcon, Resource.Id.CreateUser };
			foreach (var id in rIds) {
				var item = menu.FindItem(id);
				if (id == currItem) {
					item.Icon?.SetColorFilter(GlobalConstants.DefaultColors.Red.GetAndroidColor(), PorterDuff.Mode.SrcIn);
					item.SetEnabled(false);
				} else {
					item.Icon?.SetColorFilter(GlobalConstants.DefaultColors.Black.GetAndroidColor(), PorterDuff.Mode.SrcIn);
					item.SetEnabled(true);
				}
			}
			if (Session.User.Type != UserRoleType.Admin) {
				menu.RemoveItem(Resource.Id.ButtonIcon);
				menu.RemoveItem(Resource.Id.ClientIcon);
			}
			if (activity.GetType() != typeof(UserListActivity)) {
				menu.RemoveItem(Resource.Id.CreateUser);
			}
		}

		static void ResetMenuItems(IMenu menu, int currItem)
		{
			var rIds = new int[] { Resource.Id.DashboardIcon, Resource.Id.ButtonIcon, Resource.Id.UserIcon, Resource.Id.ClientIcon, Resource.Id.CreateUser };
			foreach (var id in rIds) {
				var item = menu.FindItem(id);
				if (id == currItem) {
					item.Icon?.SetColorFilter(GlobalConstants.DefaultColors.Red.GetAndroidColor(), PorterDuff.Mode.SrcIn);
					item.SetEnabled(false);
				} else {
					item.Icon?.SetColorFilter(GlobalConstants.DefaultColors.Black.GetAndroidColor(), PorterDuff.Mode.SrcIn);
					item.SetEnabled(true);
				}
			}
		}

		public static void StartActivityMenuItem(this Activity activity, IMenuItem item)
		{
			int[] icons = { Resource.Id.DashboardIcon, Resource.Id.ButtonIcon, Resource.Id.UserIcon, Resource.Id.ClientIcon, Resource.Id.CreateUser };
			Type[] activityTypes = { typeof(RequestListActivity), typeof(ButtonListActivity), typeof(UserListActivity), typeof(ClientListActivity), typeof(UserCreateActivity) };

			for (int i = 0; i < icons.Length; i++) {
				if (icons[i] == item.ItemId) {
					var activityType = activityTypes[i];
					bool isNonAdminUserDetails = Session.User.Type != UserRoleType.Admin && activityType == typeof(UserListActivity);
					if (isNonAdminUserDetails) {
						activityType = typeof(UserDetailsActivity);
					}
					if (typeof(Activity) == activityType) return;
					var intent = new Intent(activity, activityType);
					intent.SetFlags(ActivityFlags.ReorderToFront);
					activity.StartActivityIfNeeded(intent, 0);
					item.Icon?.SetColorFilter(GlobalConstants.DefaultColors.Red.GetAndroidColor(), PorterDuff.Mode.SrcIn);
				}
			}
		}

		public static void ShowPrompt(this Activity activity, string message)
		{
			var newFragment = new PromptDialogFragment(message);
			newFragment.Show(activity.FragmentManager, message);
		}

		public static View GetHorizontalLine(this Activity activity)
		{
			var inflater = (LayoutInflater)activity.GetSystemService(Context.LayoutInflaterService);
			return inflater.Inflate(Resource.Layout.HorizontalLine, null);
		}

		public static HorizontalScrollView GetTopButtons<T>(this Activity activity, T[] array)
		{
			var inflater = (LayoutInflater)activity.GetSystemService(Context.LayoutInflaterService);
			var headerView = (HorizontalScrollView)inflater.Inflate(Resource.Layout.TableButtonHeader, null);
			var layout = headerView.FindViewById<LinearLayout>(Resource.Id.buttonLayout);

			foreach (var obj in array) {
				var button = (Button)inflater.Inflate(Resource.Layout.TopButton, null);
				button.Text = obj.ToString();
				layout.AddView(button);
			}
			return headerView;
		}

		public static TableRow GetSectionHeader(this Activity activity, string label)
		{
			var inflater = (LayoutInflater)activity.GetSystemService(Context.LayoutInflaterService);
			var headerView = (TableRow)inflater.Inflate(Resource.Layout.TableSectionHeader, null);
			var headerLabel = headerView.FindViewById<TextView>(Resource.Id.headerText);
			headerLabel.Text = label;
			return headerView;
		}

		public static TableRow GetTableRow(this Activity activity, Request request)
		{
			var inflater = (LayoutInflater)activity.GetSystemService(Context.LayoutInflaterService);
			var rowView = (TableRow)inflater.Inflate(Resource.Layout.RequestRow, null);
			var descriptionLabel = rowView.FindViewById<TextView>(Resource.Id.descriptionText);
			var locationLabel = rowView.FindViewById<TextView>(Resource.Id.locationText);
			var timeLabel = rowView.FindViewById<TextView>(Resource.Id.timeText);
			var statusLabel = rowView.FindViewById<TextView>(Resource.Id.statusText);
			var urgentIndicator = rowView.FindViewById(Resource.Id.urgentIndicator);
			var deleteButton = rowView.FindViewById<Button>(Resource.Id.deleteButton);

			var SharedRow = new RequestTableRow(request);
			locationLabel.Text = SharedRow.LocationText;
			timeLabel.Text = SharedRow.TimeText;
			statusLabel.Text = SharedRow.StatusText;
			statusLabel.Hidden(SharedRow.StatusLabelIsHidden);
			urgentIndicator.Hidden(SharedRow.UrgentIndicatorIsHidden);
			descriptionLabel.Text = SharedRow.DescriptionText;

			deleteButton.Hidden(true);
			deleteButton.Click += (object sender, EventArgs e) => {
				try {
					RequestApi.DeleteRequest(request._id);
				} catch (Exception) {
					activity.ShowPrompt(Localizable.PromptMessages.DeleteError);
					return;
				}
				var table = activity.FindViewById<TableLayout>(Resource.Id.tableLayout);
				table.RemoveView(rowView);
				var intent = new Intent(activity, activity.GetType());
				intent.SetFlags(ActivityFlags.ReorderToFront);
				activity.StartActivityIfNeeded(intent, 0);
			};
			if (Session.User.Type == UserRoleType.Admin) {
				rowView.LongClick += (object sender, View.LongClickEventArgs e) => {
					deleteButton.ToggleVisibility();
				};
			}

			rowView.Click += (object sender, EventArgs e) => {
				var intent = new Intent(activity, typeof(RequestDetailsActivity));
				intent.PutExtra(typeof(Request).Name, JsonConvert.SerializeObject(request));
				activity.StartActivity(intent);
			};
			return rowView;
		}

		public static TableRow GetTableRow(this Activity activity, FlicButton button)
		{
			var inflater = (LayoutInflater)activity.GetSystemService(Context.LayoutInflaterService);
			var rowView = (TableRow)inflater.Inflate(Resource.Layout.ButtonRow, null);
			var buttonLabel = rowView.FindViewById<TextView>(Resource.Id.buttonText);
			var clientLabel = rowView.FindViewById<TextView>(Resource.Id.clientText);
			var descriptionLabel = rowView.FindViewById<TextView>(Resource.Id.descriptionText);
			var locationLabel = rowView.FindViewById<TextView>(Resource.Id.locationText);

			var sharedRow = new ButtonTableRow(button);
			buttonLabel.Text = sharedRow.ButtonText;
			clientLabel.Text = sharedRow.ClientText;
			descriptionLabel.Text = sharedRow.DescriptionText;
			locationLabel.Text = sharedRow.LocationText;

			rowView.Click += (object sender, EventArgs e) => {
				var intent = new Intent(activity, typeof(ButtonDetailsActivity));
				intent.PutExtra(typeof(FlicButton).Name, JsonConvert.SerializeObject(button));
				activity.StartActivity(intent);
			};
			return rowView;
		}

		public static TableRow GetTableRow(this Activity activity, User user)
		{
			var inflater = (LayoutInflater)activity.GetSystemService(Context.LayoutInflaterService);
			var rowView = (TableRow)inflater.Inflate(Resource.Layout.UserRow, null);
			var lastFirstLabel = rowView.FindViewById<TextView>(Resource.Id.lastFirstText);
			var emailLabel = rowView.FindViewById<TextView>(Resource.Id.emailText);
			var deleteButton = rowView.FindViewById<Button>(Resource.Id.deleteButton);

			var sharedRow = new UserTableRow(user);
			lastFirstLabel.Hidden(sharedRow.LastFirstNameLabelIsHidden);
			lastFirstLabel.Text = sharedRow.LastFirstNameText;
			emailLabel.Text = sharedRow.EmailText;

			deleteButton.Hidden(true);
			deleteButton.Click += (object sender, EventArgs e) => {
				try {
					UserApi.DeleteUser(user.id);
				} catch (Exception) {
					activity.ShowPrompt(Localizable.PromptMessages.DeleteError);
					return;
				}
				var table = activity.FindViewById<TableLayout>(Resource.Id.tableLayout);
				table.RemoveView(rowView);
				var intent = new Intent(activity, activity.GetType());
				intent.SetFlags(ActivityFlags.ReorderToFront);
				activity.StartActivityIfNeeded(intent, 0);
			};
			if (UserList.CanEditRow(user)) {
				rowView.LongClick += (object sender, View.LongClickEventArgs e) => {
					deleteButton.ToggleVisibility();
				};
			}

			rowView.Click += (object sender, EventArgs e) => {
				var intent = new Intent(activity, typeof(UserDetailsActivity));
				intent.PutExtra(typeof(User).Name, JsonConvert.SerializeObject(user));
				activity.StartActivity(intent);
			};
			return rowView;
		}

		public static TableRow GetTableRow(this Activity activity, Client client)
		{
			var inflater = (LayoutInflater)activity.GetSystemService(Context.LayoutInflaterService);
			var rowView = (TableRow)inflater.Inflate(Resource.Layout.ClientRow, null);
			var clientIdLabel = rowView.FindViewById<TextView>(Resource.Id.clientIdText);
			var friendlyNameLabel = rowView.FindViewById<TextView>(Resource.Id.friendlyNameText);

			var sharedRow = new ClientTableRow(client);
			clientIdLabel.Text = sharedRow.IdText;
			friendlyNameLabel.Text = sharedRow.FriendlyNameText;
			friendlyNameLabel.Hidden(sharedRow.FriendlyNameLabelIsHidden);

			rowView.Click += (object sender, EventArgs e) => {
				var intent = new Intent(activity, typeof(ClientDetailsActivity));
				intent.PutExtra(typeof(Client).Name, JsonConvert.SerializeObject(client));
				activity.StartActivity(intent);
			};
			return rowView;
		}

		public static Spinner GetStatusSpinner(this Activity activity)
		{
			Spinner spinner = activity.FindViewById<Spinner>(Resource.Id.statusSpinner);
			var adapter = new ArrayAdapter(activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, StatusPicker.Statuses);
			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinner.Adapter = adapter;
			return spinner;
		}

		public static Spinner GetClientSpinner(this Activity activity, FlicButton button)
		{
			Spinner spinner = activity.FindViewById<Spinner>(Resource.Id.clientSpinner);
			var adapter = new ArrayAdapter(activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, ButtonDetails.GetPickerOptions(button));
			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinner.Adapter = adapter;
			return spinner;
		}

		public static Spinner GetUserSpinner(this Activity activity)
		{
			Spinner spinner = activity.FindViewById<Spinner>(Resource.Id.userSpinner);
			var adapter = new ArrayAdapter(activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, RequestDetails.GetUserPickerOptions());
			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinner.Adapter = adapter;
			return spinner;
		}


		public static Spinner GetUserRoleSpinner(this Activity activity)
		{
			Spinner spinner = activity.FindViewById<Spinner>(Resource.Id.userRoleSpinner);
			var adapter = new ArrayAdapter(activity, Android.Resource.Layout.SimpleSpinnerDropDownItem, UserRolePicker.Roles);
			adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinner.Adapter = adapter;
			return spinner;
		}
	}
}