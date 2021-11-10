using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{

    public GameObject LoadingBG;
    public GameObject advancedSettings, settings;
    public Slider slider;
    public Text ProgressText;
    // Start is called before the first frame update
    public Text sizeText;
    public Text volcText;
    public Text caveText;
    public Text heightText;
    public Text noiseText;
    public Text noiseScaleText;
    public Text BeachColor, VolcSize, TreeSpawn, RockSpawn, NoiseP, NoiseL;
    public static int volcs, size, caves, height, noiseOffset, beachColor, volcSize, treeSpawn, rockSpawn;
    public static float noiseP, noiseL, noiseScale;
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
    public void SetNoiseScale()
    {
        string text = noiseScaleText.text.ToString();
        float.TryParse(text, out Loader.noiseScale);
    }
    public void SetBC()
    {
        string text = BeachColor.text.ToString();
        int.TryParse(text, out Loader.beachColor);
    }
    public void SetVolcSize()
    {
        string text = VolcSize.text.ToString();
        int.TryParse(text, out Loader.volcSize);
    }
    public void SetTreeSpawn()
    {
        string text = TreeSpawn.text.ToString();
        int.TryParse(text, out Loader.treeSpawn);
    }
    public void SetRockSpawn()
    {
        string text = RockSpawn.text.ToString();
        int.TryParse(text, out Loader.rockSpawn);
    }
    public void SetNoiseP()
    {
        string text = NoiseP.text.ToString();
        float.TryParse(text, out Loader.noiseP);
    }
    public void SetNoiseL()
    {
        string text = NoiseL.text.ToString();
        float.TryParse(text, out Loader.noiseL);
    }


    public void ToggleAdvanced()
    {
        advancedSettings.SetActive(!advancedSettings.activeSelf);
        settings.SetActive(!settings.activeSelf);
    }
}
