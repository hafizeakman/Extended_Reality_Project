using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractMoveThenLoadScene : MonoBehaviour
{
    [Header("Object To Move")]
    public Transform objectToMove;

    [Header("Movement")]
    public Vector3 moveDirection = Vector3.up;
    public float moveSpeed = 1f;
    public float moveDuration = 2f;

    [Header("Scene Change")]
    public string sceneToLoad;
    public float sceneLoadDelay = 3f;

    [Header("Safety")]
    public bool triggerOnlyOnce = true;

    private bool hasTriggered = false;

    public void TriggerSequence()
    {
        if (triggerOnlyOnce && hasTriggered)
            return;

        hasTriggered = true;

        if (objectToMove == null)
        {
            Debug.LogWarning("Object To Move is not assigned.");
            return;
        }

        StartCoroutine(MoveObject());
        StartCoroutine(LoadSceneAfterDelay());
    }

    private IEnumerator MoveObject()
    {
        float timer = 0f;
        Vector3 normalizedDirection = moveDirection.normalized;

        while (timer < moveDuration)
        {
            objectToMove.position += normalizedDirection * moveSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(sceneLoadDelay);

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene To Load is empty.");
        }
    }
}