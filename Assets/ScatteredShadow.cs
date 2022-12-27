using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatteredShadow : MonoBehaviour
{
    Scattered parent_scatter;
    SpriteRenderer sprite_renderer;

    public float collapse_speed = 1f;

    private void Start()
    {
        parent_scatter = transform.parent.GetComponent<Scattered>();
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    public void Collapse()
    {
        StartCoroutine(CollapseCoroutine());
    }

    IEnumerator CollapseCoroutine()
    {
        Destroy(GetComponent<Collider2D>());

        float move_distance = collapse_speed * Time.deltaTime;

        while (transform.localPosition.magnitude > move_distance)
        {
            transform.localPosition -= transform.localPosition.normalized * move_distance;
            yield return null;

            move_distance = collapse_speed * Time.deltaTime;
        }

        Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        parent_scatter.active_children.Add(sprite_renderer);
        parent_scatter.update_color = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        parent_scatter.active_children.Remove(sprite_renderer);
        parent_scatter.update_color = true;

        Color color = sprite_renderer.color;
        color.a = 0;
        sprite_renderer.color = color;
    }
}
