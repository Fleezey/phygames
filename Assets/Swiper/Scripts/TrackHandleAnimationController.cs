using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using PhyGames;


[RequireComponent(typeof (TrackHandle))]
public class TrackHandleAnimationController : MonoBehaviour
{
    private TrackHandle m_Handle;
    private RectTransform m_SymbolRectTransform;


    private void Awake()
    {
        m_Handle = GetComponent<TrackHandle>();
        m_SymbolRectTransform = m_Handle.SymbolImage.GetComponent<RectTransform>();
    }


    public void Appear()
    {
        StartCoroutine(StartAppear());
    }

    private IEnumerator StartAppear()
    {
        float scaleTime = 0.125f;
        float symbolFadeDelay = scaleTime / 2f;

        LeanTween.scale(m_Handle.RectTransform, new Vector3(0.25f, 0.25f, 0.25f), 0f);
        LeanTween.alpha(m_SymbolRectTransform, 0f, 0f);

        LeanTween.scale(m_Handle.RectTransform, Vector3.one, scaleTime);
        yield return new WaitForSeconds(symbolFadeDelay);
        AppearSymbol();

        yield return null;
    }


    public void AppearSymbol()
    {
        StartCoroutine(StartAppearSymbol());
    }

    private IEnumerator StartAppearSymbol()
    {
        LeanTween.scale(m_SymbolRectTransform, new Vector3(0.25f, 0.25f, 0.25f), 0f);
        LeanTween.scale(m_SymbolRectTransform, Vector3.one, 0.125f);

        LeanTween.alpha(m_SymbolRectTransform, 0f, 0f);
        LeanTween.alpha(m_SymbolRectTransform, 1f, 0.125f);
        yield return null;
    }
}
