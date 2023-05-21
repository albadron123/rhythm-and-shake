using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    SpriteRenderer sr;

    int direction;
    bool isFading = false;
    [SerializeField]
    float fadingParam = 0.5f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
    }

    void Update()
    {
        if (isFading)
        {
            sr.color += new Color(0, 0, 0, 1) * Time.deltaTime * Time.deltaTime * fadingParam * direction;
            if (direction == 1 && sr.color.a == 1) isFading = false;
            if (direction == -1 && sr.color.a == 0) isFading = false;
        }
    }

    public void FadeIn()
    {
        isFading = true;
        direction = -1;
    }

    public void FadeOut()
    {
        isFading = true;
        direction = 1;
    }
}
