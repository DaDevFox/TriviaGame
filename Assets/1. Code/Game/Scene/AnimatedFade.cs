using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedFade : MonoBehaviour
{
    public AnimationCurve curve;
    public float multiplier;

    public float wavelength = 15;

    private float time;

    public Sprite[] slides;
    private int last;
    private int current;

    public static string slidesDir { get; } = "UI/MenuSlides";


    void Start(){
        slides = Resources.LoadAll<Sprite>(slidesDir);
    }

    void Update()
    {
        time += Time.deltaTime;
        if(time > wavelength){
            time %= wavelength;
            current++;
            current %= slides.Length;
            GetComponent<Image>().sprite = slides[current];
        }

        
        GetComponent<Image>().color = new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, curve.Evaluate((time % wavelength)/wavelength) * multiplier);

    }
}
