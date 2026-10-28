using UnityEngine;

public class BoatUpgradeSystem : MonoBehaviour
{
    public int engineLevel = 0, maxEngineLevel = 4, engineBaseCost = 300;
    public float speedGainPerLevel = 2f, accelerationGainPerLevel = 0.8f;
    public int handlingLevel = 0, maxHandlingLevel = 4, handlingBaseCost = 250;
    public float turnGainPerLevel = 5f;
    public bool speedboatUnlocked = true, fishingBoatUnlocked = false;
    public int speedboatCost = 1200, fishingBoatCost = 1800;
    private BoatController boat;
    void Awake() { boat = GetComponent<BoatController>(); }

    public bool PurchaseEngineUpgrade()
    {
        if (!boat || engineLevel >= maxEngineLevel || !Spend(engineBaseCost * (engineLevel + 1))) return false;
        engineLevel++; boat.maxForwardSpeed += speedGainPerLevel; boat.forwardAcceleration += accelerationGainPerLevel;
        Notify("Engine upgrade level " + engineLevel); return true;
    }
    public bool PurchaseHandlingUpgrade()
    {
        if (!boat || handlingLevel >= maxHandlingLevel || !Spend(handlingBaseCost * (handlingLevel + 1))) return false;
        handlingLevel++; boat.turnSpeed += turnGainPerLevel; Notify("Handling upgrade level " + handlingLevel); return true;
    }
    public bool UnlockSpeedboat()
    {
        if (speedboatUnlocked || !Spend(speedboatCost)) return false;
        speedboatUnlocked = true; Notify("Speedboat unlocked!"); return true;
    }
    public bool UnlockFishingBoat()
    {
        if (fishingBoatUnlocked || !Spend(fishingBoatCost)) return false;
        fishingBoatUnlocked = true; Notify("Fishing boat unlocked!"); return true;
    }
    private bool Spend(int cost)
    {
        if (!BoatGameManager.Instance || BoatGameManager.Instance.credits < cost) return false;
        BoatGameManager.Instance.credits -= cost; return true;
    }
    private void Notify(string message)
    {
        if (BoatGameManager.Instance) BoatGameManager.Instance.latestMessage = message;
    }
}