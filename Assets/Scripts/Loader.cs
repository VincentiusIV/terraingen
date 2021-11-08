using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{

    public GameObject LoadingBG;
    public Slider slider;
    public Text ProgressText;
    // Start is called before the first frame update
    public Text sizeText;
    public Text volcText;
    public Text caveText;
    public Text heightText;
    public Text noiseText;
    public static int volcs, size, caves, height, noiseOffset;
    private float time = 120f;
    public void Load(int index)
    {
        slider.value = 0f;
        Debug.LogFormat("Size: {0} -- Height: {3} -- Noise Offset: {4} -- volcanos: {1} -- caves: {2}", size, volcs, caves, height, noiseOffset);
        StartCoroutine(LoadAsync(index));
        
    }

    IEnumerator LoadAsync (int index)
    {

        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        LoadingBG.SetActive(true);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            slider.value += progress;
            ProgressText.text = "Loading Scene.... (This may take a while depending on size)";
            yield return null;
        }
    }

    public void SetSize()
    {
        string text = sizeText.text.ToString();
        int.TryParse(text, out Loader.size);
    }
    public void SetVolcs()
    {
        string text = volcText.text.ToString();
        int.TryParse(text, out Loader.volcs);
    }
    public void SetCaves()
    {
        string text = caveText.text.ToString();
        int.TryParse(text, out Loader.caves);
    }
    public void SetHeight()
    {
        string text = heightText.text.ToString();
        int.TryParse(text, out Loader.height);
    }
    public void SetNoiseOffset()
    {
        string text = noiseText.text.ToString();
        int.TryParse(text, out Loader.noiseOffset);
    }

}
