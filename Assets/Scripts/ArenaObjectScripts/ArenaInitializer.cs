using System.Linq.Expressions;
using UnityEngine;

public class ArenaInitializer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ManagerScript.CurrentLevelManagerInstance.InstantiatePlayerObject();
        GameObject PlayerObject = GameObject.FindWithTag("Player");
        SpriteRenderer spriteRenderer = PlayerObject.GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = ManagerScript.Instance.SpriteDict[ManagerScript.CurrentLevelManagerInstance.selectedHull];

        print(spriteRenderer.sprite.ToString());
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
