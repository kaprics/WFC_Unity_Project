using DG.Tweening;
using UnityEngine;

public class SpawnTween : MonoBehaviour
{
    public Ease ease;
    
    private void Start()
    {
        transform.DOMove(new Vector3(transform.position.x, 0, transform.position.z), Random.Range(1f, 1.5f)).SetEase(ease);
    }
}
