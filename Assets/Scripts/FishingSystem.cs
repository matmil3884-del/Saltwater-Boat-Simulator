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
        public bool freshwater;
        public FishSpecies(string fishName, float min, float max, int value, float rarity, bool isFreshwater = false)
        { name = fishName; minWeight = min; maxWeight = max; creditsPerPound = value; rarityWeight = rarity; freshwater = isFreshwater; }
    }

    public Transform boat;
    public float minimumBiteDelay = 4f, maximumBiteDelay = 12f, biteWindow = 5f;
    public float fishingRange = 24f;
    [Header("Fishing pole progression")]
    public int poleLevel = 0, maxPoleLevel = 5, poleUpgradeBaseCost = 250;
    public float rewardMultiplierPerLevel = 0.15f, biteDelayReductionPerLevel = 0.8f, biteWindowGainPerLevel = 0.75f;

    private readonly List<FishSpecies> species = new List<FishSpecies>();
    private readonly Dictionary<string, int> fishLog = new Dictionary<string, int>();
    private enum FishingState { Ready, Waiting, Bite, Fighting }
    private bool freshwaterMode = false;
    private FishingState state = FishingState.Ready;
    private float timer;
    private string status = "Find a quiet patch of water and cast your line.";
    private float fishFightTimer;
    private float lineTension = 0.35f;
    private bool bobberSubmerged;
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
        species.Add(new FishSpecies("Largemouth Bass", 0.5f, 12f, 11, 35f, true));
        species.Add(new FishSpecies("Smallmouth Bass", 0.5f, 7f, 12, 20f, true));
        species.Add(new FishSpecies("Crappie", 0.2f, 4f, 10, 22f, true));
        species.Add(new FishSpecies("Bluegill", 0.1f, 2f, 8, 17f, true));
        species.Add(new FishSpecies("Channel Catfish", 1f, 25f, 13, 6f, true));
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
        else if (state == FishingState.Fighting)
        {
            fishFightTimer -= Time.deltaTime;
            lineTension = Mathf.Clamp01(lineTension + Mathf.Sin(Time.time * 3.4f) * Time.deltaTime * 0.28f + Time.deltaTime * 0.10f);
            if (lineTension > 0.94f)
            {
                state = FishingState.Ready;
                status = "The line snapped! Ease off and try another cast.";
                RemoveFishingVisuals();
            }
            else if (fishFightTimer <= 0f) LandFish();
        }

        if (bobber && boat)
        {
            Vector3 target = boat.position + boat.forward * 7f + boat.right * 2.5f;
            target.y = 0.12f + Mathf.Sin(Time.time * 2.2f) * 0.08f;
            bobber.transform.position = Vector3.Lerp(bobber.transform.position, target, Time.deltaTime * 2.5f);
            if (state == FishingState.Bite || state == FishingState.Fighting)
            {
                bobberSubmerged = Mathf.Sin(Time.time * 11f) > 0.15f;
                target.y = bobberSubmerged ? -0.22f : 0.02f;
            }
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
        float panelW = 310f * scale, panelH = 326f * scale;
        float x = Screen.width - panelW - 18f;
        GUI.Box(new Rect(x, 14f, panelW, panelH), "");
        GUI.Label(new Rect(x + 12f, 20f, panelW - 24f, 32f), "BOATING & FISHING", titleStyle);
        string waterLabel = freshwaterMode ? "FRESHWATER - SWITCH TO SALTWATER" : "SALTWATER - SWITCH TO FRESHWATER";
        if (GUI.Button(new Rect(x + 12f, 52f, panelW - 24f, 34f), waterLabel))
        {
            if (state != FishingState.Ready) RetrieveLine();
            freshwaterMode = !freshwaterMode;
            status = freshwaterMode ? "Freshwater selected. Try bass, crappie, bluegill, or catfish." : "Saltwater selected. Head offshore for coastal species.";
        }
        GUI.Label(new Rect(x + 12f, 90f, panelW - 24f, 50f), status, panelStyle);
        GUI.Label(new Rect(x + 12f, 142f, panelW - 24f, 24f), "Fish caught: " + totalFish);
        GUI.Label(new Rect(x + 12f, 166f, panelW - 24f, 24f), lastFish == "" ? "Last catch: none yet" : "Last: " + lastFish + " (" + lastWeight.ToString("0.0") + " lb)");
        GUI.Label(new Rect(x + 12f, 190f, panelW - 24f, 24f), lastFish == "" ? "Explore and build your fishing log." : "Reward: " + lastReward + " credits");
        GUI.Label(new Rect(x + 12f, 214f, panelW - 24f, 24f), state == FishingState.Fighting ? "Line tension: " + Mathf.RoundToInt(lineTension * 100f) + "%" : "Fishing pole level: " + poleLevel + "/" + maxPoleLevel);
        int poleCost = poleUpgradeBaseCost * (poleLevel + 1);
        string poleLabel = poleLevel >= maxPoleLevel ? "POLE FULLY UPGRADED" : "UPGRADE POLE - " + poleCost + " credits";
        if (GUI.Button(new Rect(x + 12f, 242f, panelW - 24f, 42f), poleLabel)) PurchasePoleUpgrade();

        float buttonW = 210f * scale, buttonH = 62f * scale;
        Rect button = new Rect(Screen.width - buttonW - 22f, Screen.height - buttonH - 24f, buttonW, buttonH);
        string label = state == FishingState.Ready ? "CAST LINE" : state == FishingState.Waiting ? "REEL IN" : state == FishingState.Bite ? "SET HOOK! " + Mathf.CeilToInt(timer) + "s" : "FIGHT! " + Mathf.CeilToInt(fishFightTimer) + "s";
        if (GUI.Button(button, label))
        {
            if (state == FishingState.Ready) CastLine();
            else if (state == FishingState.Waiting) RetrieveLine();
            else if (state == FishingState.Bite) SetHook();
            else if (state == FishingState.Fighting) EaseLineTension();
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
        status = "Cast! Fish are approaching the bait. Watch for the bobber to dip.";
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
            FishSpecies speciesToShow = ChooseFish();
            Transform fish = CreateFishModel(speciesToShow, i);
            visibleFish.Add(fish);
        }
        UpdateFishingLine();
        AnimateFish();
    }

    Transform CreateFishModel(FishSpecies speciesToShow, int index)
    {
        var root = new GameObject("Visible " + speciesToShow.name);
        root.transform.SetParent(transform, false);
        float size = UnityEngine.Random.Range(0.82f, 1.22f);
        Color bodyColor;
        if (speciesToShow.name == "Largemouth Bass") bodyColor = new Color(0.34f, 0.48f, 0.20f);
        else if (speciesToShow.name == "Smallmouth Bass") bodyColor = new Color(0.56f, 0.55f, 0.31f);
        else if (speciesToShow.name == "Crappie") bodyColor = new Color(0.48f, 0.53f, 0.58f);
        else if (speciesToShow.name == "Bluegill") bodyColor = new Color(0.30f, 0.48f, 0.66f);
        else if (speciesToShow.name == "Channel Catfish") bodyColor = new Color(0.40f, 0.43f, 0.42f);
        else bodyColor = new Color(0.66f, 0.62f, 0.43f);
        bool deepBody = speciesToShow.name == "Bluegill" || speciesToShow.name == "Crappie";
        bool catfish = speciesToShow.name == "Channel Catfish";

        var body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        body.name = "Body";
        body.transform.SetParent(root.transform, false);
        body.transform.localScale = new Vector3((deepBody ? 0.56f : 0.92f) * size, (deepBody ? 0.42f : 0.34f) * size, 0.42f * size);
        SetColor(body, bodyColor); RemoveCollider(body);

        var tail = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        tail.name = "Tail";
        tail.transform.SetParent(root.transform, false);
        tail.transform.localPosition = new Vector3(0f, 0f, -0.48f * size);
        tail.transform.localScale = new Vector3(0.34f * size, 0.42f * size, 0.10f * size);
        SetColor(tail, bodyColor * 0.82f); RemoveCollider(tail);

        var dorsal = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dorsal.name = "Dorsal Fin";
        dorsal.transform.SetParent(root.transform, false);
        dorsal.transform.localPosition = new Vector3(0f, 0.22f * size, 0.02f);
        dorsal.transform.localScale = new Vector3((deepBody ? 0.45f : 0.68f) * size, 0.16f * size, 0.20f * size);
        SetColor(dorsal, bodyColor * 0.72f); RemoveCollider(dorsal);

        for (int side = -1; side <= 1; side += 2)
        {
            var eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            eye.name = "Eye";
            eye.transform.SetParent(root.transform, false);
            eye.transform.localPosition = new Vector3(0.16f * size, 0.07f * size, 0.13f * size * side);
            eye.transform.localScale = Vector3.one * 0.065f * size;
            SetColor(eye, Color.black); RemoveCollider(eye);

            var fin = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            fin.name = "Pectoral Fin";
            fin.transform.SetParent(root.transform, false);
            fin.transform.localPosition = new Vector3(-0.04f * size, -0.08f * size, 0.19f * size * side);
            fin.transform.localScale = new Vector3(0.18f * size, 0.07f * size, 0.12f * size);
            SetColor(fin, bodyColor * 0.75f); RemoveCollider(fin);
        }

        if (speciesToShow.name.Contains("Bass"))
        {
            for (int spot = 0; spot < 5; spot++)
            {
                var mark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                mark.name = "Bass Lateral Mark";
                mark.transform.SetParent(root.transform, false);
                mark.transform.localPosition = new Vector3(-0.22f * size + spot * 0.10f * size, -0.015f * size, 0.205f * size);
                mark.transform.localScale = new Vector3(0.10f * size, 0.055f * size, 0.018f * size);
                SetColor(mark, new Color(0.18f, 0.29f, 0.12f)); RemoveCollider(mark);
            }
        }
        if (catfish)
        {
            for (int whisker = -1; whisker <= 1; whisker++)
            {
                var barbel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                barbel.name = "Catfish Whisker";
                barbel.transform.SetParent(root.transform, false);
                barbel.transform.localPosition = new Vector3(0.34f * size, -0.06f * size, whisker * 0.10f * size);
                barbel.transform.localRotation = Quaternion.Euler(0f, 0f, 72f);
                barbel.transform.localScale = new Vector3(0.018f * size, 0.16f * size, 0.018f * size);
                SetColor(barbel, new Color(0.18f, 0.19f, 0.17f)); RemoveCollider(barbel);
            }
        }
        return root.transform;
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
            float speciesSpeed = fish.name.Contains("Catfish") ? 0.72f : fish.name.Contains("Bluegill") ? 1.75f : fish.name.Contains("Bass") ? 1.25f : 1.05f;
            float phase = Time.time * speciesSpeed + i * 2.1f;
            float approach = state == FishingState.Waiting ? Mathf.Lerp(5.2f, 1.0f, Mathf.PingPong(Time.time * 0.09f + i * 0.2f, 1f)) : 1.1f;
            float dart = state == FishingState.Bite ? Mathf.Sin(Time.time * 13f + i) * 0.85f : 0f;
            fish.position = bobber.transform.position + new Vector3(Mathf.Sin(phase) * (approach + i * 0.25f) + dart, -0.62f - i * 0.18f, Mathf.Cos(phase * 0.7f) * (approach * 0.7f));
            fish.rotation = Quaternion.Euler(0f, Mathf.Atan2(Mathf.Cos(phase), Mathf.Sin(phase)) * Mathf.Rad2Deg + 90f, Mathf.Sin(phase * 2f) * 8f);
        }
    }

    void RetrieveLine()
    {
        state = FishingState.Ready;
        status = "Line retrieved. Cast again when you're ready.";
        RemoveFishingVisuals();
    }

    void SetHook()
    {
        state = FishingState.Fighting;
        fishFightTimer = UnityEngine.Random.Range(2.5f, 5.5f);
        lineTension = 0.30f;
        status = "Fish hooked! Tap REEL IN to control the fight. Don't let tension max out.";
    }

    void EaseLineTension()
    {
        lineTension = Mathf.Max(0.08f, lineTension - 0.22f);
        fishFightTimer -= 0.45f;
        status = "Good control! Line tension: " + Mathf.RoundToInt(lineTension * 100f) + "%";
        if (fishFightTimer <= 0f) LandFish();
    }

    void LandFish()
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
        foreach (var fish in species)
            if (fish.freshwater == freshwaterMode) total += fish.rarityWeight;

        float roll = UnityEngine.Random.Range(0f, total);
        foreach (var fish in species)
        {
            if (fish.freshwater != freshwaterMode) continue;
            roll -= fish.rarityWeight;
            if (roll <= 0f) return fish;
        }
        foreach (var fish in species) if (fish.freshwater == freshwaterMode) return fish;
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