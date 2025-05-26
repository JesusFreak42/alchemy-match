using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    
    [SerializeField] private Slider slider;

    public float GetValue(){
        return slider.value;
    }

    public void SetValue(float val){
        if (val < slider.minValue){
            slider.value = slider.minValue;
            return;
        }
        
        if (val > slider.maxValue){
            slider.value = slider.maxValue;
            return;
        }

        slider.value = val;
    }

    public float GetMinValue(){
        return slider.minValue;
    }

    public void SetMinValue(float val){
        if (val < 0) return;

        slider.minValue = val;
    }

    public float GetMaxValue(){
        return slider.maxValue;
    }

    public void SetMaxValue(float val){
        if (val < 0) return;
        // Debug.Log("setting max value " + val);
        slider.maxValue = val;
    }

    public void IncreaseValue(float val){
        if (slider.value + val < slider.minValue){
            slider.value = slider.minValue;
            return;
        }
        
        if (slider.value + val > slider.maxValue){
            slider.value = slider.maxValue;
            return;
        }

        slider.value += + val;
    }

    public void DecreaseValue(float val){
        if (slider.value + val < slider.minValue){
            slider.value = slider.minValue;
            return;
        }
        
        if (slider.value + val > slider.maxValue){
            slider.value = slider.maxValue;
            return;
        }

        slider.value -= val;
    }

}
