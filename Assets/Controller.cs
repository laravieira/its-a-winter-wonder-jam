using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float move_speed = 5;
    public float jump_speed = 4;
    Rigidbody2D rigidbody;

    HashSet<Collider2D> grounds_set = new HashSet<Collider2D>();

    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        float direction = Input.GetAxis("Horizontal");

        rigidbody.velocity = new Vector2(direction * move_speed, rigidbody.velocity.y);

        if (grounds_set.Count > 0)
        {
            if (Input.GetButton("Jump"))
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, jump_speed);
        }
    }

    void Update()
    {
        if (Input.GetButton("Fire1"))
            CollapseClosest();
    }

    void CollapseClosest()
    {
        float closest_distance_sqr = Mathf.Infinity;
        Scattered closest = null;

        foreach (var scattered in Scattered.scattereds)
        {
            float distance_sqr = (transform.position - scattered.transform.position).sqrMagnitude;
            if (distance_sqr < closest_distance_sqr)
            {
                closest_distance_sqr = distance_sqr;
                closest = scattered;
            }
        }

        if (closest)
            closest.Collapse();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        List<ContactPoint2D> contact_points = new List<ContactPoint2D>();
        int number_of_contacts = collision.GetContacts(contact_points);

        foreach (var contact_point in contact_points)
        {
            if (contact_point.normal.y > Mathf.Cos(3.14f / 4))              // 45 degrees
            {
                grounds_set.Add(collision.otherCollider);
                break;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        List<ContactPoint2D> contact_points = new List<ContactPoint2D>();
        int number_of_contacts = collision.GetContacts(contact_points);

        if (number_of_contacts == 0)
            grounds_set.Remove(collision.otherCollider);
    }
}
