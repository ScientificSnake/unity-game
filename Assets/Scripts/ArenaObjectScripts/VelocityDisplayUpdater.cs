using TMPro;
using UnityEngine;

public class VelocityDisplayUpdater : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    public PlayerObjectScript PlayerRef;

    private bool playerFound = false;

    void Update()
    {
        // Keep looking for player if not found yet
        if (!playerFound && PlayerRef == null)
        {
            GameObject playerGO = GameObject.FindWithTag("Player");
            if (playerGO != null)
            {
                PlayerRef = playerGO.GetComponent<PlayerObjectScript>();
                playerFound = true;
                Debug.Log("Player found!");
            }
        }

        // Update display
        if (textMeshProUGUI != null && PlayerRef != null)
        {
            textMeshProUGUI.text = $"{PlayerRef.Velocity.magnitude.ToString("F2")} m/s";
        }
    }
}