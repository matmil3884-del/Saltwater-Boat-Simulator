using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mobile fishing loop with a visible fishing line, bobber and animated fish silhouettes.
/// </summary>
public class FishingSystem : MonoBehaviour
{
    [Serializable]
    public class FishSpecies
    {
        public string name;
        public float minWeight, maxWeight;
        public int creditsPerPound;
        public float rarityWeight;
        public FishSpecies(string fishName, float min, float max, int value, float rarity)
        { name = fishName; minWeight = min; maxWeight = max; creditsPerPound = value; rarityWeight = rarity; }
    }

    public Transform boat;
    public float minimumBiteDelay = 4f, maximumBiteDelay = 12f, biteWindow = 5f;
    public float fishingRange = 24f;
    [Header("Fishing pole progression")]
    public int poleLevel = 0, maxPoleLevel = 5, poleUpgradeBaseCost = 250;
    public float rewardMultiplierPerLevel = 0.15f, biteDelayReductionPerLevel = 0.8f, biteWindowGainPerLevel = 0.75f;

    private readonly List<FishSpecies> species = new List<FishSpecies>();
    private readonly Dictionary<string, int> fishLog = new Dictionary<string, int>();
    private enum FishingState { Ready, Waiting, Bite }
    private FishingState state = FishingState.Ready;
    private float timer;
    private string status = "Find a quiet patch of water and cast your line.";
    private GameObject bobber, lineObject;
    private LineRenderer fishingLine;
    private readonly List<Transform> visibleFish = new List<Transform>();
    private int totalFish;
    private float lastWeight;
    private string lastFish = "";
    private int lastReward;
    private GUIStyle panelStyle, titleStyle;

    void Awake()
    {
        species.Add(new FishSpecies("Spotted Seatrout", 1f, 8.5f, 8, 42f));
        species.Add(new FishSpecies("Redfish", 2f, 18f, 10, 28f));
        species.Add(new FishSpecies("Sheepshead", 1f, 10f, 9, 18f));
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
                timer = biteWindow + poleLevel * biteWindowGainPerLevel;
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
                RemoveFishingVisuals();
            }
        }

        if (bobber && boat)
        {
            Vector3 target = boat.position + boat.forward * 7f + boat.right * 2.5f;
            target.y = 0.12f + Mathf.Sin(Time.time * 2.2f) * 0.08f;
            bobber.transform.position = Vector3.Lerp(bobber.transform.position, target, Time.deltaTime * 2.5f);
            UpdateFishingLine();
            AnimateFish();
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
        float panelW = 310f * scale, panelH = 278f * scale;
        float x = Screen.width - panelW - 18f;
        GUI.Box(new Rect(x, 14f, panelW, panelH), "");
        GUI.Label(new Rect(x + 12f, 20f, panelW - 24f, 32f), "SALTWATER FISHING", titleStyle);
        GUI.Label(new Rect(x + 12f, 54f, panelW - 24f, 55f), status, panelStyle);
        GUI.Label(new Rect(x + 12f, 112f, panelW - 24f, 26f), "Fish caught: " + totalFish);
        GUI.Label(new Rect(x + 12f, 138f, panelW - 24f, 26f), lastFish == "" ? "Last catch: none yet" : "Last: " + lastFish + " (" + lastWeight.ToString("0.0") + " lb)");
        GUI.Label(new Rect(x + 12f, 164f, panelW - 24f, 26f), lastFish == "" ? "Explore and build your fishing log." : "Reward: " + lastReward + " credits");
        GUI.Label(new Rect(x + 12f, 190f, panelW - 24f, 24f), "Fishing pole level: " + poleLevel + "/" + maxPoleLevel);
        int poleCost = poleUpgradeBaseCost * (poleLevel + 1);
        string poleLabel = poleLevel >= maxPoleLevel ? "POLE FULLY UPGRADED" : "UPGRADE POLE - " + poleCost + " credits";
        if (GUI.Button(new Rect(x + 12f, 218f, panelW - 24f, 42f), poleLabel)) PurchasePoleUpgrade();

        float buttonW = 210f * scale, buttonH = 62f * scale;
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
        float reduction = poleLevel * biteDelayReductionPerLevel;
        timer = UnityEngine.Random.Range(Mathf.Max(1f, minimumBiteDelay - reduction), Mathf.Max(2f, maximumBiteDelay - reduction));
        status = "Line cast. Watch the bobber and look for fish below the surface...";
        CreateFishingVisuals();
    }

    void CreateFishingVisuals()
    {
        RemoveFishingVisuals();
        bobber = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bobber.name = "Fishing Bobber";
        bobber.transform.localScale = Vector3.one * 0.30f;
        SetColor(bobber, new Color(1f, 0.24f, 0.045f));
        RemoveCollider(bobber);
        bobber.transform.position = boat.position + boat.forward * 7f + boat.right * 2.5f + Vector3.up * 0.12f;

        lineObject = new GameObject("Animated Fishing Line");
        fishingLine = lineObject.AddComponent<LineRenderer>();
        fishingLine.positionCount = 12;
        fishingLine.useWorldSpace = true;
        fishingLine.startWidth = 0.025f;
        fishingLine.endWidth = 0.012f;
        fishingLine.numCapVertices = 2;
        fishingLine.material = new Material(Shader.Find("Sprites/Default"));
        fishingLine.startColor = new Color(0.08f, 0.09f, 0.08f);
        fishingLine.endColor = new Color(0.12f, 0.14f, 0.13f);

        for (int i = 0; i < 3; i++)
        {
            var fish = new GameObject("Visible Fish " + (i + 1));
            fish.transform.SetParent(transform, false);
            var body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            body.name = "Fish Body";
            body.transform.SetParent(fish.transform, false);
            body.transform.localScale = new Vector3(0.75f + i * 0.13f, 0.25f + i * 0.03f, 0.34f + i * 0.08f);
            SetColor(body, i == 0 ? new Color(0.63f, 0.70f, 0.61f) : i == 1 ? new Color(0.76f, 0.47f, 0.22f) : new Color(0.37f, 0.60f, 0.64f));
            RemoveCollider(body);
            var tail = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            tail.name = "Fish Tail";
            tail.transform.SetParent(fish.transform, false);
            tail.transform.localPosition = new Vector3(0f, 0f, -0.40f - i * 0.04f);
            tail.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            tail.transform.localScale = new Vector3(0.22f, 0.22f, 0.18f);
            SetColor(tail, i == 0 ? new Color(0.63f, 0.70f, 0.61f) : i == 1 ? new Color(0.76f, 0.47f, 0.22f) : new Color(0.37f, 0.60f, 0.64f));
            RemoveCollider(tail);
            visibleFish.Add(fish.transform);
        }
        UpdateFishingLine();
        AnimateFish();
    }

    void UpdateFishingLine()
    {
        if (!fishingLine || !boat || !bobber) return;
        Vector3 start = boat.position + boat.forward * 1.2f + Vector3.up * 0.55f;
        Vector3 end = bobber.transform.position;
        for (int i = 0; i < fishingLine.positionCount; i++)
        {
            float t = i / (float)(fishingLine.positionCount - 1);
            Vector3 point = Vector3.Lerp(start, end, t);
            point.y -= Mathf.Sin(t * Mathf.PI) * (0.12f + Mathf.Sin(Time.time * 2f) * 0.025f);
            fishingLine.SetPosition(i, point);
        }
    }

    void AnimateFish()
    {
        if (!bobber) return;
        for (int i = 0; i < visibleFish.Count; i++)
        {
            Transform fish = visibleFish[i];
            if (!fish) continue;
            float phase = Time.time * (1.1f + i * 0.23f) + i * 2.1f;
            fish.position = bobber.transform.position + new Vector3(Mathf.Sin(phase) * (1.2f + i * 0.45f), -0.55f - i * 0.22f, Mathf.Cos(phase * 0.7f) * (1.0f + i * 0.4f));
            fish.rotation = Quaternion.Euler(0f, Mathf.Sin(phase) * 25f + 180f, Mathf.Sin(phase * 2f) * 8f);
        }
    }

    void RetrieveLine()
    {
        state = FishingState.Ready;
        status = "Line retrieved. Cast again when you're ready.";
        RemoveFishingVisuals();
    }

    void CatchFish()
    {
        FishSpecies fish = ChooseFish();
        lastWeight = UnityEngine.Random.Range(fish.minWeight, fish.maxWeight);
        lastFish = fish.name;
        lastReward = Mathf.Max(1, Mathf.RoundToInt(lastWeight * fish.creditsPerPound * (1f + poleLevel * rewardMultiplierPerLevel)));
        totalFish++;
        if (!fishLog.ContainsKey(fish.name)) fishLog[fish.name] = 0;
        fishLog[fish.name]++;
        if (BoatGameManager.Instance) BoatGameManager.Instance.AwardCredits(lastReward, "Caught " + fish.name + "!");
        status = "Nice catch! " + fish.name + " landed. Cast again!";
        state = FishingState.Ready;
        RemoveFishingVisuals();
    }

    public bool PurchasePoleUpgrade()
    {
        if (poleLevel >= maxPoleLevel || !BoatGameManager.Instance) return false;
        int cost = poleUpgradeBaseCost * (poleLevel + 1);
        if (BoatGameManager.Instance.credits < cost) { status = "Not enough credits for the next pole upgrade."; return false; }
        BoatGameManager.Instance.credits -= cost;
        poleLevel++;
        status = "Fishing pole upgraded to level " + poleLevel + "!";
        BoatGameManager.Instance.latestMessage = status;
        return true;
    }

    FishSpecies ChooseFish()
    {
        float total = 0f;
        foreach (var fish in species) total += fish.rarityWeight;
        float roll = UnityEngine.Random.Range(0f, total);
        foreach (var fish in species) { roll -= fish.rarityWeight; if (roll <= 0f) return fish; }
        return species[0];
    }

    void SetColor(GameObject go, Color color)
    {
        var renderer = go.GetComponent<Renderer>();
        if (renderer) { var material = new Material(Shader.Find("Standard")); material.color = color; renderer.material = material; }
    }

    void RemoveCollider(GameObject go)
    {
        var collider = go.GetComponent<Collider>();
        if (collider) Destroy(collider);
    }

    void RemoveFishingVisuals()
    {
        if (bobber) Destroy(bobber);
        if (lineObject) Destroy(lineObject);
        bobber = null;
        lineObject = null;
        fishingLine = null;
        foreach (var fish in visibleFish) if (fish) Destroy(fish.gameObject);
        visibleFish.Clear();
    }

    void OnDestroy() { RemoveFishingVisuals(); }
}