using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AsyncProcessing : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button runBlockingButton;
    [SerializeField] private Button runBackgroundButton;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text resultsText;

    [Header("Workload")]
    [SerializeField] private int iterations = 10_000_000;

    private bool isRunning;
    private bool isDestroyed;

    private void Awake()
    {
        runBlockingButton.onClick.AddListener(RunBlockingTest);
        runBackgroundButton.onClick.AddListener(RunBackgroundTest);

        statusText.text = "Ready";
        resultsText.text = "Run either test to begin.";
    }

    private void OnDestroy()
    {
        isDestroyed = true;

        if (runBlockingButton != null)
            runBlockingButton.onClick.RemoveListener(RunBlockingTest);

        if (runBackgroundButton != null)
            runBackgroundButton.onClick.RemoveListener(RunBackgroundTest);
    }

    public void RunBlockingTest()
    {
        if (isRunning)
            return;

        isRunning = true;
        SetButtonsInteractable(false);

        statusText.text = "Running on main thread...";
        resultsText.text = "Calculating...";

        Stopwatch stopwatch = Stopwatch.StartNew();

        double result = Calculate(iterations);

        stopwatch.Stop();

        ShowResults(
            "Blocking",
            stopwatch.Elapsed.TotalMilliseconds,
            result);
    }

    public async void RunBackgroundTest()
    {
        if (isRunning)
            return;

        isRunning = true;
        SetButtonsInteractable(false);

        statusText.text = "Calculating on background thread...";
        resultsText.text = "Working...";

        int workIterations = iterations;
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            double result = await Task.Run(
                () => Calculate(workIterations));

            stopwatch.Stop();

            if (isDestroyed)
                return;

            ShowResults(
                "Background",
                stopwatch.Elapsed.TotalMilliseconds,
                result);
        }
        catch (Exception exception)
        {
            stopwatch.Stop();

            if (!isDestroyed)
            {
                statusText.text = "Background test failed";
                resultsText.text = exception.Message;
                UnityEngine.Debug.LogException(exception);
            }
        }
    }

    private static double Calculate(int count)
    {
        double result = 0;

        for (int i = 1; i <= count; i++)
        {
            result += Math.Sqrt(i) * Math.Sin(i);
        }

        return result;
    }

    private void ShowResults(
        string testName,
        double elapsedMs,
        double result)
    {
        if (isDestroyed)
            return;

        statusText.text = $"{testName} test completed";

        resultsText.text =
            $"Test: {testName}\n" +
            $"Iterations: {iterations:N0}\n" +
            $"Elapsed: {elapsedMs:F2} ms\n" +
            $"Result: {result:F4}";

        UnityEngine.Debug.Log(
            $"{testName} test: {elapsedMs:F2} ms, " +
            $"result = {result:F4}");

        isRunning = false;
        SetButtonsInteractable(true);
    }

    private void SetButtonsInteractable(bool interactable)
    {
        if (runBlockingButton != null)
            runBlockingButton.interactable = interactable;

        if (runBackgroundButton != null)
            runBackgroundButton.interactable = interactable;
    }
}
