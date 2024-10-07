using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UICarousel : MonoBehaviour, ModalReset
{ 
    private int maxPage;
    private int currentPage;
    Vector3 targetPos;
    Vector3 defaultPos;
    
    
    [SerializeField]  RectTransform levelPagesRect;

    [SerializeField] Vector3 pageStep;
    [SerializeField] float tweenTime;
    [SerializeField] LeanTweenType tweenType;

    public Button previousBtn;
    public Button nextBtn;
    
    

    public void ResetCarousel() {
        
        currentPage = 1;
        defaultPos = new Vector3(0, 0, 0);
        maxPage = levelPagesRect.childCount;
        ChangeBtnState();
    }
    
    void MovePage() {
    if (levelPagesRect == null) {
        Debug.LogError("levelPagesRect가 null입니다.");
        return;
    }
    
    levelPagesRect.LeanMoveLocal(targetPos, tweenTime).setEase(tweenType);
    Debug.Log($"페이지 이동: 현재 페이지 {currentPage}, 목표 위치 {targetPos}");
}

// 버튼 관련
public void Previous() {
    if (currentPage > 1) {
        currentPage--;
        targetPos = defaultPos + pageStep * (currentPage - 1);
        MovePage();
        ChangeBtnState();
    } else {
        Debug.LogWarning("이전 페이지가 없습니다.");
    }
}

public void Next() {
    if (currentPage < maxPage) {
        currentPage++;
        targetPos = defaultPos + pageStep * (currentPage - 1);
        MovePage();
        ChangeBtnState();
    } else {
        Debug.LogWarning("다음 페이지가 없습니다.");
    }
}



    private void ChangeBtnState() {
        if (currentPage == 1) {
            DisablePrevious();
            EnableNext();
        } else if (currentPage == maxPage) {
            DisableNext();
            EnablePrevious();
        } else {
            EnablePrevious();
            EnableNext();
        }
        if (maxPage == 1) {
            DisableNext();
        }
    }

    private void DisablePrevious() {
        if (previousBtn == null) return;
        previousBtn.interactable = false;
        previousBtn.GetComponentInChildren<SVGImage>().enabled = false;
    }

    private void EnablePrevious() {
        if (previousBtn == null) return;
        previousBtn.interactable = true;
        previousBtn.GetComponentInChildren<SVGImage>().enabled = true;
    }
    
    private void DisableNext() {
        if (nextBtn == null) return;
        nextBtn.interactable = false;
        nextBtn.GetComponentInChildren<SVGImage>().enabled = false;
    }

    private void EnableNext() {
        if (nextBtn == null) return;
        nextBtn.interactable = true;
        nextBtn.GetComponentInChildren<SVGImage>().enabled = true;
    }

    public void SetPage(int page) {
        currentPage = page;
        targetPos = defaultPos + pageStep * (page - 1);
        MovePage();
    }

    public void ModalOptionReset() {
        SetPage(1);
        ChangeBtnState();
    }
}
