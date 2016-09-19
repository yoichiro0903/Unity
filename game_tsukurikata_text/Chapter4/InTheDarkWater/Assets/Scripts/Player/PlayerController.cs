using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {


    [System.Serializable]
    public class SpeedValue
    {
        [SerializeField]
        private float max = 10.0f;
        [SerializeField]
        private float step = 0.5f;

        public float current = 1.0f;
        /// <summary>
        /// スピード変更.
        /// </summary>
        /// <param name="value"></param>
        public void Change( float value )
        {
            current += value * step;
            current = Mathf.Clamp(current, 0.0f, max);
        }

        public void Stop() { current = 0.0f; }

        public float Rate() { return Mathf.InverseLerp(0.0f, max, current); }
    };
    [SerializeField]
    private SpeedValue speed = new SpeedValue();

    [System.Serializable]
    public class RotationValue
    {
        public Vector3	current = Vector3.zero;
        private float	attenuationStart;
        private float	attenuationTime = 0.0f;
        private float	currentRot;               // 現在の減衰率（attenuationRot/slowdownRot).

        [SerializeField]
        private float max = 30.0f;
        [SerializeField]
        private float blend = 0.8f;
        [SerializeField]
        private float margin = 0.01f;
        [SerializeField]
        private float attenuationRot = 0.2f;    // 『ボタンを押していない』ときの減衰率.
        [SerializeField]
        private float slowdownRot = 0.4f;       // 『ボタンは押しているが、マウスは動いていない』ときの減衰率.

        public void Init()
        {
            currentRot = attenuationTime;
        }

		// マウスのX軸方向の移動量を使って、旋回速度を更新する.
        public void Change(float value)
        {
            // マウスの動きが小さいときは更新しない.
            // （マウスを動かし続けないと旋回を止めてしまうようにするため）.
            if (-margin < value && value < margin) return;

            // 回転量のブレンド.
            current.y = Mathf.Lerp(current.y, current.y + value, blend);
            if (current.y > max) current.y = max;

            // 減衰リセット.
            attenuationStart = current.y;

			// 減衰用のタイマーをリセット.
            attenuationTime = 0.0f;
        }

		// 旋回速度を減衰する.
        // time		:	時間変位.
        // return	:	衰中/減衰してない.
        public bool Attenuate(float time)
        {
            if (current.y == 0.0f) return false;
            attenuationTime += time;
            current.y = Mathf.SmoothStep(attenuationStart, 0.0f, currentRot * attenuationTime);
            return true;
        }

		// 旋回速度の減衰率を設定（ボタンを押した瞬間）.
        public void BrakeAttenuation()
		{
			currentRot = slowdownRot;
		}

		// 旋回速度の減衰率を設定（ボタンを離した瞬間）.
		public void UsualAttenuation()
        {
			// 旋回速度が変化しないよう、減衰が始まってからの時間を計算しなおす.
            attenuationTime = (slowdownRot * attenuationTime) / attenuationRot;
            currentRot = attenuationRot;
        }

        public void Stop() { current = Vector3.zero; }
    };
    [SerializeField]
    private RotationValue rot = new RotationValue();

    private Quaternion deltaRot;

    [SerializeField]
    private bool valid = false;

    private Controller controller;
    private MarineSnow marinesnowEffect;
    private TorpedoGenerator torpedo;

	void Start () 
    {
        // MarinSnowのエフェクトはスピード依存。頻繁に更新するので参照を持っておく.
        GameObject effect = GameObject.Find("Effect_MarineSnow");
        if (effect)  marinesnowEffect = effect.GetComponent<MarineSnow>();
        // 魚雷発射スクリプト.
        torpedo = GetComponent<TorpedoGenerator>();
    }

    void OnGameStart()
    {
        Debug.Log("OnGameStart");
        valid = true;
        // UIのコントローラー。頻繁に更新するので参照を持っておく.
        GameObject uiObj = GameObject.Find("/UI/Controller");
        if (uiObj) controller = uiObj.GetComponent<Controller>();
        // コントローラ表示.
        if (controller) controller.Enable(true);
        rot.Init();
    }

    void OnGameClear()
    {
        InvalidPlayer();
    }

    void OnGameOver()
    {
        InvalidPlayer();

        // 沈む演出.
        // 軸の固定を解除して、重力を有効にする.
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        GetComponent<Rigidbody>().useGravity = true;
    }
	
	void FixedUpdate () 
    {
        // 旋回速度の減衰.
        rot.Attenuate(Time.deltaTime);

        if (valid)
        {
            // 魚雷発射.
            if (Input.GetKeyDown(KeyCode.B))
            {
                torpedo.Generate();
            }

            // ドラッグ中.
            if (Input.GetMouseButton(0))
            {
                // 回転.
				//マウスのX軸方向の移動量を使って、旋回速度を更新する.
                rot.Change(Input.GetAxis("Mouse X"));

                // 加速.
                speed.Change(Input.GetAxis("Mouse Y"));
            }

            // ドラッグ開始.
            if (Input.GetMouseButtonDown(0))
            {
				// 旋回速度の減衰定数を設定.
                rot.BrakeAttenuation();
            }

            // ドラッグ終了.
            if (Input.GetMouseButtonUp(0))
            {
				// 旋回速度の減衰定数を設定.
                rot.UsualAttenuation();
            }
        }		
        // 旋回する.
        Rotate();
        // 前に進む.
        MoveForward();
	}

    private void InvalidPlayer()
    {
        valid = false;
        speed.Stop();
        rot.Stop();
        if (controller) controller.Enable(false);
    }

    private void Rotate() 
    {
        Quaternion deltaRot = Quaternion.Euler(rot.current * Time.deltaTime);
        GetComponent<Rigidbody>().MoveRotation(GetComponent<Rigidbody>().rotation * deltaRot);
        // 回転演出.
        if (controller) controller.SetAngle(transform.localEulerAngles.y);
    }

    private void MoveForward()
    {
        Vector3 vec = speed.current * transform.forward.normalized;
        GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + vec * Time.deltaTime);
        // スピードの変化演出.
        if (marinesnowEffect) marinesnowEffect.SetSpeed(speed.Rate());
    }

    public void AddSpeed(float value) 
    {
        speed.Change( value );
    }

    public float SpeedRate()
    {
        return speed.Rate();
    }

}
