using Steel.Services;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Steel.Presentation.UI.Boot
{
    /// <summary>
    /// The only object in the Boot scene. Shows splash, start menu and loading, then opens the city.
    /// Decisions live in MenuFlow (Services), wording in MenuText, look in BootTheme.
    /// </summary>
    public sealed class BootScreen : MonoBehaviour
    {
        public const string Scene = "Boot", City = "City";
        public MenuFlow Flow { get; private set; }
        public SplashView Splash { get; private set; }
        public MenuView Menu { get; private set; }
        public ConfirmView Confirm { get; private set; }
        public LoadingView Loading { get; private set; }
        public RectTransform SafeRoot { get; private set; }
        private const float MaxStep = .1f;
        private AsyncOperation load;
        private MenuStage drawn;

        private void Start()
        {
            if (Flow == null) Show(CareerLaunch.Peek(), GraphicsOption.ForDevice());
        }

        /// <summary>Builds the screens for a known career; Start calls it with what is on this device.</summary>
        public void Show(CareerSummary career, GraphicsOption graphics)
        {
            var theme = BootTheme.Load();
            Flow = new MenuFlow(career, theme.SplashSeconds);
            SafeRoot = BootCanvas.Build(transform, theme);
            Menu = new MenuView(SafeRoot, theme, Flow, graphics);
            Confirm = new ConfirmView(SafeRoot, theme, Flow);
            Loading = new LoadingView(SafeRoot, theme);
            Splash = new SplashView(SafeRoot, theme, Flow.Skip);
            Draw();
        }

        private void Update()
        {
            if (Flow == null) return;
            // One slow first frame must not use up the splash before the player has seen it.
            Flow.Tick(Mathf.Min(Time.unscaledDeltaTime, MaxStep));
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) Flow.Back();
            if (Flow.Stage != drawn) Draw();
            if (load == null) return;
            Loading.Set(load.progress / .9f);
            // The city arrives in one long frame; hold it until the loading card fully covers the menu.
            if (Loading.Screen.Shown) load.allowSceneActivation = true;
        }

        private void Draw()
        {
            drawn = Flow.Stage;
            Splash.Screen.Visible = drawn == MenuStage.Splash;
            Menu.Screen.Visible = drawn == MenuStage.Menu || drawn == MenuStage.Confirm;
            Confirm.Screen.Visible = drawn == MenuStage.Confirm;
            Loading.Screen.Visible = drawn == MenuStage.Loading;
            if (drawn != MenuStage.Loading || load != null) return;
            CareerLaunch.Choose(Flow.Fresh);
            load = SceneManager.LoadSceneAsync(City);
            load.allowSceneActivation = false;
        }
    }
}
