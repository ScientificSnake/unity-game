using TMPro;
using UnityEngine;

public class VelocityDisplayUpdater : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    public Rigidbody2D PlayerRef;

    private bool playerFound = false;

    void Update()
    {
        // Keep looking for player if not found yet
        if (!playerFound && PlayerRef == null)
        {
            GameObject playerGO = GameObject.FindWithTag("Player");
            if (playerGO != null)
            {
                PlayerRef = playerGO.GetComponent<Rigidbody2D>();
                playerFound = true;
                Debug.Log("Player found!");
            }
        }

        // Update display
        if (textMeshProUGUI != null && PlayerRef != null)
        {
            textMeshProUGUI.text = $"{((int) PlayerRef.linearVelocity.magnitude)} m/s";
        }
    }
}