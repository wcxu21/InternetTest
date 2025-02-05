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
using InternetTest.Windows;
using System.Windows;

namespace InternetTest;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	private void Application_Startup(object sender, StartupEventArgs e)
	{
		Global.ChangeTheme();
		Global.ChangeLanguage();

		Global.HomePage = new();
		Global.HistoryPage = new();
		Global.SettingsPage = new();
		Global.StatusPage = new();
		Global.DownDetectorPage = new();
		Global.MyIpPage = new();
		Global.LocateIpPage = new();
		Global.PingPage = new();
		Global.IpConfigPage = new();
		Global.WiFiPasswordsPage = new();
		Global.DnsPage = new();
		Global.TraceroutePage = new();
		Global.WiFiNetworksPage = new();

		if (!Global.Settings.IsFirstRun)
		{
			new MainWindow().Show();
		}
		else
		{
			new FirstRunWindow().Show();
		}
	}

	private void Application_Exit(object sender, ExitEventArgs e)
	{
		SynethiaManager.Save(Global.SynethiaConfig);
		HistoryManager.Save(Global.History);
		SettingsManager.Save();
	}
}
