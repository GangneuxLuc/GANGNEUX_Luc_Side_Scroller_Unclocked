using UnityEngine;
using UnityEngine.UI;

public class ignoreForce : MonoBehaviour
{
    PlayerController playerController;
    private void Awake()
    { 
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    public void IgnoreForce()
    {
        if (playerController != null)
        {
            playerController.ignoreForce = !playerController.ignoreForce; // Met le ignoreForce en on/off
            if (playerController.ignoreForce)
            {
                Button godModeButton = GetComponent<Button>();
                godModeButton.colors = new ColorBlock()
                {
                    normalColor = Color.green,
                    highlightedColor = Color.green,
                    pressedColor = Color.green,
                    selectedColor = Color.green,
                    disabledColor = Color.gray,
                    colorMultiplier = 1f,
                    fadeDuration = 0.1f
                };

            }
            else
            {

                Button godModeButton = GetComponent<Button>();
                godModeButton.colors = new ColorBlock()
                {
                    normalColor = Color.white,
                    highlightedColor = Color.white,
                    pressedColor = Color.white,
                    selectedColor = Color.white,
                    disabledColor = Color.gray,
                    colorMultiplier = 1f,
                    fadeDuration = 0.1f
                };
            }
        }
    }
}
