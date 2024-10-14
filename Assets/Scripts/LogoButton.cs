using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogoButton : MonoBehaviour {
    public UIModal modal;
    private void Start() {
        Button btn = transform.GetComponent<Button>();
        btn.onClick.AddListener(() => {
            
            modal.ResetMain();
            SceneManager.LoadScene("Login");
        });
    }
}
