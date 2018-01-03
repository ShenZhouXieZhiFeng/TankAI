using UnityEngine;
using System.Collections;

public class MapCreatorTile : MonoBehaviour 
{
	public TileType type;					//The type of tile this is.
	public SpriteRenderer spriteRenderer;	//The tile's SpriteRenderer component.

	void Start ()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();	//Sets spriteRenderer to the tile's SpriteRenderer component.
	}
}
