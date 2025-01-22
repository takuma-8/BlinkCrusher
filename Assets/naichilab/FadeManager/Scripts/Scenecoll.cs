using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenecoll : MonoBehaviour
{
    // Start is called before the first frame update
    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            // ƒV[ƒ“‘JˆÚ
            FadeManager.Instance.LoadScene("newPlayScene", 1.0f);
        }
    }
}
