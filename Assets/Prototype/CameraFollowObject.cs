using System.Collections;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float _flipRotationTime = 0.5f;

    private Coroutine _turnCoroutine;
    private PersonMovement _player;
    private bool _IsFacingRight;

    private void Awake()
    {
        _player = _playerTransform.gameObject.GetComponent<PersonMovement>();

        if (_player.transform.rotation.y == 0)
        {
            _IsFacingRight = true;
        }
        else if (_player.transform.rotation.y == 180)
        {
            _IsFacingRight = false;
        }
    }
    private void Update()
    {
        if (!PauseMenu.GameIsPaused)
        {
            transform.position = _player.transform.position;
        }
    }
    public void CallTurn()
    {
        _turnCoroutine = StartCoroutine(FlipYLerp());
    }
    private IEnumerator FlipYLerp()
    {
        float startRotationY = transform.localEulerAngles.y;
        float endRotationAmount = DetermineEndRotation();

        float elapsedTime = 0f;
        while (elapsedTime < _flipRotationTime)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / _flipRotationTime);
            float yRotation = Mathf.LerpAngle(startRotationY, endRotationAmount, t);

            Vector3 currentRotation = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(currentRotation.x, yRotation, currentRotation.z);

            yield return null;
        }
    }
    private float DetermineEndRotation()
    {
        _IsFacingRight = !_IsFacingRight;
        if (_IsFacingRight)
        {
            return -180f;
        }
        else
        {
            return 0f;
        }
    }
}
