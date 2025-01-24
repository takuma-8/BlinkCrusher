using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform cameraTransform; // カメラのTransform
    public Camera mainCamera; // 通常時のカメラ
    public Camera emperorCamera; // エンペラー状態用のカメラ

    private bool isEmperorMode = false; // エンペラー状態かどうか

    private void LateUpdate()
    {
        if (isEmperorMode)
        {
            // エンペラー状態ではカメラ操作を停止
            return;
        }
    }

    public void SetEmperorMode(bool isActive, Vector3 fixedDirection)
    {
        isEmperorMode = isActive;
        if (isEmperorMode)
        {
            if (mainCamera != null) mainCamera.gameObject.SetActive(false);
            if (emperorCamera != null)
            {
                emperorCamera.gameObject.SetActive(true);
                emperorCamera.transform.rotation = Quaternion.Euler(fixedDirection);
                Debug.Log("エンペラー状態のカメラに切り替え、方向を固定しました。");
            }
        }
        else
        {
            if (mainCamera != null) mainCamera.gameObject.SetActive(true);
            if (emperorCamera != null) emperorCamera.gameObject.SetActive(false);
            Debug.Log("通常状態のカメラに戻りました。");
        }
    }

    private void OnDrawGizmos()
    {
        if (cameraTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(cameraTransform.position, 0.5f);
        }
    }
}
