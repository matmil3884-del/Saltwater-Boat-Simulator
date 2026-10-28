using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lightweight mobile fishing loop for the Saltwater prototype.
/// Tap CAST LINE, wait for a bite, then tap REEL IN while the fish is biting.
/// </summary>
public class FishingSystem : MonoBehaviour
{
    [Serializable]
    public class FishSpecies
    {
        public string name;
        public float minWeight;
        public float maxWeight;
        public int creditsPerPound;
        public float rarityWeight;
        public FishSpecies(string fishName, float min, float max, int value, float rarity)
        {
            name = fishName; minWeight = min; maxWeight = max;
            creditsPerPound = value; rarityWeight = rarity;
        }
    }

    public Transform boat;
    public float minimumBiteDelay = 4f;
    public float maximumBiteDelay = 12f;
    public float biteWindow = 5f;
    public float fishingRange = 24f;

    private readonly List<FishSpecies> species = new List<FishSpecies>();
    private readonly Dictionary<string, int> fishLog = new Dictionary<string, int>();
    private enum FishingState { Ready, Waiting, Bite }
    private FishingState state = FishingState.Ready;
    private float timer;
    private string status = "Find a quiet patch of water and cast your line.";
    private GameObject bobber;
    private int totalFish;
    private float lastWeight;
    private string lastFish = "";
    private int lastReward;
    private GUIStyle panelStyle;
    private GUIStyle titleStyle;

    void Awake()
    {
        species.Add(new FishSpecies("Spotted Seatrout", 1.0f, 8.5f, 8, 42f));
        species.Add(new FishSpecies("Redfish", 2.0f, 18f, 10, 28f));
        species.Add(new FishSpecies("Sheepshead", 1.0f, 10f, 9, 18f));
        species.Add(new FishSpecies("Flounder", 0.5f, 7f, 12, 9f));
        species.Add(new FishSpecies("Tarpon", 8f, 55f, 16, 3f));
    }

    void Update()
    {
        if (state == FishingState.Waiting)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                state = FishingState.Bite;
                timer = biteWindow;
                status = "BITE! Tap REEL IN now!";
            }
        }
        else if (state == FishingState.Bite)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                state = FishingState.Ready;
                status = "The fish got away. Cast again and watch for the bite!";
                RemoveBobber();
            }
        }

        if (bobber && boat)
        {
            Vector3 target = boat.position + boat.forward * 7f + boat.right * 2.5f;
            target.y = 0.12f + Mathf.Sin(Time.time * 2.2f) * 0.08f;
            bobber.transform.position = Vector3.Lerp(bobber.transform.position, target, Time.deltaTime * 1.5f);
        }
    }

    void OnGUI()
    {
        if (panelStyle == null)
        {
            panelStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.UpperLeft, fontSize = Mathf.Max(14, Screen.height / 55), wordWrap = true };
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = Mathf.Max(18, Screen.height / 38), fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
        }

        float scale = Mathf.Clamp(Screen.height / 900f, 0.75f, 1.35f);
        float panelW = 310f * scale;
        float panelH = 220f * scale;
        float x = Screen.width - panelW - 18f;
        GUI.Box(new Rect(x, 14f, panelW, panelH), "");
        GUI.Label(new Rect(x + 12f, 20f, panelW - 24f, 32f), "SALTWATER FISHING", titleStyle);
        GUI.Label(new Rect(x + 12f, 54f, panelW - 24f, 55f), status, panelStyle);
        GUI.Label(new Rect(x + 12f, 112f, panelW - 24f, 26f), "Fish caught: " + totalFish);
        GUI.Label(new Rect(x + 12f, 138f, panelW - 24f, 26f), lastFish == "" ? "Last catch: none yet" : "Last: " + lastFish + " (" + lastWeight.ToString("0.0") + " lb)");
        GUI.Label(new Rect(x + 12f, 164f, panelW - 24f, 26f), lastFish == "" ? "Explore and build your fishing log." : "Reward: " + lastReward + " credits");

        float buttonW = 210f * scale;
        float buttonH = 62f * scale;
        Rect button = new Rect(Screen.width - buttonW - 22f, Screen.height - buttonH - 24f, buttonW, buttonH);
        string label = state == FishingState.Ready ? "CAST LINE" : state == FishingState.Waiting ? "REEL IN" : "REEL IN! " + Mathf.CeilToInt(timer) + "s";
        if (GUI.Button(button, label))
        {
            if (state == FishingState.Ready) CastLine();
            else if (state == FishingState.Waiting) RetrieveLine();
            else CatchFish();
        }
    }

    void CastLine()
    {
        if (!boat)
        {
            var controller = FindObjectOfType<BoatController>();
            if (controller) boat = controller.transform;
        }
        if (!boat) { status = "Boat not found. Restart the scene."; return; }
        state = FishingState.Waiting;
        timer = UnityEngine.Random.Range(minimumBiteDelay, maximumBiteDelay);
        status = "Line cast. Watch the water for a bite...";
        if (!bobber)
        {
            bobber = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bobber.name = "Fishing Bobber";
            bobber.transform.localScale = Vector3.one * 0.32f;
            var renderer = bobber.GetComponent<Renderer>();
            if (renderer)
            {
                var material = new Material(Shader.Find("Standard"));
                material.color = new Color(1f, 0.32f, 0.08f);
                renderer.material = material;
            }
            var collider = bobber.GetComponent<Collider>();
            if (collider) Destroy(collider);
        }
    }

    void RetrieveLine()
    {
        state = FishingState.Ready;
        status = "Line retrieved. Cast again when you're ready.";
        RemoveBobber();
    }

    void CatchFish()
    {
        FishSpecies fish = ChooseFish();
        lastWeight = UnityEngine.Random.Range(fish.minWeight, fish.maxWeight);
        lastFish = fish.name;
        lastReward = Mathf.Max(1, Mathf.RoundToInt(lastWeight * fish.creditsPerPound));
        totalFish++;
        if (!fishLog.ContainsKey(fish.name)) fishLog[fish.name] = 0;
        fishLog[fish.name]++;
        if (BoatGameManager.Instance)
            BoatGameManager.Instance.AwardCredits(lastReward, "Caught " + fish.name + "!");
        status = "Nice catch! " + fish.name + " landed. Cast again!";
        state = FishingState.Ready;
        RemoveBobber();
    }

    FishSpecies ChooseFish()
    {
        float total = 0f;
        foreach (var fish in species) total += fish.rarityWeight;
        float roll = UnityEngine.Random.Range(0f, total);
        foreach (var fish in species)
        {
            roll -= fish.rarityWeight;
            if (roll <= 0f) return fish;
        }
        return species[0];
    }

    void RemoveBobber()
    {
        if (bobber) Destroy(bobber);
        bobber = null;
    }

    void OnDestroy() { RemoveBobber(); }
}
