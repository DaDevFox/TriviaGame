using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using TMPro;

public class NumbersGameSlideController : MonoBehaviour
{
    public event Action OnComplete;
    public int[] points;

    /// <summary>
    /// player from hose perspective the display ill be shon
    /// </summary>
    public int localDisplayPerspective = 0;

    public MCQSlideController mcqController;
    public TextMeshProUGUI playerTurnText;
    public TextMeshProUGUI[] categoryDisplays;

    public static bool gamepadSelecterNavigation = true;

    public Dictionary<int, List<Vector2Int>> usedQuestions = new Dictionary<int, List<Vector2Int>>();

    public Button[,] buttons = new Button[5, 5];

    public static Color usedColor = Color.blue;
    public static Color availableColor = Color.white;

    public static InputAction[] categoryTriggerAction;

    public int askingCategory;
    public int askingIdx;

    public Category[] categories;

    void Start()
    {
        mcqController = RoundMode.instance.MCQSlide;
        mcqController.OnComplete += OnMCQComplete;

        // UpdateDisplay();
    }

    public void Init(){
        usedQuestions.Clear();
        for (int i = 0; i < Game.players.Length; i++)
            usedQuestions.Add(i, new List<Vector2Int>());

        points = new int[Game.players.Length];

        Transform parent = transform.GetChild(2);
        for (int cat = 0; cat < 5; cat++)
        {
            Transform container = parent.GetChild(cat);
            for (int idx = 0; idx < 5; idx++)
                buttons[cat, idx] = container.GetChild(idx).GetComponent<Button>();
        }


        if (gamepadSelecterNavigation)
        {
            Button button = buttons[0, 0];
            if (button != null)
                button.Select();
        }
    }

    public bool CheckComplete(){
        int min = int.MaxValue;
        for (int i = 0; i < Game.players.Length; i++){
            if(usedQuestions[i].Count < min)
                min = usedQuestions[i].Count;
        }
        // Debug.Log($"min val: {min}");
        return min >= 5;
    }

    public void UnlinkMCQEvent() => mcqController.OnComplete -= OnMCQComplete;

    public void OnMCQComplete(){
        if (mcqController.playerAnswers[localDisplayPerspective] == mcqController.current.correct)
        {
            Category old = RoundMode.currentCategory;
            RoundMode.currentCategory = categories[askingCategory];
            RoundMode.AddPoints(localDisplayPerspective, (askingIdx + 1) * 200, (askingIdx + 1) * 200);
            RoundMode.currentCategory = old;
        }
        else{
            RoundMode.AddPoints(localDisplayPerspective, 0, (askingIdx + 1) * 200);
        }

        mcqController.main.SetActive(false);
        mcqController.individualPlayer = -1;

        localDisplayPerspective++;
        localDisplayPerspective %= Game.players.Length;
        UpdateDisplay();

        if (gamepadSelecterNavigation)
        {
            Button button = null;
            for (int i = 0; i < 5; i++)
            {
                if (buttons[0, i].interactable)
                {
                    button = buttons[0, i];
                    break;
                }
            }
            if (button != null)
                button.Select();
        }

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(true);

        if(CheckComplete()){
            RoundMode.CompleteSlide();
        }
    }

    void Update(){
        if(!mcqController.main.activeSelf){
            if (!gamepadSelecterNavigation)
            {
                categoryTriggerAction = new InputAction[] {
                    InputManager.MainInput.MCQMapping.OptionBottom,
                    InputManager.MainInput.MCQMapping.OptionRight,
                    InputManager.MainInput.MCQMapping.OptionLeft,
                    InputManager.MainInput.MCQMapping.OptionTop,
                    InputManager.MainInput.MCQMapping.OptionMisc
                };

                for (int i = 0; i < categoryTriggerAction.Length; i++)
                {
                    InputAction action = categoryTriggerAction[i];
                    if (action.activeControl != null)
                    {
                        if (action.activeControl is ButtonControl buttonControl)
                        {
                            if (buttonControl.wasPressedThisFrame)
                            {
                                for (int idx = 0; idx < 5; idx++)
                                {
                                    if (buttons[i, idx].interactable == true)
                                    {
                                        OnButtonClicked((i * 5) + idx);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void OnButtonClicked(int idx){
        if(!usedQuestions.ContainsKey(localDisplayPerspective))
            usedQuestions.Add(localDisplayPerspective, new List<Vector2Int>());
        usedQuestions[localDisplayPerspective].Add(new Vector2Int(idx/5, idx%5));

        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);

        mcqController.main.SetActive(true);
        mcqController.individualPlayer = localDisplayPerspective;
        mcqController.current = categories[(int)idx/5].RandomChild().Random() as MCQ;
        mcqController.ResetSlide();

        askingCategory = (int)idx / 5;
        askingIdx = (int)idx%5;
    }

    public void UpdateDisplay(){
        categoryTriggerAction = new InputAction[] {
                InputManager.MainInput.MCQMapping.OptionBottom,
                InputManager.MainInput.MCQMapping.OptionRight,
                InputManager.MainInput.MCQMapping.OptionLeft,
                InputManager.MainInput.MCQMapping.OptionTop,
                InputManager.MainInput.MCQMapping.OptionMisc
            };

        for (int cat = 0; cat < 5; cat++){
            for (int idx = 0; idx < 5; idx++){
                buttons[cat, idx].interactable = false;
                buttons[cat, idx].GetComponent<Image>().color = availableColor;
                buttons[cat, idx].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
            }
        }

        for (int cat = 0; cat < 5; cat++){
            buttons[cat,0].interactable = true;
        }

        for (int i = 0; i < usedQuestions[localDisplayPerspective].Count; i++){
            // Debug.Log("past questions: " + usedQuestions[localDisplayPerspective].Count);
            Vector2Int vector = usedQuestions[localDisplayPerspective][i];
            buttons[vector.x, vector.y].interactable = false;
            buttons[vector.x, vector.y].GetComponent<Image>().color = usedColor;
            buttons[vector.x, vector.y].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            if(vector.y + 1 < 5){
                buttons[vector.x, vector.y + 1].interactable = true;
                buttons[vector.x, vector.y + 1].GetComponent<Image>().color = availableColor;
            }
        }

        for (int i = 0; i < categories.Length; i++){
            string text = categories[i].name;
            if(InputManager.GamepadConnected && !gamepadSelecterNavigation)
                text += $" ({categoryTriggerAction[i].GetBindingDisplayString()})";

            categoryDisplays[i].text = text;
        }

        playerTurnText.text = Game.players[localDisplayPerspective].name;
        playerTurnText.color = Game.players[localDisplayPerspective].color;

        if (gamepadSelecterNavigation)
        {
            Button button = null;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (buttons[i, j].interactable)
                    {
                        button = buttons[i, j];
                        break;
                    }
                }
            }
            if (button != null)
                button.Select();
        }
    }
}
