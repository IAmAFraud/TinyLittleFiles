using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedGrunt : Enemy
{
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private SpriteRenderer spRnd;
	[SerializeField] private Sprite walk1;
	[SerializeField] private Sprite walk2;
	[SerializeField] private Sprite leap;

	private List<GameObject> bulletList = new List<GameObject>();
	private bool firstAttack = true;



	protected override void Update()
	{
		base.Update();

		if (Mathf.Ceil(Time.timeSinceLevelLoad * 10f) % 2 == 0)
		{
			spRnd.sprite = walk1;
		}
		else
		{
			spRnd.sprite = walk2;
		}
	}


	/// <summary>
	/// Author: John Heiden
	/// Launches a projectile at the player once it is within the attack range
	/// </summary>
	protected override IEnumerator Attack()
	{
		if (firstAttack)
		{
			yield return new WaitForSeconds(cooldownTimer);
			firstAttack = false;
		}
		else if (!onCooldown)
		{
			bulletList.Add(Instantiate(bulletPrefab, position, Quaternion.identity));
			Vector2 bulletVelocity = ((Vector2)player.transform.position - position).normalized * 10.0f;
			bulletList[bulletList.Count - 1].GetComponent<Rigidbody2D>().velocity = bulletVelocity;
			bulletList[bulletList.Count - 1].GetComponent<Bullet>().Damage = 1;

			onCooldown = true;
			yield return new WaitForSeconds(cooldownTimer);
			onCooldown = false;
		}
	}

	/// <summary>
	/// Author: John Heiden
	/// Moves the enemy towards the player, up to a certain radius around the player
	/// </summary>
	protected override Vector2 Seek(Vector2 targetPosition)
	{
		Vector2 desiredVelocity = targetPosition - position;

		float sqrDistance = Vector2.SqrMagnitude(desiredVelocity);

		if (sqrDistance < attackRange * attackRange)
		{
			desiredVelocity *= -1;
		}

		desiredVelocity = desiredVelocity.normalized * maxSpeed;

		Vector2 seekingForce = desiredVelocity - velocity;
		if (seekingForce.x > 0)
		{
			spRnd.gameObject.transform.localScale = new Vector3(-1, 1, 1);
		}
		else
		{
			spRnd.gameObject.transform.localScale = new Vector3(1, 1, 1);
		}
		return seekingForce;
	}

	/// <summary>
	/// Author: John Heiden
	/// Receives damage from the player
	/// </summary>
	public override void TakeDamage(int amtDamage)
	{
		currentHealth -= amtDamage;

		if (currentHealth <= 0)
		{
			enemyList.Enemies.Remove(this);
			Destroy(gameObject);
		}
	}
}
