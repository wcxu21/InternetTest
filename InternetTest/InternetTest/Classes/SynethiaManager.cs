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
using PeyrSharp.Env;
using System.IO;
using System.Text.Json;

namespace InternetTest.Classes;
public static class SynethiaManager
{
	public static SynethiaConfig Load()
	{
		if (!Global.Settings.UseSynethia) return new();
		if (!Directory.Exists($@"{FileSys.AppDataPath}\Léo Corporation\InternetTest Pro\"))
		{
			Directory.CreateDirectory($@"{FileSys.AppDataPath}\Léo Corporation\InternetTest Pro\");
		}

		if (!File.Exists(Global.SynethiaPath)) // If no Synethia config exists
		{
			Global.SynethiaConfig = new();
			string json = JsonSerializer.Serialize(Global.SynethiaConfig, new JsonSerializerOptions { WriteIndented = true });
			File.WriteAllText(Global.SynethiaPath, json);
			return new();
		}

		// If Synethia config exists
		// Deserialize the file to Synethia config (using JSON)
		var config = JsonSerializer.Deserialize<SynethiaConfig>(File.ReadAllText(Global.SynethiaPath)) ?? new();
		config.DnsPageInfo ??= new();
		config.TraceRoutePageInfo ??= new();
		config.WiFiNetworksPageInfo ??= new();
		CheckActions(ref config);
		return config;
	}

	public static void Save(SynethiaConfig synethiaConfig)
	{
		if (!Global.Settings.UseSynethia) return;
		string json = JsonSerializer.Serialize(synethiaConfig, new JsonSerializerOptions { WriteIndented = true });
		File.WriteAllText(Global.SynethiaPath, json);
	}

	private static void CheckActions(ref SynethiaConfig synethiaConfig)
	{
		bool hasDns = false;
		bool hasTracert = false;
		bool hasWiFiNetworks = false;
		for (int i = 0; i < synethiaConfig.ActionInfos.Count; i++)
		{
			if (synethiaConfig.ActionInfos[i].Action == Enums.AppActions.GetDnsInfo) hasDns = true;
			if (synethiaConfig.ActionInfos[i].Action == Enums.AppActions.TraceRoute) hasTracert = true;
			if (synethiaConfig.ActionInfos[i].Action == Enums.AppActions.ConnectWiFi) hasWiFiNetworks = true;
		}

		if (!hasDns)
		{
			synethiaConfig.ActionInfos.Add(new() { Action = Enums.AppActions.GetDnsInfo, UsageCount = 0 }); 
		}

		if (!hasTracert)
		{
			synethiaConfig.ActionInfos.Add(new() { Action = Enums.AppActions.TraceRoute, UsageCount = 0 });
		}

		if (!hasWiFiNetworks)
		{
			synethiaConfig.ActionInfos.Add(new() { Action = Enums.AppActions.ConnectWiFi, UsageCount = 0 });
		}
	}
}
