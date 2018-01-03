using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour 
{
	public GameObject mapButtonParent;	//The game object which the map select buttons will be a child of.

	[Header("Pages")]
	public GameObject menuPage;			
	public GameObject playPage;

	[Header("Components / Objects")]
	public GameObject mapButtonPrefab;	//The map select button prefab.
	public Menu menu;					//The Menu.cs script, located on the Menu game object.

	//Called when the player pressed the PLAY button or the BACK button on the menu.
	//Changes the page to either the menu page, or play page.
	public void SetPage (string page)
	{
		if(page == "menu"){				
			menuPage.active = true;
			playPage.active = false;
		}
		if(page == "play"){
			menuPage.active = false;
			playPage.active = true;
		}
	}

	//Spawns in the map select buttons, located in the play page.
	//The "maps" value, is an array of all the map names.
	public void LoadMapButtons (string[] maps)
	{
		for(int x = 0; x < maps.Length; x++){												//Loops through the map names. And for each map...
			GameObject mapBut = Instantiate(mapButtonPrefab, mapButtonParent.transform.position, Quaternion.identity) as GameObject;	//Spawns the button.
			mapBut.transform.parent = mapButtonParent.transform;							//Sets the button's parent to the mapButtonParent.
			mapBut.transform.localScale = Vector3.one;										//Sets the button's scale to 1.

			mapBut.transform.Find("Text").GetComponent<Text>().text = maps[x];			//Sets the button's text to display the map name.
			string map = maps[x];															//Creates a temp variable which holds the map name.

			//Gets the Button component from the button game object and adds a listener to it, so that when the player clicks on it,
			//the PlayMap function gets called in the Menu.cs script, located in the Menu game object. It also sends over the map name
			//so that the function can then load up that map.
			mapBut.GetComponent<Button>().onClick.AddListener(() => {menu.PlayMap(map);});
		}
	}
}
