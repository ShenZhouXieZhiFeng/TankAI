using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MapCreatorUI : MonoBehaviour 
{
	[Header("UI")]
	public InputField mapName;		//The input field where the map name is entered.
	public Text errorText;			//The text that displays errors.

	[Header("Components")]
	public MapCreator mapCreator;	//The MapCreator.cs script that is located in the MapCreator game object.

	//Called when the SAVE & PLAY button gets pressed.
	public void SaveAndPlay ()
	{
		if(!mapCreator.HasMapError() && !mapCreator.HasNameError()){
			mapCreator.SaveMap();
			PlayerPrefs.SetString("MapToLoad", mapName.text);
			#if UNITY_EDITOR
				UnityEditor.AssetDatabase.Refresh();
			#endif
			Application.LoadLevel(1);
		}
	}

	//Called to set the error text.
	public void SetErrorText (string error)
	{
		errorText.text = error;
	}
}
