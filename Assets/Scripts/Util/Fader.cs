using UnityEngine;
using System.Collections;

public class Fader : MonoSingleton<Fader>
{
   public const float DEFAULT_FADE_TIME = .3f;

   public Color StartFadeColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
   public Color EndFadeColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
   public float FadeTime = 0.0f;
   public float TimeIntoFade = 0.0f;

   public Color CurrentColor;
   public Material MyMaterial;
   public Texture2D TexWhite; 

   public bool isRunning;
   public Canvas m_canvas; 

   protected override void Awake()
   {
      base.Awake();
      if (!IsInstance()) {
         return;
      }

      // create a camera to take over UI
      Camera c = gameObject.AddComponent<Camera>();;
      c.clearFlags = CameraClearFlags.Depth;
      c.depth = 100;
      c.cullingMask = 1 << LayerMask.NameToLayer("UI"); 
      c.orthographic = true;

      if (m_canvas) {
         m_canvas.worldCamera = c; 
      }

      // Shader to use for my tint; 
      MyMaterial = new Material( Shader.Find("LD40/ImageEffect/Tint") );

      TexWhite = new Texture2D( 1, 1, TextureFormat.ARGB32, false, false ); 
      TexWhite.SetPixel( 0, 0, new Color(1, 1, 1, 1) ); 
   }

   public void Update()
   {
      if (m_canvas == null) {
         m_canvas = GameObject.FindObjectOfType<Canvas>(); 
         if (m_canvas != null) {
            m_canvas.renderMode = RenderMode.ScreenSpaceCamera; 
            m_canvas.worldCamera = GetComponent<Camera>(); 
         }
      }

      if (isRunning) {
         TimeIntoFade += Time.deltaTime;
         UpdateFade();
      }
   }

   public void UpdateFade()
   {
      float t = 1.0f; 
      if (FadeTime > 0.0f) {
         t = Mathf.Clamp01( TimeIntoFade / FadeTime );
      }
      
      // Are we finished fading
      if (t == 1.0f) {
         isRunning = false;
         if (EndFadeColor.a == 0.0f) {
            gameObject.SetActive(false);
            if (m_canvas) {
               m_canvas.renderMode = RenderMode.ScreenSpaceOverlay; 
            }
         }
      } else {
         if (!gameObject.activeSelf) {
            gameObject.SetActive(true); 
            if (m_canvas) {
               m_canvas.renderMode = RenderMode.ScreenSpaceCamera; 
               m_canvas.worldCamera = GetComponent<Camera>(); 
            }
         }
      }

      CurrentColor = Color.Lerp( StartFadeColor, EndFadeColor, t );
      MyMaterial.SetColor( "_Tint", CurrentColor );
   }

   void OnRenderImage( RenderTexture source, RenderTexture destination )
   {
      Graphics.Blit( source, destination, MyMaterial );
   }

   void StartFade( Color startColor, Color endColor, float time )
   {
      StartFadeColor = startColor;
      EndFadeColor = endColor;
      TimeIntoFade = 0.0f;
      FadeTime = time;
      isRunning = true;
      UpdateFade();
   }

   void FadeTo( Color c, float time )
   {
      StartFade( CurrentColor, c, time );
   }

   public static void FadeIn( Color c, float time = DEFAULT_FADE_TIME )
   {
      Fader f = Fader.GetInstance();

      // Fade in assumes an alpha of 0.0f as the final
      c.a = 0.0f;

      f.FadeTo( c, time );
   }

   public static void FadeIn( float time = DEFAULT_FADE_TIME )
   {
      Fader f = Fader.GetInstance();
      Color c = f.CurrentColor;
      FadeIn( c, time );
   }

   public static void FadeOut( Color c, float time = DEFAULT_FADE_TIME )
   {
      Fader f = Fader.GetInstance();

      // Fade in assumes an alpha of 0.0f as the final
      c.a = 1.0f;

      f.FadeTo( c, time );
   }

   public static float GetFadeTimeRemaining()
   {
      Fader f = GetInstance();
      float t = Mathf.Clamp( f.FadeTime - f.TimeIntoFade, 0.0f, f.FadeTime );
      return t;
   }
}
