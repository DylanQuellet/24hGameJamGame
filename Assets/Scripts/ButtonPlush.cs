using UnityEngine;

public class ButtonPlush : MonoBehaviour
{
    public PlayerMovement Player;
    public GameObject ChestUI;

    public void OnButtonClick()
    {
        ChestUI.SetActive(false);
        Player.BountyFound = true;
        print("j'ai mon tresor");
    }
}
