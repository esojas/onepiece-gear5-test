using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class AnimationScript : MonoBehaviour
{
    [SerializeField] private Animator animatorController;

    private string currentAnimation = "";

    public event Action<string> CurrentAnimationEvent;

    public void ChangeAnimation(string animation, float crossFade = .5f, float time = 0)
    {
        if (time > 0) StartCoroutine(Wait());
        else Validate();


        IEnumerator Wait()
        {
            yield return new WaitForSeconds(time - crossFade);
            Validate();
        }


        void Validate()
        {
            if (currentAnimation != animation)
            {
                currentAnimation = animation;
                animatorController.CrossFade(animation, crossFade);
                CurrentAnimationEvent?.Invoke(currentAnimation);
            }
        }
    }

    private void Awake()
    {
        animatorController = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
