using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : MonoBehaviour where T : RecycleObject// 제네릭을 쓸거면 클래스 뒤에 <>를 쓰고 대신 쓸 T나 이름들을 쓰면된다.
{                               // T타입은 MonoBehaviour이거나 MoniBehaviour를 상속받은 클래스만 가능
    public GameObject originalPrefeb;

    /// <summary>
    /// 풀의 크기, 처음에 생성하는 오브제그의 개수 모든 개수는 2^n으로 잡는 것이 좋다.
    /// </summary>
    public int poolSize = 64;

    /// <summary>
    /// T타입으로 지정된 오브젝트의 배열
    /// </summary>
    T[] pool;

    /// <summary>
    /// 현재 사용가능한(비활성화되어있는) 오브젝트들을 관리하는 큐
    /// </summary>
    Queue<T> readyQueue;

    /*    private void Awake()
        {
            Initialize();
        }
    */
    public void Initialize()
    {
        if (pool == null) // pool이 아직 만들어지지 않은 경우
        {
            pool = new T[poolSize]; // 배열의 크기만큼 new
            readyQueue = new Queue<T>(poolSize); // 레디큐를 만들고 capactiy를 poolSize로 지정

            GenerateObjects(0, poolSize, pool);
        }
        else // 풀이 이미 만들어져 있는 경우(ex : 씬이 추가로 로딩 or 씬이 다시 시작)
        {
            foreach (T obj in pool)
            {
                obj.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 풀에서 사용하지 않는 오브젝트를 하나 꺼낸 후 리턴하는 함수
    /// </summary>
    /// <param name="position">배치될 위치(월드좌표)</param>
    /// <param name="eulerAngle">배치될 때의 각도</param>
    /// <returns> 플에서 꺼낸 오브젝트 (활성화됨)</returns>
    public T GetObject(Vector3? position = null, Vector3? eulerAngle = null)
    {
        if (readyQueue.Count > 0) // 레디큐 오브젝트 확인
        {
            T comp = readyQueue.Dequeue();// 남아있으면 하나 꺼내고
            //comp.gameObject.SetActive(true);// 활성화 시키기
            comp.transform.position = position.GetValueOrDefault(); // 지정된 위치로 이동
            comp.transform.rotation = Quaternion.Euler(eulerAngle.GetValueOrDefault()); // 지정된 각도로 회전
            comp.gameObject.SetActive(true);// 활성화시킨후
            OnGetObject(comp);              // 오브젝트별 추가 처리
            return comp;                    // 리턴
        }
        else
        {
            // 레디큐가 비어있다. == 남은 오브젝트가 없음
            ExpandPool();       // 풀을 두배로 확장하고
            return GetObject(position, eulerAngle); // 새로 하나 꺼낸다.
        }
    }


    /// <summary>
    /// 각 오브젝트 별로 특별히 처리해야 할 일이 있을 경우 실행하는 함수
    /// </summary>
    protected virtual void OnGetObject(T component)
    {

    }

    /// <summary>
    /// 풀을 두배로 확장시키는 함수
    /// </summary>
    void ExpandPool()
    {
        // 최대한 일어나면 안되니 경고 디버그
        Debug.LogWarning($"{gameObject.name} 풀사이즈 증가. {poolSize} -> {poolSize * 2}");

        int newSize = poolSize * 2;         // 새로운 풀의 크기 지정
        T[] newPool = new T[newSize];       // 새로운 풀 생성
        for (int i = 0; i < poolSize; i++)   // 이전 풀을 새풀에 복사
        {
            newPool[i] = pool[i];
        }

        GenerateObjects(poolSize, newSize, newPool);    // 새 풀의 남은 부분에 오브젝트 생성후 추가
        pool = newPool;     // 새 풀사이즈 설정
        poolSize = newSize; // 새 풀을 풀로 설정
    }
    /// <summary>
    /// 풀에서 사용할 오브젝트를 생성하는 함수
    /// </summary>
    /// <param name="start">새로 생성 시작할 인덱스</param>
    /// <param name="end">새로 생성이 끝나는 인덱스 +1</param>
    /// <param name="result">새로 생성된 오브젝트가 들어갈 배열</param>
    /// <returns></returns>
    void GenerateObjects(int start, int end, T[] result)
    {
        for (int i = start; i < end; i++)
        {
            GameObject obj = Instantiate(originalPrefeb, transform); // 프리펩 생성후
            obj.name = $"{originalPrefeb.name}_{i}"; // 이름 바꾸고

            T comp = obj.GetComponent<T>();
            comp.onDisable += () => readyQueue.Enqueue(comp); // 재활용 오브젝트가 비활성화 되면 레디큐로 되돌려라
            //readyQueue.Enqueue(comp);  // 레디큐에 저장후 (위에 델리게이트가 활성화 되면 아래에 비활성화 코드때문에 자동처리)

            OnGenerateObject(comp);

            result[i] = comp; // 배열에 넣고
            obj.SetActive(false); // 비활성화 시키기
        }
    }

    /// <summary>
    /// 각 T타입별로 생성 직후에 필요한 추가 작업을 처리하는 함수
    /// </summary>
    /// <param name="comp">T 타입의 컴포넌트</param>
    protected virtual void OnGenerateObject(T comp)
    {

    }
}
