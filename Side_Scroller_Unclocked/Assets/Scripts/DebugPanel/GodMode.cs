using UnityEngine;
using UnityEngine.UI;

public class GodMode : MonoBehaviour // Script pour activer/désactiver le GODMODE du joueur en un clic depuis le panneau de debug
{
    private PlayerController playerController; 
    private void Awake()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    public void ToggleGodMode() // Fonction pour basculer le God Mode du joueur et feedback visuel du bouton en fonction de l'état du God Mode
    {
        if (playerController != null)
        {
            playerController.GodModeIsOn = !playerController.GodModeIsOn; // Met God Mode en on/off
                if (playerController.GodModeIsOn)
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

