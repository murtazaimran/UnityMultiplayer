using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity = 3f;
    [SerializeField] private float distance = 6f;
    [SerializeField] private float height = 3f;

    private Transform target;

    private float yaw;
    private float pitch = 15f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CreateCrosshair();
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            FindLocalPlayer();
            return;
        }

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * mouseSensitivity;
        pitch -= mouseY * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -20f, 60f);

        Quaternion rotation = Quaternion.Euler(
            pitch,
            yaw,
            0f
        );

        Vector3 targetPosition =
            target.position + Vector3.up * height;

        transform.position =
            targetPosition -
            rotation * Vector3.forward * distance;

        transform.LookAt(targetPosition);

        RotatePlayer();
    }

    private void FindLocalPlayer()
    {
        PlayerController[] players =
            FindObjectsByType<PlayerController>(
                FindObjectsSortMode.None
            );

        foreach (PlayerController player in players)
        {
            if (player.IsOwner)
            {
                target = player.transform;

                yaw = target.eulerAngles.y;

                break;
            }
        }
    }

    private void RotatePlayer()
    {
        Vector3 direction = transform.forward;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        target.rotation = Quaternion.LookRotation(direction);
    }

    private void CreateCrosshair()
    {
        GameObject crosshair = new GameObject("Crosshair");

        Canvas canvas = crosshair.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        GameObject dot = new GameObject("Dot");

        dot.transform.SetParent(crosshair.transform);

        RectTransform rect =
            dot.AddComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;

        rect.sizeDelta = new Vector2(30f, 30f);

        UnityEngine.UI.Image image =
            dot.AddComponent<UnityEngine.UI.Image>();

        image.color = Color.white;
    }
}