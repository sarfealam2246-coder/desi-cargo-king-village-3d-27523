using UnityEngine;

public class BharatVehicleController : MonoBehaviour
{
    public float speed = 0f;
    public float maxSpeed = 26f;
    public float turnSpeed = 52f;
    public float steerInput = 0f;
    public bool gasDown = true;
    public bool brkDown = false;
    public bool reverseGear = false;

    void Update()
    {
        float h = Input.GetAxis("Horizontal") + steerInput;
        float v = Input.GetAxis("Vertical");
        if (Mathf.Abs(v) > 0.05f) speed = v * maxSpeed;
        else if (gasDown) speed = Mathf.MoveTowards(speed, reverseGear ? -12f : maxSpeed, 14f * Time.deltaTime);
        else if (brkDown) speed = Mathf.MoveTowards(speed, 0f, 24f * Time.deltaTime);
        else speed = Mathf.MoveTowards(speed, 0f, 6f * Time.deltaTime);

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        transform.Rotate(Vector3.up * h * turnSpeed * (Mathf.Abs(speed) > 0.5f ? 1f : 0.2f) * Time.deltaTime);
    }

    AudioSource engSrc;
    AudioSource hornSrc;

    void Start()
    {
        engSrc = gameObject.AddComponent<AudioSource>();
        engSrc.loop = true;
        int sr = 22050;
        var eClip = AudioClip.Create("Engine", sr, 1, sr, false);
        float[] eData = new float[sr];
        for (int i = 0; i < sr; i++) eData[i] = Mathf.Sin(2f * Mathf.PI * 58f * i / sr) * 0.25f + Mathf.Sin(2f * Mathf.PI * 116f * i / sr) * 0.12f;
        eClip.SetData(eData, 0);
        engSrc.clip = eClip;
        engSrc.Play();

        hornSrc = gameObject.AddComponent<AudioSource>();
        var hClip = AudioClip.Create("Horn", sr / 2, 1, sr, false);
        float[] hData = new float[sr / 2];
        for (int i = 0; i < hData.Length; i++) hData[i] = (Mathf.Sin(2f * Mathf.PI * 340f * i / sr) + Mathf.Sin(2f * Mathf.PI * 425f * i / sr)) * 0.25f;
        hClip.SetData(hData, 0);
        hornSrc.clip = hClip;
    }

    void LateUpdate()
    {
        if (engSrc != null) engSrc.pitch = 0.75f + (Mathf.Abs(speed) / maxSpeed) * 1.15f;
    }

    void OnGUI()
    {
        float w = Screen.width, h = Screen.height;
        GUIStyle st = new GUIStyle(GUI.skin.button);
        st.fontSize = (int)(h * 0.038f);
        if (GUI.RepeatButton(new Rect(25, h - 135, 110, 105), "LEFT", st)) steerInput = -1f;
        else if (GUI.RepeatButton(new Rect(150, h - 135, 110, 105), "RIGHT", st)) steerInput = 1f;
        else steerInput = 0f;

        if (GUI.Button(new Rect(w - 415, h - 135, 95, 105), "HORN", st) && hornSrc != null) hornSrc.Play();
        if (GUI.Button(new Rect(w - 310, h - 135, 85, 105), reverseGear ? "R" : "D", st)) reverseGear = !reverseGear;
        brkDown = GUI.RepeatButton(new Rect(w - 215, h - 135, 95, 105), "BRK", st);
        gasDown = GUI.RepeatButton(new Rect(w - 110, h - 155, 95, 125), "GAS", st);
        GUI.Box(new Rect(w * 0.5f - 95, h - 75, 190, 50), Mathf.RoundToInt(Mathf.Abs(speed) * 3.6f) + " km/h", st);
    }
}
