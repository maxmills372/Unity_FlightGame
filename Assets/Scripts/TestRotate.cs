using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class GameInputManager
{
	
	static Dictionary<string, KeyCode> keyMapping;
	static Dictionary<string, KeyCode> keyMappingPS4;

	static Dictionary<string, Dictionary<string, KeyCode>> testDictionary;
	static string[] keyMaps = new string[6]
	{
		"Throttle",
		"Block",
		"Forward",
		"Backward",
		"Left",
		"Right"
	};
	static string[] PCControls = new string[6]
	{
		"Throttle",
		"Block",
		"Forward",
		"Backward",
		"Left",
		"Right"
	};
	static string[] PS4Controls = new string[6]
	{
		"ThrottlePS4",
		"BlockPS4",
		"ForwardPS4",
		"BackwardPS4",
		"Left",
		"Right"
	};

	static GameInputManager()
	{
		//InitializeDictionary();
	}
	/*
	private static void InitializeDictionary()
	{
		testDictionary = new Dictionary<string, Dictionary<string, string>>();

		keyMapping = new Dictionary<string, string>();
		for(int i=0;i<keyMaps.Length;++i)
		{
			keyMapping.Add(keyMaps[i], PCControls[i]);
		}

		testDictionary.Add("PC",keyMapping);

		keyMappingPS4 = new Dictionary<string, string>();
		for(int i=0;i<keyMaps.Length;++i)
		{
			keyMappingPS4.Add(keyMaps[i], PS4Controls[i]);
		}

		testDictionary.Add("PS4",keyMappingPS4);
	}

	public static void SetKeyMap(string keyMap,string key)
	{
		if (!keyMapping.ContainsKey(keyMap))
			throw new ArgumentException("Invalid KeyMap in SetKeyMap: " + keyMap);
		keyMapping[keyMap] = key;
	}

	public static bool GetKeyDown(string keyMap)
	{
		return Input.GetKeyDown(keyMapping[keyMap]);
	}*/
}

