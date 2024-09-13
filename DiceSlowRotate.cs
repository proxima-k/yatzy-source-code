using UnityEngine;
using Random = UnityEngine.Random;

public class DiceSlowRotate : MonoBehaviour {
    [SerializeField] private float _rotateSpeed = 6f;
    [SerializeField] private Vector3 _rotateDirection = Vector3.up;

    private void Start() {
        _rotateDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        _rotateSpeed = Random.Range(3, 6);
    }

    private void Update() {
        transform.Rotate(_rotateDirection * (_rotateSpeed * Time.deltaTime));
    }
}
