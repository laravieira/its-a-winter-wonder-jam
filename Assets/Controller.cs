using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float move_speed = 5;
    public float jump_speed = 4;
    Rigidbody2D rigidbody;
    Collider2D collider;

    HashSet<Collider2D> grounds_set = new HashSet<Collider2D>();

    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody2D>();
        collider = this.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float direction = Input.GetAxis("Horizontal");

        rigidbody.velocity = new Vector2(direction * move_speed, rigidbody.velocity.y);

        if (grounds_set.Count > 0)
        {
            if (Input.GetButton("Jump"))
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, jump_speed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        List<ContactPoint2D> contact_points = new List<ContactPoint2D>();
        int number_of_contacts = collision.GetContacts(contact_points);

        foreach (var contact_point in contact_points)
        {
            if (contact_point.normal.y > Mathf.Cos(3.14f / 6))              // 30 degrees
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
