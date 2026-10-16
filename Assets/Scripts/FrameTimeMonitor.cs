
using TMPro;
using UnityEngine;

public class FrameTimeMonitor : MonoBehaviour
{
    [SerializeField] private TMP_Text frameTimeText;

    private float smoothedDeltaTime;
    private float longestFrameMs;
    private int frameCount;

    private void Update()
    {
        float delta = Time.unscaledDeltaTime;

        smoothedDeltaTime = smoothedDeltaTime <= 0f
            ? delta
            : Mathf.Lerp(smoothedDeltaTime, delta, 0.1f);

        float frameMs = delta * 1000f;
        float fps = delta > 0f ? 1f / delta : 0f;

        longestFrameMs = Mathf.Max(longestFrameMs, frameMs);
        frameCount++;

        frameTimeText.text =
            $"FPS: {fps:F0}\n" +
            $"Frame time: {frameMs:F1} ms\n" +
            $"Smoothed: {smoothedDeltaTime * 1000f:F1} ms\n" +
            $"Longest frame: {longestFrameMs:F1} ms\n" +
            $"Frames observed: {frameCount}";
    }

    public void ResetMeasurements()
    {
        longestFrameMs = 0f;
        frameCount = 0;
        smoothedDeltaTime = 0f;
    }
}
