using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controlador de la cámara (añadir a la cámara de la escena)
/// Sitúa la cámara en la posición del jugador (con unos offsets)
/// El target debe ser del tipo PlayerMovementController para poder obtener la posición a la que apunta
/// Modifica ligeramente su posición en función de la posición a la que apunta el jugador
/// </summary>
public class CameraMovementController : MonoBehaviour
{
    #region VARIABLES
    // Referencias
    public PlayerMovementController target; // Objetivo al que sigue

    // Ajustes
    private float height = -30;  // Altura de la camara en relación al target
    private float offsetY = -11;    // Offset eje Y en relaciónj al target
    private float aimOffsetFactor = 0.03f;    // Influencia del punto de dónde se apunta al offset de la cámara en el plano XY
    #endregion VARIABLES

    #region UNITY CALLBACKS
    /// <summary>
    /// Calcula y asigna la posición a la cámara
    /// </summary>
    void Update()
    {
        transform.position = CalculateCameraPosition();
        transform.position = ApplyCameraOffset(transform.position);
    }
    #endregion UNITY CALLBACKS

    #region PRIVATE METHODS
    /// <summary>
    /// Recibe una posición y la modifica en función del punto al que está apuntando el jugador
    /// </summary>
    /// <param name="cameraPosition"></param>
    /// <returns></returns>
    private Vector3 ApplyCameraOffset(Vector3 cameraPosition)
    {
        Vector2 offset;
        Vector3 value = target.GetAimPoint();
        offset.x = value.x;
        offset.y = value.y;
        offset *= value.z;
        offset *= aimOffsetFactor;
        cameraPosition.x += offset.x;
        cameraPosition.y += offset.y;
        return cameraPosition;
    }

    /// <summary>
    /// Devuelve la posición de la cámara, teniendo en cuenta el offset de altura y del plano y
    /// </summary>
    /// <returns></returns>
    private Vector3 CalculateCameraPosition()
    {
        return new Vector3(target.transform.position.x, target.transform.position.y + offsetY, target.transform.position.z + height);
    }
    #endregion PRIVATE METHODS
}
