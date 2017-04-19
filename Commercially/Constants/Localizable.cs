﻿namespace Commercially {
	public struct Localizable {
		public struct PromptMessages {
			public const string FriendlyStart = "Oops! ";
			public const string InvalidEmail = FriendlyStart + "Check that your email is valid.";
			public const string InvalidPassword = FriendlyStart + "Check that your password is valid.";
			public const string LoginError = FriendlyStart + "Check your email address or password.";
			public const string ButtonsError = FriendlyStart + "Buttons were unable to load.";
			public const string RequestsError = FriendlyStart + "FlicButton requests were unable to load.";
			public const string ButtonSaveError = FriendlyStart + "Changes to this button were unable to save.";
			public const string RequestSaveError = FriendlyStart + "Changes to this request were unable to save.";
			public const string AssignError = FriendlyStart + "Assiging the request was unsuccessful.";
		}
		public struct ExceptionMessages {
			public const string CannotConnectServer = "Cannot connect to server";
			public const string NoConstraint = "No Constraint Matches the given attribute";
		}
		public struct Labels {
			public const string ToDo = "To Do";
			public const string Complete = "Complete";
			public const string InProgress = "In Progress";
			public const string MyTasks = "My Tasks";
		}
	}
}