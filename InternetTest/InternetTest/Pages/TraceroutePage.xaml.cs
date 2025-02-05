﻿/*
MIT License

Copyright (c) Léo Corporation

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. 
*/

using InternetTest.Classes;
using InternetTest.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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

namespace InternetTest.Pages;
/// <summary>
/// Interaction logic for TraceroutePage.xaml
/// </summary>
public partial class TraceroutePage : Page
{
	bool codeInjected = !Global.Settings.UseSynethia;
	public TraceroutePage()
	{
		InitializeComponent();
		Loaded += (o, e) => InjectSynethiaCode();
		InitUI();
	}

	private void InjectSynethiaCode()
	{
		if (codeInjected) return;
		codeInjected = true;
		foreach (Button b in Global.FindVisualChildren<Button>(this))
		{
			b.Click += (sender, e) =>
			{
				Global.SynethiaConfig.TraceRoutePageInfo.InteractionCount++;
			};
		}

		// For each TextBox of the page
		foreach (TextBox textBox in Global.FindVisualChildren<TextBox>(this))
		{
			textBox.GotFocus += (o, e) =>
			{
				Global.SynethiaConfig.TraceRoutePageInfo.InteractionCount++;
			};
		}

		// For each CheckBox/RadioButton of the page
		foreach (CheckBox checkBox in Global.FindVisualChildren<CheckBox>(this))
		{
			checkBox.Checked += (o, e) =>
			{
				Global.SynethiaConfig.TraceRoutePageInfo.InteractionCount++;
			};
			checkBox.Unchecked += (o, e) =>
			{
				Global.SynethiaConfig.TraceRoutePageInfo.InteractionCount++;
			};
		}

		foreach (RadioButton radioButton in Global.FindVisualChildren<RadioButton>(this))
		{
			radioButton.Checked += (o, e) =>
			{
				Global.SynethiaConfig.TraceRoutePageInfo.InteractionCount++;
			};
			radioButton.Unchecked += (o, e) =>
			{
				Global.SynethiaConfig.TraceRoutePageInfo.InteractionCount++;
			};
		}
	}

	private void InitUI()
	{
		TitleTxt.Text = $"{Properties.Resources.Commands} > {Properties.Resources.TraceRoute}"; // Set the title
	}

	private void DismissBtn_Click(object sender, RoutedEventArgs e)
	{
		AddressTxt.Text = "";
	}

	private async void TraceBtn_Click(object sender, RoutedEventArgs e)
	{
		// Increment the interaction count of the ActionInfo in Global.SynethiaConfig
		Global.SynethiaConfig.ActionInfos.First(a => a.Action == Enums.AppActions.TraceRoute).UsageCount++;

		// Show the waiting screen
		TraceBtn.IsEnabled = false;
		StatusPanel.Visibility = Visibility.Collapsed;
		WaitingScreen.Visibility = Visibility.Visible;
		WaitTxt.Text = Properties.Resources.TraceProgress;
		WaitIconTxt.Text = "\uF2DE";
		TracertPanel.Visibility = Visibility.Collapsed;
		TracertPanel.Children.Clear();

		try
		{
			// Get traceroute
			var route = await Global.Trace(AddressTxt.Text, Global.Settings.TraceRouteMaxHops ?? 30, Global.Settings.TraceRouteMaxTimeOut ?? 5000);
			int success = 0; int failed = 0; long time = 0;

			// Update the UI with each step
			for (int i = 0; i < route.Count; i++)
			{
				TracertPanel.Children.Add(new TraceRouteItem(route[i], i == route.Count - 1));
				if (route[i].Status == IPStatus.Success || route[i].Status == IPStatus.TtlExpired) success++;
				else failed++;
				time += route[i].RoundtripTime;
			}

			// Set the values of the overview panel
			SucessTxt.Text = success.ToString();
			FailedTxt.Text = failed.ToString();
			DurationTxt.Text = $"{time}ms";
			HopsTxt.Text = $"{route.Count} {Properties.Resources.HopsLower}";

			// Show the overview and the traceroute
			StatusPanel.Visibility = Visibility.Visible;
			WaitingScreen.Visibility = Visibility.Collapsed;
			TracertPanel.Visibility = Visibility.Visible;
			
			WaitTxt.Text = Properties.Resources.TraceRouteInformation; // Reset text to default
			WaitIconTxt.Text = "\uF4AB";
		}
		catch { }

		TraceBtn.IsEnabled = true;
	}
}
