using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour //Classe qui est statique car elle doit être accessible de n'importe où dans le projet pour pouvoir changer la caméra active. Elle gère les caméras virtuelles de Cinemachine en leur attribuant une priorité pour déterminer laquelle est active.
                                           // ( Code récupéré grâce à un tuto)
{
    static List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();

    public static CinemachineVirtualCamera ActiveCamera = null;

    public static bool IsActiveCamera(CinemachineVirtualCamera camera) // Fonction pour vérifier si une caméra est la caméra active
    {
        return camera == ActiveCamera;
    }

    public static void SwitchCamera(CinemachineVirtualCamera newCamera) // Fonction pour changer la caméra active
    {
        newCamera.Priority = 10;
        ActiveCamera = newCamera;

        foreach (CinemachineVirtualCamera cam in cameras) // Check toutes les caméras enresgistrées et met leur priorité à 0 si elles ne sont pas la caméra active
        {
            if (cam != newCamera)
            {
                cam.Priority = 0;
            }
        }
    }

    public static void Register(CinemachineVirtualCamera camera)
    {
        cameras.Add(camera); // Ajoute la caméra à la liste des caméras enregistrées
    }

    public static void Unregister(CinemachineVirtualCamera camera)
    {
        cameras.Remove(camera); // Supprime la caméra de la liste des caméras enregistrées
    }
}