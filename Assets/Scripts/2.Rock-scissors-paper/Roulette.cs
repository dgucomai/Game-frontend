using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Roulette : MonoBehaviour // 클래스 이름을 파일 이름과 동일하게 수정
{
    [Header("슬롯 설정")]
    public List<GameObject> glowImages; 

    [Header("시간 설정")]
    public float minDuration = 4.0f;    
    public float maxDuration = 7.0f;    
    public float slowPhaseTime = 1.5f;  

    [Header("속도 설정")]
    public float fastTickTime = 0.05f;  
    public float slowTickMaxTime = 0.5f; 

    private bool isSpinning = false;

    // 유니티 상단 재생(세모) 버튼을 누르면 자동으로 실행됨
    void Start()
    {
        StartSpin();
    }

    public void StartSpin()
    {
        if (!isSpinning)
        {
            StartCoroutine(SpinRoutine());
        }
    }

    IEnumerator SpinRoutine()
    {
        isSpinning = true;

        // 1. 시작 위치 랜덤 설정
        int currentIndex = Random.Range(0, glowImages.Count);
        float totalDuration = Random.Range(minDuration, maxDuration);
        float elapsedTime = 0f;
        float currentTickTime = fastTickTime;

        // 모든 빛 끄고 시작
        foreach (var img in glowImages) 
        {
            if(img != null) img.SetActive(false);
        }

        // 2. 룰렛 회전 시작
        while (elapsedTime < totalDuration)
        {
            // 이전 빛 끄고 현재 빛 켜기
            glowImages[currentIndex].SetActive(false);
            currentIndex = (currentIndex + 1) % glowImages.Count;
            glowImages[currentIndex].SetActive(true);

            // 3. 속도 조절 (마지막 1.5초 동안 점점 느려짐)
            float remainingTime = totalDuration - elapsedTime;
            if (remainingTime <= slowPhaseTime)
            {
                float t = 1f - (remainingTime / slowPhaseTime);
                currentTickTime = Mathf.Lerp(fastTickTime, slowTickMaxTime, t);
            }

            yield return new WaitForSeconds(currentTickTime);
            elapsedTime += currentTickTime;
        }

        // 4. 결과 연출 (마지막 당첨 칸 깜빡이기)
        yield return StartCoroutine(BlinkEffect(glowImages[currentIndex]));

        isSpinning = false;
    }

    IEnumerator BlinkEffect(GameObject target)
    {
        for (int i = 0; i < 3; i++)
        {
            target.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            target.SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }
    }
}