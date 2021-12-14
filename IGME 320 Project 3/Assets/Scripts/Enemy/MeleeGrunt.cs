using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeGrunt : Enemy
{
	// Fields for the melee grunt
	[SerializeField]
	private float dashBoost;
	[SerializeField] private SpriteRenderer spRnd;
	[SerializeField] private Sprite walk1;
	[SerializeField] private Sprite walk2;
	[SerializeField] private Sprite leap;


	protected override void Update()
	{
		base.Update();

		if (!onCooldown)
		{
			if (Mathf.Ceil(Time.timeSinceLevelLoad * 10f) % 2 == 0)
			{
				spRnd.sprite = walk1;
			}
			else
			{
				spRnd.sprite = walk2;
			}
		}
		else
		{
			spRnd.sprite = leap;
		}



	}

	/// <summary>
	/// Author: John Heiden
	/// Dashes into the player once the enemy is within the attack range
	/// </summary>
	protected override IEnumerator Attack()
	{
		float sqrDistance = Vector2.SqrMagnitude((Vector2)player.transform.position - position);

		if (sqrDistance < attackRange * attackRange && !onCooldown)
		{
			velocity += dashBoost * ((Vector2)player.transform.position - position).normalized;
			onCooldown = true;
			yield return new WaitForSeconds(cooldownTimer);
			onCooldown = false;
		}
	}

	/// <summary>
	/// Author: John Heiden
	/// Moves the enemy towards the player
	/// </summary>
	protected override Vector2 Seek(Vector2 targetPosition)
	{
		Vector2 desiredVelocity = targetPosition - position;

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

		if(currentHealth <= 0)
		{
			enemyList.Enemies.Remove(this);
			Destroy(gameObject);
		}
	}
}
