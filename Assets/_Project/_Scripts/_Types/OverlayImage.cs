using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class OverlayImage : Service<OverlayImage>
{
    public Image image;
    List<float> queueDurations = new List<float>();
    bool playing = false;
    bool pingPong = false;

    Sequence lastSeq;

    public OverlayImage FadeIn(float duration = 0.5f, bool forceThis = false)
    {
        if(forceThis)
        {
            queueDurations.Clear();
            playing = false;
            lastSeq.Kill();
        }

        queueDurations.Add(duration);
        if(!playing)
            PlayNext();
        
        return this;
    }

    public OverlayImage FadeOut(float duration = 0.5f)
    {
        queueDurations.Add(duration);
        if (!playing)
            PlayNext();

        return this;
    }

    void PlayNext()
    {
        if (queueDurations.Count > 0)
        {
            playing = true;

            var d = queueDurations[0];
            queueDurations.RemoveAt(0);

            var endValue = pingPong ? 0.0f : 1.0f;

			pingPong = !pingPong;

            Sequence seq = DOTween.Sequence();
            lastSeq = seq;
            seq.SetUpdate(true);

            seq.Append(image.DOFade(endValue, d));
            seq.AppendCallback(() =>
            {
                PlayNext();
            });
        }
        else
            playing = false;
    }
}
