using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour 
{
	[HideInInspector]
	public string[] maps;	//An array of all the map names.

	[Header("Components")]
	public MenuUI ui;		//The MenuUI.cs component, located in the Menu game object.

	void Start ()
	{
		LoadMaps();			
	}

	//Called when the scene starts.
	void LoadMaps ()
	{
		TextAsset[] m = Resources.LoadAll<TextAsset>("Maps"); 	//Loads all the maps in as TextAssets from the resources folder.
		maps = new string[m.Length];							//Sets the maps variable to be the same length as m.

		for(int x = 0; x < m.Length; x++){						//Loops through all the maps.
			maps[x] = m[x].name;								//Sets the maps variable to be an array of all the map names.
		}

		ui.LoadMapButtons(maps);								//Calls the LoadMapButtons function in the MenuUI.cs script, sending over the array of maps.
	}

	//Called when the player clicks on a map select button in the play menu.
	//The "map" value, is the name of the map, requested to be loaded.
	public void PlayMap (string map)
	{
		PlayerPrefs.SetString("MapToLoad", map);	//Sets the PlayerPrefs string: "MapToLoad" to be the map name that was sent.
		Application.LoadLevel(1);					//Loads the game.
	}

	//Called when the player wants to quit the game.
	public void QuitGame ()
	{
		Application.Quit();	//Exits out of the game.
	}
}
