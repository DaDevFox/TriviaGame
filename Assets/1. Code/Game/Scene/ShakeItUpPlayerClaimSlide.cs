using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using TMPro;


public class ShakeItUpPlayerClaimSlide : MonoBehaviour
{
    public event Action OnComplete;

    public int[] playerOrder;

    public Dictionary<int, Category> categoriesPicked = new Dictionary<int, Category>();

    public int idx = 0;
    public int playerTurn = 0;

    public int decidingForPlayer = 0;
    public CategoryClaimSlide categoryClaimSlide;

    public TextMeshProUGUI[] playerTexts;
    public TextMeshProUGUI[] categoryTexts;

    public TextMeshProUGUI playerTurnText;
    public int bgOpacity = 123;

    public static InputAction[] claimInputActions;

    public void Init(){
        // asdf
        claimInputActions = new InputAction[] {
                InputManager.MainInput.MCQMapping.OptionBottom,
                InputManager.MainInput.MCQMapping.OptionRight,
                InputManager.MainInput.MCQMapping.OptionLeft,
                InputManager.MainInput.MCQMapping.OptionTop,
                InputManager.MainInput.MCQMapping.OptionMisc
            };

        categoryClaimSlide.gameObject.SetActive(false);
        categoryClaimSlide.OnSelected += OnCategorySelected;
        List<int> points = new List<int>();
        for (int i = 0; i < Game.players.Length; i++)
            points.Add(Game.players[i].points);

        points.Sort();

        List<int> playerOrder = new List<int>();
        for (int i = 0; i < points.Count; i++){

            for (int j = 0; j < Game.players.Length; j++){
                if(points[i] == Game.players[j].points && !playerOrder.Contains(j)){
                    playerOrder.Add(j);
                    break;
                }
            }
        }


        for (int i = 0; i < playerTexts.Length; i++){
            string text = Game.players[i].name;
            if(InputManager.GamepadConnected)
                text += $" ({claimInputActions[i].GetBindingDisplayString()})";
            playerTexts[i].text = text;
            playerTexts[i].transform.parent.parent.GetComponent<Image>().color = Game.players[i].color;
            categoryTexts[i].text = "No Category Selected";
        }

        idx = 0;
        playerTurn = playerOrder[0];

        this.playerOrder = playerOrder.ToArray();

        playerTurnText.text = Game.players[playerTurn].name;
        playerTurnText.color = Game.players[playerTurn].color;

        Debug.Log("turn order:");
        for (int i = 0; i < playerOrder.Count; i++)
            Debug.Log(Game.players[playerOrder[i]].name);

    }

    void Update(){
        for (int i = 0; i < claimInputActions.Length; i++)
        {
            InputAction action = claimInputActions[i];
            if (action.activeControl != null)
            {
                if (action.activeControl is ButtonControl buttonControl)
                {
                    if (buttonControl.wasPressedThisFrame)
                    {
                        ReportPlayerClicked(i);
                    }
                }
            }
        }
    }

    public void UpdateDisplay(){
        for(int i = 0; i < playerTexts.Length; i++){
            string text = Game.players[i].name;
            if(InputManager.GamepadConnected)
                text += $" ({claimInputActions[i].GetBindingDisplayString()})";
            playerTexts[i].text = text;
            Color color = Game.players[i].color;
            color.a = bgOpacity;
            playerTexts[i].transform.parent.parent.GetComponent<Image>().color = color;
            categoryTexts[i].text = categoriesPicked.ContainsKey(i) ? categoriesPicked[i].name : "No Category Selected";
            playerTexts[i].transform.parent.parent.GetComponent<Button>().interactable = !categoriesPicked.ContainsKey(i);
        }

        if (idx < Game.players.Length)
        {
            playerTurnText.text = Game.players[playerTurn].name;
            playerTurnText.color = Game.players[playerTurn].color;
        }else{
            playerTurnText.text = "All categories assigned";
            playerTurnText.color = Color.white;
        }
    }

    public void ReportPlayerClicked(int num){
        if(num == playerTurn)
            return;

        if(categoriesPicked.ContainsKey(num))
            return;

        gameObject.SetActive(false);
        decidingForPlayer = num;

        List<Category> categories = new List<Category>();
        while (categories.Count < 3)
        {
            Category category = Categories.RandomMCQ();
            if (!categories.Contains(category))
                categories.Add(category);
        }

        categoryClaimSlide.categories = categories.ToArray();
        categoryClaimSlide.gameObject.SetActive(true);
        categoryClaimSlide.UpdateDisplay();
    }

    public void OnCategorySelected(Category category)
    {
        categoryClaimSlide.gameObject.SetActive(false);
        gameObject.SetActive(true);

        categoriesPicked[decidingForPlayer] = category;
        idx++;

        if (idx == Game.players.Length)
        {
            StartCoroutine("CompleteProcedure");
        }else
            playerTurn = playerOrder[idx];


        UpdateDisplay();
    }

    public IEnumerator CompleteProcedure(){
        yield return new WaitForSeconds(3f);

        for (int i = 0; i < Game.players.Length; i++)
            Debug.Log($"p{i}: {categoriesPicked[i].name}");

        OnComplete.Invoke();
    }

    public bool CheckComplete(){
        for (int i = 0; i < Game.players.Length; i++)
            if (!categoriesPicked.ContainsKey(i))
                return false;
        return true;
    }
}
