using UnityEngine;

public class LightReceiver : MonoBehaviour
{
    //[HideInInspector]
    public bool isIlluminated;

    public void SetIlluminated(bool value)
    {
        isIlluminated = value;
    }
}
