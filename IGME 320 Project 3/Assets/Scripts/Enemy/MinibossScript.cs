using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinibossScript : Enemy
{
    private enum MinibossAttack
    {
        Attack1,
        Attack2,
        Attack3
    };

    // Fields
    [SerializeField] private int minibossHealth;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject player;
    [SerializeField] private RectTransform hpTransform;
    private MinibossAttack curAttack = MinibossAttack.Attack1;
    private bool attackRunning = false;
    private GameObject bullet;
    private float delta = 45.0f;

	public GameObject Player { get => player; set => player = value; }

    // Update is called once per frame
    protected override void Update()
    {

        hpTransform.transform.localScale = new Vector3((minibossHealth / 50f), 1f, 1f);
        if (!attackRunning)
        {
            switch (curAttack)
            {
                case MinibossAttack.Attack1:
                    StartCoroutine("AttackPattern1");
                    attackRunning = true;
                    break;

                case MinibossAttack.Attack2:
                    StartCoroutine("AttackPattern2");
                    attackRunning = true;
                    break;

                case MinibossAttack.Attack3:
                    StartCoroutine("AttackPattern3");
                    attackRunning = true;
                    break;
            }
        }
    }


    // Three Attack Patterns

    /// <summary>
    /// Creates bullets in a cross pattern
    /// </summary>
    private IEnumerator AttackPattern1()
    {
        for (int i = 0; i < 30; i++)
        {
            bullet = Instantiate(bulletPrefab);
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(1.0f, 0.0f) * bulletSpeed;
            bullet = Instantiate(bulletPrefab);
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 1.0f) * bulletSpeed;
            bullet = Instantiate(bulletPrefab);
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(-1.0f, 0.0f) * bulletSpeed;
            bullet = Instantiate(bulletPrefab);
            bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, -1.0f) * bulletSpeed;

            yield return new WaitForSeconds(0.25f);
        }

        // Clears the bullet variable
        bullet = null;

        yield return new WaitForSeconds(3);

        // Randomly Selects the next attack
        float randNum = Random.Range(0.0f, 1.0f);
        if (randNum < 0.5f) curAttack = MinibossAttack.Attack2;
        else curAttack = MinibossAttack.Attack3;

        attackRunning = false;
    }

    /// <summary>
    /// Creates bullets in a circle
    /// </summary>
    private IEnumerator AttackPattern2()
    {
        float angle = 0.0f;
        for (int i = 0; i < 15; i++)
        {
            angle = 0.0f + 20.0f * i;
            for (int j = 0; j < 8; j++)
            {
                bullet = Instantiate(bulletPrefab);
                bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sin(Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle)) * bulletSpeed;
                angle += delta;
            }
            yield return new WaitForSeconds(0.20f);
        }

        // Clears the bullet variable
        bullet = null;

        yield return new WaitForSeconds(3);

        // Randomly Selects Next Attack
        float randNum = Random.Range(0.0f, 1.0f);
        if (randNum < 0.1) curAttack = MinibossAttack.Attack1;
        else if (randNum < 0.2) curAttack = MinibossAttack.Attack2;
        else curAttack = MinibossAttack.Attack3;

        curAttack = MinibossAttack.Attack3;
        attackRunning = false;
    }

    /// <summary>
    /// Creates a line of slightly varied bullets directed at the player
    /// </summary>
    private IEnumerator AttackPattern3()
    {
        int numShots = 0;
        while (numShots < 20)
        {
            if (bullet == null)
            {
                bullet = Instantiate(bulletPrefab);
                Vector2 dir = (new Vector2(player.transform.position.x, player.transform.position.y) - new Vector2(transform.position.x, transform.position.y)).normalized;
                bullet.GetComponent<Rigidbody2D>().velocity = dir * bulletSpeed;
                numShots++;
            }
            else if (bullet.transform.position.x > 0.3 || bullet.transform.position.y > 0.3 || bullet.transform.position.x < -0.3 || bullet.transform.position.y < -0.3)
            {
                bullet = null;
            }

            yield return null;
        }

        yield return new WaitForSeconds(3);

        // Randomly Selects Next Attack
        float randNum = Random.Range(0.0f, 1.0f);
        if (randNum < 0.5) curAttack = MinibossAttack.Attack1;
        else if (randNum < 0.9) curAttack = MinibossAttack.Attack2;
        else curAttack = MinibossAttack.Attack3;

        attackRunning = false;
    }



	protected override IEnumerator Attack()
	{
        yield return null;
	}

	protected override Vector2 Seek(Vector2 targetPosition)
	{
        return Vector2.zero;
	}

	public override void TakeDamage(int amtDamage)
	{

        minibossHealth -= amtDamage;

        if (minibossHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
