using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Enemy : MonoBehaviour
{
	// Fields for autonomous agent information
	protected GameObject player;
	[SerializeField]
	protected EnemyList enemyList;
	[SerializeField]
	protected Vector2 position;
	[SerializeField]
	protected Vector2 velocity;
	[SerializeField]
	protected Vector2 acceleration;
	[SerializeField]
	protected float frictionCoefficient;
	[SerializeField]
	protected float maxSpeed;
	[SerializeField]
	protected float maxForce;
	[SerializeField, Min(0.01f)]
	protected float personalSpace;
	[SerializeField, Min(0.01f)]
	protected float mass;
	private Vector4 borderRectangle = new Vector4(-13, 13, -11, 11);

	// Fields for enemy information
	protected int maxHealth = 1;
	[SerializeField]
	protected int attackDamage;
	[SerializeField]
	protected float attackRange;
	[SerializeField]
	protected float cooldownTimer;
	protected bool onCooldown = false;
	protected int currentHealth;

	public int MaxHealth
	{
		get { return maxHealth; }
	}

	public int CurrentHealth
	{
		get { return currentHealth; }
		set { currentHealth = value; }
	}

	public int AttackDamage
	{
		get { return attackDamage; }
	}

	// Start is called before the first Update call
	private void Start()
	{
		position = transform.position;
		player = GameObject.Find("Player");
	}

	// Update is called once per frame
	protected virtual void Update()
	{
		CalculateSteeringForces();
		ApplyFriction(frictionCoefficient);
		StartCoroutine(Attack());
		UpdatePosition();
	}

	abstract protected IEnumerator Attack();

	abstract protected Vector2 Seek(Vector2 targetPosition);

	abstract public void TakeDamage(int amtDamage);

	/// <summary>
	/// Author: John Heiden
	/// Increases the enemy's position by its velocity
	/// </summary>
	protected void UpdatePosition()
	{




		velocity += acceleration * Time.deltaTime;

		position += velocity * Time.deltaTime;

		acceleration = Vector2.zero;

		transform.position = position;
	}

	/// <summary>
	/// Author: John Heiden
	/// Calculates the net acceleration of the enemy based on all steering forces
	/// </summary>
	protected void CalculateSteeringForces()
	{
		ApplyForce(Seek(player.transform.position));

		ApplyForce(Separate(enemyList.Enemies));

		acceleration = Vector2.ClampMagnitude(acceleration, maxForce);

		// Keep enemies in bounds
		if (position.x >= borderRectangle.y || position.x < borderRectangle.x)
		{
			velocity.x = 0;
		}

		if(position.y > borderRectangle.w || position.y < borderRectangle.z)
		{
			velocity.y = 0;
		}
	}

	/// <summary>
	/// Author: John Heiden
	/// Applies force to the enemy to keep some separation between it and other surrounding enemies
	/// </summary>
	/// <param name="enemies">The list of active enemies</param>
	/// <returns>The force the enemy is exerting on other enemies to push them away</returns>
	protected Vector2 Separate(List<Enemy> enemies)
	{
		Vector2 desiredVelocity = Vector2.zero;

		foreach (Enemy enemy in enemies)
		{
			// Don't separate from yourself
			if (enemy == this)
			{
				continue;
			}

			// Using the square distance to avoid square roots, using the square of the personal space to compensate
			float sqrDistance = Vector2.SqrMagnitude(enemy.position - position);

			if (sqrDistance < Mathf.Pow(personalSpace, 2) && sqrDistance > 0f)
			{
				Vector2 enemySeparation = position - enemy.position;
				enemySeparation = enemySeparation.normalized * (personalSpace / sqrDistance);
				desiredVelocity += enemySeparation;
			}
		}

		Vector2 separationForce = desiredVelocity - velocity;

		return separationForce;
	}

	/// <summary>
	/// Author: John Heiden
	/// Applies a force to the enemy's acceleration
	/// </summary>
	/// <param name="force">The force being applied</param>
	protected void ApplyForce(Vector2 force)
	{
		acceleration += force / mass;
	}

	/// <summary>
	/// Author: John Heiden
	/// Applies friction to the enemy's acceleration
	/// </summary>
	/// <param name="frictionCoefficient">The coefficient of friction</param>
	protected void ApplyFriction(float frictionCoefficient)
	{
		acceleration += (-velocity).normalized * frictionCoefficient;
	}
}
