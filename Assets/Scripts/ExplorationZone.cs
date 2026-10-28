using UnityEngine;

public class ExplorationZone : MonoBehaviour
{
    public string zoneName = "Hidden Cove";
    public int firstVisitReward = 150;
    private bool visited;

    void OnTriggerEnter(Collider other)
    {
        if (visited || !other.GetComponentInParent<BoatController>()) return;
        visited = true;
        if (BoatGameManager.Instance)
            BoatGameManager.Instance.AwardCredits(firstVisitReward, "Discovered " + zoneName + "!");
    }
}