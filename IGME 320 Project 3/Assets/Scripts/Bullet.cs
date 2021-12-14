using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] private LayerMask enemyLayer;
	[SerializeField] private LayerMask playerLayer;
	[SerializeField] private LayerMask ownerLayer;
	[SerializeField] private int damage;


	private bool shouldDestroyOnCollide = true;


	private float deathClock = 0;

	public int Damage { get => damage; set => damage = value; }
	public bool ShouldDestroyOnCollide { get => shouldDestroyOnCollide; set => shouldDestroyOnCollide = value; }

	private void Start()
	{
		deathClock = Time.timeSinceLevelLoad + 10f;
	}
	private void Update()
	{
		if (deathClock < Time.timeSinceLevelLoad)
		{
			Destroy(this.gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		int layer = 1 << collision.gameObject.layer;

		//Debug.Log("Hit Layer: " + collision.gameObject.layer + ", Owner layer: " + ownerLayer.value);
		
		if (layer == ownerLayer.value)
		{
			return;
		}

		if (layer == enemyLayer.value)
		{
			Enemy enm = collision.gameObject.GetComponent<Enemy>();
			if (enm != null)
			{
				enm.TakeDamage(damage);
				if (shouldDestroyOnCollide) Destroy(gameObject);
				return;
			}

			MinibossScript mBoss = collision.gameObject.GetComponent<MinibossScript>();
			if (mBoss != null)
			{
				mBoss.TakeDamage(damage);
				Destroy(gameObject);
				return;
			}

			BossScript boss = collision.gameObject.GetComponent<BossScript>();
			if (boss != null)
			{
				boss.TakeDamage(damage);
				Destroy(gameObject);
				return;
			}	
		}
		else if (layer == playerLayer.value)
		{
			Player ply = collision.gameObject.GetComponent<Player>();
			if (ply != null)
			{
				ply.TakeDamage(damage);
				Destroy(gameObject);
			}

		}

		
	}
}