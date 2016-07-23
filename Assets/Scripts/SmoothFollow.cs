using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour {

    public Transform target;

    public float height = 10f;
    public float distance = 16f;
    public float smoothSpeed = 1;
   

  
	// Update is called once per frame
	void Update () {
        //因为是计算方向，所以只需要拿单位向量计算就可以了
        Vector3 targetForward = target.forward;//Z轴，取得正前方的方向
        targetForward.y = 0;

        Vector3 currentForward = transform.forward;
        currentForward.y = 0;

        Vector3 forward = Vector3.Lerp(currentForward.normalized, targetForward.normalized,//normalized表示返回向量的长度为1（只读）
            smoothSpeed *Time .deltaTime );

        Vector3 targetPos = target.position + Vector3.up * height - forward * distance;
        this.transform.position = targetPos;
        transform.LookAt(target);

	}
}
