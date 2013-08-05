using UnityEngine;
using System.Collections;

public class Cube : MonoBehaviour
{
    Quaternion spin;
    bool flag;

    void Start ()
    {
        spin = Quaternion.AngleAxis (Random.Range (60, 120), Vector3.up);
        spin *= Quaternion.AngleAxis (Random.Range (60, 120), Vector3.right);
    }
    
    void Update ()
    {
        transform.localRotation =
            Quaternion.Slerp (Quaternion.identity, spin, Time.deltaTime) *
            transform.localRotation;

        Vector3 targetScale = Vector3.one * (flag ? 2 : 1);
        transform.localScale = Vector3.Lerp(targetScale, transform.localScale, Mathf.Exp(-10.0f * Time.deltaTime));
    }

    void OnOscMessage(int value)
    {
        flag = value > 0;
        if (flag) {
            transform.localScale = Vector3.one * 3.0f;
            audio.Play ();
        }
    }
}
