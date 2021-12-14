using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // Fields
    [SerializeField] private int damage;
    private float speed = 0.05f;
    private Vector2 velocity;

    // Properties
    public Vector2 Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Updates the position
        transform.position += new Vector3(velocity.x * speed, velocity.y * speed, 0.0f);

        // If out of frame, delete
        if (transform.position.x > 10.0f || transform.position.y > 10.0f || transform.position.x < -10.0 || transform.position.y < -10.0)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Set the direction of the bullet
    /// </summary>
    /// <param name="direction">The direction to be assigned to the velocity</param>
    public void SetVelocity(Vector2 direction)
    {
        velocity = direction;
    }
}
