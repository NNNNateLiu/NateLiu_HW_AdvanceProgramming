using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleJSON;

public class JSONLoader : MonoBehaviour {
	//Load JSON from StreamingAssetPath
	public string filePath = "/fileName.json";
	public delegate void JSONRefreshed();
	public JSONRefreshed jsonRefreshed;

	public JSONNode currentJSON;
	// Use this for initialization
	void Start()
	{
		StartRefreshJSON();
	}

	public void StartRefreshJSON()
	{
		//For external use
		RefreshJSON();
	}

	void RefreshJSON ()
	{
		string currentReadingPath = filePath;
		string jsonText = File.ReadAllText(Application.streamingAssetsPath + currentReadingPath);
		currentJSON = JSON.Parse(jsonText);
		if(jsonRefreshed != null) jsonRefreshed.Invoke();
	}
}
