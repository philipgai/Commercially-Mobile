﻿using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Commercially
{
	public class UserDetails
	{
		public User User;
		public Request[] Requests;
		public const double HeaderHeight = 50;
		public const double RowHeight = 88;
		public const double RowAlphaDouble = 0.33;
		public const byte RowAlphaByte = 0x54;
		public readonly static string HeaderTitle = RequestStatusType.Assigned.ToString();
		public readonly static Color TableHeaderColor = GlobalConstants.DefaultColors.Yellow;

		public string NameText {
			get {
				return User.firstname + " " + User.lastname;
			}
		}

		public string EmailText {
			get {
				return User.username;
			}
		}

		public string PhoneText {
			get {
				return User.phone;
			}
		}

		public bool PhoneFieldIsHidden {
			get {
				return string.IsNullOrWhiteSpace(User.phone);
			}
		}

		public bool IsEditable {
			get {
				return User == Session.User || Session.User.Type == UserRoleType.Admin;
			}
		}

		public bool NameChanged(string name)
		{
			if (string.IsNullOrWhiteSpace(User.firstname) || string.IsNullOrWhiteSpace(User.lastname)) return !string.IsNullOrWhiteSpace(name);
			return !name.Equals(User.firstname + " " + User.lastname);
		}

		public bool EmailChanged(string email)
		{
			if (string.IsNullOrWhiteSpace(User.username)) return !string.IsNullOrWhiteSpace(email);
			return !email.Equals(User.username);
		}

		public bool PhoneChanged(string phone)
		{
			if (string.IsNullOrWhiteSpace(User.phone)) return !string.IsNullOrWhiteSpace(phone);
			return !phone.Equals(User.phone);
		}

		public void GetRequests(Action OnSuccess, Action<Exception> IfException)
		{
			Session.TaskFactory.StartNew(delegate {
				try {
					Requests = RequestApi.GetRequests(User);
					OnSuccess.Invoke();
				} catch (Exception e) {
					IfException.Invoke(e);
				}
			});
		}

		public bool FieldsChanged(string name, string email, string phone)
		{
			return NameChanged(name) || EmailChanged(email) || PhoneChanged(phone);
		}

		public string SaveButtonPress(string name, string email, string phone)
		{
			var jsonBody = new JObject();
			if (NameChanged(name) && name.Split(' ').Length >= 2) {
				string[] names = name.Split(' ');
				string firstName = names[0];
				string lastName = names[1];
				for (int i = 2; i < names.Length; i++) {
					lastName += " " + names[i];
				}
				jsonBody.Add("firstname", firstName);
				jsonBody.Add("lastname", lastName);
			}
			if (EmailChanged(email) && Validator.Email(email)) {
				jsonBody.Add("username", email);
				jsonBody.Add("email", email);
			}
			if (PhoneChanged(phone)) {
				string numbers = phone.GetNumbers();
				if (numbers.Length > 0) {
					jsonBody.Add("phone", numbers);
				}
			}
			return UserApi.PatchUser(User.id, jsonBody.ToString());
		}
	}
}
