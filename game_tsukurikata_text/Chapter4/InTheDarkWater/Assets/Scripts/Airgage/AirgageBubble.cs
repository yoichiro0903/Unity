using UnityEngine;
using System.Collections;

/// <summary>
/// 空気残量の泡が出てるエフェクト.
/// </summary>
public class AirgageBubble : MonoBehaviour {

    void OnDisplayDamageLv(int value)
    {
		ParticleSystem.EmissionModule	em = GetComponent<ParticleSystem>().emission;

		em.rate = new ParticleSystem.MinMaxCurve(5 + 10 * (float)(value));
    }

    void OnGameOver()
    {
        GetComponent<ParticleSystem>().Stop();
    }
}
