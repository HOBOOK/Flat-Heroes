using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class faceOff : MonoBehaviour
{
    public Sprite defaltFace;
    public Sprite doFace;
    public Sprite eyeOffFace;
    public Sprite hitFace;
    public Sprite mouceFace;
    public Sprite currentSprite;
    public FaceType faceType;
    private Animator faceAnimator;
    private void Start()
    {
        currentSprite = GetComponent<SpriteRenderer>().sprite;
        faceAnimator = this.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if(GetCurrentSprite()!=null&&!currentSprite.name.Equals(GetCurrentSprite().name))
        {
            ChangeFace(faceType);
        }
    }

    public Animator GetFaceAnimator()
    {
        if (this.GetComponent<Animator>())
            return this.GetComponent<Animator>();
        else
            return null;
    }

    public enum FaceType
    {
        defaltFace,
        doFace,
        eyeOffFace,
        hitFace,
        hurt_defaultFace,
        hurt_eyeOffFace,
        mouseFace
    }
    public Sprite GetCurrentSprite()
    {
        switch (faceType)
        {
            case FaceType.defaltFace:
                return defaltFace;
            case FaceType.doFace:
                return doFace;
            case FaceType.eyeOffFace:
                return eyeOffFace;
            case FaceType.hitFace:
                return hitFace;
            case FaceType.mouseFace:
                return mouceFace;
            default:
                return defaltFace;
        }
    }

    public void ChangeFace(FaceType ft)
    {
        faceType = ft;
        switch (faceType)
        {
            case FaceType.defaltFace:
                currentSprite = defaltFace;
                break;
            case FaceType.doFace:
                currentSprite = doFace;
                break;
            case FaceType.eyeOffFace:
                currentSprite = eyeOffFace;
                break;
            case FaceType.hitFace:
                currentSprite = hitFace;
                break;
            case FaceType.mouseFace:
                currentSprite = mouceFace;
                break;
        }
        if(currentSprite!=null)
        {
            this.GetComponent<SpriteRenderer>().sprite = currentSprite;
            currentSprite = this.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            currentSprite = defaltFace;
        }
    }

    public void Face_Mouse()
    {
        faceAnimator.SetTrigger("Mouse");
    }
}
