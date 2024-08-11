using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Common.Assets;

public class Category
{
    public string name => Categories.GetNameFor(id);
    public string id;
    public string parentId => parent != null ? parent.id : id;
    public List<Question> questions = new List<Question>();

    public bool MCQOnly { get; set; } = false;
    public bool FRQOnly { get; set; } = false;

    public List<Category> children = new List<Category>();
    public Category parent;


    public Category RandomChild() => children.Count > 0 ? children[(int)UnityEngine.Random.Range(0f, children.Count - 0.000000000000001f)] : this;

    public Question Random() => questions[(int)UnityEngine.Random.Range(0f, questions.Count - 0.0001f)];
}

public abstract class Question
{
    public string supercategory;
    public string subcategory;
    public string groupingFormat;

    public string text;

    public string explanation;
    public string hint;

    public int localId;

    public string fullEncoding;
}

public class MCQ : Question
{
    public string[] options;
    public int correct;
}

public class FRQ : Question
{
    public string correct;
}

public static class Profiles
{
    public static Dictionary<int, Profile> registry = new Dictionary<int, Profile>();

}

public class Profile
{
    public int id;
    public string name;
    public Color color;

    public Dictionary<string, int> points = new Dictionary<string, int>();
    public Dictionary<string, int> possiblePoints = new Dictionary<string, int>();
    public Dictionary<string, int> questionsCorrect = new Dictionary<string, int>();
    public Dictionary<string, int> questionsSeen = new Dictionary<string, int>();
    public int gamesWon;
    public int gamesPlayed;

    public string FileText()
    {
        string text = "";
        text += $"{name}\n";
        text += $"{(int)(color.r * 255f)},{(int)(color.g * 255f)},{(int)(color.b * 255f)}\n";
        text += $"points:\n";
        foreach (KeyValuePair<string, int> pointCategory in points)
            text += $"\t{pointCategory.Key}:{pointCategory.Value}\n";
        text += $"possiblePoints:\n";
        foreach (KeyValuePair<string, int> pointCategory in possiblePoints)
            text += $"\t{pointCategory.Key}:{pointCategory.Value}\n";
        text += $"questionsCorrect:\n";
        foreach (KeyValuePair<string, int> pointCategory in questionsCorrect)
            text += $"\t{pointCategory.Key}:{pointCategory.Value}\n";
        text += $"questionsSeen:\n";
        foreach (KeyValuePair<string, int> pointCategory in questionsSeen)
            text += $"\t{pointCategory.Key}:{pointCategory.Value}\n";

        text += $"gamesWon:{gamesWon}\n";
        text += $"gamesPlayed:{gamesPlayed}\n";

        return text;
    }

    public void ReadFileText(string text)
    {
        string[] lines = text.Split('\n');
        string name = lines[0];

        string[] colorComponents = lines[1].Split(',');
        Color color = new Color(int.Parse(colorComponents[0]), int.Parse(colorComponents[1]), int.Parse(colorComponents[2]));

        Dictionary<string, int> points = new Dictionary<string, int>();
        Dictionary<string, int> possiblePoints = new Dictionary<string, int>();
        Dictionary<string, int> questionsCorrect = new Dictionary<string, int>();
        Dictionary<string, int> questionsSeen = new Dictionary<string, int>();

        int currBaseline = 2;
        try
        {
            if (!lines[currBaseline].StartsWith("\t") && lines[currBaseline + 1].StartsWith("\t"))
            {
                currBaseline++;
                while (lines[currBaseline].StartsWith("\t"))
                {
                    string[] split = lines[currBaseline].Replace("\t", "").Split(':');
                    points.Add(split[0], int.Parse(split[1]));

                    currBaseline++;
                }
            }

            if (!lines[currBaseline].StartsWith("\t") && lines[currBaseline + 1].StartsWith("\t"))
            {
                currBaseline++;
                while (lines[currBaseline].StartsWith("\t"))
                {
                    string[] split = lines[currBaseline].Replace("\t", "").Split(':');
                    possiblePoints.Add(split[0], int.Parse(split[1]));

                    currBaseline++;
                }
            }
            if (!lines[currBaseline].StartsWith("\t") && lines[currBaseline + 1].StartsWith("\t"))
            {
                currBaseline++;
                while (!lines[currBaseline].StartsWith("\t"))
                {
                    string[] split = lines[currBaseline].Replace("\t", "").Split(':');
                    questionsCorrect.Add(split[0], int.Parse(split[1]));

                    currBaseline++;
                }
            }
            if (!lines[currBaseline].StartsWith("\t") && lines[currBaseline + 1].StartsWith("\t"))
            {
                currBaseline++;
                while (!lines[currBaseline].StartsWith("\t"))
                {
                    string[] split = lines[currBaseline].Replace("\t", "").Split(':');
                    questionsSeen.Add(split[0], int.Parse(split[1]));

                    currBaseline++;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error at line {currBaseline + 1}: {ex}");
        }

        this.name = name;
        this.color = color;
        this.points = points;
        this.possiblePoints = possiblePoints;
        this.questionsCorrect = questionsCorrect;
        this.questionsSeen = questionsSeen;

        //TODO: games won and games played
    }
}

public static class Categories
{
    public static Dictionary<string, Category> categories = new Dictionary<string, Category>();

    public static List<string> blacklist;
    public static List<string> mixed;
    public static List<string> special;

    public static Dictionary<string, string> subcategoryLookup;

    public static List<string> superCategories;

    public static Dictionary<string, string> categoryNames;
    public static Dictionary<string, string> categoryDescriptions;

    public static string GetNameFor(string id) => categoryNames[id];
    public static string GetDescriptionFor(string id) => categoryDescriptions[id];

    public static bool IsSpecialCategory(string subcategory) => blacklist.Contains(subcategory) || mixed.Contains(subcategory) || special.Contains(subcategory);


    /// <summary>
    /// returns the category key for the subcategory key given
    /// </summary>
    /// <param name="subcategory"></param>
    /// <returns></returns>
    public static string GetSuperCategoryFor(string subcategory) => subcategoryLookup[subcategory];

    public static Category Random() => categories[superCategories[(int)UnityEngine.Random.Range(0f, superCategories.Count - 0.000000000000001f)]];

    public static Category RandomMCQ()
    {
        List<string> _categories = new List<string>();
        foreach (string categoryId in superCategories)
            if (categories[categoryId].MCQOnly)
                _categories.Add(categoryId);

        return categories[_categories[(int)UnityEngine.Random.Range(0f, _categories.Count - 0.01f)]];
    }

    public static Category RandomFRQ()
    {
        List<string> _categories = new List<string>(superCategories);
        foreach (string categoryId in superCategories)
            if (!categories[categoryId].FRQOnly)
                _categories.Remove(categoryId);

        return categories[_categories[(int)UnityEngine.Random.Range(0f, _categories.Count - 0.01f)]];
    }

} //TODO: Complete random mcq frqs



public static class Reader
{
    public static bool saveStats = true;

    public static string questionDataSource { get; } = "BaseData/categories";
    public static string profilesDataSource { get; } = "Profiles";
    public static string dataDir { get; } = "QuestionData";
    public static string manifestDir { get; } = "Config/question_manifest.txt";
    public static string categoryDescriptionsDir { get; } = "Config/category_descriptions.txt";

    public static string questionFileName { get; } = "questions.txt";
    public static string metadataFileName { get; } = "metadata.txt";

    // public static void ConvertPasteDump(){
    //     TextAsset pasteDump = Assets.LoadStreaming<TextAsset>("BaseData/PasteDumps");

    // }


    public static void CreateEndOfGameData(int victor)
    {
        if (!saveStats)
            return;

        int i = 0;
        foreach (Player player in Game.players)
        {
            Profile profile = Profiles.registry[player.profileId];

            profile.gamesPlayed += 1;
            if (i == victor)
                profile.gamesWon += 1;

            foreach (KeyValuePair<string, int> pointPair in player.pointsByCategory)
            {
                if (!profile.points.ContainsKey(pointPair.Key))
                    profile.points.Add(pointPair.Key, 0);
                profile.points[pointPair.Key] = profile.points[pointPair.Key] + pointPair.Value;
            }

            foreach (KeyValuePair<string, int> pointPair in player.possiblePointsByCategory)
            {
                if (!profile.possiblePoints.ContainsKey(pointPair.Key))
                    profile.possiblePoints.Add(pointPair.Key, 0);
                profile.possiblePoints[pointPair.Key] = profile.possiblePoints[pointPair.Key] + pointPair.Value;
            }

            i++;
        }

        foreach (Profile profile in Profiles.registry.Values)
        {
            string outputPath = $"{Application.streamingAssetsPath}/{profilesDataSource}/{profile.id}.txt";
            if (!File.Exists(outputPath))
                File.Create(outputPath);
            File.WriteAllText(outputPath, profile.FileText());
        }
    }

    /// <summary>
    /// <b>Must</b> be called at start of game
    /// </summary>
#if UNITY_EDITOR
    [MenuItem("Parser/Parse Base Data from files")]
#endif
    public static void ParseData()
    {
        // descriptions
        ParseDescriptions();

        // manifest
        ParseManifest();

        // load categories
        ParseQuestions();

        // load profiles
        ParseProfiles();

        Debug.Log("finished parsing");
    }

    private static Color[] defaultColors = new Color[]
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow
    };

    public static void ParseProfiles()
    {
        TextAsset[] profiles = Assets.LoadAllStreaming<TextAsset>(profilesDataSource);
        int i = 0;
        foreach (TextAsset profileText in profiles)
        {
            if (!Profiles.registry.ContainsKey(i))
                Profiles.registry.Add(i, new Profile());

            Profile target = Profiles.registry[i];
            target.id = i;
            target.ReadFileText(profileText.text);

            i++;
        }

        while (i < 4)
        {
            Profile profile =
            new Profile()
            {
                id = i,
                name = $"Player {i + 1}",
                color = defaultColors[i]
            };

            Profiles.registry[i] = profile;

            string outputPath = $@"{Application.streamingAssetsPath}\{profilesDataSource}\{profile.id}.txt";
            using (var stream = new FileStream(outputPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(profile.FileText());
                }
            }
            i++;
        }
    }

    public static void ParseQuestions()
    {
        TextAsset[] categories = Assets.LoadAllStreaming<TextAsset>(questionDataSource);
        SetupFolders(categories);
        ReadCategoryFiles(categories);
    }

    public static void ReadCategoryFiles(TextAsset[] categories)
    {
        foreach (TextAsset category in categories)
        {
            bool special = Categories.IsSpecialCategory(category.name);

            string outputDirPath;

            if (!special)
            {
                string superCategory = Categories.subcategoryLookup[category.name];
                outputDirPath = Application.streamingAssetsPath + "/" + dataDir + "/" + superCategory + "/" + category.name;
            }
            else
                outputDirPath = Application.streamingAssetsPath + "/" + dataDir + "/" + category.name;

            if (!File.Exists($"{outputDirPath}/{questionFileName}"))
                File.Create($"{outputDirPath}/{questionFileName}");
            if (!File.Exists($"{outputDirPath}/{metadataFileName}"))
                File.Create($"{outputDirPath}/{metadataFileName}");

            string text = category.text;

            string[] lines = text.Replace("\r", "").Split('\n');


            Category categoryObj = Categories.categories.ContainsKey(category.name) ? Categories.categories[category.name] : new Category();
            if (!special)
            {
                string supercategory = Categories.GetSuperCategoryFor(category.name);
                // Debug.Log(supercategory);
                if (!Categories.categories.ContainsKey(supercategory))
                    Categories.categories.Add(supercategory, new Category()
                    {
                        id = supercategory
                    });
                Categories.categories[supercategory].children.Add(categoryObj);
                categoryObj.parent = Categories.categories[supercategory];
            }
            else
            {
                Categories.categories[category.name] = categoryObj;
            }
            categoryObj.id = category.name;

            int currId = 0;
            int currBaseline = 1;
            string currGrouping = "";
            try
            {
                while (lines[currBaseline].Length > 1 && lines[currBaseline].Substring(0, 2) == "#Q")
                {
                    string qText = "";


                    int i = 0;
                    while (lines[currBaseline + i].Length == 0 || lines[currBaseline + i].Substring(0, 2) != "^ ")
                    {
                        string subtext = "";
                        if (lines[currBaseline + i].Length == 0)
                            subtext = "\n";
                        else if (lines[currBaseline + i].Substring(0, 2) == "#Q")
                            subtext = lines[currBaseline + i].Substring(3) + "\n";
                        else
                            subtext = lines[currBaseline + i] + "\n";

                        qText += subtext;

                        i++;
                    }

                    currBaseline += i - 1;

                    Question question;

                    if (lines[currBaseline + 2].Length > 0)
                    {
                        question = new MCQ();

                        // Debug.Log(lines[curr]);

                        int startLine = i;
                        string qCorrect = lines[currBaseline + 1].Substring(2);
                        int qCorrectIdx = 0;
                        List<String> qOptions = new List<String>();
                        int subCurr = 0;
                        while (lines[currBaseline + 2 + subCurr].Length > 0)
                        {
                            qOptions.Add(lines[currBaseline + 2 + subCurr].Substring(2));
                            if (lines[currBaseline + 2 + subCurr].Substring(2) == qCorrect)
                                qCorrectIdx = subCurr;
                            subCurr++;
                        }
                        (question as MCQ).correct = qCorrectIdx;
                        (question as MCQ).options = qOptions.ToArray();

                        currBaseline += (question as MCQ).options.Length + 1 + 2;

                    }
                    else
                    {
                        question = new FRQ();
                        (question as FRQ).correct = lines[currBaseline + 1].Substring(2);


                        currBaseline += 3;
                    }

                    question.groupingFormat = currGrouping;
                    question.text = qText;
                    question.subcategory = category.name;
                    question.localId = currId;
                    if (!special)
                        question.supercategory = Categories.GetSuperCategoryFor(category.name);
                    else
                        question.supercategory = category.name;

                    categoryObj.questions.Add(question);


                    if (lines[currBaseline] == "#/end")
                    {
                        currGrouping = "";
                        currBaseline += 2;
                    }


                    if (lines[currBaseline].Length > 0 && lines[currBaseline].Substring(0, 2) == "#/")
                    {
                        currGrouping = lines[currBaseline].Substring(2);
                        currBaseline += 2;
                    }

                    currId++;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error evaluating question at line {currBaseline} of file {category.name}.txt:\n\t {ex}");
            }

            bool mcqOnly = true;
            bool frqOnly = true;

            foreach (Question question in categoryObj.questions)
            {
                if (question is MCQ)
                    frqOnly = false;
                if (question is FRQ)
                    mcqOnly = false;
            }

            categoryObj.MCQOnly = mcqOnly;
            categoryObj.FRQOnly = frqOnly;
        }

        // Debug.Log("finished parsing");
        // foreach(string categoryName in Categories.categories.Keys)
        //     Debug.Log(categoryName);
    }

    public static void SetupFolders(TextAsset[] categories)
    {
        foreach (TextAsset asset in categories)
        {
            string subcategory = asset.name;
            // Debug.Log(subcategory);
            bool special = Categories.IsSpecialCategory(subcategory);

            string directoryPath = "";

            if (!special)
            {
                string superCategory = Categories.subcategoryLookup[subcategory];
                directoryPath = Application.streamingAssetsPath + "/" + dataDir + "/" + superCategory + "/" + subcategory;
            }
            else
                directoryPath = Application.streamingAssetsPath + "/" + dataDir + "/" + subcategory;

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }
    }

    public static void ParseDescriptions()
    {
        TextAsset descriptionsFile = Assets.LoadStreaming<TextAsset>(categoryDescriptionsDir);
        string text = descriptionsFile.text;

        string[] lines = text.Replace("\r", "").Split('\n');

        Dictionary<string, string> names = new Dictionary<string, string>();
        Dictionary<string, string> descriptions = new Dictionary<string, string>();

        int curr = 0;
        while (curr < lines.Length)
        {
            string categoryId = lines[curr];
            string name = lines[curr + 1].Remove(0, 1);
            string description = lines[curr + 2].Remove(0, 1);

            if (categoryId == "brain-teasers")
                Debug.Log("foudn brainteasers: " + name);
            if (categoryId == "wordwise")
                Debug.Log("foudn wordwise: " + name);

            names.Add(categoryId, name);
            descriptions.Add(categoryId, description);

            curr += 3;
        }

        Categories.categoryNames = names;
        Categories.categoryDescriptions = descriptions;
    }

    /// <summary>
    /// Parses the question/category manifest using the standardized manifest format in StreamingAssets/Config/example_manifest
    /// </summary>
    public static void ParseManifest()
    {


        TextAsset manifest = Assets.LoadStreaming<TextAsset>(manifestDir);
        string text = manifest.text;

        string[] lines = text.Replace("\r", "").Split('\n');

        List<string> blacklist;
        List<string> mixed;
        List<string> special;

        List<string> supers = new List<string>();

        blacklist = ParseSubList(lines, 0, out int endBlacklist);
        mixed = ParseSubList(lines, endBlacklist + 1, out int endMixed);
        special = ParseSubList(lines, endMixed + 1, out int endSpecial);

        Categories.blacklist = blacklist;
        Categories.mixed = mixed;
        Categories.special = special;


        // Debug.Log(lines[endSpecial + 1]);

        if (lines[endSpecial + 1].Trim() != "subcategories:")
        {
            Debug.LogError("No recognizable subcategories section of parsed question/category manifest");
            return;
        }

        Dictionary<string, string> subcategoryLookup = new Dictionary<string, string>();

        int currIdx = endSpecial + 1;
        while (currIdx + 1 < lines.Length && lines[currIdx + 1].Length > 0 && lines[currIdx + 1][0] != '\t')
        {
            List<string> subcategories = ParseSubList(lines, currIdx + 1, out int endIdx);



            string mainCategory = lines[currIdx + 1];
            if (!subcategories.Contains(mainCategory))
                subcategories.Add(mainCategory);

            if (!supers.Contains(mainCategory))
                supers.Add(mainCategory);

            foreach (string subcategory in subcategories)
                subcategoryLookup[subcategory] = mainCategory;

            currIdx = endIdx;
        }

        Categories.subcategoryLookup = subcategoryLookup;
        Categories.superCategories = supers;

        // foreach (KeyValuePair<string, string> pair in subcategoryLookup)
        // {
        //     Debug.Log($"{pair.Key} => {pair.Value}");
        // }
    }

    public static List<string> ParseSubList(string[] lines, int startIdx, out int endIdx)
    {
        List<String> list = new List<String>();

        int currIdx = startIdx;
        while (currIdx + 1 < lines.Length && lines[currIdx + 1].Length > 0 && lines[currIdx + 1][0] == '\t')
        {
            currIdx++;
            list.Add(lines[currIdx].Remove(0, 1));
        }

        endIdx = currIdx;

        return list;
    }
}
