#if UNITY_WEBGL
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TestMicrophoneWebGL : MonoBehaviour
{
    public Text InitResultText;
    public RectTransform RecordingDialog;

    private const int BufferSize = 16384;

    private AudioClip _clip;

    public IEnumerator Start()
    {
        MicrophoneWebGL.Init(BufferSize, 1);

        string initResult;
        do
        {
            initResult = MicrophoneWebGL.PollInit();
            InitResultText.text = initResult;
            yield return null;
        } while (initResult == "pending");

        if (initResult == "ready")
            RecordingDialog.gameObject.SetActive(true);
    }

    public void Record()
    {
        MicrophoneWebGL.Start();
    }

    public void Stop()
    {
        MicrophoneWebGL.Stop();

        var numBuffers = MicrophoneWebGL.GetNumBuffers();
        Debug.Log("recorded " + numBuffers + " buffers");

        var buffer = new float[BufferSize];

        int pos = -1;
        _clip = AudioClip.Create("recordedclip", numBuffers * BufferSize, 1, 44100, false,
            data =>
            {
                for (int i = 0; i < data.Length; ++i)
                {
                    pos = (pos + 1) % BufferSize;
                    if (pos == 0)
                    {
                        var ok = MicrophoneWebGL.GetBuffer(buffer);
                        if (!ok)
                            Debug.LogError("not ok");
                    }
                    data[i] = buffer[pos];
                }
            });
    }

    public void Play()
    {
        var audioSource = GetComponent<AudioSource>();
        audioSource.clip = _clip;
        audioSource.Play();
    }
}
#endif
