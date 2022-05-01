using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AnimationType { ONCE, LOOP}


public class AnimationControl : MonoBehaviour
{
    private SpriteRenderer Renderer;
    private List<Sprite> AnimationSprite;
    private float speedAnimation;
    private float RateAnimation;
    private int currentSprite;
    private AnimationType Type;


    public void InitialAnimation(SpriteRenderer _renderer, List<Sprite> _animationSprite, float _speed, AnimationType _type)
    {
        Renderer = _renderer;
        AnimationSprite = _animationSprite;
        speedAnimation = _speed;
        currentSprite = 0;
        Type = _type;
    }

    void Update()
    {
        // Zeitzähler
        RateAnimation += Time.deltaTime;
        if(RateAnimation >= speedAnimation)
        {
            Renderer.sprite = AnimationSprite[currentSprite];
            currentSprite++;
            if(currentSprite >= AnimationSprite.Count)
            {
                if (Type == AnimationType.LOOP)
                {
                    currentSprite = 0;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            RateAnimation = 0;
        }
    }
}
