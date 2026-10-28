using UnityEngine;

public class BoatGameManager : MonoBehaviour
{
    public static BoatGameManager Instance { get; private set; }
    public int credits = 0;
    public string latestMessage = "Explore the coast and reach the glowing buoys.";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void AwardCredits(int amount, string message)
    {
        credits += Mathf.Max(0, amount);
        latestMessage = message + "  +" + amount + " credits";
        Debug.Log(latestMessage);
    }

    void OnGUI()
    {
        GUI.Box(new Rect(14, 14, 370, 82), "SALTWATER BOAT SIMULATOR");
        GUI.Label(new Rect(26, 40, 340, 20), "Credits: " + credits);
        GUI.Label(new Rect(26, 62, 340, 20), latestMessage);
    }
}