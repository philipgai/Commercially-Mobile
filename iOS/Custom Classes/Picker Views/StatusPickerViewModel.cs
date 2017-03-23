using System;
using UIKit;

namespace Commercially.iOS
{
	public class StatusPickerViewModel : UIPickerViewModel
	{
		readonly string[] Statuses = { RequestStatusType.New.ToString(), RequestStatusType.Assigned.ToString(), RequestStatusType.Completed.ToString(), RequestStatusType.Cancelled.ToString() };
		readonly Action<UIPickerView, nint, nint> OnSelect;

		public StatusPickerViewModel(Action<UIPickerView, nint, nint> OnSelect)
		{
			this.OnSelect = OnSelect;
		}

		public override nint GetComponentCount(UIPickerView pickerView)
		{
			return 1;
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			return Statuses.Length;
		}

		public override string GetTitle(UIPickerView pickerView, nint row, nint component)
		{
			return Statuses[row];
		}

		public override void Selected(UIPickerView pickerView, nint row, nint component)
		{
			OnSelect(pickerView, row, component);
		}
	}
}
