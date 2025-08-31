using BGJ_14;
using GRD.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class YouWinSceneController : SceneController<YouWinSceneData>
{
    [SerializeField] private CanvasGroup _text1CanvasGroup;
    [SerializeField] private CanvasGroup _text2CanvasGroup;

    private bool showText1;
    private bool showText2;

    public override async Task OnLoad()
    {
        showText1 = false; 
        showText2 = false;
        _text1CanvasGroup.alpha = 0;
        _text2CanvasGroup.alpha = 0;

        await Task.Delay(2000);

        showText1 = true;
        while (_text1CanvasGroup.alpha < 1) 
        {
            await Task.Delay(500);
        }

        await Task.Delay(2000);

        showText2 = true;
        while (_text2CanvasGroup.alpha < 1)
        {
            await Task.Delay(500);
        }

        await Task.Delay(5000);

        await base.OnLoad();
    }

    public override Task OnPostLoad()
    {
        SceneOrchestrator.LoadScene(new TitleSceneData());
        return base.OnPostLoad();
    }

    private void Update()
    {
        if (showText1) 
        {
            _text1CanvasGroup.alpha += Time.deltaTime;
            _text1CanvasGroup.alpha = Mathf.Clamp01(_text1CanvasGroup.alpha);
        }

        if (showText2)
        {
            _text2CanvasGroup.alpha += Time.deltaTime;
            _text2CanvasGroup.alpha = Mathf.Clamp01(_text2CanvasGroup.alpha);
        }
    }
}

public class YouWinSceneData : ISceneData<YouWinSceneController>
{
    public string SceneName => "YouWinScene";
}
