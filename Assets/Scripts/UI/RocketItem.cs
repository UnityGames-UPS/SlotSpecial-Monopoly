                    using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

// One rocket in the bonus-wheel background. Rises from a random point along the bottom of a spawn area
// RectTransform up to a random height (never below the spawn area's half-height), plays its ImageAnimation
// once at the top of the rise, then destroys itself. Modeled on CoinFountainItem's launch pattern, but
// self-destructing instead of pool-returning, and play-once instead of looping.
[RequireComponent(typeof(RectTransform))]
public class RocketItem : MonoBehaviour
{
  [SerializeField] private RectTransform rect;
  [SerializeField] private ImageAnimation anim;

  private Tween _riseTween;
  private Coroutine _waitForAnimCoroutine;

  private void OnValidate()
  {
    if (rect == null) rect = GetComponent<RectTransform>();
    if (anim == null) anim = GetComponent<ImageAnimation>();
  }

  // Launches this rocket from a random point along the bottom edge of spawnArea, rising to a random
  // height at or above spawnArea's midline. riseSpeed is in units/second, so every rocket travels at the
  // same speed regardless of how far it happens to be going — a shorter rise just finishes sooner rather
  // than crawling. onComplete fires once the rocket has fully finished (its ImageAnimation played through
  // once) — right before it destroys itself.
  internal void Launch(RectTransform spawnArea, float riseSpeed, Ease riseEase, Action onComplete)
  {
    KillRise();

    if (rect == null || spawnArea == null)
    {
      onComplete?.Invoke();
      Destroy(gameObject);
      return;
    }

    Rect area = spawnArea.rect;
    float spawnX = UnityEngine.Random.Range(area.xMin, area.xMax);
    float halfY = (area.yMin + area.yMax) * 0.5f;
    float targetY = UnityEngine.Random.Range(halfY, area.yMax);

    rect.anchoredPosition = new Vector2(spawnX, area.yMin);

    float distance = Mathf.Abs(targetY - area.yMin);
    float riseDuration = riseSpeed > 0f ? distance / riseSpeed : 0f;

    _riseTween = rect.DOAnchorPosY(targetY, riseDuration).SetEase(riseEase).OnComplete(() =>
    {
      PlayAnimationThenFinish(onComplete);
    });
  }

  private void PlayAnimationThenFinish(Action onComplete)
  {
    // No animation assigned yet (placeholder prefab) — finish immediately instead of hanging forever.
    if (anim == null || anim.textureArray == null || anim.textureArray.Count == 0)
    {
      onComplete?.Invoke();
      Destroy(gameObject);
      return;
    }

    anim.StartAnimation();
    _waitForAnimCoroutine = StartCoroutine(WaitForAnimationFinish(onComplete));
  }

  private IEnumerator WaitForAnimationFinish(Action onComplete)
  {
    yield return new WaitUntil(() => anim.currentAnimationState == ImageAnimation.ImageState.FINISHED);
    _waitForAnimCoroutine = null;
    onComplete?.Invoke();
    Destroy(gameObject);
  }

  private void OnDestroy()
  {
    KillRise();
    if (_waitForAnimCoroutine != null)
    {
      StopCoroutine(_waitForAnimCoroutine);
      _waitForAnimCoroutine = null;
    }
  }

  private void KillRise()
  {
    _riseTween?.Kill();
    _riseTween = null;
  }
}
