using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Procedurally generates background and visible bounds of gameplay area once and for all gameplay runs
public class PlayAreaBGGenerator : MonoBehaviour
{
    [SerializeField] private float _generationBoundsWidth = 25.0f;
    [SerializeField] private float _generationBoundsHeight = 25.0f;
    [SerializeField] private float _generationBoundsDepth = 25.0f;
    [SerializeField] private float _levelBoundsObjectsDensity = 1.0f;
    [SerializeField] private int _generateBackgroundObjects = 30;
    [SerializeField] private List<GameObject> _backgroundObjectPrefabs;
    [SerializeField] private List<GameObject> _levelBoundsPrefabs;
    [SerializeField] private float _levelBoundsRadius;
    [SerializeField] private Transform _levelBoundsCenter;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        GenerateBackground();
        GenerateBounds();
    }

    private void GenerateBounds()
    {
        if (_levelBoundsCenter == null) return;
        Vector3 boundsCenter = _levelBoundsCenter.position;
        int counter = 0;

        // Doing a full circle around level bounds center at _levelBoundsObjectsDensity step
        for (float angle = 0f; angle < 360f; angle+= _levelBoundsObjectsDensity)
        {
            var newBoundsObj = Instantiate(_levelBoundsPrefabs[Random.Range(0, _levelBoundsPrefabs.Count)]);
            newBoundsObj.gameObject.name = "boundsObject_" + counter.ToString("D3");
            newBoundsObj.transform.SetParent(transform, false);
            // Place new bounds object at distance of _levelBoundsRadius from arena center and at a randomized height
            Vector3 newPos = boundsCenter + new Vector3(0, Random.Range(-7,7), _levelBoundsRadius);
            newBoundsObj.transform.position = newPos;
            newBoundsObj.transform.Rotate(Vector3.up, Random.Range(-180, 180)); //rotates asteroid around its center
            newBoundsObj.transform.RotateAround(boundsCenter, Vector3.up, angle); //positions asteroid on the bounds circle
            counter++;
        }
    }

    private void GenerateBackground()
    {
        for (int i = 0; i < _generateBackgroundObjects; i++)
        {
            Vector3 newPos = new Vector3(
                Random.Range(-_generationBoundsWidth / 2f, _generationBoundsWidth / 2f),
                Random.Range(-_generationBoundsHeight / 2f, _generationBoundsHeight / 2f),
                Random.Range(-_generationBoundsDepth / 2f, _generationBoundsDepth / 2f));
            var newBackgroundObj = Instantiate(_backgroundObjectPrefabs[Random.Range(0, _backgroundObjectPrefabs.Count)]);
            newBackgroundObj.gameObject.name = "backgroundObject_" + i.ToString("D3");
            newBackgroundObj.transform.SetParent(transform, false);
            newBackgroundObj.transform.localPosition = newPos;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(_generationBoundsWidth, _generationBoundsHeight, _generationBoundsDepth));
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(_levelBoundsCenter.position, _levelBoundsRadius);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_levelBoundsCenter.position, _levelBoundsRadius);
    }
}
