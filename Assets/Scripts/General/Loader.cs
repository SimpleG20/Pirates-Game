using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    [SerializeField] float _scaleSpeed;

    [SerializeField] Image _backImage;
    [SerializeField] Image _loadImage;
    public bool Loading {  get; private set; }

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Loading)
        {
            _backImage.rectTransform.offsetMax += Vector2.one * Time.deltaTime * _scaleSpeed;
            _backImage.rectTransform.offsetMin += Vector2.one * Time.deltaTime * _scaleSpeed * -1;
            _loadImage.transform.Rotate(Vector3.forward * Time.deltaTime * 60f);
        }
    }

    public void StartGame()
    {
        StartCoroutine(SceneProgress(1, true));
    }

    public void Menu()
    {
        StartCoroutine(SceneProgress(0));
    }

    IEnumerator SceneProgress(int sceneIndex, bool startGame = false)
    {
        yield return null;

        Loading = true;
        var operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);

        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            var progress = Mathf.Clamp01(operation.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");

            if (operation.progress == 0.9f)
            {
                yield return new WaitForSeconds(3);
                Debug.Log("Loading completed");
                operation.allowSceneActivation = true;
            }
            yield return null;
        }

        Loading = false;

        if (startGame) FindObjectOfType<GameManager>().InitiateGameplay();
        Destroy(gameObject);
    }
}
