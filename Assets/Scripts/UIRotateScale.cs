using System;
using UnityEngine;

public class UIRotateScale : MonoBehaviour
{
    [SerializeField] private Vector3 _scale = Vector3.zero;
    [SerializeField] private Vector3 _rotation = Vector3.zero;
    [SerializeField] private float _speed;

    private void Update()
    {
        float time = (Mathf.Sin(_speed * Time.time) + 1) / 2;
        transform.rotation = Quaternion.Euler(time * _rotation);
        transform.localScale = time * _scale + (1 - time) * Vector3.one;
    }
}
