using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scattered : MonoBehaviour
{
    [Tooltip("Scattering initial and end point, considering a rectangle. In local space")]
    public Vector2[] scatter_range = new Vector2[2];
    public Vector2 scatter_density;

    public GameObject game_object;

    public static HashSet<Scattered> scattereds = new HashSet<Scattered>();

    [HideInInspector]
    public HashSet<SpriteRenderer> active_children = new HashSet<SpriteRenderer>();
    [HideInInspector]
    public bool update_color = false;

    void Start()
    {
        int x_steps = Mathf.Abs(Mathf.CeilToInt((scatter_range[1].x - scatter_range[0].x) * scatter_density.x));
        x_steps = x_steps == 0 ? 1 : x_steps;
        float x_distance = scatter_range[1].x - scatter_range[0].x;
        float x_start = scatter_range[0].x;

        int y_steps = Mathf.Abs(Mathf.CeilToInt((scatter_range[1].y - scatter_range[0].y) * scatter_density.y));
        y_steps = y_steps == 0? 1 : y_steps;
        float y_distance = scatter_range[1].y - scatter_range[0].y;
        float y_start = scatter_range[0].y;

        for (int i = 0; i < x_steps; i++)
        {
            float x_position = x_start + (1 + 2 * i) * x_distance / (2 * x_steps);
            for (int j = 0; j < y_steps; j++)
            {
                float y_position = y_start + (1 + 2 * j) * y_distance / (2 * y_steps);

                GameObject shadow = Instantiate(game_object, transform);
                shadow.transform.localPosition = new Vector3(x_position, y_position, 0);
                SpriteRenderer sprite_renderer = shadow.GetComponent<SpriteRenderer>();
                active_children.Add(sprite_renderer);
            }
        }

        update_color = true;
        scattereds.Add(this);
    }

    void UpdateColors()
    {
        update_color = false;
        foreach (var child in active_children)
        {
            Color color = child.color;
            color.a = 1f / active_children.Count;
            child.color = color;
        }

        if (active_children.Count == 1)
            Collapse();
    }

    public void Collapse()
    {
        float random_value = Random.value;
        float total_sum = 0;
        GameObject chosen = null;

        foreach (var child in active_children)
        {
            total_sum += 1f / active_children.Count;
            if (total_sum >= random_value)
            {
                Color color = child.color;
                color.a = 1;
                child.color = color;

                Collider2D collider = child.GetComponent<Collider2D>();
                collider.isTrigger = false;
                collider.callbackLayers |= (1 << 6);            // Shadow scattering layer

                Rigidbody2D rigidbody = child.GetComponent<Rigidbody2D>();
                rigidbody.isKinematic = false;

                child.transform.parent = null;
                child.gameObject.layer = 0;         // default
                Destroy(child.GetComponent<ScatteredShadow>());

                chosen = child.gameObject;
                break;
            }
        }

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).GetComponent<ScatteredShadow>().Collapse();

        transform.parent = chosen.transform;
        transform.localPosition = Vector3.zero;
        update_color = false;
        scattereds.Remove(this);
        Destroy(this);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        active_children.Add(collision.gameObject.GetComponent<SpriteRenderer>());
        UpdateColors();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        SpriteRenderer renderer = collision.GetComponent<SpriteRenderer>();
        Color color = renderer.color;
        color.a = 0;
        renderer.color = color;
        active_children.Remove(renderer);

        UpdateColors();
    }

    void Update()
    {
        if (update_color)
            UpdateColors();
    }
}
