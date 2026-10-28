using UnityEngine;

public class MissionBuoy : MonoBehaviour
{
    public int rewardCredits = 250;
    public string missionName = "Harbor checkpoint";
    public bool oneTimeReward = true;
    private bool collected;

    void OnTriggerEnter(Collider other)
    {
        if (collected && oneTimeReward) return;
        if (!other.GetComponentInParent<BoatController>()) return;
        collected = true;
        if (BoatGameManager.Instance)
            BoatGameManager.Instance.AwardCredits(rewardCredits, missionName + " complete!");
        if (oneTimeReward) gameObject.SetActive(false);
    }
}