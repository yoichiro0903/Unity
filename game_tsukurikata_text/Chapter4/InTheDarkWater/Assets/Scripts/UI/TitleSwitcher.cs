using UnityEngine;
using System.Collections;

/// <summary>
/// ユーザクリック後、Adapterにシーン終了を伝える.
/// </summary>
public class TitleSwitcher : MonoBehaviour {

    [SerializeField]
    private float waitTime = 3.0f;

    private bool pushed = false;
    private bool fade = false;
   
    void Start()
    {
        GetComponent<GUIText>().enabled = false;
        Color basecolor = GetComponent<GUIText>().material.color;
        GetComponent<GUIText>().material.color = new Color(basecolor.r, basecolor.g, basecolor.b, 0.0f);
    }

    void Update()
    {
        if (!GetComponent<GUIText>().enabled) return;

        if ( !pushed && Input.GetMouseButtonDown(0))
        {
            pushed = true;
            GetComponent<AudioSource>().Play();
            // シーン終了を伝える.
            GameObject adapter = GameObject.Find("/Adapter");
            if (adapter) adapter.SendMessage("OnSceneEnd");
            else Debug.Log("adapter is not exist...");
        }
	}

    /// <summary>
    /// フェード終了時に呼ばれる.
    /// </summary>
    void OnEndTextFade()
    {
        if (!GetComponent<GUIText>().enabled) return;
        StartCoroutine("Delay");
    }

    /// <summary>
    /// スイッチのスタート.
    /// </summary>
    void OnStartSwitcher()
    {
        Debug.Log("OnStartSwitcher");
        GetComponent<GUIText>().enabled = true;
        fade = true;
        SendMessage("OnTextFadeIn");
    }
    /// <summary>
    /// ステージリセット.
    /// </summary>
    void OnStageReset()
    {
        GetComponent<GUIText>().enabled = false;
        Color basecolor = GetComponent<GUIText>().material.color;
        GetComponent<GUIText>().material.color = new Color(basecolor.r, basecolor.g, basecolor.b, 0.0f);
        pushed = false;
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(waitTime);
        // FadeInとFadeOutを切り替えて実行.
        fade = !fade;
        if (fade) SendMessage("OnTextFadeIn");
        else SendMessage("OnTextFadeOut");
    }

}