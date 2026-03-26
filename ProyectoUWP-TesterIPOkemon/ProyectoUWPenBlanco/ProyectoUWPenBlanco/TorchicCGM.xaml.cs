using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
using ProyectoUWPenBlanco;

namespace ProyectoUWPenBlanco
{
    public sealed partial class TorchicCGM : UserControl, iPokemon
    {
        private DispatcherTimer _timer;
        private double healthIncrement = 0;

        private ProgressBar _pbHealth;
        private ProgressBar _pbEnergy;
        private Image _imRedPotion;
        private Rectangle _rectHealthPulse;

        private bool _estaCansado = false;
        private bool _estaHerido = false;
        private bool _tieneEscudo = false;
        private bool _estaDerrotado = false;
        private bool _idleActivo = true;

        private DispatcherTimer _energyTimer;
        private double energyIncrement = 0;

        private DispatcherTimer _restTimer;
        private double _restRecovered = 0;

        private Image _imYellowPotion;

        public TorchicCGM()
        {
            this.InitializeComponent();

            // Buscar los elementos necesarios en el XAML
            _pbHealth = this.FindName("pbHealth") as ProgressBar;
            _pbEnergy = this.FindName("pbEnergy") as ProgressBar;
            _imRedPotion = this.FindName("imRedPotion") as Image;
            _imYellowPotion = this.FindName("imYellowPotion") as Image;
            _rectHealthPulse = this.FindName("rectHealthPulse") as Rectangle;

            // Inicializar valores de vida y energía
            if (_pbHealth != null) _pbHealth.Value = 60;
            if (_pbEnergy != null) _pbEnergy.Value = 80;

            // Lanzar animaciones automáticas al cargar la página
            this.Loaded += (s, ev) =>
            {
                this.Focus(FocusState.Programmatic);
                actualizarAnimacionIdle();
            };


        }
        private void actualizarAnimacionIdle()
        {
            var animRespiracion = this.Resources["AnimacionRespiracion"] as Storyboard;
            var animParpadeo = this.Resources["AnimacionParpadeo"] as Storyboard;

            bool puedeAnimar = _idleActivo && !_estaCansado && !_estaDerrotado;

            if (puedeAnimar)
            {
                animRespiracion?.Begin();
                animParpadeo?.Begin();
            }
            else
            {
                animRespiracion?.Stop();
                animParpadeo?.Stop();
            }
        }

        private void actualizarEstadoVisual()
        {
            var vb = this.FindName("vbPokemon") as Viewbox;
            var escudo = this.FindName("EscudoProtector") as Ellipse;
            var ct = this.FindName("transPokemon") as CompositeTransform;
            var aniHerido = this.Resources["AniHerido"] as Storyboard;
            var ojoIzq = this.FindName("scaleOjoIzq") as ScaleTransform;
            var ojoDer = this.FindName("scaleOjoDer") as ScaleTransform;

            if (escudo != null)
            {
                escudo.Opacity = _tieneEscudo ? 1 : 0;
            }

            if (vb == null || ct == null)
                return;

            // Estado base
            vb.Opacity = 1.0;
            ct.Rotation = 0;
            ct.TranslateY = 0;

            aniHerido?.Stop();
            ct.TranslateX = 0;

            if (ojoIzq != null) ojoIzq.ScaleY = 1;
            if (ojoDer != null) ojoDer.ScaleY = 1;

            // Derrota
            if (_estaDerrotado)
            {
                actualizarAnimacionIdle();

                vb.Opacity = 0.25;
                ct.Rotation = 90;
                ct.TranslateY = 100;

                if (ojoIzq != null) ojoIzq.ScaleY = 0.1;
                if (ojoDer != null) ojoDer.ScaleY = 0.1;
                return;
            }

            // Cansado + herido
            if (_estaCansado && _estaHerido)
            {
                actualizarAnimacionIdle();

                vb.Opacity = 0.65;
                ct.Rotation = -10;
                ct.TranslateY = 20;

                if (ojoIzq != null) ojoIzq.ScaleY = 0.35;
                if (ojoDer != null) ojoDer.ScaleY = 0.35;

                aniHerido?.Begin();
                return;
            }

            // Solo cansado
            if (_estaCansado)
            {
                actualizarAnimacionIdle();

                vb.Opacity = 0.80;
                ct.Rotation = -10;
                ct.TranslateY = 20;

                if (ojoIzq != null) ojoIzq.ScaleY = 0.45;
                if (ojoDer != null) ojoDer.ScaleY = 0.45;
                return;
            }

            // A partir de aquí sí puede haber idle normal
            actualizarAnimacionIdle();

            // Solo herido
            if (_estaHerido)
            {
                vb.Opacity = 0.75;
                ct.Rotation = -3;
                ct.TranslateY = 6;
                aniHerido?.Begin();
                return;
            }
        }

        private void useRedPotion(object sender, PointerRoutedEventArgs e)
        {
            if (_imRedPotion != null)
            {
                _imRedPotion.IsTapEnabled = false;
                _imRedPotion.Opacity = 0.5;
            }

            var animSalto = this.Resources["AnimacionSalto"] as Storyboard;
            animSalto?.Begin();

            var sb = this.Resources["HealPulseStoryboard"] as Storyboard;
            sb?.Begin();

            if (_timer != null)
            {
                _timer.Stop();
                _timer.Tick -= increaseHealth;
                _timer = null;
            }

            healthIncrement = 0;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += increaseHealth;
            _timer.Start();
        }

        private void useYellowPotion(object sender, PointerRoutedEventArgs e)
        {
            if (_imYellowPotion != null)
            {
                _imYellowPotion.IsTapEnabled = false;
                _imYellowPotion.Opacity = 0.5;
            }

            var animSalto = this.Resources["AnimacionSalto"] as Storyboard;
            animSalto?.Begin();

            if (_energyTimer != null)
            {
                _energyTimer.Stop();
                _energyTimer.Tick -= increaseEnergy;
                _energyTimer = null;
            }

            energyIncrement = 0;
            _energyTimer = new DispatcherTimer();
            _energyTimer.Interval = TimeSpan.FromMilliseconds(100);
            _energyTimer.Tick += increaseEnergy;
            _energyTimer.Start();
        }

        private void increaseEnergy(object sender, object e)
        {
            if (_pbEnergy != null)
            {
                _pbEnergy.Value = Math.Min(100, _pbEnergy.Value + 0.5);
            }

            energyIncrement += 0.5;

            if ((_pbEnergy != null && _pbEnergy.Value >= 100) || energyIncrement >= 40)
            {
                if (_energyTimer != null)
                {
                    _energyTimer.Stop();
                    _energyTimer.Tick -= increaseEnergy;
                    _energyTimer = null;
                }

                energyIncrement = 0;

                if (_imYellowPotion != null)
                {
                    _imYellowPotion.IsTapEnabled = true;
                    _imYellowPotion.Opacity = 1.0;
                }
            }
        }

        private void increaseHealth(object sender, object e)
        {
            if (_pbHealth != null)
            {
                _pbHealth.Value = Math.Min(100, _pbHealth.Value + 0.5);
            }
            healthIncrement += 0.5;

            if ((_pbHealth != null && _pbHealth.Value >= 100) || healthIncrement >= 40)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Tick -= increaseHealth;
                    _timer = null;
                }
                healthIncrement = 0;

                var sb = this.Resources["HealPulseStoryboard"] as Storyboard;
                sb?.Stop();

                if (_imRedPotion != null)
                {
                    _imRedPotion.IsTapEnabled = true;
                    _imRedPotion.Opacity = 1.0;
                }
            }
        }

        private void iniciarDescanso()
        {
            var sb = this.Resources["AniDescanso"] as Storyboard;
            sb?.Begin();

            if (_restTimer != null)
            {
                _restTimer.Stop();
                _restTimer.Tick -= aumentarDescanso;
                _restTimer = null;
            }

            _restRecovered = 0;
            _restTimer = new DispatcherTimer();
            _restTimer.Interval = TimeSpan.FromMilliseconds(100);
            _restTimer.Tick += aumentarDescanso;
            _restTimer.Start();
        }

        private void aumentarDescanso(object sender, object e)
        {
            if (_pbHealth != null)
                _pbHealth.Value = Math.Min(100, _pbHealth.Value + 0.5);

            if (_pbEnergy != null)
                _pbEnergy.Value = Math.Min(100, _pbEnergy.Value + 0.5);

            _restRecovered += 0.5;

            if (((_pbHealth != null && _pbHealth.Value >= 100) &&
                 (_pbEnergy != null && _pbEnergy.Value >= 100)) ||
                _restRecovered >= 15)
            {
                if (_restTimer != null)
                {
                    _restTimer.Stop();
                    _restTimer.Tick -= aumentarDescanso;
                    _restTimer = null;
                }

                _restRecovered = 0;
            }
        }

        // ==========================================
        // IMPLEMENTACIÓN DE LA INTERFAZ iPokemon
        // ==========================================

        public double Vida
        {
            get
            {
                return this._pbHealth != null ? this._pbHealth.Value : 0;
            }
            set
            {
                if (this._pbHealth != null)
                    this._pbHealth.Value = Math.Max(0, Math.Min(100, value));
            }
        }

        public double Energia
        {
            get
            {
                return this._pbEnergy != null ? this._pbEnergy.Value : 0;
            }
            set
            {
                if (this._pbEnergy != null)
                    this._pbEnergy.Value = Math.Max(0, Math.Min(100, value));
            }
        }

        public string Nombre
        {
            get
            {
                var textBlock = this.FindName("tbNombre") as TextBlock;
                return textBlock != null ? textBlock.Text : "Torchic";
            }
            set
            {
                var textBlock = this.FindName("tbNombre") as TextBlock;
                var textBlockOutline = this.FindName("tbNombreOutline") as TextBlock;

                if (textBlock != null)
                    textBlock.Text = value;

                if (textBlockOutline != null)
                    textBlockOutline.Text = value;
            }
        }

        public string Categoría { get; set; } = "Pollito";
        public string Tipo { get; set; } = "Fuego";
        public double Altura { get; set; } = 0.4;
        public double Peso { get; set; } = 2.5;
        public string Evolucion { get; set; } = "Combusken";
        public string Descripcion { get; set; } = "Torchic es un Pokémon de tipo Fuego con aspecto de pequeño pollito. Tiene en su interior una bolsa de fuego que mantiene su cuerpo caliente, por lo que al abrazarlo transmite mucho calor. Está cubierto de un plumaje suave y brillante, y aunque parece pequeño e inocente, puede lanzar potentes ataques de fuego. Con entrenamiento y experiencia, evoluciona en Combusken.";

        public void verFondo(bool ver)
        {
            var fondo = this.FindName("imFondo") as Image;
            if (fondo != null) fondo.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        public void verFilaVida(bool ver)
        {
            if (this.FindName("gridGeneral") is Grid grid)
                grid.RowDefinitions[0].Height = ver ? new GridLength(50) : new GridLength(0);
        }

        public void verFilaEnergia(bool ver)
        {
            if (this.FindName("gridGeneral") is Grid grid)
                grid.RowDefinitions[1].Height = ver ? new GridLength(50) : new GridLength(0);
        }

        public void verPocionVida(bool ver)
        {
            if (this._imRedPotion != null)
                this._imRedPotion.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        public void verPocionEnergia(bool ver)
        {
            var pocionAmarilla = this.FindName("imYellowPotion") as Image;
            if (pocionAmarilla != null)
                pocionAmarilla.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        public void verNombre(bool ver)
        {
            var bordeNombre = this.FindName("bdNombre") as Border;
            if (bordeNombre != null)
                bordeNombre.Visibility = ver ? Visibility.Visible : Visibility.Collapsed;
        }

        public void verEscudo(bool ver)
        {
            _tieneEscudo = ver;
            actualizarEstadoVisual();
        }

        public void activarAniIdle(bool activar)
        {
            _idleActivo = activar;
            actualizarAnimacionIdle();
        }

        public void animacionAtaqueFlojo()
        {
            var sb = this.Resources["AniAtaqueFlojo"] as Storyboard;
            sb?.Begin();
        }

        public void animacionAtaqueFuerte()
        {
            var sb = this.Resources["AniAtaqueFuerte"] as Storyboard;
            sb?.Begin();
        }

        public void animacionDefensa()
        {
            _tieneEscudo = true;
            var sb = this.Resources["AniDefensa"] as Storyboard;
            sb?.Begin();
            actualizarEstadoVisual();
        }

        public void animacionDescasar()
        {
            iniciarDescanso();
        }

        public void animacionCansado()
        {
            _estaCansado = true;
            actualizarEstadoVisual();
        }
        public void animacionNoCansado()
        {
            _estaCansado = false;
            actualizarEstadoVisual();
        }

        public void animacionHerido()
        {
            _estaHerido = true;
            actualizarEstadoVisual();
        }

        public void animacionNoHerido()
        {
            _estaHerido = false;
            actualizarEstadoVisual();
        }

        public void animacionDerrota()
        {
            _estaDerrotado = true;
            actualizarEstadoVisual();
        }
    }
}
