using UnityEngine;
using System.Collections;

public class Tank : MonoBehaviour 
{
	[Header("Stats")]
	public int id;							//The unique identifier for this player.
	public int health;						//The current health of the tank.
	public int maxHealth;					//The maximum health of this tank.
	public int damage;						//How much damage this tank can do when shooting a projectile.
	public float moveSpeed;					//How fast the tank can move.
	public float turnSpeed;					//How fast the tank can turn.
	public float projectileSpeed;			//How fast the tank's projectiles can move.
	public float reloadSpeed;				//How many seconds it takes to reload the tank, so that it can shoot again.
	private float reloadTimer;				//A timer counting up and resets after shooting.

	[HideInInspector]
	public Vector3 direction;				//The direction that the tank is facing. Used for movement direction.

	[Header("Bools")]
	public bool canMove;					//Can the tank move if it wants to?
	public bool canShoot;					//Can the tank shoot if it wants to?

	[Header("Components / Objects")]
	public Rigidbody2D rig;					//The tank's Rigidbody2D component. 
	public GameObject projectile;			//The projectile prefab of which the tank can shoot.
	public GameObject deathParticleEffect;	//The particle effect prefab that plays when the tank dies.
	public Transform muzzle;				//The muzzle of the tank. This is where the projectile will spawn.
	public Game game;						//The Game.cs script, located on the GameManager game object.

	void Start ()
	{
		direction = Vector3.up;	//Sets the tank's direction up, as that is the default rotation of the sprite.
	}

	//Called by the Game.cs script when the game starts.
	public void SetStartValues ()
	{
		//Sets the tank's stats based on the Game.cs start values.
		health = game.tankStartHealth;
		maxHealth = game.tankStartHealth;
		damage = game.tankStartDamage;
		moveSpeed = game.tankStartMoveSpeed;
		turnSpeed = game.tankStartTurnSpeed;
		projectileSpeed = game.tankStartProjectileSpeed;
		reloadSpeed = game.tankStartReloadSpeed;
	}

	void Update ()
	{
		reloadTimer += Time.deltaTime;
	}

	//Called by the Controls.cs script. When a player presses their movement keys, it calls this function
	//sending over a "y" value, set to either 1 or 0, depending if they are moving forward or backwards.
	public void Move (int y)
	{
		rig.velocity = direction * y * moveSpeed * Time.deltaTime;	
	}

	//Called by the Controls.cs script. When a player presses their turn keys, it calls this function
	//sending over an "x" value, set to either 1 or 0, depending if they are moving left or right.
	public void Turn (int x)
	{
		transform.Rotate(-Vector3.forward * x * turnSpeed * Time.deltaTime);
		direction = transform.rotation * Vector3.up;
	}

	//Called by the Contols.cs script. When a player presses their shoot key, it calls this function, making the tank shoot.
	public void Shoot ()
	{
		if(reloadTimer >= reloadSpeed){													//Is the reloadTimer more than or equals to the reloadSpeed? Have we waiting enough time to reload?
			GameObject proj = Instantiate(projectile, muzzle.transform.position, Quaternion.identity) as GameObject;	//Spawns the projectile at the muzzle.
			Projectile projScript = proj.GetComponent<Projectile>();					//Gets the Projectile.cs component of the projectile object.
			projScript.tankId = id;														//Sets the projectile's tankId, so that it knows which tank it was shot by.
			projScript.damage = damage;													//Sets the projectile's damage.
			projScript.game = game;														

			projScript.rig.velocity = direction * projectileSpeed * Time.deltaTime;		//Makes the projectile move in the same direction that the tank is facing.
			reloadTimer = 0.0f;															//Sets the reloadTimer to 0, so that we can't shoot straight away.
		}
	}

	//Called when the tank gets hit by a projectile. It sends over a "dmg" value, which is how much health the tank will lose. 
	public void Damage (int dmg)
	{
		if(game.oneHitKill){	//Is the game set to one hit kill?
			Die();				//If so instantly kill the tank.
			return;
		}

		if(health - dmg <= 0){	//If the tank's health will go under 0 when it gets damaged.
			Die();				//Kill the tank since its health will be under 0.
		}else{					//Otherwise...
			health -= dmg;		//Subtract the dmg from the tank's health.
		}
	}

	//Called when the tank's health is or under 0.
	public void Die ()
	{
		if(id == 0){				//If the tank is player 1.
			game.player2Score++;	//Add 1 to player 2's score.
		}
		if(id == 1){				//If the tank is player 2.
			game.player1Score++;	//Add 1 to player 1's score.
		}

		canMove = false;			//The tank can now not move.
		canShoot = false;			//The tank can now not shoot.

		//Particle Effect
		GameObject deathEffect = Instantiate(deathParticleEffect, transform.position, Quaternion.identity) as GameObject;	//Spawn the death particle effect at the tank's position.
		Destroy(deathEffect, 1.5f);						//Destroy that effect in 1.5 seconds.

		transform.position = new Vector3(0, 100, 0);	//Set the tanks position outside of the map, so that it is not visible when dead.

		StartCoroutine(RespawnTimer());					//Start the RespawnTimer coroutine.
	}

	//Called when the tank has been dead and is ready to rejoin the game.
	public void Respawn ()
	{
		canMove = true;
		canShoot = true;

		health = maxHealth;

		transform.position = game.spawnPoints[Random.Range(0, game.spawnPoints.Count)].transform.position;	//Sets the tank's position to a random spawn point.
	}

	//Called when the tank dies, and needs to wait a certain time before respawning.
	IEnumerator RespawnTimer ()
	{
		yield return new WaitForSeconds(game.respawnDelay);	//Waits how ever long was set in the Game.cs script.

		Respawn();											//Respawns the tank.
	}
}
