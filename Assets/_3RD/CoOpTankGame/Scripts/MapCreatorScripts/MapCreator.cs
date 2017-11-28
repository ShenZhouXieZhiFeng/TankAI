using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.IO;

public class MapCreator : MonoBehaviour 
{
	public GameObject tilesParent;			//The parent object of all the tiles.
	public SpriteRenderer ghostTile;		//The translucent tile which shows the tile that the player's cursor is on.

	[HideInInspector]
	public MapCreatorTile selectedTile;		//The tile that the player is currently hovering over.

	[HideInInspector]
	public string[] existingMapNames;		//An array of all the map names.

	public TileType curTile;				//The currently selected tile. This is what will the player will place down.

	[Header("Sprites")]
	public Sprite wallSprite;				//The wall sprite.
	public Sprite spawnPointSprite;			//The spawn point indicator sprite.

	[Header("Components")]
	public MapCreatorUI ui;					//The MapCreatorUI.cs script located in the MapCreator game object.

	void Start ()
	{
		TextAsset[] maps = Resources.LoadAll<TextAsset>("Maps") as TextAsset[];	//Loads all the maps as textassets.
		existingMapNames = new string[maps.Length];								//Sets the existingMapNames variable to the maps variable.

		for(int x = 0; x < maps.Length; x++){			//Loops through the maps.
			existingMapNames[x] = maps[x].name;			
			Debug.Log(maps[x].name);
		}
	}

	void Update ()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit, 100.0f)){
			if(hit.collider.name.Contains("Tile")){
				if(selectedTile == null || selectedTile.gameObject != hit.collider.gameObject){
					selectedTile = hit.collider.GetComponent<MapCreatorTile>();
				}
			}
		}
			
		if(Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()){	//Is the player pressing left mouse button and not hovering over UI?
			PlaceTile();
		}

		if(Input.GetMouseButton(1) && !EventSystem.current.IsPointerOverGameObject()){	//Is the player pressing right mouse button and not hovering over UI?
			selectedTile.type = TileType.Empty;			
			selectedTile.spriteRenderer.sprite = null;
		}

		if(selectedTile != null){
			ghostTile.transform.position = selectedTile.transform.position;
		}
	}

	//Called when the player presses to place down a tile.
	void PlaceTile ()
	{
		selectedTile.type = curTile;

		if(curTile == TileType.Wall){
			selectedTile.spriteRenderer.sprite = wallSprite;
		}
		if(curTile == TileType.SpawnPoint){
			selectedTile.spriteRenderer.sprite = spawnPointSprite;
		}
	}

	//Called when the player pressed the UI buttons to change their tile they want to place down.
	public void SetCurTile (string type)
	{
		curTile = (TileType)System.Enum.Parse(typeof(TileType), type);

		if(curTile == TileType.Wall){
			ghostTile.sprite = wallSprite;
		}
		if(curTile == TileType.SpawnPoint){
			ghostTile.sprite = spawnPointSprite;
		}
	}

	//Saves the map.
	public void SaveMap ()
	{
		string text = "";

		for(int x = 0; x < tilesParent.transform.childCount; x++){
			GameObject child = tilesParent.transform.GetChild(x).gameObject;
			if(child.GetComponent<MapCreatorTile>().type != TileType.Empty){
				text += child.GetComponent<MapCreatorTile>().type.ToString();
				text += "," + child.transform.position.x + "," + child.transform.position.y + "\n";
			}
		}

		TextWriter tw = new StreamWriter("Assets/CoOpTankGame/Resources/Maps/" + ui.mapName.text + ".txt");
		tw.Write(text);
		tw.Close();
	}

	//Is there an error with the map?
	public bool HasMapError ()
	{
		int spawnPoints = 0;

		for(int x = 0; x < tilesParent.transform.childCount; x++){
			if(tilesParent.transform.GetChild(x).GetComponent<MapCreatorTile>().type == TileType.SpawnPoint){
				spawnPoints++;
			}
		}

		if(spawnPoints < 2){
			ui.SetErrorText("There must be a minimum of 2 spawn points on the map.");
			return true;
		}

		return false;
	}

	//Is there an error in the map name?
	public bool HasNameError ()
	{
		foreach(string name in existingMapNames){
			if(name == ui.mapName.text){
				ui.SetErrorText("Map name already exists.");
				return true;
			}
		}

		if(ui.mapName.text.Length == 0){
			ui.SetErrorText("You have not named the map.");
			return true;
		} 

		return false;
	}
}

public enum TileType {Empty = 0, Wall = 1, SpawnPoint = 2}
