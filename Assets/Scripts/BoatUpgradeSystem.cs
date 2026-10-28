using UnityEngine;

/// <summary>
/// Buy/select boat classes and upgrade engine and handling performance.
/// </summary>
public class BoatUpgradeSystem : MonoBehaviour
{
    [Header("Performance upgrades")]
    public int engineLevel = 0, maxEngineLevel = 4, engineBaseCost = 300;
    public float speedGainPerLevel = 2f, accelerationGainPerLevel = 0.8f;
    public int handlingLevel = 0, maxHandlingLevel = 4, handlingBaseCost = 250;
    public float turnGainPerLevel = 5f;
    [Header("Boat fleet")]
    public bool speedboatUnlocked = false, fishingBoatUnlocked = false;
    public int speedboatCost = 1200, fishingBoatCost = 1800;
    public string activeBoat = "Starter Skiff";

    private BoatController boat;
    private BoatVisualModel visualModel;
    private Vector3 originalHullScale;
    private Renderer hullRenderer;
    private Color originalHullColor = Color.white;
    private GUIStyle panelStyle, titleStyle;

    void Awake()
    {
        boat = GetComponent<BoatController>();
        visualModel = GetComponent<BoatVisualModel>();
        var hull = transform.Find("Hull");
        if (hull)
        {
            originalHullScale = hull.localScale;
            hullRenderer = hull.GetComponent<Renderer>();
            if (hullRenderer) originalHullColor = hullRenderer.sharedMaterial.color;
        }
        ApplyBoatStats();
    }

    void Start() { ApplyBoatStats(); }

    public bool PurchaseEngineUpgrade()
    {
        if (!boat || engineLevel >= maxEngineLevel || !Spend(engineBaseCost * (engineLevel + 1))) return false;
        engineLevel++;
        ApplyBoatStats();
        Notify("Engine upgrade level " + engineLevel);
        return true;
    }

    public bool PurchaseHandlingUpgrade()
    {
        if (!boat || handlingLevel >= maxHandlingLevel || !Spend(handlingBaseCost * (handlingLevel + 1))) return false;
        handlingLevel++;
        ApplyBoatStats();
        Notify("Handling upgrade level " + handlingLevel);
        return true;
    }

    public bool UnlockSpeedboat()
    {
        if (speedboatUnlocked || !Spend(speedboatCost)) return false;
        speedboatUnlocked = true;
        Notify("Speedboat unlocked! Select it in the boat shop.");
        return true;
    }

    public bool UnlockFishingBoat()
    {
        if (fishingBoatUnlocked || !Spend(fishingBoatCost)) return false;
        fishingBoatUnlocked = true;
        Notify("Fishing boat unlocked! Select it in the boat shop.");
        return true;
    }

    public bool SelectStarterSkiff() { activeBoat = "Starter Skiff"; ApplyBoatStats(); Notify("Starter Skiff selected."); return true; }
    public bool SelectSpeedboat()
    {
        if (!speedboatUnlocked) return false;
        activeBoat = "Speedboat"; ApplyBoatStats(); Notify("Speedboat selected."); return true;
    }
    public bool SelectFishingBoat()
    {
        if (!fishingBoatUnlocked) return false;
        activeBoat = "Fishing Boat"; ApplyBoatStats(); Notify("Fishing Boat selected."); return true;
    }

    void ApplyBoatStats()
    {
        if (!boat) return;
        float speed = 22f, accel = 8f, turn = 35f, mass = 1000f;
        Vector3 hullScale = originalHullScale == Vector3.zero ? new Vector3(2.2f, 0.65f, 4.8f) : originalHullScale;
        Color hullColor = originalHullColor;
        if (activeBoat == "Speedboat")
        {
            speed = 34f; accel = 12f; turn = 40f; mass = 800f;
            hullScale = new Vector3(1.9f, 0.55f, 4.4f); hullColor = new Color(0.9f, 0.16f, 0.08f);
        }
        else if (activeBoat == "Fishing Boat")
        {
            speed = 18f; accel = 6.5f; turn = 28f; mass = 1300f;
            hullScale = new Vector3(2.6f, 0.75f, 5.3f); hullColor = new Color(0.12f, 0.58f, 0.42f);
        }
        boat.maxForwardSpeed = speed + engineLevel * speedGainPerLevel;
        boat.forwardAcceleration = accel + engineLevel * accelerationGainPerLevel;
        boat.turnSpeed = turn + handlingLevel * turnGainPerLevel;
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.mass = mass;
        var hull = transform.Find("Hull");
        if (hull) hull.localScale = hullScale;
        if (hullRenderer && hullRenderer.sharedMaterial) hullRenderer.material.color = hullColor;
        if (!visualModel) visualModel = GetComponent<BoatVisualModel>();
        if (visualModel) visualModel.Configure(activeBoat);
    }

    bool Spend(int cost)
    {
        if (!BoatGameManager.Instance || BoatGameManager.Instance.credits < cost) return false;
        BoatGameManager.Instance.credits -= cost;
        return true;
    }

    void Notify(string message)
    {
        if (BoatGameManager.Instance) BoatGameManager.Instance.latestMessage = message;
    }

    void OnGUI()
    {
        if (panelStyle == null)
        {
            panelStyle = new GUIStyle(GUI.skin.box) { alignment = TextAnchor.UpperLeft, fontSize = Mathf.Max(12, Screen.height / 70), wordWrap = true };
            titleStyle = new GUIStyle(GUI.skin.label) { fontSize = Mathf.Max(16, Screen.height / 48), fontStyle = FontStyle.Bold, normal = { textColor = Color.white } };
        }
        float s = Mathf.Clamp(Screen.height / 900f, 0.7f, 1.1f);
        float w = 300f * s, h = 365f * s, x = 14f, y = Screen.height - h - 14f;
        GUI.Box(new Rect(x, y, w, h), "");
        GUI.Label(new Rect(x + 10f, y + 5f, w - 20f, 28f), "BOAT SHOP & UPGRADES", titleStyle);
        GUI.Label(new Rect(x + 10f, y + 34f, w - 20f, 22f), "Active: " + activeBoat);
        float bw = w - 20f, bh = 32f * s, gap = 4f * s, by = y + 60f;
        if (GUI.Button(new Rect(x + 10f, by, bw, bh), "Engine L" + engineLevel + " - " + (engineLevel >= maxEngineLevel ? "MAX" : (engineBaseCost * (engineLevel + 1)) + " credits"))) PurchaseEngineUpgrade();
        by += bh + gap;
        if (GUI.Button(new Rect(x + 10f, by, bw, bh), "Handling L" + handlingLevel + " - " + (handlingLevel >= maxHandlingLevel ? "MAX" : (handlingBaseCost * (handlingLevel + 1)) + " credits"))) PurchaseHandlingUpgrade();
        by += bh + gap;
        if (GUI.Button(new Rect(x + 10f, by, bw, bh), "Use Starter Skiff")) SelectStarterSkiff();
        by += bh + gap;
        if (!speedboatUnlocked)
        {
            if (GUI.Button(new Rect(x + 10f, by, bw, bh), "Unlock Speedboat - " + speedboatCost)) UnlockSpeedboat();
        }
        else if (GUI.Button(new Rect(x + 10f, by, bw, bh), "Select Speedboat")) SelectSpeedboat();
        by += bh + gap;
        if (!fishingBoatUnlocked)
        {
            if (GUI.Button(new Rect(x + 10f, by, bw, bh), "Unlock Fishing Boat - " + fishingBoatCost)) UnlockFishingBoat();
        }
        else if (GUI.Button(new Rect(x + 10f, by, bw, bh), "Select Fishing Boat")) SelectFishingBoat();
        by += bh + gap;
        GUI.Label(new Rect(x + 10f, by, bw, 25f), "Earn credits by fishing and missions.");
    }
}