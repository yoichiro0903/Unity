using UnityEngine;
using System.Collections;

/// <summary>
/// テキストの点滅.
/// </summary>
public class TextBlinker : MonoBehaviour
{
    [SerializeField]
    private bool valid = true;
    [SerializeField]
    private float blinkTime = 0.8f;
    [SerializeField]
    private int num = 5;

    private int count = 0;

    void Start()
    {
    }

    // 点滅スタート.
    void OnStartTextBlink()
    {
        if (GetComponent<GUIText>() == null || !valid) return;
        count = 0;
        GetComponent<GUIText>().enabled = true;
        StartCoroutine("Delay", blinkTime);
    }


    private IEnumerator Delay(float delaytime)
    {
        yield return new WaitForSeconds(delaytime);
        GetComponent<GUIText>().enabled = !GetComponent<GUIText>().enabled;
        count++;
        if (count < num)
        {
            StartCoroutine("Delay", blinkTime);
        }
        else
        {
            // 終了通知.
            SendMessage("OnEndTextBlink", SendMessageOptions.DontRequireReceiver);
        }
    }
}
