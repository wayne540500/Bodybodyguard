using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class ChangePlayerColor : MonoBehaviour
{
    public bool isPlayMode;
    public Color[] originalColors;
    public Color[] newColors;
    public ReadyCountDownController readyCountDownController;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Texture2D texture2D;
    public Texture2D textureTemp;
    public Point2[][] point2s;

    private int playerIndex;
    private Sprite originalSprite;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponentInParent<PlayerInput>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        texture2D = spriteRenderer.sprite.texture;
        originalSprite = spriteRenderer.sprite;
        textureTemp = duplicateTexture(texture2D);
    }
    private void Start()
    {
        if (!isPlayMode)
            playerIndex = playerInput.playerIndex;
        else
        {
            playerIndex = GetComponentInParent<PlayerController>().playerIndex;
        }
        if (!isPlayMode)
        {
            List<Color> colorList = new List<Color>();
            foreach (var color in originalColors)
            {
                colorList.Add(color);
            }
            GameDataManager.playerDatas.Add(new PlayerData(colorList, playerInput.devices[0].deviceId, playerInput.devices[0].name));
            //Debug.Log(GameDataManager.playerDatas[GameDataManager.playerDatas.Count - 1].input.playerIndex, gameObject);
        }
        else
            LoadColorsFromPlayerConfig();
        point2s = GetColorPosition(originalColors);
        for (int i = 0; i < point2s.Length; i++)
        {
            Debug.Log(point2s[i].Length);
        }
    }

    private void Update()
    {
        if (isPlayMode)
            return;

        if (readyCountDownController.isReady.Length == 0 || readyCountDownController.isReady.Length == playerIndex)
            return;

        if (readyCountDownController.isReady[playerIndex])
            animator.enabled = true;
        else
        {
            animator.enabled = false;
            spriteRenderer.sprite = originalSprite;
        }
    }

    private void LateUpdate()
    {
        SetColor();
    }


    public Point2[][] GetColorPosition(Color[] targetColor)
    {
        Point2[][] point2s_temp = new Point2[targetColor.Length][];
        for (int i = 0; i < targetColor.Length; i++)
        {
            List<Point2> temp = new List<Point2>();
            for (int y = 0; y < textureTemp.height; y++)
            {
                for (int x = 0; x < textureTemp.width; x++)
                {
                    var colorInTexture = textureTemp.GetPixel(x, y);
                    //Debug.Log(colorInTexture);
                    if (colorInTexture == targetColor[i])
                    {
                        temp.Add(new Point2(x, y));
                        //Debug.Log(temp.Count);
                    }
                }
            }
            point2s_temp[i] = new Point2[temp.Count];
            point2s_temp[i] = temp.ToArray();
        }

        return point2s_temp;
    }

    private void LoadColorsFromPlayerConfig()
    {
        if (GameDataManager.playerDatas.Count == 0)
            return;

        if (GameDataManager.playerDatas[playerIndex].colors.Count == 0)
            return;

        for (int i = 0; i < GameDataManager.playerDatas[playerIndex].colors.Count; i++)
        {
            newColors[i] = GameDataManager.playerDatas[playerIndex].colors[i];
        }
    }

    private void SetColor()
    {
        LoadColorsFromPlayerConfig();
        for (int i = 0; i < point2s.Length; i++)
        {
            foreach (var point in point2s[i])
            {
                textureTemp.SetPixel(point.x, point.y, newColors[i]);
            }
        }
        textureTemp.Apply();
        //spriteRenderer.material.SetTexture("_MainTex", textureTemp);
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetTexture("_MainTex", textureTemp);
        spriteRenderer.SetPropertyBlock(block);

    }

    public Texture2D duplicateTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.filterMode = FilterMode.Point;
        readableText.wrapMode = TextureWrapMode.Clamp;
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

    public void RandomSetColors()
    {
        for (int i = 0; i < newColors.Length; i++)
        {
            newColors[i] = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)
            );
        }
    }
}
