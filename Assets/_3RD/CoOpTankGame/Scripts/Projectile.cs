using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour 
{
	[Header("Stats")]
	public int tankId;						//The tank which shot this projectile.
	public int damage;						//How much damage this projectile will deal on impact.

	[Header("Components / Objects")]
	public GameObject hitParticleEffect;	//The particle effect prefab that will spawn when the projectile hits something.
	public Rigidbody2D rig;					//The Rigidbody2D component of the projectile.
	public Game game;

	private int bounces;					//The amount of times the projectile has bounced off a wall.

	//Called when the projectile's CircleCollider2D component enters a another collider or trigger. 
	//The "col" parameter is the collider that it enters.
	void OnCollisionEnter2D (Collision2D col)
	{
		bounces++;

		if(col.gameObject.tag == "Tank"){						//Is the object we hit a tank?
			Tank tank = col.gameObject.GetComponent<Tank>();	//Get the tank's Tank.cs component.

			if(!game.canDamageOwnTank){							//Can we not damage our own tank?
				if(tank.id != tankId){							//Is the tank we hit not the one that shot this projectile?
					tank.Damage(damage);						//Call the damage function on that tank to damage it.
				}
			}else{
				tank.Damage(damage);
			}
		}

		if(bounces >= game.maxProjectileBounces || col.gameObject.tag == "Tank"){
			//Particle Effect
			GameObject hitEffect = Instantiate(hitParticleEffect, transform.position, Quaternion.identity) as GameObject;	//Spawn the hit particle effect at the position of impact.
			Destroy(hitEffect, 1.0f);	//Destroy that effect after 1 second.

			Destroy(gameObject);		//Destroy the projectile.
		}
	}
}
