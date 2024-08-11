using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Common.UI;
using TMPro;
using UnityEngine.UI;

public abstract class RoundController
{
    public string name = "Round %s";

    public int numSlides;
    public int currSlide;

    /// <summary>
    /// Procedure for end of a slide
    /// </summary>
    public abstract void CompleteSlide();
    /// <summary>
    /// Procedure for initializing the next slide
    /// </summary>
    public abstract void NextSlide();


    public abstract void StartRound();

    public abstract void EndRound();



    // /// <summary>
    // /// -1 for player queries a group decision and no specified allowed categories means all are allowed
    // /// </summary>
    // /// <param name="player"></param>
    // /// <param name="possible"></param>
    // /// <returns></returns>
    // protected Category QueryCategory(int player, params string[] allowed){

    // }
}

public class ShakeItUpRound : RoundController
{
    public int points = 400;

    public bool selectionPhase = false;

    private Dictionary<int, Category> playerCategories;

    public MCQ currentQuestion;

    public int currentPlayer = 0;


    public ShakeItUpRound(int numQuestions)
    {
        numSlides = 4 * numQuestions;
        name = "Shake It Up";
    }

    public override void StartRound()
    {
        selectionPhase = true;
        RoundMode.instance.ShakeItUpSlide.gameObject.SetActive(true);
        RoundMode.instance.ShakeItUpSlide.Init();
        RoundMode.instance.ShakeItUpSlide.OnComplete += OnPlayersClaimed;
        RoundMode.instance.MCQSlide.OnComplete += RoundMode.CompleteSlide;
    }

    public void OnPlayersClaimed()
    {
        RoundMode.instance.ShakeItUpSlide.gameObject.SetActive(false);
        RoundMode.instance.MCQSlide.main.SetActive(true);
        playerCategories = RoundMode.instance.ShakeItUpSlide.categoriesPicked;
        for (int i = 0; i < Game.players.Length; i++)
        {
            Debug.Log(RoundMode.instance.ShakeItUpSlide.categoriesPicked[i].name);
        }

        currentPlayer = 0;
        RoundMode.CompleteSlide();
    }

    public override void CompleteSlide()
    {
        if (RoundMode.instance.MCQSlide.playerAnswers.Length > 0)
        {
            if (RoundMode.instance.MCQSlide.playerAnswers[currentPlayer] == RoundMode.instance.MCQSlide.current.correct)
                RoundMode.AddPoints(currentPlayer, points, points);
            // else
            //     RoundMode.AddPoints(currentPlayer, 0, points);

        }

        currentPlayer++;
        currentPlayer %= Game.players.Length;

    }

    public override void NextSlide()
    {
        Question question = playerCategories[currentPlayer].RandomChild().Random();
        RoundMode.instance.MCQSlide.individualPlayer = currentPlayer;
        RoundMode.instance.MCQSlide.current = question as MCQ;
        RoundMode.instance.MCQSlide.ResetSlide();
    }

    public override void EndRound()
    {
        RoundMode.instance.ShakeItUpSlide.OnComplete -= OnPlayersClaimed;
        RoundMode.instance.MCQSlide.OnComplete -= RoundMode.CompleteSlide;
    }
}

public class SimpleMCQRound : RoundController
{
    public bool individualQuestions = false;
    public bool individualCategories = false;
    public bool anonymous = false;


    public int basePoints = 200;
    public int pointIncrement = 0;

    /// <summary>
    /// Points awardable for the current question
    /// </summary>
    public int currentPoints;
    public int[] playerAnswers;

    public MCQ currentQuestion;

    private string[] categoryChoices = new String[0];


    public SimpleMCQRound(string name, int points, int pointIncrement, int numQuestions, bool individualQuestions = false, bool individualCategories = true, bool anonymous = false, params string[] categoryChoices)
    {
        this.name = name;
        this.numSlides = numQuestions;
        this.currSlide = 0;
        this.individualQuestions = individualQuestions;
        this.individualCategories = individualCategories;
        this.anonymous = anonymous;
        this.basePoints = points;
        this.pointIncrement = pointIncrement;

        if (categoryChoices.Length > 0)
        {
            this.categoryChoices = new string[categoryChoices.Length];
            for (int i = 0; i < categoryChoices.Length; i++)
                this.categoryChoices[i] = categoryChoices[i];
        }
    }

    public override void CompleteSlide()
    {
        playerAnswers = RoundMode.instance.MCQSlide.playerAnswers;
        for (int i = 0; i < playerAnswers.Length; i++)
        {
            if (playerAnswers[i] != -1 && currentQuestion.correct == playerAnswers[i])
                RoundMode.AddPoints(i, currentPoints, currentPoints);
            else
                RoundMode.AddPoints(i, 0, currentPoints);
        }

        ClearAnswers();
    }

    public Category RandomCategory() => (categoryChoices.Length > 0 ? Categories.categories[categoryChoices[(int)(UnityEngine.Random.Range(0f, categoryChoices.Length - 0.00001f))]] : Categories.categories[Categories.superCategories[(int)(UnityEngine.Random.Range(0f, Categories.superCategories.Count - 0.00001f))]]).RandomChild();

    public override void NextSlide()
    {
        this.currentPoints = basePoints + (pointIncrement * currSlide);
        if (individualCategories || RoundMode.currentCategory == null)
            RoundMode.currentCategory = RandomCategory();
        currentQuestion = RoundMode.currentCategory.Random() as MCQ;

        RoundMode.instance.MCQSlide.blindAnswers = anonymous;
        RoundMode.instance.MCQSlide.individualPlayer = -1;
        RoundMode.instance.MCQSlide.current = currentQuestion;
        RoundMode.instance.MCQSlide.ResetSlide();
        if (Game.currentMPType == Game.MPType.TurnBased)
            RoundMode.instance.MCQSlide.playerTurn = 0;
    }

    public override void StartRound()
    {
        RoundMode.currentCategory = RandomCategory();
        currentQuestion = RoundMode.currentCategory.Random() as MCQ;

        RoundMode.instance.MCQSlide.OnComplete += RoundMode.CompleteSlide;

        this.currentPoints = basePoints;

        RoundMode.instance.MCQSlide.blindAnswers = anonymous;
        RoundMode.instance.MCQSlide.main.SetActive(true);
        RoundMode.instance.MCQSlide.individualPlayer = -1;
        RoundMode.instance.MCQSlide.current = currentQuestion;
        RoundMode.instance.MCQSlide.ResetSlide();
        if (Game.currentMPType == Game.MPType.TurnBased)
            RoundMode.instance.MCQSlide.playerTurn = 0;
    }

    public override void EndRound()
    {
        RoundMode.instance.MCQSlide.OnComplete -= RoundMode.CompleteSlide;
        RoundMode.instance.MCQSlide.main.SetActive(false);
    }

    public void ClearAnswers()
    {
        for (int i = 0; i < playerAnswers.Length; i++)
            playerAnswers[i] = -1;
    }
}

public class NumbersGameRound : RoundController
{
    public NumbersGameRound()
    {
        this.currSlide = 0;
        this.numSlides = 1;
        name = "Numbers Game";
    }

    public override void CompleteSlide()
    {

    }

    public override void NextSlide()
    {
        List<Category> categories = new List<Category>();
        while (categories.Count < 5)
        {
            Category category = Categories.RandomMCQ();
            if (!categories.Contains(category))
                categories.Add(category);
        }

        RoundMode.instance.NumbersGameSlide.categories = categories.ToArray();
        RoundMode.instance.NumbersGameSlide.localDisplayPerspective = 0;
        RoundMode.instance.NumbersGameSlide.Init();
        RoundMode.instance.NumbersGameSlide.UpdateDisplay();
    }

    public override void StartRound()
    {
        List<Category> categories = new List<Category>();
        while (categories.Count < 5)
        {
            Category category = Categories.Random();
            if (!categories.Contains(category))
                categories.Add(category);
        }

        RoundMode.instance.NumbersGameSlide.categories = categories.ToArray();
        RoundMode.instance.NumbersGameSlide.localDisplayPerspective = 0;
        RoundMode.instance.NumbersGameSlide.Init();
        RoundMode.instance.NumbersGameSlide.UpdateDisplay();
        RoundMode.instance.NumbersGameSlide.gameObject.SetActive(true);


        RoundMode.instance.NumbersGameSlide.OnComplete += RoundMode.CompleteSlide;
    }

    public override void EndRound()
    {
        RoundMode.instance.NumbersGameSlide.gameObject.SetActive(false);
        RoundMode.instance.NumbersGameSlide.OnComplete -= RoundMode.CompleteSlide;
        RoundMode.instance.NumbersGameSlide.UnlinkMCQEvent();
    }
}


public class RoundMode : MonoBehaviour
{
    public static RoundMode instance;
    public static int currentRoundIdx = 1;

    public static Dictionary<int, RoundController[]> roundTypes { get; set; } = new Dictionary<int, RoundController[]>{
         {1, new RoundController[] { new SimpleMCQRound("Themed Questions", 200, 0, 5, false, false, false), new SimpleMCQRound("Randomagories", 200, 0, 5, false, true)}},
         {2, new RoundController[] { new NumbersGameRound()}},
         {3, new RoundController[] { new SimpleMCQRound("Think on it", 1000, 500, 3, false, false, true, "brain-teasers") }},
        //{4, new RoundController[] { new ShakeItUpRound(5)}}
    };

    [Header("Scene Controllers")]
    public HorizontalProgressBar roundProgressBar;
    public TextMeshProUGUI roundNameText;
    public Scoreboard scoreboard;
    public TextMeshProUGUI roundNameText_transition;

    public GameObject RoundTransitionUI;

    public static RoundController currentRound;

    [Header("Game Controllers")]
    public GameObject controllersContainer;
    public MCQSlideController MCQSlide;
    public NumbersGameSlideController NumbersGameSlide;
    public CategoryClaimSlide CategoryClaimSlide;
    public ShakeItUpPlayerClaimSlide ShakeItUpSlide;


    public static Category currentCategory;

    public static void CompleteGame()
    {
        int victor = 0;
        for (int i = 0; i < Game.players.Length; i++)
            if (Game.players[i].points > Game.players[victor].points)
                victor = i;

        Reader.CreateEndOfGameData(victor);
        Debug.Log("Game Complete");
    }


    public static void CompleteSlide()
    {
        currentRound.CompleteSlide();
        currentRound.currSlide++;

        if (currentRound.currSlide >= currentRound.numSlides)
        {
            currentRoundIdx++;
            Common.Utils.GameObjectProxy.proxy.StartCoroutine(RoundChange());
        }
        else
            currentRound.NextSlide();
    }

    public static IEnumerator RoundChange()
    {
        currentRound.EndRound();

        instance.controllersContainer.SetActive(false);
        instance.RoundTransitionUI.SetActive(true);
        instance.roundProgressBar.progress = 0f;
        instance.roundNameText.text = "";
        float transitionTime = 3f;
        Scoreboard scoreboard = instance.RoundTransitionUI.transform.GetChild(0).GetChild(1).GetComponent<Scoreboard>();
        scoreboard.retractedBars = false;
        scoreboard.Init();
        instance.roundNameText_transition.text = currentRound.name;


        yield return new WaitForSecondsRealtime(transitionTime);



        // sho end of round summaries
        yield return new WaitForSeconds(3f);

        if (currentRoundIdx > roundTypes.Count || roundTypes[currentRoundIdx].Length == 0)
        {
            CompleteGame();
            yield break;
        }
        currentRound = roundTypes[currentRoundIdx][(int)UnityEngine.Random.Range(0f, roundTypes[currentRoundIdx].Length - 0.0001f)];
        instance.roundNameText_transition.text = $"Next Round: {currentRound.name}";

        scoreboard.retractedBars = true;
        yield return new WaitForSecondsRealtime(transitionTime);


        instance.controllersContainer.SetActive(true);
        instance.RoundTransitionUI.SetActive(false);
        currentRound.StartRound();
    }

    public static void AddPoints(int player, int amount, int possibleAmount)
    {
        Player _player = Game.players[player];
        _player.points += amount;
        if (!_player.pointsByCategory.ContainsKey(currentCategory.parentId))
            _player.pointsByCategory.Add(currentCategory.parentId, 0);
        _player.pointsByCategory[currentCategory.parentId] = _player.pointsByCategory[currentCategory.parentId] + amount;

        if (!_player.possiblePointsByCategory.ContainsKey(currentCategory.parentId))
            _player.possiblePointsByCategory.Add(currentCategory.parentId, 0);
        _player.possiblePointsByCategory[currentCategory.parentId] = _player.possiblePointsByCategory[currentCategory.parentId] + possibleAmount;

        instance.scoreboard.OnPointsChange(player, amount);
    }

    public static void AddPoints(int player, int amount, Category category)
    {
        Player _player = Game.players[player];
        _player.points += amount;
        if (!_player.pointsByCategory.ContainsKey(category.parentId))
            _player.pointsByCategory.Add(category.parentId, 0);

        if (!_player.possiblePointsByCategory.ContainsKey(currentCategory.parentId))
            _player.possiblePointsByCategory.Add(currentCategory.parentId, 0);
        _player.possiblePointsByCategory[currentCategory.parentId] = _player.possiblePointsByCategory[currentCategory.parentId] + amount;

        _player.pointsByCategory[category.parentId] = _player.pointsByCategory[category.parentId] + amount;
    }

    Player CreatePlayer(Profile input, int id) => new Player()
    {
        color = input.color,
        id = id,
        profileId = input.id,
        name = input.name,
    };

    void Start()
    {
        instance = this;
        Reader.ParseData();
        int i = 0;
        foreach (Profile profile in Profiles.registry.Values.Take(4))
        {
            Debug.Log(i);
            // todo fill in by menu selection
            Game.players[i] = CreatePlayer(profile, i);
            i++;
        }

        Init();
    }

    private void Init()
    {
        currentRound = roundTypes[1][(int)UnityEngine.Random.Range(0f, roundTypes[1].Length - 0.0001f)];
        currentRound.StartRound();



        scoreboard.Init();
    }

    void Update()
    {
        roundProgressBar.progress = currentRound != null ? (float)currentRound.currSlide / (float)currentRound.numSlides : 0f;
        roundNameText.text = currentRound.name;
    }
}
