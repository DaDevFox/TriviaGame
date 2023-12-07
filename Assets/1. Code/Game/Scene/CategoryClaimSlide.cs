using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CategoryClaimSlide : MonoBehaviour
{
    public event Action<Category> OnSelected;

    public int playerChoosing = -1;

    /// <summary>
    /// maximum of 3
    /// </summary>
    public Category[] categories;
    public TextMeshProUGUI[] CategoryTextDislays;

    public void UpdateDisplay(){
        CategoryTextDislays[0].transform.parent.GetComponent<Button>().Select();
        for (int i = 0; i < categories.Length; i++){
            CategoryTextDislays[i].text = categories[i].name;
        }
    }

    public void OnCategoryButtonClicked(int catIdx){
        OnSelected?.Invoke(categories[catIdx]);
    }
}
