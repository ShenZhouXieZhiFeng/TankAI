using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour 
{
	[Header("Player 1 Controls")]
	public KeyCode p1MoveForward;
	public KeyCode p1MoveBackwards;
	public KeyCode p1TurnLeft;
	public KeyCode p1TurnRight;
	public KeyCode p1Shoot;

	[Header("Player 2 Controls")]
	public KeyCode p2MoveForward;
	public KeyCode p2MoveBackwards;
	public KeyCode p2TurnLeft;
	public KeyCode p2TurnRight;
	public KeyCode p2Shoot;

	[Header("Components")]
	public Game game;

	void Update ()
	{
		//Quit Game
		if(Input.GetKeyDown(KeyCode.Escape)){
			game.ui.GoToMenu();
		}

		//Player 1
		game.player1Tank.rig.velocity = Vector2.zero;

		if(game.player1Tank.canMove){
			if(Input.GetKey(p1MoveForward)){
				game.player1Tank.Move(1);
			}
			if(Input.GetKey(p1MoveBackwards)){
				game.player1Tank.Move(-1);
			}
			if(Input.GetKey(p1TurnLeft)){
				game.player1Tank.Turn(-1);
			}
			if(Input.GetKey(p1TurnRight)){
				game.player1Tank.Turn(1);
			}
		}
		if(game.player1Tank.canShoot){
			if(Input.GetKeyDown(p1Shoot)){
				game.player1Tank.Shoot();
			}
		}

		//Player 2
		game.player2Tank.rig.velocity = Vector2.zero;

		if(game.player2Tank.canMove){
			if(Input.GetKey(p2MoveForward)){
				game.player2Tank.Move(1);
			}
			if(Input.GetKey(p2MoveBackwards)){
				game.player2Tank.Move(-1);
			}
			if(Input.GetKey(p2TurnLeft)){
				game.player2Tank.Turn(-1);
			}
			if(Input.GetKey(p2TurnRight)){
				game.player2Tank.Turn(1);
			}
		}
		if(game.player2Tank.canShoot){
			if(Input.GetKeyDown(p2Shoot)){
				game.player2Tank.Shoot();
			}
		}
	}
}
