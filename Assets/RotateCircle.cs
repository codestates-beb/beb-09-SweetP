using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotationSpeed = 500f; // 이 값을 변경하여 회전 속도를 조절할 수 있습니다.

    // Update는 매 프레임마다 호출됩니다.
    void Update()
    {
        // Rotate 함수는 각 축을 중심으로 객체를 회전시킵니다. 
        // Time.deltaTime을 사용하여 프레임 속도에 따른 차이를 보정합니다.
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}