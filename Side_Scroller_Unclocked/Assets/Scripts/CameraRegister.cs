using Cinemachine;
using UnityEngine;

public class CameraRegister : MonoBehaviour //Script qui enregistre les caméras virtuelles auprès du CameraManager pour qu'elles puissent être utilisées dans le jeu.
{
    private void OnEnable()
    {
        CameraManager.Register(GetComponent<CinemachineVirtualCamera>());
    }
    private void OnDisable()
    {
        CameraManager.Unregister(GetComponent<CinemachineVirtualCamera>());
    }
}