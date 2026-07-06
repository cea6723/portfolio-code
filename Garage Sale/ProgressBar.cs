using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private ParticleSystem particles;

    [SerializeField] private float fillSpeed = 0.5f;
    [SerializeField] private float particleDelay = 0.3f;

    private float goal;
    private float timer;

    void Start()
    {
        slider.minValue = 0;
        slider.maxValue = 1;

        slider.value = 0;

        goal = GameManager.instance.CurrentLevel.MoneyGoal;

        if (particles != null )
        {
            particles.Stop();
        }
        //particles.Play();
    }

    void Update()
    {
        float targetProgress = 0;
        if (goal > 0)
        {
            targetProgress = GameManager.instance.Money / goal;
        }

        // interpolate slider towards target
        if (Mathf.Abs(slider.value - targetProgress) > 0.01f)
        {
            slider.value = Mathf.Lerp(slider.value, targetProgress, fillSpeed * Time.deltaTime);

            if (particles != null && !particles.isPlaying)
            {
                particles.Play();
            }
            timer = 0f;
        }
        else
        {
            // add slight delay before stopping particles so they can be seen longer
            timer += Time.deltaTime;
            if (timer > particleDelay)
            {
                if (particles != null && particles.isPlaying)
                {
                    particles.Stop();
                }
            }
        }
    }

    private void OnEnable()
    {
        GameManager.OnLevelLoaded += UpdateGoal;
    }

    /// <summary>
    /// Would unsubscribe before UpdateGoal was called, so commented out for now...
    /// </summary>
    private void OnDisable()
    {
        //GameManager.OnLevelLoaded -= UpdateGoal;
    }

    private void UpdateGoal()
    {  
        goal = GameManager.instance.CurrentLevel.MoneyGoal;
        slider.value = 0;
        Debug.Log("UPDATE GOAL TO " + goal);
    }

}
