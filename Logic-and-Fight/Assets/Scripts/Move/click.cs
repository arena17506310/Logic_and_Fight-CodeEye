using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCreator : MonoBehaviour
{
    // 1. 유니티 에디터에서 원본 큐브 프리팹을 넣어줄 공간
    public GameObject cubePrefab; 

    // 2. 큐브가 생성될 좌표를 미리 정해둡니다. (X=0, Y=1, Z=0)
    public Vector3 spawnPosition = new Vector3(0, 1, 0);

    void Start()
    {
        // 게임이 시작되자마자 단 한 번 실행됩니다.
        CreateSingleCube();
    }

    void CreateSingleCube()
    {   
        
            Instantiate(cubePrefab, spawnPosition, Quaternion.identity);
        
        // 3. 가장 중요한 함수: Instantiate(생성할 것, 위치, 회전)
        // 설계도(cubePrefab)를 가지고 정해진 위치에 큐브를 실체화합니다.
        
    }
}