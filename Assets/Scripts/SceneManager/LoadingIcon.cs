using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LoadingIcon : MonoBehaviour
{
    [SerializeField] float TweenTime = 1.5f;
    [SerializeField] float TweenHeight = 0.1f;
    [SerializeField] AnimationCurve animationCurve;

    private Tweener tween; // Store reference to the active tween

    private void Start()
    {
        // Start the tween and store the reference
        tween = transform.DOMoveY(transform.position.y + TweenHeight, TweenTime)
            .SetEase(animationCurve)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        // Stop the tween when the object is destroyed
        if (tween != null && tween.IsActive())
        {
            tween.Kill();
        }
    }
}
